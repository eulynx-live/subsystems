using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record TrainDetectionSystemFcCommand(string SenderIdentifier, string ReceiverIdentifier, TrainDetectionSystemFcCommandModeOfFc ModeOfFc) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ModeOfFcOffset = 43;

    public new static TrainDetectionSystemFcCommand FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ModeOfFc = (TrainDetectionSystemFcCommandModeOfFc)message[ModeOfFcOffset];
        return new TrainDetectionSystemFcCommand(SenderIdentifier, ReceiverIdentifier, ModeOfFc);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0001).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ModeOfFcOffset] = (byte)ModeOfFc;
        return result;
    }
}

public enum TrainDetectionSystemFcCommandModeOfFc : byte
{
    FcU = 0x01,
    FcC = 0x02,
    FcPA = 0x03,
    FcP = 0x04,
    AcknowledgmentAfterFcPACommand = 0x05
}


public record TrainDetectionSystemUpdateFillingLevelCommand(string SenderIdentifier, string ReceiverIdentifier) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public new static TrainDetectionSystemUpdateFillingLevelCommand FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);

        return new TrainDetectionSystemUpdateFillingLevelCommand(SenderIdentifier, ReceiverIdentifier);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[43];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0002).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);

        return result;
    }
}




public record TrainDetectionSystemCancelCommand(string SenderIdentifier, string ReceiverIdentifier) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public new static TrainDetectionSystemCancelCommand FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);

        return new TrainDetectionSystemCancelCommand(SenderIdentifier, ReceiverIdentifier);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[43];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0008).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);

        return result;
    }
}




public record TrainDetectionSystemDisableTheRestrictionToForceSectionStatusToClearCommand(string SenderIdentifier, string ReceiverIdentifier) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public new static TrainDetectionSystemDisableTheRestrictionToForceSectionStatusToClearCommand FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);

        return new TrainDetectionSystemDisableTheRestrictionToForceSectionStatusToClearCommand(SenderIdentifier, ReceiverIdentifier);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[43];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0003).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);

        return result;
    }
}





public record TrainDetectionSystemTvpsOccupancyStatusMessage(string SenderIdentifier, string ReceiverIdentifier, TrainDetectionSystemTvpsOccupancyStatusMessageOccupancyStatus OccupancyStatus, TrainDetectionSystemTvpsOccupancyStatusMessageAbilityToBeForcedToClear AbilityToBeForcedToClear, ushort FillingLevel, TrainDetectionSystemTvpsOccupancyStatusMessagePomStatus PomStatus, TrainDetectionSystemTvpsOccupancyStatusMessageDisturbanceStatus DisturbanceStatus, TrainDetectionSystemTvpsOccupancyStatusMessageChangeTrigger ChangeTrigger) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int OccupancyStatusOffset = 43;
    private const int AbilityToBeForcedToClearOffset = 44;
    private const int FillingLevelOffset = 45;
    private const int PomStatusOffset = 47;
    private const int DisturbanceStatusOffset = 48;
    private const int ChangeTriggerOffset = 49;

    public new static TrainDetectionSystemTvpsOccupancyStatusMessage FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var OccupancyStatus = (TrainDetectionSystemTvpsOccupancyStatusMessageOccupancyStatus)message[OccupancyStatusOffset];
        var AbilityToBeForcedToClear = (TrainDetectionSystemTvpsOccupancyStatusMessageAbilityToBeForcedToClear)message[AbilityToBeForcedToClearOffset];

        var FillingLevelBytes = new byte[] { message[FillingLevelOffset], message[FillingLevelOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(FillingLevelBytes);
        var FillingLevel = BitConverter.ToUInt16(FillingLevelBytes);

        var PomStatus = (TrainDetectionSystemTvpsOccupancyStatusMessagePomStatus)message[PomStatusOffset];
        var DisturbanceStatus = (TrainDetectionSystemTvpsOccupancyStatusMessageDisturbanceStatus)message[DisturbanceStatusOffset];
        var ChangeTrigger = (TrainDetectionSystemTvpsOccupancyStatusMessageChangeTrigger)message[ChangeTriggerOffset];
        return new TrainDetectionSystemTvpsOccupancyStatusMessage(SenderIdentifier, ReceiverIdentifier, OccupancyStatus, AbilityToBeForcedToClear, FillingLevel, PomStatus, DisturbanceStatus, ChangeTrigger);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[50];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0007).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[OccupancyStatusOffset] = (byte)OccupancyStatus;
        result[AbilityToBeForcedToClearOffset] = (byte)AbilityToBeForcedToClear;

        var FillingLevelBytes = BitConverter.GetBytes(FillingLevel);
        if (!BitConverter.IsLittleEndian) Array.Reverse(FillingLevelBytes);
        FillingLevelBytes.Take(2).ToArray().CopyTo(result, FillingLevelOffset);

        result[PomStatusOffset] = (byte)PomStatus;
        result[DisturbanceStatusOffset] = (byte)DisturbanceStatus;
        result[ChangeTriggerOffset] = (byte)ChangeTrigger;
        return result;
    }
}

public enum TrainDetectionSystemTvpsOccupancyStatusMessageOccupancyStatus : byte
{
    TvpsIsInStateVacant = 0x01,
    TvpsIsInStateOccupied = 0x02,
    TvpsIsInStateDisturbed = 0x03,
    TvpsIsInStateWaitingForASweepingTrainAfterFcPAOrFcPCommand = 0x04,
    TvpsIsInStateWaitingForAnAcknowledgmentAfterFcPACommand = 0x05,
    TvpsIsInStateSweepingTrainDetected = 0x06
}

public enum TrainDetectionSystemTvpsOccupancyStatusMessageAbilityToBeForcedToClear : byte
{
    TvpsIsNotAbleToBeForcedToClear = 0x01,
    TvpsIsAbleToBeForcedToClear = 0x02
}

public enum TrainDetectionSystemTvpsOccupancyStatusMessagePomStatus : byte
{
    PowerSupplyOk = 0x01,
    PowerSupplyNok = 0x02,
    PomStatusIsNotApplicable = 0xFF
}

public enum TrainDetectionSystemTvpsOccupancyStatusMessageDisturbanceStatus : byte
{
    DisturbanceIsOperational = 0x01,
    DisturbanceIsTechnical = 0x02,
    DisturbanceStatusIsNotApplicable = 0xFF
}

public enum TrainDetectionSystemTvpsOccupancyStatusMessageChangeTrigger : byte
{
    PassingDetected = 0x01,
    CommandFromEilAccepted = 0x02,
    CommandFromMaintainerAccepted = 0x03,
    TechnicalFailure = 0x04,
    InitialSectionState = 0x05,
    InternalTrigger = 0x06,
    ChangeTriggerIsNotApplicable = 0xFF
}


public record TrainDetectionSystemCommandRejectedMessage(string SenderIdentifier, string ReceiverIdentifier, TrainDetectionSystemCommandRejectedMessageReasonForRejection ReasonForRejection) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReasonForRejectionOffset = 43;

    public new static TrainDetectionSystemCommandRejectedMessage FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReasonForRejection = (TrainDetectionSystemCommandRejectedMessageReasonForRejection)message[ReasonForRejectionOffset];
        return new TrainDetectionSystemCommandRejectedMessage(SenderIdentifier, ReceiverIdentifier, ReasonForRejection);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0006).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReasonForRejectionOffset] = (byte)ReasonForRejection;
        return result;
    }
}

public enum TrainDetectionSystemCommandRejectedMessageReasonForRejection : byte
{
    OperationalRejected = 0x01,
    TechnicalRejected = 0x02
}


public record TrainDetectionSystemTvpsFcPFailedMessage(string SenderIdentifier, string ReceiverIdentifier, TrainDetectionSystemTvpsFcPFailedMessageReasonForFailure ReasonForFailure) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReasonForFailureOffset = 43;

    public new static TrainDetectionSystemTvpsFcPFailedMessage FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReasonForFailure = (TrainDetectionSystemTvpsFcPFailedMessageReasonForFailure)message[ReasonForFailureOffset];
        return new TrainDetectionSystemTvpsFcPFailedMessage(SenderIdentifier, ReceiverIdentifier, ReasonForFailure);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0010).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReasonForFailureOffset] = (byte)ReasonForFailure;
        return result;
    }
}

public enum TrainDetectionSystemTvpsFcPFailedMessageReasonForFailure : byte
{
    IncorrectCountOfTheSweepingTrain = 0x01,
    TimeoutConTmaxResponseTimeFcPHadExpired = 0x02,
    BoundingDetectionPointIsConfiguredAsNotPermittedForFcP = 0x03,
    IntentionallyDeleted = 0x04,
    OutgoingAxleDetectedBeforeExpirationOfMinimumTimer = 0x05,
    ProcessCancelled = 0x06
}


public record TrainDetectionSystemTvpsFcPAFailedMessage(string SenderIdentifier, string ReceiverIdentifier, TrainDetectionSystemTvpsFcPAFailedMessageReasonForFailure ReasonForFailure) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReasonForFailureOffset = 43;

    public new static TrainDetectionSystemTvpsFcPAFailedMessage FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReasonForFailure = (TrainDetectionSystemTvpsFcPAFailedMessageReasonForFailure)message[ReasonForFailureOffset];
        return new TrainDetectionSystemTvpsFcPAFailedMessage(SenderIdentifier, ReceiverIdentifier, ReasonForFailure);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0011).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReasonForFailureOffset] = (byte)ReasonForFailure;
        return result;
    }
}

public enum TrainDetectionSystemTvpsFcPAFailedMessageReasonForFailure : byte
{
    IncorrectCountOfTheSweepingTrain = 0x01,
    TimeoutConTmaxResponseTimeFcPAHadExpired = 0x02,
    BoundingDetectionPointIsConfiguredAsNotPermittedForFcPA = 0x03,
    IntentionallyDeleted = 0x04,
    OutgoingAxleDetectedBeforeExpirationOfMinimumTimer = 0x05,
    ProcessCancelled = 0x06
}


public record TrainDetectionSystemTdpStatusMessage(string SenderIdentifier, string ReceiverIdentifier, TrainDetectionSystemTdpStatusMessageStateOfPassing StateOfPassing, TrainDetectionSystemTdpStatusMessageDirectionOfPassing DirectionOfPassing) : Message
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int StateOfPassingOffset = 43;
    private const int DirectionOfPassingOffset = 44;

    public new static TrainDetectionSystemTdpStatusMessage FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var StateOfPassing = (TrainDetectionSystemTdpStatusMessageStateOfPassing)message[StateOfPassingOffset];
        var DirectionOfPassing = (TrainDetectionSystemTdpStatusMessageDirectionOfPassing)message[DirectionOfPassingOffset];
        return new TrainDetectionSystemTdpStatusMessage(SenderIdentifier, ReceiverIdentifier, StateOfPassing, DirectionOfPassing);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[45];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x000B).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[StateOfPassingOffset] = (byte)StateOfPassing;
        result[DirectionOfPassingOffset] = (byte)DirectionOfPassing;
        return result;
    }
}

public enum TrainDetectionSystemTdpStatusMessageStateOfPassing : byte
{
    NotPassed = 0x01,
    Passed = 0x02,
    Disturbed = 0x03
}

public enum TrainDetectionSystemTdpStatusMessageDirectionOfPassing : byte
{
    ReferenceDirection = 0x01,
    AgainstReferenceDirection = 0x02,
    WithoutIndicatedDirection = 0x03
}

