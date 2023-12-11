using EulynxLive.Messages.Baseline4R1;
using EulynxLive.FieldElementSubsystems.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using EulynxLive.FieldElementSubsystems.Configuration;


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
    private readonly CancellationToken _stoppingToken;

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
        _remoteId = config.RemoteId;
        Configuration = config;
    }

    public void Connect(IConnection connection)
    {
        ResetTimeout();
        _currentConnection = connection;
    }

    public async Task<bool> InitializeConnection(GenericPointState state, CancellationToken cancellationToken)
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

    public async Task SendGenericMessage(byte[] message)
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        await SendMessage(message);
    }

    public async Task SendAbilityToMoveMessage(GenericAbiliyToMove abilityToMove)
    {
        if (_currentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        if (abilityToMove == GenericAbiliyToMove.Unknown) 
        {
            _logger.LogInformation("Ability to move cannot be unknown.");
            return;
        }
        
        var abilityToMoveConverted = abilityToMove switch
        {
            GenericAbiliyToMove.CanMove => PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsAbleToMove,
            GenericAbiliyToMove.CannotMove => PointAbilityToMovePointMessageReportedAbilityToMovePointStatus.PointIsUnableToMove,
            _ => throw new NotImplementedException(),
        };
        var response = new PointAbilityToMovePointMessage(_localId, _remoteId, abilityToMoveConverted);
        await SendMessage(response);
    }
}
