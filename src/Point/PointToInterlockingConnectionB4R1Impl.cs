using EulynxLive.Messages.Baseline4R1;
using EulynxLive.Point;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Sci;
using EulynxLive.Messages.IPointToInterlockingConnection;
using static EulynxLive.Messages.IPointToInterlockingConnection.IPointToInterlockingConnection;
using static Sci.Rasta;

public class PointToInterlockingConnectionB4R1Impl : IPointToInterlockingConnection
{
    private readonly ILogger _logger;
    private readonly string _localId;
    private readonly string _localRastaId;
    private readonly string _remoteId;
    private readonly string _remoteEndpoint;
    AsyncDuplexStreamingCall<SciPacket, SciPacket>? _currentConnection;
    private CancellationTokenSource _timeout;

    public PointToInterlockingConnectionB4R1Impl(
        ILogger<PointToInterlockingConnectionB4R1Impl> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _currentConnection = null;

        var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
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
        if (await ReceiveMessage<PointPdiVersionCheckCommand>() == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { });
        await SendMessage(versionCheckResponse);

        if (await ReceiveMessage<PointInitialisationRequestCommand>() == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var startInitialization = new PointStartInitialisationMessage(_localId, _remoteId);
        await SendMessage(startInitialization);

        var pointState = new B4R1PointStateImpl(state);
        var initialPosition = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await SendMessage(initialPosition);

        var completeInitialization = new PointInitialisationCompletedMessage(_localId, _remoteId);
        await SendMessage(completeInitialization);
        return true;
    }

    public async Task SendPointPosition(PointState state)
    {
        var pointState = new B4R1PointStateImpl(state);
        var response = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await SendMessage(response);
    }

    async public Task SendTimeoutMessage()
    {
        var response = new PointTimeoutMessage(_localId, _remoteId);
        await SendMessage(response);
    }

    public async Task<PointPosition?> ReceivePointPosition()
    {
        var message = await ReceiveMessage<PointMovePointCommand>();

        return (message != null)? message.CommandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointPosition.RIGHT,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointPosition.LEFT,
        } : null;
    }

    public void Dispose()
    {
        _currentConnection?.Dispose();
    }

    private async Task SendMessage(Message message)
    {
        if (_currentConnection == null) throw new NullReferenceException("Connection is null. Did you call Connect()?");
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(message.ToByteArray()) });
    }

    private async Task<T?> ReceiveMessage<T>() where T : Message
    {
        if (_currentConnection == null) throw new NullReferenceException("Connection is null. Did you call Connect()?");
        if (!await _currentConnection.ResponseStream.MoveNext(_timeout.Token)) return null;
        
        var message = Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());
        if (message is not T)
        {
            _logger.LogError("Unexpected message: {}", message);
            return null;
        }
        return message as T;
    }
}

public class B4R1PointStateImpl
{
    private PointState _state;
    public PointPointPositionMessageReportedPointPosition PointPosition { get => map(_state.PointPosition); }
    public PointPointPositionMessageReportedDegradedPointPosition DegradedPointPosition { get => map(_state.DegradedPointPosition); }

    public B4R1PointStateImpl(PointState state)
    {
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
