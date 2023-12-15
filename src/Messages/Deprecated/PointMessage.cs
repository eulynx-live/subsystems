namespace EulynxLive.Messages.Deprecated
{
    public enum PointMessageType: ushort {
        InitializationRequest = 0x0021,
        StartInitialization = 0x0022,
        InitializationCompleted = 0x0023,
        VersionCheckCommand = 0x0024,
        VersionCheckMessage = 0x0025,
        PDIAvailable = 0x0029,
        PDINotAvailable = 0x002A,
        MovePointCommand = 0x0001,
        PointPositionMessage = 0x000B,
        TimeoutMessage = 0x000C,
    }

    public abstract class PointMessage : EulynxMessage
    {
        public PointMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {}

        public override ProtocolType ProtocolType => ProtocolType.Point;
        public override ushort MessageTypeRaw => (ushort) MessageType;
        public abstract PointMessageType MessageType { get; }
    }
    public class PointVersionCheckCommand : PointMessage
    {
        private const int SENDER_PDI_VERSION_OFFSET = 43;
        public PointVersionCheckCommand(string senderId, string receiverId, byte senderPdiVersion) : base(senderId, receiverId)
        {
            SenderPdiVersion = senderPdiVersion;
        }

        public override PointMessageType MessageType => PointMessageType.VersionCheckCommand;

        public byte SenderPdiVersion { get; }

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new PointVersionCheckCommand(senderId, receiverId, message[SENDER_PDI_VERSION_OFFSET]);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
        }
    }

    public class PointVersionCheckMessage : PointMessage
    {
        private const int RESULT_PDI_VERSION_CHECK_OFFSET = 43;
        private const int SENDER_PDI_VERSION_OFFSET = 44;
        private const int CHECKSUM_LENGTH_OFFSET = 45;
        private const int CHECKSUM_DATA_OFFSET = 46;
        public PointVersionCheckMessage(string senderId, string receiverId, PdiVersionCheckResult resultPdiVersionCheck, byte senderPdiVersion, byte checksumLength) : base(senderId, receiverId)
        {
            ResultPdiVersionCheck = resultPdiVersionCheck;
            SenderPdiVersion = senderPdiVersion;
            ChecksumLength = checksumLength;
        }

        public override PointMessageType MessageType => PointMessageType.VersionCheckMessage;

        public PdiVersionCheckResult ResultPdiVersionCheck { get; }
        public byte SenderPdiVersion { get; }
        public byte ChecksumLength { get; }

        public override int Size => 46 + ChecksumLength;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var resultPdiVersionCheck = (PdiVersionCheckResult) message[RESULT_PDI_VERSION_CHECK_OFFSET];
            var senderPdiVersion = message[SENDER_PDI_VERSION_OFFSET];
            var checksumLength = message[CHECKSUM_LENGTH_OFFSET];
            // var CHECKSUM_DATA_OFFSET = message[CHECKSUM_DATA_OFFSET];
            return new PointVersionCheckMessage(senderId, receiverId, resultPdiVersionCheck, senderPdiVersion, checksumLength);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[RESULT_PDI_VERSION_CHECK_OFFSET] = (byte)ResultPdiVersionCheck;
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
            bytes[CHECKSUM_LENGTH_OFFSET] = ChecksumLength;
        }
    }

    public class PointPDIAvailableMessage : PointMessage
    {
        public PointPDIAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override PointMessageType MessageType => PointMessageType.PDIAvailable;

        public byte SenderPdiVersion { get; }

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new PointPDIAvailableMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class PointPDINotAvailableMessage : PointMessage
    {
        public PointPDINotAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override PointMessageType MessageType => PointMessageType.PDINotAvailable;

        public byte SenderPdiVersion { get; }

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new PointPDINotAvailableMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class PointInitializationRequestMessage : PointMessage
    {
        public PointInitializationRequestMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override PointMessageType MessageType => PointMessageType.InitializationRequest;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new PointInitializationRequestMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class PointStartInitializationMessage : PointMessage
    {
        public PointStartInitializationMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override PointMessageType MessageType => PointMessageType.StartInitialization;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new PointStartInitializationMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class PointInitializationCompletedMessage : PointMessage
    {
        public PointInitializationCompletedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override PointMessageType MessageType => PointMessageType.InitializationCompleted;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new PointInitializationCompletedMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public enum CommandedPointPosition : byte {
        Right = 0x01,
        Left = 0x02,
    }

    public class PointMovePointCommand : PointMessage
    {
        private const int COMMANDED_POINT_POSITION_OFFSET = 43;
        public override PointMessageType MessageType => PointMessageType.MovePointCommand;
        public override int Size => 44;
        public PointMovePointCommand(string senderId, string receiverId, CommandedPointPosition commandedPointPosition) : base(senderId, receiverId)
        {
            CommandedPointPosition = commandedPointPosition;
        }

        public CommandedPointPosition CommandedPointPosition { get; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var commandedPointPosition = (CommandedPointPosition) message[COMMANDED_POINT_POSITION_OFFSET];

            return new PointMovePointCommand(senderId, receiverId, commandedPointPosition);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[COMMANDED_POINT_POSITION_OFFSET] = (byte)CommandedPointPosition;
        }
    }

    public enum ReportedPointPosition : byte {
        Right = 0x01,
        Left = 0x02,
        NoEndPosition = 0x03,
        Trailed = 0x04,
    }

    public class PointPositionMessage : PointMessage
    {
        private const int REPORTED_POINT_POSITION_OFFSET = 43;
        public override PointMessageType MessageType => PointMessageType.PointPositionMessage;
        public override int Size => 44;
        public PointPositionMessage(string senderId, string receiverId, ReportedPointPosition reportedPointPosition) : base(senderId, receiverId)
        {
            ReportedPointPosition = reportedPointPosition;
        }

        public ReportedPointPosition ReportedPointPosition { get; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var reportedPointPosition = (ReportedPointPosition) message[REPORTED_POINT_POSITION_OFFSET];

            return new PointPositionMessage(senderId, receiverId, reportedPointPosition);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[REPORTED_POINT_POSITION_OFFSET] = (byte)ReportedPointPosition;
        }
    }

    public class PointTimeoutMessage : PointMessage
    {
        public override PointMessageType MessageType => PointMessageType.TimeoutMessage;
        public override int Size => 43;
        public PointTimeoutMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public ReportedPointPosition ReportedPointPosition { get; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new PointTimeoutMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }
}
