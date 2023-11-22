using System.Diagnostics;
using EulynxLive.Point.Proto;
using EulynxLive.Messages.IPointToInterlockingConnection;
using PointPosition = EulynxLive.Messages.IPointToInterlockingConnection.IPointToInterlockingConnection.PointPosition;

class PointToInterlockingConnectionMockImpl : IPointToInterlockingConnection
{
    private readonly ILogger _logger;
    int _timeoutS = 3;
    Stopwatch _sw = new Stopwatch();
    PointPosition commandedPosition = PointPosition.RIGHT;

    public PointToInterlockingConnectionMockImpl(ILogger logger)
    {
        _logger = logger;
    }

    public void Connect() { }

    public void Dispose() { }

    async public Task<bool> InitializeConnection(IPointToInterlockingConnection.PointState state)
    {
        _sw = Stopwatch.StartNew();
        await Task.Delay(1000);
        return true;
    }

    async public Task<PointPosition?> ReceivePointPosition()
    {
        await Task.Delay(1000);
        if (_sw.Elapsed.TotalSeconds >= _timeoutS)
        {
            _sw = Stopwatch.StartNew();
            commandedPosition = commandedPosition switch
            {
                PointPosition.RIGHT => PointPosition.LEFT,
                PointPosition.LEFT => PointPosition.RIGHT,
                PointPosition.NO_ENDPOSITION => throw new NotImplementedException(),
                PointPosition.TRAILED => throw new NotImplementedException(),
            };
            return commandedPosition;
        }
        return null;
    }

    async public Task SendPointPosition(IPointToInterlockingConnection.PointState state)
    {
        await Task.Delay(1000);
    }

    async public Task SendTimeoutMessage()
    {
        await Task.Delay(1000);
    }
}
