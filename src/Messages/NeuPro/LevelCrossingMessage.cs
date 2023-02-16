namespace EulynxLive.Messages.NeuPro
{
    public enum LevelCrossingMessageType: ushort {
        AnFsüCommand = 0x0055,
        AusFsüCommand = 0x0057,
        MeldungZustandGleisbezogenMessage = 0x0060
    }

    public abstract class LevelCrossingMessage : NeuProMessage
    {
        public LevelCrossingMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {}

        public override ProtocolType ProtocolType => ProtocolType.LevelCrossing;
        public override ushort MessageTypeRaw => (ushort) MessageType;
        public abstract LevelCrossingMessageType MessageType { get; }
    }

    public class AnFsüCommand : LevelCrossingMessage
    {
        private const int PLANUNGSPARAMETER_J_OFFSET = 43;
        private const int EP_OFFSET = 44;

        public AnFsüCommand(string senderId, string receiverId, byte planungsparameterJ, byte ep) : base(senderId, receiverId)
        {
            PlanungsparameterJ = planungsparameterJ;
            EP = ep;
        }

        public byte PlanungsparameterJ { get; }
        public byte EP { get; }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.AnFsüCommand;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var planungsparameterJ = message[PLANUNGSPARAMETER_J_OFFSET];
            var ep = message[EP_OFFSET];
            return new AnFsüCommand(senderId, receiverId, planungsparameterJ, ep);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
            bytes[PLANUNGSPARAMETER_J_OFFSET] = PlanungsparameterJ;
            bytes[EP_OFFSET] = EP;
            bytes[45] = 0;
            bytes[46] = 0;
        }
    }

    public class AusFsüCommand : LevelCrossingMessage
    {
        private const int PLANUNGSPARAMETER_J_OFFSET = 43;
        public AusFsüCommand(string senderId, string receiverId, byte planungsparameterJ) : base(senderId, receiverId)
        {
            PlanungsparameterJ = planungsparameterJ;
        }

        public byte PlanungsparameterJ { get; }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.AusFsüCommand;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var planungsparameterJ = message[PLANUNGSPARAMETER_J_OFFSET];
            return new AusFsüCommand(senderId, receiverId, planungsparameterJ);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
            bytes[PLANUNGSPARAMETER_J_OFFSET] = PlanungsparameterJ;
            bytes[44] = 0;
            bytes[45] = 0;
            bytes[46] = 0;
        }
    }

    public class MeldungZustandGleisbezogenMessage : LevelCrossingMessage
    {
        private const int GLEISBEZOGENE_STÖRUNGSZUSTÄNDE_OFFSET = 50;
        public MeldungZustandGleisbezogenMessage(string senderId, string receiverId, byte gleisbezogeneStörungszustände) : base(senderId, receiverId)
        {
            GleisbezogeneStörungszustände = gleisbezogeneStörungszustände;
        }

        public byte GleisbezogeneStörungszustände { get; }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.MeldungZustandGleisbezogenMessage;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var gleisbezogeneStörungszustände = message[GLEISBEZOGENE_STÖRUNGSZUSTÄNDE_OFFSET];
            return new MeldungZustandGleisbezogenMessage(senderId, receiverId, gleisbezogeneStörungszustände);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
            bytes[GLEISBEZOGENE_STÖRUNGSZUSTÄNDE_OFFSET] = GleisbezogeneStörungszustände;
        }
    }
}
