using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
namespace FieldElementSubsystems.Test;

public class PointTest
{
    private EulynxLive.Point.Point CreateDefaultPoint(IPointToInterlockingConnection? connection = null) {
        return new(_logger, _configuration, connection ?? CreateDefaultMockConnection().Object, () => Task.CompletedTask);
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
        {"PointSettings:AllPointMachinesCrucial", "true" },
        {"PointSettings:SimulateRandomTimeouts", "false" },
    };
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(TestSettings)
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
        Assert.Equal(GenericPointPosition.NoEndPosition, point.PointState.PointPosition);
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
}
