using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
namespace FieldElementSubsystems.Test;

public class PointTest
{
    private EulynxLive.Point.Point CreateDefaultPoint(IPointToInterlockingConnection? connection = null) =>
        new(_logger, _configuration, connection ?? Mock.Of<IPointToInterlockingConnection>(), () => Task.CompletedTask);

    private static Mock<IPointToInterlockingConnection> CreateDefaultMockConnection() {
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

    private static readonly IDictionary<string, string?> _testSettings = new Dictionary<string, string?> {
        {"PointSettings:LocalId", "99W1" },
        {"PointSettings:LocalRastaId", "100" },
        {"PointSettings:RemoteId", "INTERLOCKING" },
        {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
        {"PointSettings:AllPointMachinesCrucial", "true" },
        {"PointSettings:SimulateRandomTimeouts", "false" },
    };
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_testSettings)
            .Build();
    private readonly ILogger<EulynxLive.Point.Point> _logger = Mock.Of<ILogger<EulynxLive.Point.Point>>();

    [Fact]
    public void Test_Parse_Configuration()
    {
        var point = CreateDefaultPoint();

        Assert.True(point.AllPointMachinesCrucial);
    }

    [Fact]
    public void Test_Default_Position()
    {
        var point = CreateDefaultPoint();
        Assert.Equal(IPointToInterlockingConnection.PointPosition.NoEndPosition, point.PointState.PointPosition);
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
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection();
        var cancel = new CancellationTokenSource();

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
        await point.StartAsync(cancel.Token);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(IPointToInterlockingConnection.PointPosition.Right, point.PointState.PointPosition);
    }

    [Fact]
    public async Task Test_Turnover()
    {
        // Arrange
        var point = CreateDefaultPoint();
        var mockConnection = CreateDefaultMockConnection();
        var cancel = new CancellationTokenSource();

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
        await point.StartAsync(cancel.Token);

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>(), It.IsAny<CancellationToken>()));
        Assert.Equal(IPointToInterlockingConnection.PointPosition.Left, point.PointState.PointPosition);
    }
}
