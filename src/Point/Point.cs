using EulynxLive.FieldElementSubsystems.Configuration;
using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point.Hubs;
using EulynxLive.Point.Proto;

using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.SignalR;

using PropertyChanged.SourceGenerator;

using System.ComponentModel;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EulynxLive.Point
{
    public partial class Point : BackgroundService, IPoint
    {
        public IPointToInterlockingConnectionBuilder? ConnectionBuilder { get; }

        public IPointToInterlockingConnection? Connection { get; private set; }

        public bool AllPointMachinesCrucial { get; }
        public bool ObserveAbilityToMove { get; }
        public ConnectionProtocol? ConnectionProtocol => _config.ConnectionProtocol;

        [Notify]
        private GenericPointState _pointState;
        [Notify]
        private SimulatedPointState _simulatedPointState;
        [Notify]
        private bool _initialized;

        private readonly ILogger<Point> _logger;
        private readonly IPointToInterlockingConnectionBuilder _connectionBuilder;
        private readonly IConnectionProvider _connectionProvider;
        private readonly PointConfiguration _config;
        private CancellationTokenSource _resetTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);

        public Point(ILogger<Point> logger, IConfiguration configuration, IPointToInterlockingConnectionBuilder connectionBuilder, IConnectionProvider connectionProvider, IHubContext<StatusHub> statusHub)
        {
            _connectionBuilder = connectionBuilder;
            _connectionProvider = connectionProvider;
            _logger = logger;

            _config = configuration.GetSection("PointSettings").Get<PointConfiguration>() ?? throw new Exception("No configuration provided");
            AllPointMachinesCrucial = _config.AllPointMachinesCrucial;
            ObserveAbilityToMove = _config.ObserveAbilityToMove;

            // Validate the configuration.
            if (_config.InitialLastCommandedPointPosition == null && _config.InitialPointPosition != GenericPointPosition.NoEndPosition)
            {
                throw new InvalidOperationException("If the last commanded point position is unknown, the position reported by the point machine must be NoEndPosition.");
            }

            if (_config.AllPointMachinesCrucial && _config.InitialDegradedPointPosition != GenericDegradedPointPosition.NotApplicable)
            {
                throw new InvalidOperationException("If all point machines are crucial, the degraded point position must be NotApplicable.");
            }

            if (_config.ObserveAbilityToMove && _config.InitialAbilityToMove == null)
            {
                throw new InvalidOperationException("If the ability to move is observed, the initial ability to move must be set.");
            }

            var initialPointPosition = (
                _config.InitialLastCommandedPointPosition == GenericPointPosition.Left && _config.InitialPointPosition == GenericPointPosition.Right
                || _config.InitialLastCommandedPointPosition == GenericPointPosition.Right && _config.InitialPointPosition == GenericPointPosition.Left)
                    ? GenericPointPosition.UnintendedPosition
                    : _config.InitialPointPosition;

            _pointState = new GenericPointState
            (
                LastCommandedPointPosition: _config.InitialLastCommandedPointPosition,
                DegradedPointPosition: RespectAllPointMachinesCrucial(_config.InitialDegradedPointPosition),
                PointPosition: initialPointPosition,
                AbilityToMove: _config.InitialAbilityToMove
            );

            _simulatedPointState = new SimulatedPointState(
                PreventedPositionLeft: PreventedPosition.DoNotPrevent,
                PreventedPositionRight: PreventedPosition.DoNotPrevent,
                DegradedPositionLeft: false,
                DegradedPositionRight: false,
                SimulateInitializationTimeout: false
            );

            Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(h => PropertyChanged += h, h => PropertyChanged -= h)
                .Select(x => Unit.Default)
                .Merge(Observable.Interval(TimeSpan.FromSeconds(1)).Select(x => Unit.Default))
                .Subscribe(x => statusHub.Clients.All.SendAsync("ReceiveStatus", _initialized, PointState, SimulatedPointState, _config));
        }

        public async Task SendSciMessage(SciMessage message)
        {
            if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            _logger.LogInformation("Sending SCI message: {}", message.Message);
            await Connection.SendSciMessage(message.Message.ToByteArray());
        }

        /// <summary>
        /// Send a SCI message with a Protocol Error.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SendSciMessagePDIError(Empty request)
        {
            if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            // Arbitrary message, that is not protocol type SCI-P.
            byte[] message = new Messages.Baseline4R1.CCPdiVersionCheckCommand(
                _config.LocalId,
                _config.RemoteId,
                _config.PDIVersion).ToByteArray();
            _logger.LogInformation("Sending SCI message: {}", message);
            await Connection.SendSciMessage(message);
        }

        /// <summary>
        /// Send a SCI message with content with a Content Telegram Error.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SendSciMessageContentError(Empty request)
        {
            if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            byte[] messageBytes = ConnectionProtocol switch
            {
                FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R1 =>
                    new Messages.Baseline4R1.PointPointPositionMessage(
                        _config.LocalId,
                        _config.RemoteId,
                        Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
                        Messages.Baseline4R1.PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition
                    ).ToByteArray(),
                FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R2 =>
                    new Messages.Baseline4R2.PointPointPositionMessage(
                        _config.LocalId,
                        _config.RemoteId,
                        Messages.Baseline4R2.PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
                        Messages.Baseline4R2.PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition
                    ).ToByteArray(),
                _ => throw new NotImplementedException()
            };
            messageBytes[43] = 0x07; //undefined value
            messageBytes[44] = 0x07; //undefined value

            _logger.LogInformation("Sending SCI message: {}", BitConverter.ToString(messageBytes).Replace("-", " 0x"));
            await Connection.SendSciMessage(messageBytes);
        }

        /// <summary>
        /// Send a SCI message with content a Formal Telegram Error.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task SendSciMessageFormalError(Empty request)
        {
            if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            byte[] messageBytes = ConnectionProtocol switch
            {
                FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R1 =>
                    new Messages.Baseline4R1.PointPointPositionMessage(
                        _config.LocalId,
                        _config.RemoteId,
                        Messages.Baseline4R1.PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
                        Messages.Baseline4R1.PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition
                    ).ToByteArray(),
                FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R2 =>
                    new Messages.Baseline4R2.PointPointPositionMessage(
                        _config.LocalId,
                        _config.RemoteId,
                        Messages.Baseline4R2.PointPointPositionMessageReportedPointPosition.PointIsInNoEndPosition,
                        Messages.Baseline4R2.PointPointPositionMessageReportedDegradedPointPosition.PointIsInADegradedRightHandPosition
                    ).ToByteArray(),
                _ => throw new NotImplementedException()
            };
            messageBytes = messageBytes.Take(43).ToArray(); //remove last byte
            _logger.LogInformation("Sending SCI message: {}", BitConverter.ToString(messageBytes).Replace("-", " 0x"));
            await Connection.SendSciMessage(messageBytes);
        }

        /// <summary>
        /// Sets the sets the initialization timeout flag for the next initialization.
        /// </summary>
        public void EnableInitializationTimeout(bool enableInitializationTimeout)
        {
            if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            _logger.LogInformation("Timeout on next initialization handshake enabled: {}.", enableInitializationTimeout ? "yes" : "no");
            _simulatedPointState = _simulatedPointState with
            {
                SimulateInitializationTimeout = enableInitializationTimeout
            };
        }

        /// <summary>
        /// Sets the ability to move for the next command.
        /// </summary>
        public async Task SetAbilityToMove(AbilityToMoveMessage abilityToMoveMessage)
        {
            if (_config.ObserveAbilityToMove == false)
            {
                throw new InvalidOperationException("Ability to move is not observed, cannot set ability to move.");
            }

            _logger.LogInformation("Setting ability to move to {}.", abilityToMoveMessage.Ability);
            PointState = PointState with
            {
                AbilityToMove = abilityToMoveMessage.Ability switch
                {
                    AbilityToMove.AbleToMove => GenericAbilityToMove.AbleToMove,
                    AbilityToMove.UnableToMove => GenericAbilityToMove.UnableToMove,
                    _ => throw new InvalidOperationException($"Unknown ability to move {abilityToMoveMessage.Ability}.")
                }
            };

            if (_initialized)
            {
                if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
                await Connection.SendAbilityToMove(PointState);
            }
        }

        /// <summary>
        /// Puts the point into an unintended position immediately.
        /// </summary>
        public async Task PutIntoUnintendedPosition(DegradedPositionMessage simulatedPositionMessage)
        {
            var degradedPosition = HandleSimulatedDegradedPosition(simulatedPositionMessage);
            _logger.LogInformation("Transitioning point into unintended / trailed position.");
            SetPointState(GenericPointPosition.UnintendedPosition, degradedPosition);

            if (_initialized)
            {
                if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
                await Connection.SendPointPosition(PointState);
            }
        }

        /// <summary>
        /// Puts the point into a no end position status immediately.
        /// </summary>
        public async Task PutIntoNoEndPosition(DegradedPositionMessage simulatedPositionMessage)
        {
            var degradedPosition = HandleSimulatedDegradedPosition(simulatedPositionMessage);
            _logger.LogInformation("Transitioning point into no end position.");
            SetPointState(GenericPointPosition.NoEndPosition, degradedPosition);

            if (_initialized)
            {
                if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
                await Connection.SendPointPosition(PointState);
            }
        }

        /// <summary>
        /// Evaluates the simulated degraded position using the current point state and the correct point degraded position.
        /// Will throw an exception if the simulated degraded position is not compatible with the point state.
        /// </summary>
        /// <param name="simulatedPositionMessage"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private GenericDegradedPointPosition HandleSimulatedDegradedPosition(DegradedPositionMessage simulatedPositionMessage)
        {
            if (AllPointMachinesCrucial && simulatedPositionMessage.DegradedPosition)
            {
                throw new InvalidOperationException("All point machines are crucial, cannot set degraded position.");
            }

            if (PointState.PointPosition != GenericPointPosition.Left && PointState.PointPosition != GenericPointPosition.Right)
            {
                throw new InvalidOperationException($"Point is not in an end position, cannot put into unintended position.");
            }

            var notDegradedPosition = AllPointMachinesCrucial ? GenericDegradedPointPosition.NotApplicable : GenericDegradedPointPosition.NotDegraded;
            var degradedPosition = PointState.PointPosition == GenericPointPosition.Left ? GenericDegradedPointPosition.DegradedLeft : GenericDegradedPointPosition.DegradedRight;

            return simulatedPositionMessage.DegradedPosition ? degradedPosition : notDegradedPosition;
        }

        /// <summary>
        /// Stores the prevented position and degraded point position for the next move left command.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void PreventLeftEndPosition(PreventedPositionMessage request)
        {
            if (request.DegradedPosition && AllPointMachinesCrucial)
            {
                throw new InvalidOperationException("All point machines are crucial, cannot set degraded position.");
            }

            _simulatedPointState = _simulatedPointState with
            {
                PreventedPositionLeft = request.Position,
                DegradedPositionLeft = request.DegradedPosition,
            };
        }

        /// <summary>
        /// Stores the prevented position and degraded point position for the next move right command.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public void PreventRightEndPosition(PreventedPositionMessage request)
        {
            if (request.DegradedPosition && AllPointMachinesCrucial)
            {
                throw new InvalidOperationException("All point machines are crucial, cannot set degraded position.");
            }

            _simulatedPointState = _simulatedPointState with
            {
                PreventedPositionRight = request.Position,
                DegradedPositionRight = request.DegradedPosition,
            };
        }

        private void SetPointState(GenericPointPosition pointPosition, GenericDegradedPointPosition degradedPointPosition)
        {
            PointState = PointState with
            {
                PointPosition = pointPosition,
                DegradedPointPosition = degradedPointPosition,
            };
        }

        /// <summary>
        /// Checks if AllPointMachinesCrucial is set and returns NotApplicable if it is.
        /// </summary>
        /// <param name="degradedPointPosition"></param>
        /// <returns></returns>
        private GenericDegradedPointPosition RespectAllPointMachinesCrucial(GenericDegradedPointPosition degradedPointPosition)
        {
            if (AllPointMachinesCrucial)
            {
                if (degradedPointPosition != GenericDegradedPointPosition.NotDegraded && degradedPointPosition != GenericDegradedPointPosition.NotApplicable)
                    throw new InvalidOperationException($"All point machines are crucial, ignoring degraded point position {degradedPointPosition}.");
                return GenericDegradedPointPosition.NotApplicable;
            }
            else
            {
                return degradedPointPosition;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Log version information
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            _logger.LogInformation("Simulator software version: {}", version ?? "unknown");

            // Main loop.
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Connecting...");
                var conn = _connectionProvider.Connect(_config, stoppingToken);
                using (Connection = _connectionBuilder.Connect(conn))
                {
                    _resetTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

                    try
                    {
                        var success = await Connection.InitializeConnection(PointState, _config.ObserveAbilityToMove, _simulatedPointState.SimulateInitializationTimeout, _resetTokenSource.Token);
                        if (!success)
                        {
                            throw new Exception("Unable to initialize connection");
                        }
                        _initialized = true;

                        await AllMovePointCommands(_resetTokenSource.Token)
                            .ToObservable()
                            .Select(x => Observable.FromAsync(token => HandleCommandedPointPosition(x, token)))
                            // This will abort the previous simulated point movement if a new command is received.
                            .Switch();
                    }
                    catch (ConnectionException)
                    {
                        _logger.LogWarning("Could not communicate with remote endpoint.");
                        Reset();
                        await Task.Delay(1000, stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        Reset();
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Exception during simulation.");
                        Reset();
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
        }

        private async IAsyncEnumerable<GenericPointPosition> AllMovePointCommands([EnumeratorCancellation] CancellationToken stoppingToken)
        {
            if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            while (!stoppingToken.IsCancellationRequested)
            {
                var commandedPointPosition = await Connection.ReceiveMovePointCommand(stoppingToken);
                yield return commandedPointPosition;
            }
        }

        private async Task HandleCommandedPointPosition(GenericPointPosition commandedPointPosition, CancellationToken cancellationToken)
        {
            // Make a copy of the current state, so that it is not modified while the point is moving.
            var simulatedState = _simulatedPointState;

            PointState = PointState with { LastCommandedPointPosition = commandedPointPosition };

            if (_config.ObserveAbilityToMove && PointState.AbilityToMove == GenericAbilityToMove.UnableToMove)
            {
                _logger.LogInformation("Point is unable to move.");
                // Do not send a message, according to Eu.P.5371
                return;
            }

            if ((commandedPointPosition == GenericPointPosition.Left && PointState.PointPosition == GenericPointPosition.Left)
                || (commandedPointPosition == GenericPointPosition.Right && PointState.PointPosition == GenericPointPosition.Right))
            {
                _logger.LogInformation("Point is already in position {}.", commandedPointPosition);
                // Do not send a message, according to Eu.P.1469
                return;
            }

            if (Connection == null) throw new InvalidOperationException("Connection is null. Did you call Connect()?");
            _logger.LogDebug("Moving to {}.", commandedPointPosition);

            var notDegradedPosition = AllPointMachinesCrucial ? GenericDegradedPointPosition.NotApplicable : GenericDegradedPointPosition.NotDegraded;

            if (PointState.PointPosition != GenericPointPosition.NoEndPosition || PointState.DegradedPointPosition != notDegradedPosition)
            {
                SetPointState(GenericPointPosition.NoEndPosition, notDegradedPosition);
                await Connection.SendPointPosition(PointState);
            }

            try
            {
                // Simulate point movement
                await Task.Delay((int)(_config.SimulatedTransitioningTimeSeconds * 1000), cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (commandedPointPosition == GenericPointPosition.Left && simulatedState.PreventedPositionLeft != PreventedPosition.DoNotPrevent)
            {
                await Connection.SendTimeoutMessage();

                var (pointPosition, degradedPointPosition) = HandlePreventedPointPosition(commandedPointPosition, simulatedState);
                var sendPointPosition = pointPosition != PointState.PointPosition || degradedPointPosition != PointState.DegradedPointPosition;

                SetPointState(pointPosition, degradedPointPosition);

                if (sendPointPosition)
                    await Connection.SendPointPosition(PointState);
                _logger.LogInformation("Prevented left end position: {} {}.", PointState.PointPosition, PointState.DegradedPointPosition);
            }
            else if (commandedPointPosition == GenericPointPosition.Right && simulatedState.PreventedPositionRight != PreventedPosition.DoNotPrevent)
            {
                await Connection.SendTimeoutMessage();

                var (pointPosition, degradedPointPosition) = HandlePreventedPointPosition(commandedPointPosition, simulatedState);
                var sendPointPosition = pointPosition != PointState.PointPosition || degradedPointPosition != PointState.DegradedPointPosition;

                SetPointState(pointPosition, degradedPointPosition);

                if (sendPointPosition)
                    await Connection.SendPointPosition(PointState);
                _logger.LogInformation("Prevented right end position: {} {}.", PointState.PointPosition, PointState.DegradedPointPosition);
            }
            else
            {
                SetPointState(commandedPointPosition, notDegradedPosition);

                _logger.LogInformation("End position reached.");
                await Connection.SendPointPosition(PointState);
            }
        }

        /// <summary>
        /// Sets the pointPosition and degradedPointPosition based on the commanded position and the currently prevented position.
        /// </summary>
        /// <param name="commandedPosition"></param>
        /// <param name="pointPosition"></param>
        /// <param name="degradedPointPosition"></param>
        private (GenericPointPosition PointPosition, GenericDegradedPointPosition DegradedPointPosition) HandlePreventedPointPosition(GenericPointPosition commandedPosition, SimulatedPointState simulatedState)
        {
            var notDegradedPosition = AllPointMachinesCrucial ? GenericDegradedPointPosition.NotApplicable : GenericDegradedPointPosition.NotDegraded;

            if (commandedPosition == GenericPointPosition.Left)
            {
                if (simulatedState.PreventedPositionLeft == PreventedPosition.DoNotPrevent)
                {
                    return (commandedPosition, notDegradedPosition);
                }
                else
                {
                    if (simulatedState.PreventedPositionLeft == PreventedPosition.SetUnintendedOrTrailed)
                    {
                        return (GenericPointPosition.UnintendedPosition, simulatedState.DegradedPositionLeft ? GenericDegradedPointPosition.DegradedLeft : notDegradedPosition);
                    }
                    else if (simulatedState.PreventedPositionLeft == PreventedPosition.SetNoEndPosition)
                    {
                        return (GenericPointPosition.NoEndPosition, simulatedState.DegradedPositionLeft ? GenericDegradedPointPosition.DegradedLeft : notDegradedPosition);
                    }
                }
            }
            else if (commandedPosition == GenericPointPosition.Right)
            {
                if (simulatedState.PreventedPositionRight == PreventedPosition.DoNotPrevent)
                {
                    return (commandedPosition, notDegradedPosition);
                }
                else
                {
                    if (simulatedState.PreventedPositionRight == PreventedPosition.SetUnintendedOrTrailed)
                    {
                        return (GenericPointPosition.UnintendedPosition, simulatedState.DegradedPositionRight ? GenericDegradedPointPosition.DegradedRight : notDegradedPosition);
                    }
                    else if (simulatedState.PreventedPositionRight == PreventedPosition.SetNoEndPosition)
                    {
                        return (GenericPointPosition.NoEndPosition, simulatedState.DegradedPositionRight ? GenericDegradedPointPosition.DegradedRight : notDegradedPosition);
                    }
                }
            }

            throw new ArgumentException("Invalid commanded position", nameof(commandedPosition));
        }

        public void Reset()
        {
            _logger.LogInformation("Resetting point.");
            _resetTokenSource.Cancel();
            _initialized = false;
        }
    }
}
