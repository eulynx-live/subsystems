using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record FcCommand (string SenderIdentifier, string ReceiverIdentifier, FcCommandModeOfFc ModeOfFc) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ModeOfFcOffset = 43;

    public FcCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ModeOfFc = (FcCommandModeOfFc)message[ModeOfFcOffset];
        return new FcCommand(SenderIdentifier, ReceiverIdentifier, ModeOfFc);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0001).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ModeOfFcOffset] = (byte)ModeOfFc;
        return result;
    }
}

public enum FcCommandModeOfFc : byte {
    FcU = 0x01,
    FcC = 0x02,
    FcPA = 0x03,
    FcP = 0x04,
    AcknowledgmentAfterFcPACommand = 0x05
}


public record UpdateFillingLevelCommand (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public UpdateFillingLevelCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new UpdateFillingLevelCommand(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0002).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record CancelCommand (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public CancelCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new CancelCommand(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0008).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record DisableTheRestrictionToForceSectionStatusToClearCommand (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public DisableTheRestrictionToForceSectionStatusToClearCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new DisableTheRestrictionToForceSectionStatusToClearCommand(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0003).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}





public record CommandRejectedMessage (string SenderIdentifier, string ReceiverIdentifier, CommandRejectedMessageReasonForRejection ReasonForRejection) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReasonForRejectionOffset = 43;

    public CommandRejectedMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReasonForRejection = (CommandRejectedMessageReasonForRejection)message[ReasonForRejectionOffset];
        return new CommandRejectedMessage(SenderIdentifier, ReceiverIdentifier, ReasonForRejection);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0006).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReasonForRejectionOffset] = (byte)ReasonForRejection;
        return result;
    }
}

public enum CommandRejectedMessageReasonForRejection : byte {
    OperationalRejected = 0x01,
    TechnicalRejected = 0x02
}


public record TvpsFcPFailedMessage (string SenderIdentifier, string ReceiverIdentifier, TvpsFcPFailedMessageReasonForFailure ReasonForFailure) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReasonForFailureOffset = 43;

    public TvpsFcPFailedMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReasonForFailure = (TvpsFcPFailedMessageReasonForFailure)message[ReasonForFailureOffset];
        return new TvpsFcPFailedMessage(SenderIdentifier, ReceiverIdentifier, ReasonForFailure);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0010).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReasonForFailureOffset] = (byte)ReasonForFailure;
        return result;
    }
}

public enum TvpsFcPFailedMessageReasonForFailure : byte {
    IncorrectCountOfTheSweepingTrain = 0x01,
    TimeoutConTmaxResponseTimeFcPHadExpired = 0x02,
    BoundingDetectionPointIsConfiguredAsNotPermittedForFcP = 0x03,
    IntentionallyDeleted = 0x04,
    OutgoingAxleDetectedBeforeExpirationOfMinimumTimer = 0x05,
    ProcessCancelled = 0x06
}


public record TvpsFcPAFailedMessage (string SenderIdentifier, string ReceiverIdentifier, TvpsFcPAFailedMessageReasonForFailure ReasonForFailure) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReasonForFailureOffset = 43;

    public TvpsFcPAFailedMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReasonForFailure = (TvpsFcPAFailedMessageReasonForFailure)message[ReasonForFailureOffset];
        return new TvpsFcPAFailedMessage(SenderIdentifier, ReceiverIdentifier, ReasonForFailure);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0011).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReasonForFailureOffset] = (byte)ReasonForFailure;
        return result;
    }
}

public enum TvpsFcPAFailedMessageReasonForFailure : byte {
    IncorrectCountOfTheSweepingTrain = 0x01,
    TimeoutConTmaxResponseTimeFcPAHadExpired = 0x02,
    BoundingDetectionPointIsConfiguredAsNotPermittedForFcPA = 0x03,
    IntentionallyDeleted = 0x04,
    OutgoingAxleDetectedBeforeExpirationOfMinimumTimer = 0x05,
    ProcessCancelled = 0x06
}


public record TdpStatusMessage (string SenderIdentifier, string ReceiverIdentifier, TdpStatusMessageStateOfPassing StateOfPassing, TdpStatusMessageDirectionOfPassing DirectionOfPassing) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int StateOfPassingOffset = 43;
    private const int DirectionOfPassingOffset = 44;

    public TdpStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var StateOfPassing = (TdpStatusMessageStateOfPassing)message[StateOfPassingOffset];
        var DirectionOfPassing = (TdpStatusMessageDirectionOfPassing)message[DirectionOfPassingOffset];
        return new TdpStatusMessage(SenderIdentifier, ReceiverIdentifier, StateOfPassing, DirectionOfPassing);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[45];
        result[0] = protocolType;
        BitConverter.GetBytes(0x000B).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[StateOfPassingOffset] = (byte)StateOfPassing;
        result[DirectionOfPassingOffset] = (byte)DirectionOfPassing;
        return result;
    }
}

public enum TdpStatusMessageStateOfPassing : byte {
    NotPassed = 0x01,
    Passed = 0x02,
    Disturbed = 0x03
}

public enum TdpStatusMessageDirectionOfPassing : byte {
    ReferenceDirection = 0x01,
    AgainstReferenceDirection = 0x02,
    WithoutIndicatedDirection = 0x03
}

