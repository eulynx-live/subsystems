namespace EulynxLive.Messages.NeuPro
{
    public enum LevelCrossingMessageType: ushort {
        AnFsüCommand = 0x0055,
        AusFsüCommand = 0x0057,
        MeldungZustandGleisbezogenMessage = 0x0060,
        MeldungZustandBüBezogenMessage = 0x0040,

        // Generic
        InitializationRequest = 0x0011,
        StartInitialization = 0x0021,
        InitializationCompleted = 0x0022,
        VersionCheckCommand = 0x0010,
        VersionCheckMessage = 0x0020,
        // Not sure if these exist:
        PDIAvailable = 0x0029,
        PDINotAvailable = 0x002A,
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

    public class MeldungZustandBüBezogenMessage : LevelCrossingMessage
    {
        public MeldungZustandBüBezogenMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.MeldungZustandBüBezogenMessage;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            return new MeldungZustandBüBezogenMessage(senderId, receiverId);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
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

    public class LevelCrossingInitializationRequestMessage : LevelCrossingMessage
    {
        public LevelCrossingInitializationRequestMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.InitializationRequest;


        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new LevelCrossingInitializationRequestMessage(senderId, receiverId);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {

        }
    }

    public class LevelCrossingStartInitializationMessage : LevelCrossingMessage
    {
        public LevelCrossingStartInitializationMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.StartInitialization;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new LevelCrossingStartInitializationMessage(senderId, receiverId);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class LevelCrossingInitializationCompletedMessage : LevelCrossingMessage
    {
        public LevelCrossingInitializationCompletedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.InitializationCompleted;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new LevelCrossingInitializationCompletedMessage(senderId, receiverId);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class LevelCrossingVersionCheckCommand : LevelCrossingMessage
    {
        private const int SENDER_PDI_VERSION_OFFSET = 43;
        public LevelCrossingVersionCheckCommand(string senderId, string receiverId, byte senderPdiVersion) : base(senderId, receiverId)
        {
            SenderPdiVersion = senderPdiVersion;
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.VersionCheckCommand;

        public byte SenderPdiVersion { get; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new LevelCrossingVersionCheckCommand(senderId, receiverId, message[SENDER_PDI_VERSION_OFFSET]);

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
        }
    }

    public class LevelCrossingVersionCheckMessage : LevelCrossingMessage
    {
        private const int RESULT_PDI_VERSION_CHECK_OFFSET = 43;
        private const int SENDER_PDI_VERSION_OFFSET = 44;
        private const int CHECKSUM_LENGTH_OFFSET = 45;
        private const int CHECKSUM_DATA_OFFSET = 46;
        public LevelCrossingVersionCheckMessage(string senderId, string receiverId, PdiVersionCheckResult resultPdiVersionCheck, byte senderPdiVersion, byte checksumLength) : base(senderId, receiverId)
        {
            ResultPdiVersionCheck = resultPdiVersionCheck;
            SenderPdiVersion = senderPdiVersion;
            ChecksumLength = checksumLength;
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.VersionCheckMessage;

        public PdiVersionCheckResult ResultPdiVersionCheck { get; }
        public byte SenderPdiVersion { get; }
        public byte ChecksumLength { get; }

        // public override int Size => 46 + ChecksumLength;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var resultPdiVersionCheck = (PdiVersionCheckResult) message[RESULT_PDI_VERSION_CHECK_OFFSET];
            var senderPdiVersion = message[SENDER_PDI_VERSION_OFFSET];
            var checksumLength = message[CHECKSUM_LENGTH_OFFSET];
            // var CHECKSUM_DATA_OFFSET = message[CHECKSUM_DATA_OFFSET];
            return new LevelCrossingVersionCheckMessage(senderId, receiverId, resultPdiVersionCheck, senderPdiVersion, checksumLength);
        }

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
            bytes[RESULT_PDI_VERSION_CHECK_OFFSET] = (byte)ResultPdiVersionCheck;
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
            bytes[CHECKSUM_LENGTH_OFFSET] = ChecksumLength;
        }
    }

    public class LevelCrossingPDIAvailableMessage : LevelCrossingMessage
    {
        public LevelCrossingPDIAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.PDIAvailable;

        public byte SenderPdiVersion { get; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new LevelCrossingPDIAvailableMessage(senderId, receiverId);

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class LevelCrossingPDINotAvailableMessage : LevelCrossingMessage
    {
        public LevelCrossingPDINotAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LevelCrossingMessageType MessageType => LevelCrossingMessageType.PDINotAvailable;

        public byte SenderPdiVersion { get; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new LevelCrossingPDINotAvailableMessage(senderId, receiverId);

        protected override void NeuProWritePayloadToByteArray(byte[] bytes)
        {
        }
    }
}
