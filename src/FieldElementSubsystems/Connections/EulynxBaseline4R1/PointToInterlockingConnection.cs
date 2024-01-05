using EulynxLive.FieldElementSubsystems.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using EulynxLive.FieldElementSubsystems.Configuration;
using Grpc.Core;
using EulynxLive.Messages.Baseline4R1;
using System.Threading.Channels;
using EulynxLive.FieldElementSubsystems.Extensions;

namespace EulynxLive.FieldElementSubsystems.Connections.EulynxBaseline4R1;

public class PointToInterlockingConnection : IPointToInterlockingConnection
{
    private readonly ILogger _logger;
    private readonly string _localId;
    private readonly string _remoteId;
    private readonly byte _pdiVersion;
    private readonly byte[] _checksum;

    public PointConfiguration Configuration { get; }

    private readonly Channel<byte[]> _overrideMessages;

    public IConnection? CurrentConnection { get; private set; }
    private readonly CancellationToken _stoppingToken;

    public PointToInterlockingConnection(
        ILogger<PointToInterlockingConnection> logger,
        IConfiguration configuration,
        CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;
        _logger = logger;
        CurrentConnection = null;

        var config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
        _localId = config.LocalId;
        _remoteId = config.RemoteId;
        _pdiVersion = config.PDIVersion;
        _checksum = config.PDIChecksum.HexToByteArray();
        if (_checksum.Length != 2) throw new InvalidOperationException($"Invalid checksum length. Expected 2 bytes was ${_checksum.Length}.");
        Configuration = config;
        _overrideMessages = Channel.CreateUnbounded<byte[]>();
    }


    public IPointToInterlockingConnection Connect(IConnection connection)
    {
        CurrentConnection = connection;
        return this;
    }

    public async Task<bool> InitializeConnection(GenericPointState state, bool observeAbilityToMove, bool simulateTimeout, CancellationToken cancellationToken)
    {
        var versionCheckReceived = await ReceiveMessage<PointPdiVersionCheckCommand>(cancellationToken);
        
        if (versionCheckReceived == null)
        {
            _logger.LogError("Unexpected message.");
            return false;
        }

        if (!CheckPDIVersionReceived(versionCheckReceived.PdiVersionOfSender))
        {
            // Eu.Gen-SCI.445
            // Eu.SCI-XX.PDI.91 - Eu.SCI-XX.PDI.94
            _logger.LogError("Version check failed.");
            var versionCheckFailedResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoNotMatch, _pdiVersion, 0, []);
            await SendMessage(versionCheckFailedResponse);
            return false;
        }

        var versionCheckResponse = new PointPdiVersionCheckMessage(_localId, _remoteId, PointPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, _pdiVersion, (byte)_checksum.Length, _checksum);
        
        await SendMessage(versionCheckResponse);

        if (simulateTimeout)
        {
            // Never send the missing initialization messages
            return false;
        }
        
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

    /// <summary>
    /// Checks whether the given version matches the expected version 
    /// </summary>
    /// <param name="versionCheckResponse"></param>
    /// <returns></returns> <summary>
    private bool CheckPDIVersionReceived(byte version) => version == _pdiVersion;

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

    private async Task<T> ReceiveMessage<T>(CancellationToken cancellationToken) where T : Message
    {
        if (CurrentConnection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");

        var message = Message.FromBytes(await CurrentConnection.ReceiveAsync(cancellationToken));
        if (message is T tMessage) return tMessage;
        _logger.LogError("Unexpected message: {}", message);
        throw new InvalidOperationException("Unexpected message.");
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

    public void Dispose()
    {
        CurrentConnection?.Dispose();
    }
}
