
using InterlockingSystemStateMessage = EulynxLive.InterlockingSystem.Proto.InterlockingSystemStateMessage.Types;


namespace EulynxLive.InterlockingSystem.Components
{
    public class InterlockingSystemState
    {
        public  InterlockingSystemStateMessage.LineStatus LineStatus  { get; set; }
        public  InterlockingSystemStateMessage.LineDirectionInformation LineDirectionInformation  { get; set; }
        public  InterlockingSystemStateMessage.LineDirectionStatus LineDirectionStatus  { get; set; }
    }
}