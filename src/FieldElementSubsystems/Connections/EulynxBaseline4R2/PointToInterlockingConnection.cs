using EulynxLive.FieldElementSubsystems.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using EulynxLive.FieldElementSubsystems.Configuration;
using Grpc.Core;
using EulynxLive.Messages.Baseline4R2;
using System.Threading.Channels;

namespace EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R2;

public class PointToInterlockingConnection : IPointToInterlockingConnection
{
    private readonly ILogger _logger;
    private readonly string _localId;
    private readonly string _remoteId;
    public PointConfiguration Configuration { get; }

    private readonly Channel<byte[]> _overrideMessages;

    public CancellationToken TimeoutToken => _timeout.Token;

    public IConnection? CurrentConnection { get; private set; }
    private CancellationTokenSource _timeout;
    private readonly int _timeoutDuration;
    private readonly CancellationToken _stoppingToken;

    public PointToInterlockingConnection(
        ILogger<PointToInterlockingConnection> logger,
        IConfiguration configuration,
        CancellationToken stoppingToken,
        int timeoutDuration = 10000)
    {
        _timeoutDuration = timeoutDuration;
        _stoppingToken = stoppingToken;
        _timeout = new CancellationTokenSource();
        _logger = logger;
        CurrentConnection = null;

        var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
        _localId = config.LocalId;
        _remoteId = config.RemoteId;
        Configuration = config;
        _overrideMessages = Channel.CreateUnbounded<byte[]>();
    }

    public void Connect(IConnection connection)
    {
        ResetTimeout();
        CurrentConnection = connection;
    }

    public async Task<bool> InitializeConnection(GenericPointState state, bool observeAbilityToMove, CancellationToken cancellationToken)
    {
        if (await ReceiveMessageWithTimeout<PointPdiVersionCheckCommand>(cancellationToken) == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0, 0, new byte[] { });
        await SendMessage(versionCheckResponse);

        if (await ReceiveMessageWithTimeout<PointInitialisationRequestCommand>(cancellationToken) == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        var startInitialization = new PointStartInitialisationMessage(_localId, _remoteId);
        await SendMessage(startInitialization);

        var pointState = new PointState(state);
        var initialPosition = new PointPointPositionMessage(_localId, _remoteId, pointState.PointPosition, pointState.DegradedPointPosition);
        await SendMessage(initialPosition);

        if (observeAbilityToMove)
        {
            var abilityToMove = new AbilityToMove(state);
            var initialAbilityToMove = new PointAbilityToMovePointMessage(_localId, _remoteId, abilityToMove.AbilityToMove);
            await SendMessage(initialAbilityToMove);
        }

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
        var response = new PointMovementFailedMessage(_localId, _remoteId);
        await SendMessage(response);
    }

    public async Task<GenericPointPosition> ReceiveMovePointCommand(CancellationToken cancellationToken)
    {
        var message = await ReceiveMessage<PointMovePointCommand>(cancellationToken);

        return message.CommandedPointPosition switch
        {
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsARightHandPointMoving => GenericPointPosition.Right,
            PointMovePointCommandCommandedPointPosition.SubsystemElectronicInterlockingRequestsALeftHandPointMoving => GenericPointPosition.Left,
            _ => throw new NotImplementedException(),
        };
    }

    private async Task SendMessage(Message message)
    {
        await SendMessage(message.ToByteArray());
    }

    private async Task SendMessage(byte[] message)
    {
        if (CurrentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        if (_overrideMessages.Reader.TryRead(out var overrideMessage)) message = overrideMessage;
        await CurrentConnection.SendAsync(message);
    }

    private async Task<T> ReceiveMessageWithTimeout<T>(CancellationToken cancellationToken) where T : Message
    {
        if (CurrentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        ResetTimeout();
        var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _timeout.Token).Token;
        return await ReceiveMessage<T>(token);
    }

    private async Task<T> ReceiveMessage<T>(CancellationToken cancellationToken) where T : Message
    {
        if (CurrentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        ResetTimeout();

        var message = Message.FromBytes(await CurrentConnection.ReceiveAsync(cancellationToken));
        if (message is T tMessage) return tMessage;
        _logger.LogError("Unexpected message: {}", message);
        throw new InvalidOperationException("Unexpected message.");
    }

    private void ResetTimeout()
    {
        _timeout = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken);
        _timeout.CancelAfter(_timeoutDuration);
    }


    public async Task SendSciMessage(byte[] message)
    {
        if (CurrentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
        await SendMessage(message);
    }

    public async Task OverrideNextSciMessage(byte[] message)
    {
        await _overrideMessages.Writer.WriteAsync(message);
    }

    public async Task SendAbilityToMove(GenericPointState pointState)
    {
        var abilityToMove = new AbilityToMove(pointState);
        var abilityToMoveMessage = new PointAbilityToMovePointMessage(_localId, _remoteId, abilityToMove.AbilityToMove);
        await SendMessage(abilityToMoveMessage);
    }
}
