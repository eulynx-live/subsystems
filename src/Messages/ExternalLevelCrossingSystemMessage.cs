namespace EulynxLive.Messages
{
    public enum ExternalLevelCrossingSystemMessageType: ushort {
        // Commands
        LxActivationCommand = 0x0001,
        TrActivationCommand = 0x0002,
        LxDeactivationCommand = 0x0003,
        TrDeactivationCommand = 0x0004,
        ControlActivationPointCommand = 0x0005,
        TrackRelatedProlongActivationCommand = 0x0006,
        CrossingClearCommand = 0x0007,
        BlockLxCommand = 0x0008,
        TrackRelatedIsolationCommand = 0x0009,

        // Messages
        LxFunctionalStatusMessage = 0x0010,
        TrFunctionalStatusMessage = 0x0011,
        ObstacleDetectionStatusMessage = 0x0012,
        DetectionElementStatusMessage = 0x0013,
        LxMonitoringStatusMessage = 0x0014,
        TrMonitoringStatusMessage = 0x0015,
        LxFailureStatusMessage = 0x0016,
        TrFailureStatusMessage = 0x0017,
        TrackRelatedCommandAdmissabilityMessage = 0x0018,
        LxCommandAdmissibilityMessage = 0x0019,
        StatusOfActivationPointMessage = 0x0020,

        // Generic
        InitializationRequest = 0x0021,
        StartInitialization = 0x0022,
        InitializationCompleted = 0x0023,
        VersionCheckCommand = 0x0024,
        VersionCheckMessage = 0x0025,
        PDIAvailable = 0x0029,
        PDINotAvailable = 0x002A,
    }

    public abstract class ExternalLevelCrossingSystemMessage : EulynxMessage
    {
        public ExternalLevelCrossingSystemMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {}

        public override ProtocolType ProtocolType => ProtocolType.ExternalLevelCrossingSystem;
        public override ushort MessageTypeRaw => (ushort) MessageType;
        public abstract ExternalLevelCrossingSystemMessageType MessageType { get; }
    }

    public class ExternalLevelCrossingSystemLxActivationCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemLxActivationCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.LxActivationCommand;

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemLxActivationCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrActivationCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrActivationCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrActivationCommand;

        public override int Size => 45;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrActivationCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemLxDeactivationCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemLxDeactivationCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.LxDeactivationCommand;

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemLxDeactivationCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrDeactivationCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrDeactivationCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrDeactivationCommand;

        public override int Size => 45;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrDeactivationCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemControlActivationPointCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemControlActivationPointCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.ControlActivationPointCommand;

        public override int Size => 46;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemControlActivationPointCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrackRelatedProlongActivationCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrackRelatedProlongActivationCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrackRelatedProlongActivationCommand;

        public override int Size => 45;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrackRelatedProlongActivationCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemCrossingClearCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemCrossingClearCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.CrossingClearCommand;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemCrossingClearCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemBlockLxCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemBlockLxCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.BlockLxCommand;

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemBlockLxCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrackRelatedIsolationCommand : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrackRelatedIsolationCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrackRelatedIsolationCommand;

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrackRelatedIsolationCommand(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemLxFunctionalStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemLxFunctionalStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.LxFunctionalStatusMessage;

        public override int Size => 48;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemLxFunctionalStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrFunctionalStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrFunctionalStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrFunctionalStatusMessage;

        public override int Size => 51;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrFunctionalStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemObstacleDetectionStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemObstacleDetectionStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.ObstacleDetectionStatusMessage;

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemObstacleDetectionStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemDetectionElementStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemDetectionElementStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.DetectionElementStatusMessage;

        // TODO: Dynamic size based on number of detection elements, here: k=0
        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemDetectionElementStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemLxMonitoringStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemLxMonitoringStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.LxMonitoringStatusMessage;

        public override int Size => 49;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemLxMonitoringStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrMonitoringStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrMonitoringStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrMonitoringStatusMessage;

        public override int Size => 46;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrMonitoringStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemLxFailureStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemLxFailureStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.LxFailureStatusMessage;

        public override int Size => 45;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemLxFailureStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrFailureStatusMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrFailureStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrFailureStatusMessage;

        public override int Size => 45;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrFailureStatusMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemTrackRelatedCommandAdmissabilityMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemTrackRelatedCommandAdmissabilityMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.TrackRelatedCommandAdmissabilityMessage;

        public override int Size => 49;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemTrackRelatedCommandAdmissabilityMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemLxCommandAdmissibilityMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemLxCommandAdmissibilityMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.LxCommandAdmissibilityMessage;

        public override int Size => 47;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemLxCommandAdmissibilityMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemStatusOfActivationPointMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemStatusOfActivationPointMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.StatusOfActivationPointMessage;

        // TODO: dynamic size based on number k of activation points, here k=0
        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemStatusOfActivationPointMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemVersionCheckCommand : ExternalLevelCrossingSystemMessage
    {
        private const int SENDER_PDI_VERSION_OFFSET = 43;
        public ExternalLevelCrossingSystemVersionCheckCommand(string senderId, string receiverId, byte senderPdiVersion) : base(senderId, receiverId)
        {
            SenderPdiVersion = senderPdiVersion;
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.VersionCheckCommand;

        public byte SenderPdiVersion { get; }

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemVersionCheckCommand(senderId, receiverId, message[SENDER_PDI_VERSION_OFFSET]);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
        }
    }

    public class ExternalLevelCrossingSystemVersionCheckMessage : ExternalLevelCrossingSystemMessage
    {
        private const int RESULT_PDI_VERSION_CHECK_OFFSET = 43;
        private const int SENDER_PDI_VERSION_OFFSET = 44;
        private const int CHECKSUM_LENGTH_OFFSET = 45;
        private const int CHECKSUM_DATA_OFFSET = 46;
        public ExternalLevelCrossingSystemVersionCheckMessage(string senderId, string receiverId, PdiVersionCheckResult resultPdiVersionCheck, byte senderPdiVersion, byte checksumLength) : base(senderId, receiverId)
        {
            ResultPdiVersionCheck = resultPdiVersionCheck;
            SenderPdiVersion = senderPdiVersion;
            ChecksumLength = checksumLength;
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.VersionCheckMessage;

        public PdiVersionCheckResult ResultPdiVersionCheck { get; }
        public byte SenderPdiVersion { get; }
        public byte ChecksumLength { get; }

        public override int Size => 46 + ChecksumLength;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var resultPdiVersionCheck = (PdiVersionCheckResult) message[RESULT_PDI_VERSION_CHECK_OFFSET];
            var senderPdiVersion = message[SENDER_PDI_VERSION_OFFSET];
            var checksumLength = message[CHECKSUM_LENGTH_OFFSET];
            // var CHECKSUM_DATA_OFFSET = message[CHECKSUM_DATA_OFFSET];
            return new ExternalLevelCrossingSystemVersionCheckMessage(senderId, receiverId, resultPdiVersionCheck, senderPdiVersion, checksumLength);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[RESULT_PDI_VERSION_CHECK_OFFSET] = (byte)ResultPdiVersionCheck;
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
            bytes[CHECKSUM_LENGTH_OFFSET] = ChecksumLength;
        }
    }

    public class ExternalLevelCrossingSystemPDIAvailableMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemPDIAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.PDIAvailable;

        public byte SenderPdiVersion { get; }

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemPDIAvailableMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemPDINotAvailableMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemPDINotAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.PDINotAvailable;

        public byte SenderPdiVersion { get; }

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new ExternalLevelCrossingSystemPDINotAvailableMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemInitializationRequestMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemInitializationRequestMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.InitializationRequest;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new ExternalLevelCrossingSystemInitializationRequestMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemStartInitializationMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemStartInitializationMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.StartInitialization;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new ExternalLevelCrossingSystemStartInitializationMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class ExternalLevelCrossingSystemInitializationCompletedMessage : ExternalLevelCrossingSystemMessage
    {
        public ExternalLevelCrossingSystemInitializationCompletedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override ExternalLevelCrossingSystemMessageType MessageType => ExternalLevelCrossingSystemMessageType.InitializationCompleted;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new ExternalLevelCrossingSystemInitializationCompletedMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }
}
