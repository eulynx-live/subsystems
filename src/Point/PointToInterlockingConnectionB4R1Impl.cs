using System.Net.WebSockets;
using EulynxLive.Messages.Baseline4R1;
using EulynxLive.Point;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Sci;
using static IPointToInterlockingConnection;
using static Sci.Rasta;

public class PointToInterlockingConnectionB4R1Impl<T> : IPointToInterlockingConnection
{
    public bool AllPointMachinesCrucial { get; }
    private readonly ILogger<T> _logger;
    private readonly string _localId;
    private readonly string _localRastaId;
    private readonly string _remoteId;
    private readonly string _remoteEndpoint;
    AsyncDuplexStreamingCall<SciPacket, SciPacket>? _currentConnection;
    private CancellationTokenSource _timeout;

    public PointToInterlockingConnectionB4R1Impl(
        ILogger<T> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _currentConnection = null;

        var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
        if (config.AllPointMachinesCrucial == null)
        {
            _logger.LogInformation("Assuming all point machines are crucial.");
        }

        AllPointMachinesCrucial = config.AllPointMachinesCrucial ?? false;
        _localId = config.LocalId;
        _localRastaId = config.LocalRastaId.ToString();
        _remoteId = config.RemoteId;
        _remoteEndpoint = config.RemoteEndpoint;
        _timeout = new CancellationTokenSource();
    }

    public void Connect()
    {
        var channel = GrpcChannel.ForAddress(_remoteEndpoint);
        var client = new RastaClient(channel);
        _logger.LogTrace("Connecting...");
        _timeout = new CancellationTokenSource();
        _timeout.CancelAfter(10000);
        var metadata = new Metadata { { "rasta-id", _localRastaId } };
        _currentConnection = client.Stream(metadata, cancellationToken: _timeout.Token);
    }

    public async Task<bool> InitializeConnection(PointState state)
    {
        _logger.LogTrace("Connected. Waiting for request...");
        if (!await _currentConnection.ResponseStream.MoveNext(_timeout.Token)
            || Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is not PointPdiVersionCheckCommand)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { });
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(versionCheckResponse.ToByteArray()) });

        if (!await _currentConnection.ResponseStream.MoveNext(_timeout.Token)
            || Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is not PointInitialisationRequestCommand)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var startInitialization = new PointStartInitialisationMessage(_localId, _remoteId);
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(startInitialization.ToByteArray()) });

        var pointState = new B4R1PointStateImpl(state);
        var initialPosition = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(initialPosition.ToByteArray()) });

        var completeInitialization = new PointInitialisationCompletedMessage(_localId, _remoteId);
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(completeInitialization.ToByteArray()) });
        return true;
    }

    public async Task SendPointPosition(PointState state)
    {
        var pointState = new B4R1PointStateImpl(state);
        var response = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
    }

    async public Task SendTimeoutMessage()
    {
        var response = new PointTimeoutMessage(_localId, _remoteId);
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(response.ToByteArray()) });
    }

    public async Task<PointPosition?> ReceivePointPosition()
    {

        if (!await _currentConnection.ResponseStream.MoveNext())
        {
            return null;
        }
        var message = Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());

        if (message is PointMovePointCommand movePointCommand)
        {
            return movePointCommand.CommandedPointPosition switch
            {
                PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointPosition.RIGHT,
                PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointPosition.LEFT,
            };
        }
        
        _logger.LogInformation("Received unknown message {}", message.GetType());
        return null;
    }

    public void Dispose()
    {
        _currentConnection?.Dispose();
    }
}

public class B4R1PointStateImpl
{
    private PointState _state;
    public PointPointPositionMessageReportedPointPosition PointPosition { get => map(_state.PointPosition); }
    public PointPointPositionMessageReportedDegradedPointPosition DegradedPointPosition { get => map(_state.DegradedPointPosition); }

    public B4R1PointStateImpl(PointState state){
        _state = state;
    }

    private PointPointPositionMessageReportedPointPosition map(PointPosition value) => value switch
    {
        IPointToInterlockingConnection.PointPosition.LEFT => PointPointPositionMessageReportedPointPosition.PointIsInALeftHandPositionDefinedEndPosition,
        IPointToInterlockingConnection.PointPosition.RIGHT => PointPointPositionMessageReportedPointPosition.PointIsInARightHandPositionDefinedEndPosition,
        IPointToInterlockingConnection.PointPosition.TRAILED => PointPointPositionMessageReportedPointPosition.PointIsTrailed,
        IPointToInterlockingConnection.PointPosition.NO_ENDPOSITION => PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
    };

    private PointPointPositionMessageReportedDegradedPointPosition map(DegradedPointPosition value) => value switch
    {
        IPointToInterlockingConnection.DegradedPointPosition.DEGRADED_LEFT => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedLeftHandPosition,
        IPointToInterlockingConnection.DegradedPointPosition.DEGRADED_RIGHT => PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition,
        IPointToInterlockingConnection.DegradedPointPosition.NOT_DEGRADED => PointPointPositionMessageReportedDegradedPointPosition.PointIsNotInADegradedPosition,
        IPointToInterlockingConnection.DegradedPointPosition.NOT_APPLICABLE => PointPointPositionMessageReportedDegradedPointPosition.DegradedPointPositionIsNotApplicable,
    };
}
