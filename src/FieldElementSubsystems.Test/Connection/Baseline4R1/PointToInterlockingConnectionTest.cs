using EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R1;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Messages.Baseline4R1;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

using PointMovePointCommand = EulynxLive.Messages.Baseline4R1.PointMovePointCommand;

namespace FieldElementSubsystems.Test.Connection.Baseline4R1;

public class PointToInterlockingConnectionTest
{

    private static Mock<IConnection> CreateDefaultMockConnection()
    {
        var mockConnection = new Mock<IConnection>();
        mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1", "100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1", "100").ToByteArray());
        return mockConnection;
    }

    private static readonly IDictionary<string, string?> TestSettings = new Dictionary<string, string?> {
        {"PointSettings:LocalId", "99W1" },
        {"PointSettings:LocalRastaId", "100" },
        {"PointSettings:RemoteId", "INTERLOCKING" },
        {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
        {"PointSettings:SimulatedTransitioningTimeSeconds", "0" },
        {"PointSettings:AllPointMachinesCrucial", "false" },
        {"PointSettings:ObserveAbilityToMove", "true" },
    };

    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(TestSettings)
        .Build();

    [Fact]
    public void Test_Connect()
    {
        // Arrange
        var mockConnection = new Mock<IConnection>();
        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);

        // Assert
        Assert.Equal(connection.CurrentConnection, mockConnection.Object);
    }

    [Fact]
    public async Task Test_Initialization()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection();
        var receivedMessages = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(receivedMessages)))
            .Returns(Task.FromResult(0));

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(LastCommandedPointPosition: null, PointPosition: GenericPointPosition.Left, DegradedPointPosition: GenericDegradedPointPosition.NotDegraded, AbilityToMove: GenericAbilityToMove.AbleToMove), true, CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.ReceiveAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        mockConnection.Verify(v => v.SendAsync(It.IsAny<byte[]>()), Times.Exactly(5));
        Assert.Equal(new PointPdiVersionCheckMessage("99W1________________", "INTERLOCKING________", PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, Array.Empty<byte>()).ToByteArray(), receivedMessages[0]);
        Assert.Equal(new PointStartInitialisationMessage("99W1________________", "INTERLOCKING________").ToByteArray(), receivedMessages[1]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), receivedMessages[2]);
        Assert.Equal(new PointAbilityToMovePointMessage("99W1________________", "INTERLOCKING________", PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsAbleToMove).ToByteArray(), receivedMessages[3]);
        Assert.Equal(new PointInitialisationCompletedMessage("99W1________________", "INTERLOCKING________").ToByteArray(), receivedMessages[4]);
    }

    [Fact]
    public async Task Test_Send_Position()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection();
        var args = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(args)))
            .Returns(Task.FromResult(0));

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(LastCommandedPointPosition: null, PointPosition: GenericPointPosition.Left, DegradedPointPosition: GenericDegradedPointPosition.NotDegraded, AbilityToMove: GenericAbilityToMove.AbleToMove), true, CancellationToken.None);
        foreach (var position in new List<GenericPointPosition>() { GenericPointPosition.Left, GenericPointPosition.Right, GenericPointPosition.UnintendedPosition, GenericPointPosition.NoEndPosition })
        {
            await connection.SendPointPosition(new GenericPointState(LastCommandedPointPosition: null, PointPosition: position, DegradedPointPosition: GenericDegradedPointPosition.NotDegraded, AbilityToMove: GenericAbilityToMove.AbleToMove));
        }

        // Assert
        mockConnection.Verify(v => v.SendAsync(It.IsAny<byte[]>()), Times.Exactly(9));
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[5]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[6]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsTrailed, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[7]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[8]);
    }

    [Fact]
    public async Task Test_Receive_Position()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection();
        mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1", "100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1", "100").ToByteArray())
            .ReturnsAsync(new PointMovePointCommand("99W1", "100", PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving).ToByteArray())
            .ReturnsAsync(new PointMovePointCommand("99W1", "100", PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving).ToByteArray());

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(LastCommandedPointPosition: null, PointPosition: GenericPointPosition.Left, DegradedPointPosition: GenericDegradedPointPosition.NotDegraded, AbilityToMove: GenericAbilityToMove.AbleToMove), true, CancellationToken.None);
        var position1 = await connection.ReceiveMovePointCommand(CancellationToken.None);
        var position2 = await connection.ReceiveMovePointCommand(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.ReceiveAsync(It.IsAny<CancellationToken>()), Times.Exactly(4));
        Assert.Equal(GenericPointPosition.Left, position1);
        Assert.Equal(GenericPointPosition.Right, position2);
    }

    [Fact]
    public async Task Test_TimeoutMessage()
    {
        // Arrange
        var mockConnection = CreateDefaultMockConnection();
        var args = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(args)))
            .Returns(Task.FromResult(0));

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(LastCommandedPointPosition: null, PointPosition: GenericPointPosition.Left, DegradedPointPosition: GenericDegradedPointPosition.NotDegraded, AbilityToMove: GenericAbilityToMove.AbleToMove), true, CancellationToken.None);
        await connection.SendTimeoutMessage();

        // Assert
        mockConnection.Verify(v => v.SendAsync(It.IsAny<byte[]>()), Times.Exactly(6));
        Assert.Equal(new PointTimeoutMessage("99W1________________", "INTERLOCKING________").ToByteArray(), args[5]);
    }
}
