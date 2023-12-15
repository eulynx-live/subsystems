using EulynxLive.FieldElementSubsystems.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using EulynxLive.FieldElementSubsystems.Configuration;
using Grpc.Core;
using EulynxLive.Messages.Baseline4R1;


namespace EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R1;

public class PointToInterlockingConnection : IPointToInterlockingConnection
{
    private readonly ILogger _logger;
    private readonly string _localId;
    private readonly string _remoteId;
    public PointConfiguration Configuration { get; }
    public CancellationToken TimeoutToken => _timeout.Token;

    private IConnection? _currentConnection;
    public IConnection? CurrentConnection { get => _currentConnection; }
    private CancellationTokenSource _timeout;
    private readonly int _timeoutDuration;
    private CancellationToken _stoppingToken;

    public PointToInterlockingConnection(
        ILogger<PointToInterlockingConnection> logger,
        IConfiguration configuration, int timeoutDuration = 10000)
    {
        _timeoutDuration = timeoutDuration;
        _timeout = new CancellationTokenSource();
        _logger = logger;
        _currentConnection = null;

        var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
        _localId = config.LocalId;
        _remoteId = config.RemoteId;
        Configuration = config;
    }

    public void Connect(IConnection connection)
    {
        ResetTimeout();
        _currentConnection = connection;
    }

    public async Task<bool> InitializeConnection(GenericPointState state, CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        _logger.LogTrace("Connected. Waiting for request...");
        if (await ReceiveMessage<PointPdiVersionCheckCommand>(CancellationToken.None) == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { });
        await SendMessage(versionCheckResponse);

        if (await ReceiveMessage<PointInitialisationRequestCommand>(CancellationToken.None) == null)
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

    public async Task SendPointPosition(GenericPointState state)
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

    public async Task<GenericPointPosition?> ReceivePointPosition(CancellationToken cancellationToken)
    {
        var message = await ReceiveMessage<PointMovePointCommand>(cancellationToken);

        return (message != null)? message.CommandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => GenericPointPosition.Right,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => GenericPointPosition.Left,
            _ => throw new NotImplementedException(),
        } : null;
    }

    private async Task SendMessage(Message message)
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        await SendMessage(message.ToByteArray());
    }

    private async Task SendMessage(byte[] message)
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        await _currentConnection.SendAsync(message);
    }

    private async Task<T?> ReceiveMessage<T>(CancellationToken cancellationToken) where T : Message
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        ResetTimeout();

        try
        {
            var message = Message.FromBytes(await _currentConnection.ReceiveAsync(CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _timeout.Token).Token));
            if (message is not T)
            {
                _logger.LogError("Unexpected message: {}", message);
                return null;
            }
            return message as T;
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
        {
            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    private void ResetTimeout()
    {
        _timeout = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
        _timeout.CancelAfter(_timeoutDuration);
    }

    public async Task SendGenericMessage(byte[] message)
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        await SendMessage(message);
    }
}
