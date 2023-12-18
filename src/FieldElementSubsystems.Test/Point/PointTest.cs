using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using EulynxLive.Point.Proto;
using Google.Protobuf;
using EulynxLive.Point;
using EulynxLive.Point.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FieldElementSubsystems.Test;

public class PointTest
{
    private static (EulynxLive.Point.Point, Func<Task> simulatePoint, Mock<IPointToInterlockingConnection>, CancellationTokenSource) CreateDefaultPoint(bool allPointMachinesCrucial, GenericPointState initialPointState)
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
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var cancel = new CancellationTokenSource();
        var config = configuration.GetSection("PointSettings").Get<PointConfiguration>()!;
        var mockConnection = CreateDefaultMockConnection(config);

        var mockHubContext = new Mock<IHubContext<StatusHub>>();
        mockHubContext.Setup(x => x.Clients.All.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var point = new EulynxLive.Point.Point(Mock.Of<ILogger<EulynxLive.Point.Point>>(), configuration, mockConnection.Object, Mock.Of<IConnectionProvider>(), mockHubContext.Object);

        async Task SimulatePoint()
        {
            try
            {
                await point.StartAsync(cancel.Token);
            }
            catch (OperationCanceledException) { }
        }

        return (point, SimulatePoint, mockConnection, cancel);
    }

    private static Mock<IPointToInterlockingConnection> CreateDefaultMockConnection(PointConfiguration configuration)
    {
        var mockConnection = new Mock<IPointToInterlockingConnection>();
        mockConnection.Setup(x => x.Configuration).Returns(() => configuration);
        mockConnection.Setup(x => x.TimeoutToken).Returns(() => CancellationToken.None);
        mockConnection
            .Setup(m => m.SendPointPosition(
                It.IsAny<GenericPointState>()))
            .Returns(Task.FromResult(0));
        mockConnection
            .Setup(m => m.InitializeConnection(
                It.IsAny<GenericPointState>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));
        return mockConnection;
    }

    private readonly ILogger<EulynxLive.Point.Point> _logger = Mock.Of<ILogger<EulynxLive.Point.Point>>();

    [Fact]
    public void Test_Parse_Configuration()
    {
        var (point, _, _, _) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        Assert.True(point.AllPointMachinesCrucial);
    }

    [Fact]
    public void Test_Default_Position()
    {
        var (point, _, _, _) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_TimeoutLeft()
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                point.EnableTimeoutLeft(true);
                return Task.FromResult(GenericPointPosition.Left);
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });


        // Act
        await pointTask();

        // Assert
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
        connection.Verify(v => v.SendTimeoutMessage(), Times.Once());
    }

    [Fact]
    public async Task Test_TimeoutRight()
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                point.EnableTimeoutRight(true);
                return Task.FromResult(GenericPointPosition.Right);
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });


        // Act
        await pointTask();

        // Assert
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
        connection.Verify(v => v.SendTimeoutMessage(), Times.Once());
    }

    [Theory]
    [InlineData(AbilityToMove.AbleToMove, GenericPointPosition.Left)]
    [InlineData(AbilityToMove.UnableToMove, GenericPointPosition.NoEndPosition)]
    public async Task Test_AbilityToMove(AbilityToMove simulatedAbilityToMove, GenericPointPosition assertedPosition)
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                var message = new AbilityToMoveMessage
                {
                    Ability = simulatedAbilityToMove
                };
                point.SetAbilityToMove(message);
                return Task.FromResult(GenericPointPosition.Left);
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });


        // Act
        await pointTask();

        // Assert
        Assert.Equal(assertedPosition, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Send_SCI_Message()
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        var rawBytes = new byte[] { 0x0d, 0x0e, 0x0a, 0x0d, 0x0b, 0x0e, 0x0e, 0x0f };
        var genericMessage = new SciMessage()
        {
            Message = ByteString.CopyFrom(rawBytes)
        };

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                await point.SendSciMessage(genericMessage);
                return await new TaskCompletionSource<GenericPointPosition>().Task;
            });

        var args = new List<byte[]>();

        connection
            .SetupSequence(m => m.SendSciMessage(Capture.In(args)))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });


        // Act
        await pointTask();

        // Assert
        Assert.Equal(rawBytes, args.ToArray()[0]);
    }

    [Fact]
    public async Task Test_Turn_Left()
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(GenericPointPosition.Left))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });

        // Act
        await pointTask();

        // Assert
        connection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.Left, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turn_Right()
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(GenericPointPosition.Right))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });

        // Act
        await pointTask();

        // Assert
        connection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.Right, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turnover()
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(true, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(GenericPointPosition.Right))
            .Returns(Task.FromResult(GenericPointPosition.Left))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });

        // Act
        await pointTask();

        // Assert
        connection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.Left, point.PointState.PointPosition);
    }

    [Theory]
    [InlineData(PreventedPosition.SetNoEndPosition, true, GenericPointPosition.Left, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.DegradedLeft)]
    [InlineData(PreventedPosition.DoNotPrevent, false, GenericPointPosition.Right, GenericPointPosition.Right, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.SetNoEndPosition, true, GenericPointPosition.Right, GenericPointPosition.Right, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.SetNoEndPosition, false, GenericPointPosition.Left, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotDegraded)]
    public async Task Test_PreventEndPosition(PreventedPosition simulatedPosition, bool degradePosition, GenericPointPosition commandedPosition, GenericPointPosition expectedPosition, GenericDegradedPointPosition expectedDegradedPosition)
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(false, new GenericPointState(GenericPointPosition.Right, GenericPointPosition.Right, GenericDegradedPointPosition.NotDegraded, GenericAbilityToMove.AbleToMove));

        var message = new PreventedPositionMessage
        {
            Position = simulatedPosition,
            DegradedPosition = degradePosition
        };
        point.PreventLeftEndPosition(message);

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                return Task.FromResult(commandedPosition);
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });

        // Act
        await pointTask();

        // Assert
        connection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
        Assert.Equal(expectedPosition, point.PointState.PointPosition);
        Assert.Equal(expectedDegradedPosition, point.PointState.DegradedPointPosition);
    }

    [Theory]
    [InlineData(false, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(true, GenericDegradedPointPosition.DegradedLeft)]
    public async Task Test_PutIntoUnintendedPosition(bool simulatedDegradedPosition, GenericDegradedPointPosition expectedDegradedPosition)
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(false, new GenericPointState(GenericPointPosition.Left, GenericPointPosition.Left, GenericDegradedPointPosition.NotDegraded, GenericAbilityToMove.AbleToMove));

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(async () =>
            {
                var message = new DegradedPositionMessage
                {
                    DegradedPosition = simulatedDegradedPosition
                };
                point.PutIntoUnintendedPosition(message);

                cancel.Cancel();
                return await new TaskCompletionSource<GenericPointPosition>().Task;
            });

        // Act
        await pointTask();

        // Assert
        connection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.UnintendedPosition, point.PointState.PointPosition);
        Assert.Equal(expectedDegradedPosition, point.PointState.DegradedPointPosition);
    }

    [Fact]
    public async Task Test_PreventEndPosition_Multiple_Times()
    {
        // Arrange
        var (point, pointTask, connection, cancel) = CreateDefaultPoint(false, new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        var preventLeft = new PreventedPositionMessage
        {
            Position = PreventedPosition.SetNoEndPosition,
            DegradedPosition = true
        };
        point.PreventLeftEndPosition(preventLeft);

        var preventRight = new PreventedPositionMessage
        {
            Position = PreventedPosition.SetNoEndPosition,
            DegradedPosition = true
        };
        point.PreventRightEndPosition(preventRight);

        connection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(GenericPointPosition.Left))
            .Returns(() => Task.FromResult(GenericPointPosition.Right))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition>().Task;
            });

        // Act
        await pointTask();

        // Assert
        connection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
        Assert.Equal(GenericDegradedPointPosition.DegradedRight, point.PointState.DegradedPointPosition);
    }
}
