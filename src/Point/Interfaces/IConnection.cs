namespace EulynxLive.Point.Interfaces
{
    interface IConnection : IDisposable
    {
        Task<byte[]> ReceiveAsync(CancellationToken cancellationToken);
        Task SendAsync(byte[] bytes);
    }
}
