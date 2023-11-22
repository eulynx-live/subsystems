using System.Threading.Tasks;

public interface IPointToInterlockingConnection: System.IDisposable {
    void SendPointPosition(PointState state);
    void SendTimeoutMessage();
    void Reset();
    void Setup();
    void Connect();
    void InitializeConnection();
    public Task<PointPosition?> ReceivePointPosition();

    public record PointState
    {
        public PointPosition PointPosition { get; set; }
        public DegradedPointPosition DegradedPointPosition { get; set; }
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
