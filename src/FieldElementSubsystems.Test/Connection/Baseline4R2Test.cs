using EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R2;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Messages.Baseline4R2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using PointMovePointCommand = EulynxLive.Messages.Baseline4R2.PointMovePointCommand;

public class Baseline4R1Test{

    private static readonly IDictionary<string, string?> TestSettings = new Dictionary<string, string?> {
        {"PointSettings:LocalId", "99W1" },
        {"PointSettings:LocalRastaId", "100" },
        {"PointSettings:RemoteId", "INTERLOCKING" },
        {"PointSettings:RemoteEndpoint", "http://localhost:50051" },
        {"PointSettings:AllPointMachinesCrucial", "false" },
        {"PointSettings:SimulateRandomTimeouts", "false" },
    };

    private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(TestSettings)
            .Build();
    
    [Fact]
    public void Test_Connect(){
        // Arrange
        var mockConnection = new Mock<IConnection>();
        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);

        // Assert
        Assert.Equal(connection.CurrentConnection, mockConnection.Object);
    }
    
    [Fact]
    public async Task Test_Initialization(){
        // Arrange
        var mockConnection = new Mock<IConnection>();
        mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1","100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1","100").ToByteArray());
        var args = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(args)))
            .Returns(Task.FromResult(0));

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(){PointPosition = GenericPointPosition.Left, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded}, CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.ReceiveAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        mockConnection.Verify(v => v.SendAsync(It.IsAny<byte[]>()), Times.Exactly(4));
        Assert.Equal(new PointPdiVersionCheckMessage("99W1________________", "INTERLOCKING________", PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { }).ToByteArray(), args[0]); 
        Assert.Equal(new PointStartInitialisationMessage("99W1________________", "INTERLOCKING________").ToByteArray(), args[1]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[2]);
        Assert.Equal(new PointInitialisationCompletedMessage("99W1________________", "INTERLOCKING________").ToByteArray(), args[3]);
    }
    
    [Fact]
    public async Task Test_Send_Position(){
        // Arrange
        var mockConnection = new Mock<IConnection>();
        mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1","100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1","100").ToByteArray());
        var args = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(args)))
            .Returns(Task.FromResult(0));

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(){PointPosition = GenericPointPosition.Left, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded}, CancellationToken.None);
        await connection.SendPointPosition(new GenericPointState(){PointPosition = GenericPointPosition.Right, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded});
        await connection.SendPointPosition(new GenericPointState(){PointPosition = GenericPointPosition.Left, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded});
        await connection.SendPointPosition(new GenericPointState(){PointPosition = GenericPointPosition.UnintendedPosition, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded});
        await connection.SendPointPosition(new GenericPointState(){PointPosition = GenericPointPosition.NoEndPosition, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded});

        // Assert
        mockConnection.Verify(v => v.SendAsync(It.IsAny<byte[]>()), Times.Exactly(8));
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[4]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[5]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInUnintendedPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[6]);
        Assert.Equal(new PointPointPositionMessage("99W1________________", "INTERLOCKING________", PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition, PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition).ToByteArray(), args[7]);
    }
    
    [Fact]
    public async Task Test_Receive_Position(){
        // Arrange
        var mockConnection = new Mock<IConnection>();
        mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1","100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1","100").ToByteArray())
            .ReturnsAsync(new PointMovePointCommand("99W1", "100", PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving).ToByteArray())
            .ReturnsAsync(new PointMovePointCommand("99W1", "100", PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving).ToByteArray());

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(){PointPosition = GenericPointPosition.Left, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded}, CancellationToken.None);
        var position1 = await connection.ReceivePointPosition(CancellationToken.None);
        var position2 = await connection.ReceivePointPosition(CancellationToken.None);

        // Assert
        mockConnection.Verify(v => v.ReceiveAsync(It.IsAny<CancellationToken>()), Times.Exactly(4));
        Assert.Equal(GenericPointPosition.Left, position1);
        Assert.Equal(GenericPointPosition.Right, position2);
    }
    
    [Fact]
    public async Task Test_TimeoutMessage(){
        // Arrange
        var mockConnection = new Mock<IConnection>();
        mockConnection.SetupSequence(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PointPdiVersionCheckCommand("99W1","100", 0x01).ToByteArray())
            .ReturnsAsync(new PointInitialisationRequestCommand("99W1","100").ToByteArray());
        var args = new List<byte[]>();
        mockConnection.Setup(x => x.SendAsync(Capture.In(args)))
            .Returns(Task.FromResult(0));

        var connection = new PointToInterlockingConnection(Mock.Of<ILogger<PointToInterlockingConnection>>(), _configuration, CancellationToken.None);

        // Act
        connection.Connect(mockConnection.Object);
        await connection.InitializeConnection(new GenericPointState(){PointPosition = GenericPointPosition.Left, DegradedPointPosition = GenericDegradedPointPosition.NotDegraded}, CancellationToken.None);
        await connection.SendTimeoutMessage();

        // Assert
        mockConnection.Verify(v => v.SendAsync(It.IsAny<byte[]>()), Times.Exactly(5));
        Assert.Equal(new PointMovementFailedMessage("99W1________________", "INTERLOCKING________").ToByteArray(), args[4]);
    }
}
