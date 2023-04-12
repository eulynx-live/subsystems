using System;
using System.Collections.Generic;

namespace EulynxLive.Point.Components
{
    public class PointMachine
    {

        public PointMachineState state { get; private set; }
        public event EventHandler<PointMachineState> SubjectEvent;

        private List<IObserver<PointMachineState>> _observers = new List<IObserver<PointMachineState>>();

        public PointMachine(PointMachineState state)
        {
            this.state = state;

        }

        public void NotifyObservers()
        {
            if (SubjectEvent != null)
            {
                SubjectEvent(this, state);
            }
        }
    }
}