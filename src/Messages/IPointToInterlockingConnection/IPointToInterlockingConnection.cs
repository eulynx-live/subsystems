using System.Threading.Tasks;

namespace EulynxLive.Messages.IPointToInterlockingConnection;

public interface IPointToInterlockingConnection: System.IDisposable {
    Task SendPointPosition(PointState state);
    Task SendTimeoutMessage();
    void Connect();
    Task<bool> InitializeConnection(PointState state);
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
