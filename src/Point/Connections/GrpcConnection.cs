using EulynxLive.Point.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Sci;
using static Sci.Rasta;

namespace EulynxLive.Point;

class GrpcConnection : IConnection
{
    class GrpcConnectionException : Exception
    {
        public GrpcConnectionException(string message) : base(message) { }
    }

    AsyncDuplexStreamingCall<SciPacket, SciPacket>? _connection;
    private CancellationToken _stoppingToken;

    public GrpcConnection(Metadata? metadata, string remoteEndpoint, CancellationToken stoppingToken)
    {
        _stoppingToken = stoppingToken;

        var channel = GrpcChannel.ForAddress(remoteEndpoint);
        var client = new RastaClient(channel);
        _connection = client.Stream(metadata, cancellationToken: stoppingToken);
    }
    public void Dispose()
    {
        _connection?.Dispose();
    }

    public async Task<byte[]> ReceiveAsync(CancellationToken cancellationToken)
    {
        if (_connection == null) throw new InvalidOperationException("Grpc connection not connected.");
        if (!await _connection.ResponseStream.MoveNext(cancellationToken)) throw new GrpcConnectionException("Could not receive grpc message.");
        return _connection.ResponseStream.Current.Message.ToByteArray();
    }

    public async Task SendAsync(byte[] bytes)
    {
        if (_connection == null) throw new InvalidOperationException("Grpc connection not connected.");
        await _connection.RequestStream.WriteAsync(new SciPacket() { Message = ByteString.CopyFrom(bytes) });
    }
}
