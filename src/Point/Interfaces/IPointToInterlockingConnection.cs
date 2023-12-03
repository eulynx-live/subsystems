namespace EulynxLive.Point.Interfaces;

public interface IPointToInterlockingConnection: IDisposable {
    Task SendPointPosition(PointState state);
    Task SendTimeoutMessage();
    void Connect();
    Task<bool> InitializeConnection(PointState state, CancellationToken cancellationToken);
    public Task<PointPosition?> ReceivePointPosition(CancellationToken stoppingToken);

    public record PointState
    {
        public PointPosition PointPosition { get; set; }
        public DegradedPointPosition DegradedPointPosition { get; set; }
    }

    public enum PointPosition
    {
        Left,
        Right,
        UnintendedPosition,
        NoEndPosition
    }

    public enum DegradedPointPosition
    {
        DegradedLeft,
        DegradedRight,
        NotDegraded,
        NotApplicable
    }
}
