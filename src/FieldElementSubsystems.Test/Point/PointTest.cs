using Castle.Core.Logging;
using EulynxLive.Messages.Baseline4R1;
using EulynxLive.Messages.IPointToInterlockingConnection;
using EulynxLive.Point;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
namespace FieldElementSubsystems.Test;

public class PointTest
{
    private IDictionary<string, string?> _testSettings = new Dictionary<string, string?> {
            {"PointSettings:LocalId", "99W1" },
            {"PointSettings:LocalRastaId", "100" },
            {"PointSettings:RemoteId", "INTERLOCKING" },
            {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
            {"PointSettings:AllPointMachinesCrucial", "true" },
            {"PointSettings:SimulateRandomTimeouts", "false" },
        };

    [Fact]
    public void PointShouldParseConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_testSettings)
            .Build();

        var point = new EulynxLive.Point.Point(Mock.Of<ILogger<EulynxLive.Point.Point>>(), configuration, Mock.Of<IPointToInterlockingConnection>());

        Assert.True(point.AllPointMachinesCrucial);
    }

    [Fact]
    public async void Test_Turnover()
    {
        
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(_testSettings)
            .Build();

        var logger = Mock.Of<ILogger<EulynxLive.Point.Point>>();

        var point = new EulynxLive.Point.Point(logger, configuration, Mock.Of<IPointToInterlockingConnection>());
        var mockConnection = new Mock<IPointToInterlockingConnection>();
        mockConnection
            .Setup(m => m.SendPointPosition(
                It.IsAny<IPointToInterlockingConnection.PointState>()))
            .Returns(Task.FromResult(0));
        mockConnection
            .Setup(m => m.InitializeConnection(
                It.IsAny<IPointToInterlockingConnection.PointState>()))
            .Returns(Task.FromResult(true));

        var finished = false;
        mockConnection
            .SetupSequence(m => m.ReceivePointPosition())
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.RIGHT))
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(null))
            .Returns(Task.FromResult<IPointToInterlockingConnection.PointPosition?>(IPointToInterlockingConnection.PointPosition.LEFT))
            .Returns(() =>
            {
                finished = true;
                return Task.FromResult<IPointToInterlockingConnection.PointPosition?>(null);
            });

        point = new EulynxLive.Point.Point(logger, configuration, mockConnection.Object);

        // Act
        await point.StartAsync(CancellationToken.None);

        while (!finished)
        {
            await Task.Delay(1000);
        }

        // Assert
        mockConnection.Verify(v => v.InitializeConnection(It.IsAny<IPointToInterlockingConnection.PointState>()));
        Assert.Equal(IPointToInterlockingConnection.PointPosition.LEFT, point.PointState.PointPosition);
    }
}
