namespace EulynxLive.Messages
{
    public enum ProtocolType: byte {
        // From: EU.SCI-XX.PDI.53
        AdjacentInterlockingSystem = 0x01,
        TrainDetectionSystem = 0x20,
        LightSignal = 0x30,
        Point = 0x40,
        RadioBlockCenter = 0x50,
        SubsystemLevelCrossing = 0x70,
        TrackworkerSafetySystem = 0x80,
        GenericIO = 0x90,
        ExternalLevelCrossingSystem = 0xC0,
    }
}
