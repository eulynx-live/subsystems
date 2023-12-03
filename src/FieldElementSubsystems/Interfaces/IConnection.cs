namespace EulynxLive.FieldElementSubsystems.Interfaces
{
    public interface IConnection : IDisposable
    {
        Task<byte[]> ReceiveAsync(CancellationToken cancellationToken);
        Task SendAsync(byte[] bytes);
    }
}
