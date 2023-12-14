using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using EulynxLive.Point.Proto;
using Google.Protobuf;
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
        return new EulynxLive.Point.Point(_logger, config, connection ?? Mock.Of<IPointToInterlockingConnection>(), async () => {});
    }

    private static Mock<IPointToInterlockingConnection> CreateDefaultMockConnection() {
        var mockConnection = new Mock<IPointToInterlockingConnection>();
        mockConnection.Setup(x => x.Configuration).Returns(() => new PointConfiguration(
                "99W1",
                100,
                "INTERLOCKING",
                "http://localhost:50051",
                true,
                false,
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
        {"PointSettings:SimulateRandomTimeouts", "false" },
    };
    private IConfiguration _configuration = new ConfigurationBuilder()
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
    public async Task Test_Send_Generic_Message()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection();
        var cancel = new CancellationTokenSource();

        var rawBytes = new byte[]{ 0x0d, 0x0e, 0x0a, 0x0d, 0x0b, 0x0e, 0x0e, 0x0f };
        var genericMessage = new GenericSCIMessage()
        {
            Message = ByteString.CopyFrom (rawBytes)
        };

        var point = CreateDefaultPoint(mockConnection.Object);
        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<GenericPointPosition?>(null))
            .Returns(async () =>
            {
                await point.SendGenericMessage(genericMessage);
                return null;
            })
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<GenericPointPosition?>().Task;
            });

        var args = new List<byte[]>();

        mockConnection
            .SetupSequence(m => m.SendGenericMessage(Capture.In(args)))
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
        var mockConnection = CreateDefaultMockConnection();
        var cancel = new CancellationTokenSource();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
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
        var mockConnection = CreateDefaultMockConnection();
        var cancel = new CancellationTokenSource();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
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
        var mockConnection = CreateDefaultMockConnection();
        var cancel = new CancellationTokenSource();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
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
    [InlineData(PreventedPosition.PreventTrailed, PointDegradedPosition.NotDegraded, null, GenericPointPosition.UnintendedPosition, GenericDegradedPointPosition.NotDegraded)]
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
        var mockConnection = CreateDefaultMockConnection();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
            .Returns(async () => {
                var message = new SimulatedPositionMessage
                {
                    Position = simulatedPosition,
                    DegradedPosition = simulatedDegradedPosition
                };
                await point.PreventEndPosition(message);

                return null;
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

    [Fact]
    public async Task Test_PreventEndPosition_Multiple_Times()
    {
        var cancel = new CancellationTokenSource();
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
            .Returns(async () => {
                var message = new SimulatedPositionMessage
                {
                    Position = PreventedPosition.PreventedLeft,
                    DegradedPosition = PointDegradedPosition.DegradedLeft
                };
                await point.PreventEndPosition(message);

                return null;
            })
            .Returns(Task.FromResult<GenericPointPosition?>(GenericPointPosition.Left))
            .Returns(async () => {
                var message = new SimulatedPositionMessage
                {
                    Position = PreventedPosition.PreventedRight,
                    DegradedPosition = PointDegradedPosition.DegradedRight
                };
                await point.PreventEndPosition(message);

                return null;
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
