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

public class CommandingAndReversingTests
{
    private static (EulynxLive.Point.Point point, Task pointTask, List<byte[]> receivedBytes) CreateMockedPointConnectionWithInitializationRequestAndMovePointCommand(bool allPointMachinesCrucial, bool simulateTimeouts, GenericPointState initialPointState, decimal pointMovementSeconds, params (PointMovePointCommandCommandedPointPosition, int)[] commandedPointPositions)
    {
        var settings = new Dictionary<string, string?> {
            {"PointSettings:LocalId", "99W1" },
            {"PointSettings:LocalRastaId", "100" },
            {"PointSettings:RemoteId", "INTERLOCKING" },
            {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
            {"PointSettings:SimulatedTransitioningTimeSeconds", pointMovementSeconds.ToString() },
            {"PointSettings:AllPointMachinesCrucial", allPointMachinesCrucial.ToString() },
            {"PointSettings:ObserveAbilityToMove", "true" },
            {"PointSettings:InitialLastCommandedPointPosition", initialPointState.LastCommandedPointPosition.ToString() },
            {"PointSettings:InitialPointPosition", initialPointState.PointPosition.ToString() },
            {"PointSettings:InitialDegradedPointPosition", initialPointState.DegradedPointPosition.ToString() },
            {"PointSettings:InitialAbilityToMove", initialPointState.AbilityToMove.ToString() },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var cancel = new CancellationTokenSource();
        var testTcs = new TaskCompletionSource();

        var mockConnection = new Mock<IConnection>();
        var sequenceSetup = mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1", "100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1", "100").ToByteArray());

        foreach (var commandedPointPosition in commandedPointPositions)
        {
            sequenceSetup = sequenceSetup.Returns(async () =>
            {
                await Task.Delay(commandedPointPosition.Item2);
                return new PointMovePointCommand("99W1", "100", commandedPointPosition.Item1).ToByteArray();
            });
        }

        sequenceSetup.Returns(async () =>
        {
            // Wait for the point to move
            await Task.Delay((int)((pointMovementSeconds * 1000) + 100));
            cancel.Cancel();
            testTcs.SetResult();
            return await new TaskCompletionSource<byte[]>().Task;
        });

        var receivedBytes = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(receivedBytes)))
            .Returns(Task.FromResult(0));

        var connectionProvider = new Mock<IConnectionProvider>();
        connectionProvider
            .Setup(x => x.Connect(It.IsAny<PointConfiguration>(), It.IsAny<CancellationToken>()))
            .Returns(mockConnection.Object);

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), configuration, CancellationToken.None);

        var mockHubContext = new Mock<IHubContext<StatusHub>>();
        mockHubContext.Setup(x => x.Clients.All.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var point = new EulynxLive.Point.Point(Mock.Of<ILogger<EulynxLive.Point.Point>>(), configuration, connection, connectionProvider.Object, mockHubContext.Object);
        if (simulateTimeouts) {
            point.EnableTimeoutLeft(true);
            point.EnableTimeoutRight(true);
        }

        async Task SimulatePoint()
        {
            try
            {
                await point.StartAsync(cancel.Token);
                await testTcs.Task;
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
            true, false,
            new GenericPointState
            (
                LastCommandedPointPosition: currentPointPosition,
                PointPosition: currentPointPosition,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ),
            0,
            (commandedPointPosition, 0));

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
            || currentPointPosition == GenericPointPosition.UnintendedPosition)
        {
            Assert.Equal(PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition, receivedMessages.OfType<PointPointPositionMessage>().First().ReportedPointPosition);
            Assert.Equal(expectedReportedPointPosition, receivedMessages.OfType<PointPointPositionMessage>().Skip(1).Single().ReportedPointPosition);
        }
        else
        {
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
        var oppositeCommandedPointPosition = commandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving,
            _ => throw new NotImplementedException()
        };

        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequestAndMovePointCommand(
            true, false,
            new GenericPointState
            (
                LastCommandedPointPosition: currentPointPosition,
                PointPosition: currentPointPosition,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ),
            0.5m, // Point takes a half second to move
            (commandedPointPosition, 0), (oppositeCommandedPointPosition, 100 /* 100 ms after the first command */));

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
            || commandedPointPosition == PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving && currentPointPosition == GenericPointPosition.Left
            || currentPointPosition == GenericPointPosition.UnintendedPosition)
        {
            Assert.Equal(PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition, receivedMessages.OfType<PointPointPositionMessage>().First().ReportedPointPosition);
            Assert.Equal(expectedReportedPointPosition, receivedMessages.OfType<PointPointPositionMessage>().Skip(1).Single().ReportedPointPosition);
        }
        else
        {
            // Point was in 'no end position' already
            Assert.Equal(expectedReportedPointPosition, receivedMessages.OfType<PointPointPositionMessage>().Single().ReportedPointPosition);
        }
    }

    /// <summary>
    /// Eu.P.1284 - Irregularities
    /// </summary>
    [Theory]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.Right)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.NoEndPosition)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving, GenericPointPosition.UnintendedPosition)]

    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.Left)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.NoEndPosition)]
    [InlineData(PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving, GenericPointPosition.UnintendedPosition)]
    public async Task HandleAndReportPointOperationTimeout(PointMovePointCommandCommandedPointPosition commandedPointPosition, GenericPointPosition currentPointPosition)
    {
        // Arrange
        var oppositeCommandedPointPosition = commandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving,
            _ => throw new NotImplementedException()
        };

        var (point, pointTask, receivedBytes) = CreateMockedPointConnectionWithInitializationRequestAndMovePointCommand(
            true, simulateTimeouts: true,
            new GenericPointState
            (
                LastCommandedPointPosition: currentPointPosition,
                PointPosition: currentPointPosition,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ),
            0.1m,
            (commandedPointPosition, 0));

        // Act
        await pointTask;

        // Assert
        var receivedMessages = receivedBytes.Select(x => Message.FromBytes(x))
            .SkipWhile(x => x is not PointInitialisationCompletedMessage).Skip(1)
            .ToList();

        Assert.IsType<PointMovementFailedMessage>(receivedMessages.Last());
    }

    // Interesting sequences.
    // 6708 Sending a move command to the same direction the point is already currently moving towards should be ignored
    // 6717 - Configured as redrive point, out of scope for now
    // 6719 - Internal failure. Externally not distinguishable from timeout.
    // 1474 - Go to and remain in no end position from end position or unintended position
    // 1273 - Go to and remain in unintended position from end position or no end position
    // 3207 - Go to and remain in end position from unintended position or no end position
    // 4761 - Go to and remain in end position from opposite end position
    // 5373 - Iff configure to observe ability to move, upon detecting unability to move the point should report this
    // 5797 - Iff configure to observe ability to move, upon detecting ability to move the point should report this
    // 6716 - Irregularities as redrive point, out of scope for now
    // 5801 - Degraded point positions
    // 5806 - Transitioning out of a degraded position
}
