using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sci;
using EulynxLive.Messages.Baseline4R1;
using static Sci.Rasta;
using System.Text;
using Grpc.Net.Client;
using System;
using System.IO;
using System.Net.Sockets;
using EulynxLive.InterlockingSystem.Components;

namespace EulynxLive.InterlockingSystem
{
    // This interlocking is assumed to be primary. Perhaps the implementation of the option choice primary/secondary should be considered.
    public class InterlockingSystem : BackgroundService
    {
        private readonly ILogger<InterlockingSystem> _logger;
        private readonly IConfiguration _configuration;
        private readonly List<WebSocket> _webSockets;
        private string _localId;
        private string _localRastaId;
        private string _remoteId;
        private string _remoteEndpoint;
        private string[] _boundaryId;
        private bool _initialized;
        AsyncDuplexStreamingCall<SciPacket, SciPacket> _currentConnection;
        
        private Dictionary<string, AdjacentInterlockingSystemLineStatusMessageLineStatus> _lineStatus;
        private Dictionary<string, AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus> _lineDirectionStatus;
        private Dictionary<string, AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation> _lineDirectionInformation;


        public InterlockingSystemState interlockingSystemState { get; private set;  }

        public InterlockingSystem(ILogger<InterlockingSystem> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _webSockets = new List<WebSocket>();
            _currentConnection = null;

            _lineStatus = null;
            _lineDirectionStatus = null;
            _lineDirectionInformation = null;
        }

        public async Task HandleWebSocket(WebSocket webSocket)
        {
            _webSockets.Add(webSocket);
            try
            {
                await UpdateWebClient(webSocket);

                while (true)
                {
                    byte[] messageBuffer = new byte[1024];
                    ArraySegment<byte> buffer = new ArraySegment<byte>(messageBuffer);
                    var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        break;
                    }
                }
            }
            catch (WebSocketException)
            {
                // Do nothing, the WebSocket has died.
            }
            _webSockets.Remove(webSocket);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Command line argument parsing.
            _localId = _configuration["local-id"];
            if (_localId == null)
            {
                throw new Exception("Missing --local-id command line parameter.");
            }

            _localRastaId = _configuration["local-rasta-id"];
            if (_localRastaId == null)
            {
                throw new Exception("Missing --local-rasta-id command line parameter.");
            }

            _remoteId = _configuration["remote-id"];
            if (_remoteId == null)
            {
                throw new Exception("Missing --remote-id command line parameter.");
            }

            _remoteEndpoint = _configuration["remote-endpoint"];
            if (_remoteEndpoint == null)
            {
                throw new Exception("Missing --remote-endpoint command line parameter.");
            }
            
            var boundaryId = _configuration["boundary-id"];
            if (boundaryId == null)
            {
                throw new Exception("Missing --boundary-id command line parameter.");
            }
            
            _boundaryId = boundaryId.Split(',').Select(s => s.PadRight(20, '_')).ToArray();

            
            _lineStatus = _boundaryId.Select(x => new { Key = x, Value = AdjacentInterlockingSystemLineStatusMessageLineStatus.RequestForLineBlockReset}).ToDictionary(x => x.Key, x => x.Value);
            _lineDirectionStatus = _boundaryId.Select(x => new { Key = x, Value = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus.Released}).ToDictionary(x => x.Key, x => x.Value);
            _lineDirectionInformation = _boundaryId.Select(x => new { Key = x, Value = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.NoDirection}).ToDictionary(x => x.Key, x => x.Value);
            
            while (true)
            {
                await Reset();
                try
                {
                    var channel = GrpcChannel.ForAddress(_remoteEndpoint);
                    var client = new RastaClient(channel);
                    _logger.LogTrace("Connecting...");
                    var cancellationTokenSource = new CancellationTokenSource();
                    cancellationTokenSource.CancelAfter(10000);
                    var metadata = new Metadata { { "rasta-id", _localRastaId } };

                    using (_currentConnection = client.Stream(metadata))
                    {
                        _logger.LogTrace("Connected. Waiting for request...");
                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is AdjacentInterlockingSystemPdiVersionCheckCommand)) 
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }
                        
                        var versionCheckResponse = new AdjacentInterlockingSystemPdiVersionCheckMessage(_localId, _remoteId, AdjacentInterlockingSystemPdiVersionCheckMessageResultPdiVersionCheck.PDIVersionsFromReceiverAndSenderDoMatch, /* TODO */ 0x01, 0, new byte[] { });
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(versionCheckResponse.ToByteArray()) });

                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is AdjacentInterlockingSystemInitialisationRequestCommand))
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }
                        
                        var startInitialization = new AdjacentInterlockingSystemStartInitialisationMessage(_localId, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() {Message = ByteString.CopyFrom(startInitialization.ToByteArray())});

                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is AdjacentInterlockingSystemLineDirectionControlMessage initialLineDirectionControlMessage))
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }
                        
                        if (_boundaryId.Contains(initialLineDirectionControlMessage.BoundaryId))
                        {
                            _lineDirectionStatus[initialLineDirectionControlMessage.BoundaryId] = initialLineDirectionControlMessage.LineDirectionStatus;
                            //invert report reported direction to set own direction
                            _lineDirectionInformation[initialLineDirectionControlMessage.BoundaryId] = initialLineDirectionControlMessage.LineDirectionControlInformation.Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry)?
                                    AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Exit:AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry;
                        }
                        else
                        {
                            _logger.LogError("Unexpected boundary id");
                            break;
                        }
                        
                        if (!await _currentConnection.ResponseStream.MoveNext(cancellationTokenSource.Token)
                            || !(Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is AdjacentInterlockingSystemLineStatusMessage initialLineStatusMessage))
                        {
                            _logger.LogError("Unexpected message.");
                            break;
                        }
                        
                        if (_boundaryId.Contains(initialLineStatusMessage.BoundaryId))
                        {
                            _lineStatus[initialLineStatusMessage.BoundaryId] = initialLineStatusMessage.LineStatus;
                        }
                        else
                        {
                            _logger.LogError("Unexpected boundary id");
                            break;
                        }

                        _initialized = true;

                        var completeInitialization = new AdjacentInterlockingSystemInitialisationCompletedMessage(_localId, _remoteId);
                        await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(completeInitialization.ToByteArray()) });

                        await UpdateConnectedWebClients();

                        while (true)
                        {
                            if (!await _currentConnection.ResponseStream.MoveNext())
                            {
                                break;
                            }
                            var message = Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray());
                            
                            if (message is AdjacentInterlockingSystemLineStatusMessage lineStatusMessage)
                            {
                                _lineStatus[lineStatusMessage.BoundaryId] = lineStatusMessage.LineStatus;
                            }
                            else if (message is AdjacentInterlockingSystemLineDirectionControlMessage lineDirectionControlMessage)
                            {
                                if (lineDirectionControlMessage.LineDirectionControlInformation.Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.DirectionHandover) &&
                                    _lineDirectionStatus[lineDirectionControlMessage.BoundaryId].Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus.Released) &&
                                    !_lineStatus[lineDirectionControlMessage.BoundaryId].Equals(AdjacentInterlockingSystemLineStatusMessageLineStatus.Occupied))
                                {
                                    var newLineDirection = _lineDirectionInformation[lineDirectionControlMessage.BoundaryId].Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry) ? 
                                        AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Exit : AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry;
                                    _lineDirectionInformation[lineDirectionControlMessage.BoundaryId] = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.NoDirection;
                                    var handoverLineDirection = new AdjacentInterlockingSystemLineDirectionControlMessage(_localId, _remoteId, lineDirectionControlMessage.BoundaryId,
                                        newLineDirection,
                                        _lineDirectionStatus[lineDirectionControlMessage.BoundaryId]);
                                    await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(handoverLineDirection.ToByteArray()) });

                                    if (!await _currentConnection.ResponseStream.MoveNext()
                                        || !(Message.FromBytes(_currentConnection.ResponseStream.Current.Message.ToByteArray()) is AdjacentInterlockingSystemLineDirectionControlMessage lineDirectionControlMessage2))
                                    {
                                        _logger.LogError($"Unexpected message {message.GetType().ToString()}");
                                        break;
                                    }

                                    if (!lineDirectionControlMessage2.LineDirectionControlInformation.Equals(newLineDirection.Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry)?
                                            AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Exit:AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry))
                                    {
                                        _lineDirectionInformation[lineDirectionControlMessage.BoundaryId] = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.DirectionHandoverAborted;
                                        _logger.LogError("Wrong Direction answer");
                                        break;
                                    }

                                    _lineDirectionInformation[lineDirectionControlMessage.BoundaryId] = newLineDirection;
                                }
                                else if (!lineDirectionControlMessage.LineDirectionControlInformation.Equals(_lineDirectionInformation[lineDirectionControlMessage.BoundaryId]))
                                {
                                    if (!lineDirectionControlMessage.LineDirectionControlInformation.Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry) &&
                                        !lineDirectionControlMessage.LineDirectionControlInformation.Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Exit))
                                    {
                                        _lineDirectionInformation[lineDirectionControlMessage.BoundaryId] = lineDirectionControlMessage.LineDirectionControlInformation;
                                    }
                                    _lineDirectionStatus[lineDirectionControlMessage.BoundaryId] = lineDirectionControlMessage.LineDirectionStatus;
                                }
                                else
                                {
                                    _lineDirectionInformation[lineDirectionControlMessage.BoundaryId] = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.DirectionHandoverAborted;
                                    _logger.LogError("No change of direction permitted.");
                                    break;
                                }
                            }
                            else
                            {
                                 _logger.LogInformation($"Received unknown message {message.GetType().ToString()}");
                            }
                            
                            await UpdateConnectedWebClients();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Exception during simulation. Resetting.");
                    await Reset();
                    await Task.Delay(1000);
                }
            }
        }

        private async Task Reset()
        {
            _initialized = false;
            await UpdateConnectedWebClients();
        }

        public async Task SetBlock(string boundaryId)
        {
            boundaryId = boundaryId.PadRight(20, '_');
            if (_lineDirectionInformation[boundaryId].Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Entry) &&
                _lineStatus[boundaryId].Equals(AdjacentInterlockingSystemLineStatusMessageLineStatus.Vacant))
            {
                _lineStatus[boundaryId] = AdjacentInterlockingSystemLineStatusMessageLineStatus.Occupied;
                _lineDirectionStatus[boundaryId] = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus.Locked;
                var blockLineDirection = new AdjacentInterlockingSystemLineDirectionControlMessage(_localId, _remoteId, boundaryId, _lineDirectionInformation[boundaryId], _lineDirectionStatus[boundaryId]);
                await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(blockLineDirection.ToByteArray()) });
            }
            else
            {
                _logger.LogError("Could not block line, because direction is set to exit, or line was not vacant.");
            }
            await UpdateConnectedWebClients();
        }
        
        public async Task UnsetBlock(string boundaryId)
        {
            boundaryId = boundaryId.PadRight(20, '_');
            if (_lineDirectionInformation[boundaryId].Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.Exit) &&
                _lineStatus[boundaryId].Equals(AdjacentInterlockingSystemLineStatusMessageLineStatus.Occupied))
            {
                _lineStatus[boundaryId] = AdjacentInterlockingSystemLineStatusMessageLineStatus.Vacant;
                _lineDirectionStatus[boundaryId] = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus.Released;
                var blockLineDirection = new AdjacentInterlockingSystemLineDirectionControlMessage(_localId, _remoteId, boundaryId, _lineDirectionInformation[boundaryId], _lineDirectionStatus[boundaryId]);
                await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(blockLineDirection.ToByteArray()) });
            }
            else
            {
                _logger.LogError("Could not unblock line, because direction is set to entry or line was not occupied");
            }

            await UpdateConnectedWebClients();
        }
        
        public async Task InitiateDirectionHandover(string boundaryId)
        {
            boundaryId = boundaryId.PadRight(20, '_');
            if (!_lineStatus[boundaryId].Equals(AdjacentInterlockingSystemLineStatusMessageLineStatus.Occupied) &&
                _lineDirectionStatus[boundaryId].Equals(AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus.Released))
            {
                var initiateDirectionHandover = new AdjacentInterlockingSystemLineDirectionControlMessage(_localId, _remoteId, boundaryId, AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.DirectionRequest, _lineDirectionStatus[boundaryId]);
                await _currentConnection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(initiateDirectionHandover.ToByteArray()) });
            }
            else
            {
                _lineDirectionInformation[boundaryId] = AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation.DirectionHandoverAborted;
                _logger.LogError("No change of direction permitted.");
            }

            await UpdateConnectedWebClients();
        }
        
        
        
        private async Task UpdateConnectedWebClients()
        {
            try
            {
                var tasks = _webSockets.Select(UpdateWebClient);
                await Task.WhenAll(tasks);
            }
            catch (Exception)
            {
                // Some client likely has an issue, ignore
            }
        }

        private async Task UpdateWebClient(WebSocket webSocket)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var serializedState = JsonSerializer.Serialize(new
            {
                initialized = _initialized,
                lineStatus = _lineStatus.Select(x => new { Key = x.Key.Trim('_'), Value = x.Value.ToString() }).ToDictionary(x => x.Key, x => x.Value),
                lineDirectionInformation = _lineDirectionInformation.Select(x => new { Key = x.Key.Trim('_'), Value = x.Value.ToString() }).ToDictionary(x => x.Key, x => x.Value),
                lineDirectionStatus = _lineDirectionStatus.Select(x => new { Key = x.Key.Trim('_'), Value = x.Value.ToString() }).ToDictionary(x => x.Key, x => x.Value)
            }, options);
            var serializedStateBytes = Encoding.UTF8.GetBytes(serializedState);
            await webSocket.SendAsync(serializedStateBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
