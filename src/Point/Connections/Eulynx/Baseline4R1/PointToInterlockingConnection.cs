using EulynxLive.Messages.Baseline4R1;
using Grpc.Core;
using IPointToInterlockingConnection = EulynxLive.Point.Interfaces.IPointToInterlockingConnection;
using PointState = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointState;
using PointPosition = EulynxLive.Point.Interfaces.IPointToInterlockingConnection.PointPosition;
using EulynxLive.Point.Interfaces;


namespace EulynxLive.Point.EulynxBaseline4R1;

public class PointToInterlockingConnection : IPointToInterlockingConnection
{
    private readonly ILogger _logger;
    private readonly string _localId;
    private readonly string _localRastaId;
    private readonly string _remoteId;
    private readonly string _remoteEndpoint;
    IConnection? _currentConnection;
    private CancellationTokenSource _timeout;
    private int _timeoutDuration;
    private CancellationToken _stoppingToken;

    public PointToInterlockingConnection(
        ILogger<PointToInterlockingConnection> logger,
        IConfiguration configuration,
        CancellationToken stoppingToken, int timeoutDuration = 10000)
    {
        _timeoutDuration = timeoutDuration;
        _stoppingToken = stoppingToken;
        _timeout = new CancellationTokenSource();
        _logger = logger;
        _currentConnection = null;

        var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
        _localId = config.LocalId;
        _localRastaId = config.LocalRastaId.ToString();
        _remoteId = config.RemoteId;
        _remoteEndpoint = config.RemoteEndpoint;
    }

    public void Connect()
    {
        ResetTimeout();
        _logger.LogTrace("Connecting...");
        var metadata = new Metadata { { "rasta-id", _localRastaId } };
        _currentConnection = new GrpcConnection(metadata, _remoteEndpoint, _timeout.Token);
    }

    public async Task<bool> InitializeConnection(IPointToInterlockingConnection.PointState state, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Connected. Waiting for request...");
        if (await ReceiveMessage<PointPdiVersionCheckCommand>(cancellationToken) == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { });
        await SendMessage(versionCheckResponse);

        if (await ReceiveMessage<PointInitialisationRequestCommand>(cancellationToken) == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var startInitialization = new PointStartInitialisationMessage(_localId, _remoteId);
        await SendMessage(startInitialization);

        var pointState = new PointState(state);
        var initialPosition = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await SendMessage(initialPosition);

        var completeInitialization = new PointInitialisationCompletedMessage(_localId, _remoteId);
        await SendMessage(completeInitialization);
        return true;
    }

    public async Task SendPointPosition(IPointToInterlockingConnection.PointState state)
    {
        var pointState = new PointState(state);
        var response = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await SendMessage(response);
    }

    async public Task SendTimeoutMessage()
    {
        var response = new PointTimeoutMessage(_localId, _remoteId);
        await SendMessage(response);
    }

    public async Task<PointPosition?> ReceivePointPosition(CancellationToken cancellationToken)
    {
        var message = await ReceiveMessage<PointMovePointCommand>(cancellationToken);

        return (message != null)? message.CommandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => PointPosition.Right,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => PointPosition.Left,
            _ => throw new global::System.NotImplementedException(),
        } : null;
    }

    public void Dispose()
    {
        _currentConnection?.Dispose();
    }

    private async Task SendMessage(Message message)
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        await _currentConnection.SendAsync(message.ToByteArray());
    }

    private async Task<T?> ReceiveMessage<T>(CancellationToken cancellationToken) where T : Message
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        ResetTimeout();
        
        var message = Message.FromBytes(await _currentConnection.ReceiveAsync(_timeout.Token));
        if (message is not T)
        {
            _logger.LogError("Unexpected message: {}", message);
            return null;
        }
        return message as T;
    }

    private void ResetTimeout()
    {
        _timeout = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
        _timeout.CancelAfter(_timeoutDuration);
    }
}
