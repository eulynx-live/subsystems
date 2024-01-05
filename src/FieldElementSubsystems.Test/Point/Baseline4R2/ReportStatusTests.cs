using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R2;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Messages.Baseline4R2;
using EulynxLive.Point;
using EulynxLive.Point.Hubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

namespace FieldElementSubsystems.Test.Point.Baseline4R2;

public class ReportStatusTests
{
    private static (EulynxLive.Point.Point point, Task, List<byte[]> receivedBytes) CreateMockedPointConnectionWithInitializationRequest(bool allPointMachinesCrucial, GenericPointState initialPointState)
    {
        var settings = new Dictionary<string, string?> {
            {"PointSettings:LocalId", "99W1" },
            {"PointSettings:LocalRastaId", "100" },
            {"PointSettings:RemoteId", "INTERLOCKING" },
            {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
            {"PointSettings:SimulatedTransitioningTimeSeconds", "0" },
            {"PointSettings:AllPointMachinesCrucial", allPointMachinesCrucial.ToString() },
            {"PointSettings:ObserveAbilityToMove", "true" },
            {"PointSettings:InitialLastCommandedPointPosition", initialPointState.LastCommandedPointPosition.ToString() },
            {"PointSettings:InitialPointPosition", initialPointState.PointPosition.ToString() },
            {"PointSettings:InitialDegradedPointPosition", initialPointState.DegradedPointPosition.ToString() },
            {"PointSettings:InitialAbilityToMove", initialPointState.AbilityToMove.ToString() },
            {"PointSettings:PDIVersion", "1" },
            {"PointSettings:PDIChecksum", "0x0000" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var cancel = new CancellationTokenSource();

        var mockConnection = new Mock<IConnection>();
        mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1", "100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1", "100").ToByteArray())
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<byte[]>().Task;
            });

        var receivedBytes = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(receivedBytes)))
            .Returns(Task.FromResult(0));

        var connectionProvider = new Mock<IConnectionProvider>();
        connectionProvider
            .Setup(x => x.Connect(It.IsAny<PointConfiguration>(), It.IsAny<CancellationToken>()))
            .Returns(mockConnection.Object);

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), configuration, CancellationToken.None);
        connection.Connect(mockConnection.Object);

        var mockHubContext = new Mock<IHubContext<StatusHub>>();
        mockHubContext.Setup(x => x.Clients.All.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var point = new EulynxLive.Point.Point(Mock.Of<ILogger<EulynxLive.Point.Point>>(), configuration, connection, connectionProvider.Object, mockHubContext.Object);

        async Task SimulatePoint()
        {
            try
            {
                await point.StartAsync(cancel.Token);
            }
            catch (OperationCanceledException) { }
        }

        return (point, SimulatePoint(), receivedBytes);
    }

    /// <summary>
    /// Eu.P.1110 - Report Status
    /// </summary>
    [Fact]
    public async Task ReportStatusWithSinglePointMachine()
    {
        // Arrange
        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequest(true, new GenericPointState
        (
            LastCommandedPointPosition: null,
            PointPosition: GenericPointPosition.NoEndPosition,
            DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
            AbilityToMove: GenericAbilityToMove.AbleToMove
        ));

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x)).ToList();
        Assert.Single(receivedMessages.OfType<PointPointPositionMessage>());
        Assert.Single(receivedMessages.OfType<PointAbilityToMovePointMessage>());
    }

    /// <summary>
    /// Eu.P.5787 - Report Status
    /// </summary>
    [Theory]
    [InlineData(GenericPointPosition.Left, GenericPointPosition.Left, PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition)]
    [InlineData(GenericPointPosition.Left, GenericPointPosition.Right, PointPointPositionMessageReportedPointPosition.PointIsInUnintendedPosition)]
    [InlineData(GenericPointPosition.Left, GenericPointPosition.UnintendedPosition, PointPointPositionMessageReportedPointPosition.PointIsInUnintendedPosition)]
    [InlineData(GenericPointPosition.Left, GenericPointPosition.NoEndPosition, PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition)]

    [InlineData(GenericPointPosition.Right, GenericPointPosition.Right, PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition)]
    [InlineData(GenericPointPosition.Right, GenericPointPosition.Left, PointPointPositionMessageReportedPointPosition.PointIsInUnintendedPosition)]
    [InlineData(GenericPointPosition.Right, GenericPointPosition.UnintendedPosition, PointPointPositionMessageReportedPointPosition.PointIsInUnintendedPosition)]
    [InlineData(GenericPointPosition.Right, GenericPointPosition.NoEndPosition, PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition)]

    // A four-wire point machine has no memory about its last commanded position.
    // Therefore when the last commanded point position is unknown, the current point position must always be 'no end position'.
    [InlineData(null, GenericPointPosition.NoEndPosition, PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition)]
    public async Task ReportPointPositionWithSinglePointMachine(GenericPointPosition? lastCommandedPointPosition, GenericPointPosition currentPosition, PointPointPositionMessageReportedPointPosition expectedReportedPointPosition)
    {
        // Arrange
        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequest(true, new GenericPointState
        (
            LastCommandedPointPosition: lastCommandedPointPosition,
            PointPosition: currentPosition,
            DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
            AbilityToMove: GenericAbilityToMove.AbleToMove
        ));

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x)).ToList();
        var pointPositionMessage = receivedMessages.OfType<PointPointPositionMessage>().Single();
        Assert.Equal(expectedReportedPointPosition, pointPositionMessage.ReportedPointPosition);
    }

    /// <summary>
    /// Eu.P.5787 - Report Status
    /// </summary>
    [Theory]
    [InlineData(GenericAbilityToMove.AbleToMove, PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsAbleToMove)]
    [InlineData(GenericAbilityToMove.UnableToMove, PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsUnableToMove)]
    public async Task ReportAbilityToMoveWithSinglePointMachine(GenericAbilityToMove currentAbilityToMove, PointAbilityToMovePointMessageReportedAbilityToMovePointStatus expectedReportedAbilityToMove)
    {
        // Arrange
        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequest(true, new GenericPointState
        (
            LastCommandedPointPosition: null,
            PointPosition: GenericPointPosition.NoEndPosition,
            DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
            AbilityToMove: currentAbilityToMove
        ));

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x)).ToList();
        var pointPositionMessage = receivedMessages.OfType<PointAbilityToMovePointMessage>().Single();
        Assert.Equal(expectedReportedAbilityToMove, pointPositionMessage.ReportedAbilityToMovePointStatus);
    }

    /// <summary>
    /// Eu.P.5789 - Report Status
    /// </summary>
    [Theory]
    [InlineData(true, GenericDegradedPointPosition.NotApplicable, PointPointPositionMessageReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable)]
    [InlineData(false, GenericDegradedPointPosition.NotDegraded, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition)]
    [InlineData(false, GenericDegradedPointPosition.DegradedLeft, PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedLeftHandPosition)]
    [InlineData(false, GenericDegradedPointPosition.DegradedRight, PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition)]
    public async Task ReportPointPositionWithMultiplePointMachines(bool allPointMachinesCrucial, GenericDegradedPointPosition currentDegradedPosition, PointPointPositionMessageReportedDegradedPointPosition expectedReportedDegradedPosition)
    {
        // Arrange
        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequest(allPointMachinesCrucial, new GenericPointState
        (
            LastCommandedPointPosition: null,
            PointPosition: GenericPointPosition.NoEndPosition,
            DegradedPointPosition: currentDegradedPosition,
            AbilityToMove: GenericAbilityToMove.AbleToMove
        ));

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x)).ToList();
        var pointPositionMessage = receivedMessages.OfType<PointPointPositionMessage>().Single();
        Assert.Equal(expectedReportedDegradedPosition, pointPositionMessage.ReportedDegradedPointPosition);
    }


    /// <summary>
    /// Eu.P.5783 - Report Status
    /// </summary>
    [Theory]
    [InlineData(GenericAbilityToMove.AbleToMove, PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsAbleToMove)]
    [InlineData(GenericAbilityToMove.UnableToMove, PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsUnableToMove)]
    public async Task ReportAbilityToMoveWithMultiplePointMachines(GenericAbilityToMove currentAbilityToMove, PointAbilityToMovePointMessageReportedAbilityToMovePointStatus expectedReportedAbilityToMove)
    {
        // The only difference to the single point machine case is that we're passing 'false' for 'allPointMachinesCrucial'.

        // Arrange
        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequest(false, new GenericPointState
        (
            LastCommandedPointPosition: null,
            PointPosition: GenericPointPosition.NoEndPosition,
            DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
            AbilityToMove: currentAbilityToMove
        ));

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x)).ToList();
        var pointPositionMessage = receivedMessages.OfType<PointAbilityToMovePointMessage>().Single();
        Assert.Equal(expectedReportedAbilityToMove, pointPositionMessage.ReportedAbilityToMovePointStatus);
    }
}
