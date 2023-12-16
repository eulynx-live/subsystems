using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using EulynxLive.Point.Proto;
using Google.Protobuf;
using EulynxLive.Point;
namespace FieldElementSubsystems.Test;

public class PointTest
{
    private EulynxLive.Point.Point CreateDefaultPoint(IPointToInterlockingConnection? connection = null, IDictionary<string, string>? overwriteConfig= null) {
        var config = _configuration;
        if (overwriteConfig != null)
        {
            foreach (var (key, value) in overwriteConfig)
            {
                config[key] = value;
            }
        }
        return new EulynxLive.Point.Point(_logger, config, connection ?? Mock.Of<IPointToInterlockingConnection>(), Mock.Of<IConnectionProvider>(), () => Task.CompletedTask);
    }

    private static Mock<IPointToInterlockingConnection> CreateDefaultMockConnection(GenericPointState initialPointState) {
        var mockConnection = new Mock<IPointToInterlockingConnection>();
        mockConnection.Setup(x => x.Configuration).Returns(() => new PointConfiguration(
                "99W1",
                100,
                "INTERLOCKING",
                "http://localhost:50051",
                true,
                initialPointState.LastCommandedPointPosition,
                initialPointState.PointPosition,
                initialPointState.DegradedPointPosition,
                initialPointState.AbilityToMove,
                ConnectionProtocol.EulynxBaseline4R1
            ));
        mockConnection.Setup(x => x.TimeoutToken).Returns(() => CancellationToken.None);
        mockConnection
            .Setup(m => m.SendPointPosition(
                It.IsAny<GenericPointState>()))
            .Returns(Task.FromResult(0));
        mockConnection
            .Setup(m => m.InitializeConnection(
                It.IsAny<GenericPointState>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));
        return mockConnection;
    }

    private static readonly IDictionary<string, string?> TestSettings = new Dictionary<string, string?> {
        {"PointSettings:LocalId", "99W1" },
        {"PointSettings:LocalRastaId", "100" },
        {"PointSettings:RemoteId", "INTERLOCKING" },
        {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
        {"PointSettings:AllPointMachinesCrucial", "false" },
    };
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(TestSettings)
        .Build();
    private readonly ILogger<EulynxLive.Point.Point> _logger = Mock.Of<ILogger<EulynxLive.Point.Point>>();

    [Fact]
    public void Test_Parse_Configuration()
    {
        var point = CreateDefaultPoint(null, new Dictionary<string, string>() {{"PointSettings:AllPointMachinesCrucial", "true" }});

        Assert.True(point.AllPointMachinesCrucial);
    }

    [Fact]
    public void Test_Default_Position()
    {
        var point = CreateDefaultPoint();
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Timeout()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));
        var cancel = new CancellationTokenSource();

        var point = CreateDefaultPoint(mockConnection.Object);
        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                point.EnableTimeout();
                return Task.FromResult<GenericPointPosition?>(GenericPointPosition.Left);
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });


        // Act
        await point.StartAsync(cancel.Token);

        // Assert
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
        mockConnection.Verify(v => v.SendTimeoutMessage(), Times.Once());
    }

    [Theory]
    [InlineData(AbilityToMove.AbleToMove, GenericPointPosition.Left)]
    [InlineData(AbilityToMove.UnableToMove, GenericPointPosition.NoEndPosition)]
    [InlineData(AbilityToMove.Undefined, GenericPointPosition.NoEndPosition)]
    public async Task Test_AbilityToMove(AbilityToMove simulatedAbilityToMove, GenericPointPosition assertedPosition)
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));
        var cancel = new CancellationTokenSource();

        var point = CreateDefaultPoint(mockConnection.Object);
        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                var message = new AbilityToMoveMessage
                {
                    Ability = simulatedAbilityToMove
                };
                point.SetAbilityToMove(message);
                return Task.FromResult<GenericPointPosition?>(GenericPointPosition.Left);
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });


        // Act
        await point.StartAsync(cancel.Token);

        // Assert
        Assert.Equal(assertedPosition, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Send_SCI_Message()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));
        var cancel = new CancellationTokenSource();

        var rawBytes = new byte[]{ 0x0d, 0x0e, 0x0a, 0x0d, 0x0b, 0x0e, 0x0e, 0x0f };
        var genericMessage = new SciMessage()
        {
            Message = ByteString.CopyFrom (rawBytes)
        };

        var point = CreateDefaultPoint(mockConnection.Object);
        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<GenericPointPosition?>(null))
            .Returns(async () =>
            {
                await point.SendSciMessage(genericMessage);
                return null;
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        var args = new List<byte[]>();

        mockConnection
            .SetupSequence(m => m.SendSciMessage(Capture.In(args)))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });


        // Act
        await point.StartAsync(cancel.Token);

        // Assert
        Assert.Equal(rawBytes, args.ToArray()[0]);
    }

    [Fact]
    public async Task Test_Turn_Left()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));
        var cancel = new CancellationTokenSource();

        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<GenericPointPosition?>(GenericPointPosition.Left))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        var point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(cancel.Token);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.Left, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turn_Right()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));
        var cancel = new CancellationTokenSource();

        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<GenericPointPosition?>(GenericPointPosition.Right))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        var point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(cancel.Token);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.Right, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turnover()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));
        var cancel = new CancellationTokenSource();

        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<GenericPointPosition?>(GenericPointPosition.Right))
            .Returns(Task.FromResult<GenericPointPosition?>(null))
            .Returns(Task.FromResult<GenericPointPosition?>(GenericPointPosition.Left))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        var point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(cancel.Token);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.Left, point.PointState.PointPosition);
    }

    [Theory]
    [InlineData(PreventedPosition.PreventedLeft, PointDegradedPosition.DegradedLeft, GenericPointPosition.Left, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.DegradedLeft)]
    [InlineData(PreventedPosition.PreventedRight, PointDegradedPosition.DegradedRight, GenericPointPosition.Right, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.DegradedRight)]
    [InlineData(PreventedPosition.None, PointDegradedPosition.NotDegraded, GenericPointPosition.Right, GenericPointPosition.Right, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.PreventedLeft, PointDegradedPosition.DegradedLeft, GenericPointPosition.Right, GenericPointPosition.Right, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.PreventedRight, PointDegradedPosition.DegradedRight, GenericPointPosition.Left, GenericPointPosition.Left, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.PreventedLeft, PointDegradedPosition.NotDegraded, GenericPointPosition.Left, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.PreventedRight, PointDegradedPosition.NotDegraded, GenericPointPosition.Right, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotDegraded)]
    public async Task Test_PreventEndPosition(PreventedPosition simulatedPosition, PointDegradedPosition simulatedDegradedPosition, GenericPointPosition? actionedPosition, GenericPointPosition assertedPosition, GenericDegradedPointPosition assertedDegradedPosition)
    {
        var cancel = new CancellationTokenSource();
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() => {
                var message = new SimulatedPositionMessage
                {
                    Position = simulatedPosition,
                    DegradedPosition = simulatedDegradedPosition
                };
                point.PreventEndPosition(message);

                return Task.FromResult<GenericPointPosition?>(null);
            })
            .Returns(Task.FromResult<GenericPointPosition?>(actionedPosition))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(assertedPosition, point.PointState.PointPosition);
        Assert.Equal(assertedDegradedPosition, point.PointState.DegradedPointPosition);
    }

    [Theory]
    [InlineData(PointDegradedPosition.NotDegraded, GenericDegradedPointPosition.NotDegraded)]
    [InlineData(PointDegradedPosition.DegradedLeft, GenericDegradedPointPosition.DegradedLeft)]
    [InlineData(PointDegradedPosition.DegradedRight, GenericDegradedPointPosition.DegradedRight)]
    [InlineData(PointDegradedPosition.NotApplicable, GenericDegradedPointPosition.NotApplicable)]
    public async Task Test_PutIntoUnintendedPosition(PointDegradedPosition simulatedDegradedPosition, GenericDegradedPointPosition assertedDegradedPosition)
    {
        var cancel = new CancellationTokenSource();
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(async () => {
                var message = new SimulatedPositionMessage
                {
                    Position = PreventedPosition.PreventTrailed,
                    DegradedPosition = simulatedDegradedPosition
                };
                await point.PutIntoUnintendedPosition(message);

                return null;
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.UnintendedPosition, point.PointState.PointPosition);
        Assert.Equal(assertedDegradedPosition, point.PointState.DegradedPointPosition);
    }

    [Fact]
    public async Task Test_PreventEndPosition_Multiple_Times()
    {
        var cancel = new CancellationTokenSource();
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection(new GenericPointState(null, GenericPointPosition.NoEndPosition, GenericDegradedPointPosition.NotApplicable, GenericAbilityToMove.AbleToMove));

        mockConnection
            .SetupSequence(m => m.ReceiveMovePointCommand(It.IsAny<CancellationToken>()))
            .Returns(() => {
                var message = new SimulatedPositionMessage
                {
                    Position = PreventedPosition.PreventedLeft,
                    DegradedPosition = PointDegradedPosition.DegradedLeft
                };
                point.PreventEndPosition(message);

                return Task.FromResult<GenericPointPosition?>(null);
            })
            .Returns(Task.FromResult<GenericPointPosition?>(GenericPointPosition.Left))
            .Returns(() => {
                var message = new SimulatedPositionMessage
                {
                    Position = PreventedPosition.PreventedRight,
                    DegradedPosition = PointDegradedPosition.DegradedRight
                };
                point.PreventEndPosition(message);

                return Task.FromResult<GenericPointPosition?>(null);
            })
            .Returns(() => {
                Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
                Assert.Equal(GenericDegradedPointPosition.DegradedLeft, point.PointState.DegradedPointPosition);

                return Task.FromResult<GenericPointPosition?>(null);
            })
            .Returns(Task.FromResult<GenericPointPosition?>(GenericPointPosition.Right))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<GenericPointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
        Assert.Equal(GenericDegradedPointPosition.DegradedRight, point.PointState.DegradedPointPosition);
    }
}
