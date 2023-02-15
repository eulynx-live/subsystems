namespace EulynxLive.Messages.NeuPro
{
    public enum ExternalLevelCrossingSystemMessageType: ushort {
        TrActivationCommandUE = 0x0050,
    }

    public abstract class ExternalLevelCrossingSystemMessage : EulynxMessage
    {
        public ExternalLevelCrossingSystemMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {}

        public override ProtocolType ProtocolType => ProtocolType.ExternalLevelCrossingSystem;
        public override ushort MessageTypeRaw => (ushort) MessageType;
        public abstract ExternalLevelCrossingSystemMessageType MessageType { get; }
    }

    public class ExternalLevelCrossingSystemTrActivationCommandUE : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrActivationCommandUE(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrActivationCommandUE;

        public override int Size => 47;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrActivationCommandUE(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[43] = 0;
            bytes[44] = 0;
            bytes[45] = 0;
            bytes[46] = 0;
        }
    }
}
