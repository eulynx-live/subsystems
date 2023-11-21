using System.Threading.Tasks;

public interface IPointToInterlockingConnection {
    void SendPointPosition(IPointState state);
    void SendTimeoutMessage();
    void Reset();
    void Setup();
    void Connect();
    void InitializeConnection();
    public Task<PointPosition?> ReceivePointPosition();

    public interface IMessage
    {
        public IMessage FromBytes(byte[] message);
        public abstract byte[] ToByteArray();
    }

    public interface IPointState
    {
        PointPosition PointPosition { get; set; }
        DegradedPointPosition DegradedPointPosition { get; set; }
    }

    public enum PointPosition
    {
        LEFT,
        RIGHT,
        TRAILED,
        NO_ENDPOSITION
    }

    public enum DegradedPointPosition
    {
        DEGRADED_LEFT,
        DEGRADED_RIGHT,
        NOT_DEGRADED,
        NOT_APPLICABLE
    }
}
