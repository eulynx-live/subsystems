using EulynxLive.Messages.Baseline4R1;
using EulynxLive.Point;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Sci;
using EulynxLive.Messages.IPointToInterlockingConnection;
using static EulynxLive.Messages.IPointToInterlockingConnection.IPointToInterlockingConnection;
using static Sci.Rasta;

namespace EulynxLive.Point.EulynxBaseline4R1;

public class PointToInterlockingConnection : IPointToInterlockingConnection
{
    private readonly ILogger _logger;
    private readonly string _localId;
    private readonly string _localRastaId;
    private readonly string _remoteId;
    private readonly string _remoteEndpoint;
    AsyncDuplexStreamingCall<SciPacket, SciPacket>? _currentConnection;
    private CancellationTokenSource _timeout;

    public PointToInterlockingConnection(
        ILogger<PointToInterlockingConnection> logger,
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

        var pointState = new PointStateBaseline4R1(state);
        var initialPosition = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await SendMessage(initialPosition);

        var completeInitialization = new PointInitialisationCompletedMessage(_localId, _remoteId);
        await SendMessage(completeInitialization);
        return true;
    }

    public async Task SendPointPosition(PointState state)
    {
        var pointState = new PointStateBaseline4R1(state);
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
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointPosition.Right,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointPosition.Left,
        } : null;
    }

    public void Dispose()
    {
        _currentConnection?.Dispose();
    }

    private async Task SendMessage(Message message)
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(message.ToByteArray()) });
    }

    private async Task<T?> ReceiveMessage<T>() where T : Message
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
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
