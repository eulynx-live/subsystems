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
    [Fact]
    public void PointShouldParseConfiguration()
    {
        var testSettings = new Dictionary<string, string?> {
            {"PointSettings:LocalId", "99W1" },
            {"PointSettings:LocalRastaId", "100" },
            {"PointSettings:RemoteId", "INTERLOCKING" },
            {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
            {"PointSettings:AllPointMachinesCrucial", "true" },
            {"PointSettings:SimulateRandomTimeouts", "true" },
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(testSettings)
            .Build();

        var point = new EulynxLive.Point.Point(Mock.Of<ILogger<EulynxLive.Point.Point>>(), configuration, Mock.Of<IPointToInterlockingConnection>());

        Assert.True(point.AllPointMachinesCrucial);
    }
}
