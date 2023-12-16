using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R2;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Messages.Baseline4R2;
using EulynxLive.Point;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

namespace FieldElementSubsystems.Test.Point.Baseline4R2;

public class CommandingAndReversingTests
{
    private static (EulynxLive.Point.Point point, Task pointTask, List<byte[]> receivedBytes) CreateMockedPointConnectionWithInitializationRequestAndMovePointCommand(bool allPointMachinesCrucial, GenericPointState initialPointState, params PointMovePointCommandCommandedPointPosition[] commandedPointPositions)
    {
        var settings = new Dictionary<string, string?> {
            {"PointSettings:LocalId", "99W1" },
            {"PointSettings:LocalRastaId", "100" },
            {"PointSettings:RemoteId", "INTERLOCKING" },
            {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
            {"PointSettings:AllPointMachinesCrucial", allPointMachinesCrucial.ToString() },
            {"PointSettings:InitialLastCommandedPointPosition", initialPointState.LastCommandedPointPosition.ToString() },
            {"PointSettings:InitialPointPosition", initialPointState.PointPosition.ToString() },
            {"PointSettings:InitialDegradedPointPosition", initialPointState.DegradedPointPosition.ToString() },
            {"PointSettings:InitialAbilityToMove", initialPointState.AbilityToMove.ToString() },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var cancel = new CancellationTokenSource();

        var mockConnection = new Mock<IConnection>();
        var sequenceSetup = mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1", "100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1", "100").ToByteArray());

        foreach (var commandedPointPosition in commandedPointPositions) {
            sequenceSetup = sequenceSetup.ReturnsAsync(new PointMovePointCommand("99W1", "100", commandedPointPosition).ToByteArray());
        }

        sequenceSetup.Returns(() => {
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

        var point = new EulynxLive.Point.Point(Mock.Of<ILogger<EulynxLive.Point.Point>>(), configuration, connection, connectionProvider.Object, () => Task.CompletedTask);

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
    /// Eu.P.6714 - Commanding and reversing
    /// </summary>
    [Theory]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.Right)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.NoEndPosition)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.UnintendedPosition)]

    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.Left)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.NoEndPosition)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.UnintendedPosition)]
    public async Task MovingOfThePointWithSinglePointMachine(PointMovePointCommandCommandedPointPosition commandedPointPosition, GenericPointPosition currentPointPosition)
    {
        // Arrange
        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequestAndMovePointCommand(
            true,
            new GenericPointState
            (
                LastCommandedPointPosition: currentPointPosition,
                PointPosition: currentPointPosition,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ),
            commandedPointPosition);

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x))
            .SkipWhile(x => x is not PointInitialisationCompletedMessage).Skip(1)
            .ToList();
        var expectedReportedPointPosition = commandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
            _ => throw new NotImplementedException()
        };

        // If the point actually has to move, it should report 'no end position' first
        if (commandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving && currentPointPosition == GenericPointPosition.Right
            || commandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving && currentPointPosition == GenericPointPosition.Left
            || currentPointPosition == GenericPointPosition.UnintendedPosition) {
            Assert.Equal(PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition, receivedMessages.OfType<PointPointPositionMessage>().First().ReportedPointPosition);
            Assert.Equal(expectedReportedPointPosition, receivedMessages.OfType<PointPointPositionMessage>().Skip(1).Single().ReportedPointPosition);
        } else {
            // Point was in 'no end position' already
            Assert.Equal(expectedReportedPointPosition, receivedMessages.OfType<PointPointPositionMessage>().Single().ReportedPointPosition);
        }
    }

    /// <summary>
    /// Eu.P.6713 - Commanding and reversing
    /// </summary>
    [Theory]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.Right)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.NoEndPosition)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.UnintendedPosition)]

    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.Left)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.NoEndPosition)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.UnintendedPosition)]
    public async Task ReversingPoint(PointMovePointCommandCommandedPointPosition commandedPointPosition, GenericPointPosition currentPointPosition)
    {
        // Arrange
        var oppositeCommandedPointPosition = commandedPointPosition switch {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving,
            _ => throw new NotImplementedException()
        };

        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequestAndMovePointCommand(
            true,
            new GenericPointState
            (
                LastCommandedPointPosition: currentPointPosition,
                PointPosition: currentPointPosition,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ),
            commandedPointPosition, oppositeCommandedPointPosition);

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x))
            .SkipWhile(x => x is not PointInitialisationCompletedMessage).Skip(1)
            .ToList();
        var expectedReportedPointPosition = oppositeCommandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
            _ => throw new NotImplementedException()
        };

        // If the point actually has to move, it should report 'no end position' first
        if (commandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving && currentPointPosition == GenericPointPosition.Right
            || currentPointPosition == GenericPointPosition.UnintendedPosition) {
            Assert.Equal(PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition, receivedMessages.OfType<PointPointPositionMessage>().First().ReportedPointPosition);
            Assert.Equal(expectedReportedPointPosition, receivedMessages.OfType<PointPointPositionMessage>().Skip(1).Single().ReportedPointPosition);
        } else {
            // Point was in 'no end position' already
            Assert.Equal(expectedReportedPointPosition, receivedMessages.OfType<PointPointPositionMessage>().Single().ReportedPointPosition);
        }
    }
}
