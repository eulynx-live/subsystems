

namespace EulynxLive.Point.Components
{
    public class PointMachine
    {
        public PointMachineState state { get; private set; }

        public PointMachine(PointMachineState state)
        {
            this.state = state;
        }
    }
}