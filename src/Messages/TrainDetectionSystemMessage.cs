using System;

namespace EulynxLive.Messages
{
    public enum TrainDetectionSystemMessageType: ushort {
        InitializationRequest = 0x0021,
        StartInitialization = 0x0022,
        InitializationCompleted = 0x0023,
        VersionCheckCommand = 0x0024,
        VersionCheckMessage = 0x0025,
        PDIAvailable = 0x0029,
        PDINotAvailable = 0x002A,
        ForceClearCommand = 0x0001,
        UpdateFillingLevelCommand = 0x0002,
        DRFCCommand = 0x0003,
        TDPActivationCommand = 0x0004,
        TvpsOccupancyStatusMessage = 0x0007,
        CommandRejectedMessage = 0x0006,
        DRFCReceiptMessage = 0x0009,
        TvpsFCPFailedMessage = 0x0010,
        TvpsFCPAFailedMessage = 0x0011,
        AdditionalInformationMessage = 0x0012,
        TDPStatusMessage = 0x000B,
    }

    public abstract class TrainDetectionSystemMessage : EulynxMessage
    {
        public TrainDetectionSystemMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {}

        public override ProtocolType ProtocolType => ProtocolType.TrainDetectionSystem;
        public override ushort MessageTypeRaw => (ushort) MessageType;
        public abstract TrainDetectionSystemMessageType MessageType { get; }
    }
    public class TrainDetectionSystemVersionCheckCommand : TrainDetectionSystemMessage
    {
        private const int SENDER_PDI_VERSION_OFFSET = 43;
        public TrainDetectionSystemVersionCheckCommand(string senderId, string receiverId, byte senderPdiVersion) : base(senderId, receiverId)
        {
            SenderPdiVersion = senderPdiVersion;
        }

        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.VersionCheckCommand;

        public byte SenderPdiVersion { get; }

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new TrainDetectionSystemVersionCheckCommand(senderId, receiverId, message[SENDER_PDI_VERSION_OFFSET]);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
        }
    }

    public class TrainDetectionSystemVersionCheckMessage : TrainDetectionSystemMessage
    {
        private const int RESULT_PDI_VERSION_CHECK_OFFSET = 43;
        private const int SENDER_PDI_VERSION_OFFSET = 44;
        private const int CHECKSUM_LENGTH_OFFSET = 45;
        private const int CHECKSUM_DATA_OFFSET = 46;
        public TrainDetectionSystemVersionCheckMessage(string senderId, string receiverId, PdiVersionCheckResult resultPdiVersionCheck, byte senderPdiVersion, byte checksumLength) : base(senderId, receiverId)
        {
            ResultPdiVersionCheck = resultPdiVersionCheck;
            SenderPdiVersion = senderPdiVersion;
            ChecksumLength = checksumLength;
        }

        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.VersionCheckMessage;

        public PdiVersionCheckResult ResultPdiVersionCheck { get; }
        public byte SenderPdiVersion { get; }
        public byte ChecksumLength { get; }

        public override int Size => 46 + ChecksumLength;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var resultPdiVersionCheck = (PdiVersionCheckResult) message[RESULT_PDI_VERSION_CHECK_OFFSET];
            var senderPdiVersion = message[SENDER_PDI_VERSION_OFFSET];
            var checksumLength = message[CHECKSUM_LENGTH_OFFSET];
            // var CHECKSUM_DATA_OFFSET = message[CHECKSUM_DATA_OFFSET];
            return new TrainDetectionSystemVersionCheckMessage(senderId, receiverId, resultPdiVersionCheck, senderPdiVersion, checksumLength);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[RESULT_PDI_VERSION_CHECK_OFFSET] = (byte)ResultPdiVersionCheck;
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
            bytes[CHECKSUM_LENGTH_OFFSET] = ChecksumLength;
        }
    }
    public class TrainDetectionSystemPDIAvailableMessage : PointMessage
    {
        public TrainDetectionSystemPDIAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override PointMessageType MessageType => PointMessageType.PDIAvailable;

        public byte SenderPdiVersion { get; }

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new TrainDetectionSystemPDIAvailableMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class TrainDetectionSystemPDINotAvailableMessage : TrainDetectionSystemMessage
    {
        public TrainDetectionSystemPDINotAvailableMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.PDINotAvailable;

        public byte SenderPdiVersion { get; }

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new TrainDetectionSystemPDINotAvailableMessage(senderId, receiverId);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class TrainDetectionSystemInitializationRequestMessage : TrainDetectionSystemMessage
    {
        public TrainDetectionSystemInitializationRequestMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.InitializationRequest;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemInitializationRequestMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class TrainDetectionSystemStartInitializationMessage : TrainDetectionSystemMessage
    {
        public TrainDetectionSystemStartInitializationMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.StartInitialization;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemStartInitializationMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class TrainDetectionSystemInitializationCompletedMessage : TrainDetectionSystemMessage
    {
        public TrainDetectionSystemInitializationCompletedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.InitializationCompleted;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemInitializationCompletedMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public enum ForceClearMode : byte {
        ForceClearU = 0x01,
        ForceClearC = 0x02,
        ForceClearPA = 0x03,
        ForceClearP = 0x04,
        AcknowledgementAfterForceClearPACommand = 0x05,
    }

    public class TrainDetectionSystemForceClearCommand : TrainDetectionSystemMessage
    {

        private const int FORCE_CLEAR_MODE_OFFSET = 43;
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.ForceClearCommand;
        public override int Size => 44;
        public TrainDetectionSystemForceClearCommand(string senderId, string receiverId, ForceClearMode forceClearMode) : base(senderId, receiverId)
        {
            ForceClearMode = forceClearMode;
        }

        public ForceClearMode ForceClearMode { get; set; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var forceClearMode = (ForceClearMode) message[FORCE_CLEAR_MODE_OFFSET];
            return new TrainDetectionSystemForceClearCommand(senderId, receiverId, forceClearMode);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[FORCE_CLEAR_MODE_OFFSET] = (byte)ForceClearMode;
        }
    }

    public class TrainDetectionSystemUpdateFillingLevelCommand : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.UpdateFillingLevelCommand;
        public override int Size => 43;
        public TrainDetectionSystemUpdateFillingLevelCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemUpdateFillingLevelCommand(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class TrainDetectionSystemDRFCCommand : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.DRFCCommand;
        public override int Size => 43;
        public TrainDetectionSystemDRFCCommand(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemDRFCCommand(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public enum DirectionOfPassing : byte {
        ReferenceDirection = 0x01,
        AgainstReferenceDirection = 0x02,
        WithoutIndicatedDirection = 0x03,
    }

    public class TrainDetectionSystemTDPActivationCommand : TrainDetectionSystemMessage
    {
        private const int DIRECTION_OF_PASSING_OFFSET = 43;
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.TDPActivationCommand;
        public override int Size => 44;
        public TrainDetectionSystemTDPActivationCommand(string senderId, string receiverId, DirectionOfPassing directionOfPassing) : base(senderId, receiverId)
        {
            DirectionOfPassing = directionOfPassing;
        }
        DirectionOfPassing DirectionOfPassing { get; set; }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var directionOfPassing = (DirectionOfPassing) message[DIRECTION_OF_PASSING_OFFSET];
            return new TrainDetectionSystemTDPActivationCommand(senderId, receiverId, directionOfPassing);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[DIRECTION_OF_PASSING_OFFSET] = (byte)DirectionOfPassing;
        }
    }

    public enum TvpsOccupancyStatus : byte {
        Vacant = 0x01,
        Occupied = 0x02,
        Disturbed = 0x03,
        WaitingForSweepingTrainAfterFcPAOrFcP = 0x04,
        WaitingForAcknowledgementAfterFcPA = 0x05,
    }

    public enum TvpsAbilityToBeForcedToClear : byte {
        NotAbleToBeForcedToClear = 0x01,
        AbleToBeForcedToClear = 0x02,
    }
    public enum TvpsPomStatus : byte {
        PowerSupplyOk = 0x01,
        PowerSupplyNok = 0x02,
        NotApplicable = 0xff,
    }

    public class TrainDetectionSystemTvpsOccupancyStatusMessage : TrainDetectionSystemMessage
    {
        private const int OCCUPANCY_STATUS_OFFSET = 43;
        private const int ABILITY_TO_BE_FORCED_TO_CLEAR_OFFSET = 44;
        private const int FILLING_LEVEL_OFFSET = 45;

        private const int POM_STATUS_OFFSET = 47;
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.TvpsOccupancyStatusMessage;
        public override int Size => 48;
        public TrainDetectionSystemTvpsOccupancyStatusMessage(string senderId, string receiverId, TvpsOccupancyStatus occupancyStatus, TvpsAbilityToBeForcedToClear abilityToBeForcedToClear, ushort fillingLevel, TvpsPomStatus pomStatus) : base(senderId, receiverId)
        {
            OccupancyStatus = occupancyStatus;
            AbilityToBeForcedToClear = abilityToBeForcedToClear;
            FillingLevel = fillingLevel;
            PomStatus = pomStatus;
        }

        public TvpsOccupancyStatus OccupancyStatus { get; set; }
        public TvpsAbilityToBeForcedToClear AbilityToBeForcedToClear { get; set; }
        public ushort FillingLevel { get; set; }
        public TvpsPomStatus PomStatus { get; set; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var occupancyStatus = (TvpsOccupancyStatus) message[OCCUPANCY_STATUS_OFFSET];
            var abilityToBeForcedToClear = (TvpsAbilityToBeForcedToClear) message[ABILITY_TO_BE_FORCED_TO_CLEAR_OFFSET];
            var fillingLevel = BitConverter.ToUInt16(
                new byte[2] { message[FILLING_LEVEL_OFFSET], message[FILLING_LEVEL_OFFSET + 1] });
            var pomStatus = (TvpsPomStatus) message[POM_STATUS_OFFSET];
            return new TrainDetectionSystemTvpsOccupancyStatusMessage(senderId, receiverId, occupancyStatus, abilityToBeForcedToClear, fillingLevel, pomStatus);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[OCCUPANCY_STATUS_OFFSET] = (byte)OccupancyStatus;
            bytes[ABILITY_TO_BE_FORCED_TO_CLEAR_OFFSET] = (byte)AbilityToBeForcedToClear;
            bytes[FILLING_LEVEL_OFFSET] = (byte)FillingLevel;
            bytes[FILLING_LEVEL_OFFSET + 1] = (byte)(FillingLevel >> 8);
            bytes[POM_STATUS_OFFSET] = (byte)PomStatus;
        }
    }

    public class TrainDetectionSystemTvpsOccupancyStatusMessageNeuPro : TrainDetectionSystemMessage
    {
        private const int OCCUPANCY_STATUS_OFFSET = 43;
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.TvpsOccupancyStatusMessage;
        public override int Size => 45;
        public TrainDetectionSystemTvpsOccupancyStatusMessageNeuPro(string senderId, string receiverId, TvpsOccupancyStatus occupancyStatus) : base(senderId, receiverId)
        {
            OccupancyStatus = occupancyStatus;
        }

        public TvpsOccupancyStatus OccupancyStatus { get; set; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var occupancyStatus = (TvpsOccupancyStatus) message[OCCUPANCY_STATUS_OFFSET];
            return new TrainDetectionSystemTvpsOccupancyStatusMessageNeuPro(senderId, receiverId, occupancyStatus);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[OCCUPANCY_STATUS_OFFSET] = (byte)OccupancyStatus;
        }
    }

    public class TrainDetectionSystemTvpsOccupancyStatusMessageNeuProThales : TrainDetectionSystemMessage
    {
        private const int OCCUPANCY_STATUS_OFFSET = 43;
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.TvpsOccupancyStatusMessage;
        public override int Size => 47;
        public TrainDetectionSystemTvpsOccupancyStatusMessageNeuProThales(string senderId, string receiverId, TvpsOccupancyStatus occupancyStatus) : base(senderId, receiverId)
        {
            OccupancyStatus = occupancyStatus;
        }

        public TvpsOccupancyStatus OccupancyStatus { get; set; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var occupancyStatus = (TvpsOccupancyStatus) message[OCCUPANCY_STATUS_OFFSET];
            return new TrainDetectionSystemTvpsOccupancyStatusMessageNeuProThales(senderId, receiverId, occupancyStatus);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[OCCUPANCY_STATUS_OFFSET] = (byte)OccupancyStatus;
        }
    }

    public class TrainDetectionSystemCommandRejectedMessage : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.CommandRejectedMessage;
        public override int Size => 43;
        public TrainDetectionSystemCommandRejectedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {

        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemCommandRejectedMessage(senderId, receiverId);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {

        }
    }
    public class TrainDetectionSystemDRFCReceiptMessage : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.DRFCReceiptMessage;
        public override int Size => 43;
        public TrainDetectionSystemDRFCReceiptMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {

        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemDRFCReceiptMessage(senderId, receiverId);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {

        }
    }
    public class TrainDetectionSystemTvpsFCPFailedMessage : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.TvpsFCPFailedMessage;
        public override int Size => 43;
        public TrainDetectionSystemTvpsFCPFailedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {

        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemTvpsFCPFailedMessage(senderId, receiverId);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {

        }
    }
    public class TrainDetectionSystemTvpsFCPAFailedMessage : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.TvpsFCPAFailedMessage;
        public override int Size => 43;
        public TrainDetectionSystemTvpsFCPAFailedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {

        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemTvpsFCPAFailedMessage(senderId, receiverId);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {

        }
    }
    public class TrainDetectionSystemAdditionalInformationMessage : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.AdditionalInformationMessage;
        public override int Size => 43;
        public TrainDetectionSystemAdditionalInformationMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {

        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemAdditionalInformationMessage(senderId, receiverId);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {

        }
    }
    public class TrainDetectionSystemTDPStatusMessage : TrainDetectionSystemMessage
    {
        public override TrainDetectionSystemMessageType MessageType => TrainDetectionSystemMessageType.TDPStatusMessage;
        public override int Size => 43;
        public TrainDetectionSystemTDPStatusMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {

        }
        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new TrainDetectionSystemTDPStatusMessage(senderId, receiverId);
        }
        protected override void WritePayloadToByteArray(byte[] bytes)
        {

        }
    }
}
