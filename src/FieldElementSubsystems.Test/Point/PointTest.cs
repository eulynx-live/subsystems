using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using EulynxLive.Point.Proto;
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

    private Mock<IPointToInterlockingConnection> CreateDefaultMockConnection() {
        var mockConnection = new Mock<IPointToInterlockingConnection>();
        mockConnection
            .Setup(m => m.SendPointPosition(
                It.IsAny<IPointToInterlockingConnection.PointState>()))
            .Returns(Task.FromResult(0));
        mockConnection
            .Setup(m => m.InitializeConnection(
                It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));
        return mockConnection;
    }

    private static IDictionary<string, string?> _testSettings = new Dictionary<string, string?> {
            {"PointSettings:LocalId", "99W1" },
            {"PointSettings:LocalRastaId", "100" },
            {"PointSettings:RemoteId", "INTERLOCKING" },
            {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
            {"PointSettings:AllPointMachinesCrucial", "false" },
            {"PointSettings:SimulateRandomTimeouts", "false" },
        };
    private IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_testSettings)
            .Build();
    private ILogger<EulynxLive.Point.Point> _logger = Mock.Of<ILogger<EulynxLive.Point.Point>>();

    [Fact]
    public void Test_Parse_Configuration()
    {
        var point = CreateDefaultPoint(null, new Dictionary<string, string>() {{"PointSettings:AllPointMachinesCrucial", "true" }});

        Assert.True(point.AllPointMachinesCrucial);
    }

    [Fact]
    public async Task Test_Default_Position()
    {
        var point = CreateDefaultPoint();
        await point.StartAsync(CancellationToken.None);

        Assert.Equal(IPointToInterlockingConnection.PointPosition.NoEndposition, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turn_Left()
    {
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection();
        var cancel = new CancellationTokenSource();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.Left))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<IPointToInterlockingConnection.PointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(cancel.Token);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(IPointToInterlockingConnection.PointPosition.Left, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turn_Right()
    {
        var cancel = new CancellationTokenSource();
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.Right))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<IPointToInterlockingConnection.PointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(IPointToInterlockingConnection.PointPosition.Right, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turnover()
    {
        var cancel = new CancellationTokenSource();
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.Right))
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(null))
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.Left))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<IPointToInterlockingConnection.PointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(IPointToInterlockingConnection.PointPosition.Left, point.PointState.PointPosition);
    }

    [Theory]
    [InlineData(PreventedPosition.PreventedLeft, IPointToInterlockingConnection.PointPosition.Left, IPointToInterlockingConnection.PointPosition.NoEndposition, IPointToInterlockingConnection.DegradedPointPosition.DegradedLeft)]
    [InlineData(PreventedPosition.PreventedRight, IPointToInterlockingConnection.PointPosition.Right, IPointToInterlockingConnection.PointPosition.NoEndposition, IPointToInterlockingConnection.DegradedPointPosition.DegradedRight)]
    [InlineData(PreventedPosition.Trailed, IPointToInterlockingConnection.PointPosition.Left, IPointToInterlockingConnection.PointPosition.UnintendetPosition, IPointToInterlockingConnection.DegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.None, IPointToInterlockingConnection.PointPosition.Right, IPointToInterlockingConnection.PointPosition.Right, IPointToInterlockingConnection.DegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.PreventedLeft, IPointToInterlockingConnection.PointPosition.Right, IPointToInterlockingConnection.PointPosition.Right, IPointToInterlockingConnection.DegradedPointPosition.NotDegraded)]
    [InlineData(PreventedPosition.PreventedRight, IPointToInterlockingConnection.PointPosition.Left, IPointToInterlockingConnection.PointPosition.Left, IPointToInterlockingConnection.DegradedPointPosition.NotDegraded)]
    public async Task Test_PreventEndPosition(PreventedPosition preventedPosition, IPointToInterlockingConnection.PointPosition actionedPosition, IPointToInterlockingConnection.PointPosition assertedPosition, IPointToInterlockingConnection.DegradedPointPosition assertedDegradedPosition)
    {
        var cancel = new CancellationTokenSource();
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection();

        mockConnection
            .SetupSequence(m => m.ReceivePointPosition(It.IsAny<CancellationToken>()))
            .Returns(() => {
                var message = new PreventedPositionMessage
                {
                    Position = preventedPosition
                };
                point.PreventEndPosition(message);

                return Task.FromResult<IPointToInterlockingConnection.PointPosition?>(null);
            })
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(actionedPosition))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<IPointToInterlockingConnection.PointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()));
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
            .Returns(() => {
                var message = new PreventedPositionMessage
                {
                    Position = PreventedPosition.PreventedLeft
                };
                point.PreventEndPosition(message);

                return Task.FromResult<IPointToInterlockingConnection.PointPosition?>(null);
            })
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.Left))
            .Returns(() => {
                var message = new PreventedPositionMessage
                {
                    Position = PreventedPosition.PreventedRight
                };
                point.PreventEndPosition(message);

                return Task.FromResult<IPointToInterlockingConnection.PointPosition?>(null);
            })
            .Returns(() => {
                Assert.Equal(IPointToInterlockingConnection.PointPosition.NoEndposition, point.PointState.PointPosition);
                Assert.Equal(IPointToInterlockingConnection.DegradedPointPosition.DegradedLeft, point.PointState.DegradedPointPosition);

                return Task.FromResult<IPointToInterlockingConnection.PointPosition?>(null);
            })
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.Right))
            .Returns(() =>
            {
                cancel.Cancel();
                return new TaskCompletionSource<IPointToInterlockingConnection.PointPosition?>().Task;
            });

        point = CreateDefaultPoint(mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(IPointToInterlockingConnection.PointPosition.NoEndposition, point.PointState.PointPosition);
        Assert.Equal(IPointToInterlockingConnection.DegradedPointPosition.DegradedRight, point.PointState.DegradedPointPosition);
    }
}
