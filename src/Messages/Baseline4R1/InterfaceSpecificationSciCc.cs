using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record CCManageATrackSectionCommand (CCManageATrackSectionCommandMessageType MessageType, string SenderIdentifier, string ReceiverIdentifier, ushort Tan, string TrackSectionId, CCManageATrackSectionCommandInstruction Instruction) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int TanOffset = 43;
    private const int InformationTypeOffset = 45;
    private const int TrackSectionIdOffset = 46;
    private const int InstructionOffset = 66;
    public static readonly byte InformationType = 0x2B;

    public new static CCManageATrackSectionCommand FromBytes(byte[] message) {
        var MessageType = (CCManageATrackSectionCommandMessageType)message[MessageTypeOffset];
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var TanBytes = new byte[] { message[TanOffset], message[TanOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        var Tan = BitConverter.ToUInt16(TanBytes);
        var TrackSectionId = Encoding.Latin1.GetString(message, TrackSectionIdOffset, 20);
        var Instruction = (CCManageATrackSectionCommandInstruction)message[InstructionOffset];
        return new CCManageATrackSectionCommand(MessageType, SenderIdentifier, ReceiverIdentifier, Tan, TrackSectionId, Instruction);
    }

    public override byte[] ToByteArray() {
        var result = new byte[67];
        result[0] = (byte)ProtocolType.CC;
        result[InformationTypeOffset] = (byte)InformationType;
        {
            var bytes = BitConverter.GetBytes((ushort)MessageType);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            bytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        }
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        var TanBytes = BitConverter.GetBytes(Tan);
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        TanBytes.Take(2).ToArray().CopyTo(result, TanOffset);
        Encoding.Latin1.GetBytes(TrackSectionId.PadRight(20, ' ')).CopyTo(result, TrackSectionIdOffset);
        result[InstructionOffset] = (byte)Instruction;
        return result;
    }
}

public enum CCManageATrackSectionCommandMessageType : ushort {
    Regular = 0x0050,
    Confirmation = 0x0055
}

public enum CCManageATrackSectionCommandInstruction : byte {
    UnblockAgainstRouteSettingAllTrains = 0x01,
    BlockAgainstRouteSettingAllTrains = 0x02
}

public record CCSetPredefinedObstructionCommand (CCSetPredefinedObstructionCommandMessageType MessageType, string SenderIdentifier, string ReceiverIdentifier, ushort Tan, string ObstructionId, CCSetPredefinedObstructionCommandInstruction Instruction) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int TanOffset = 43;
    private const int InformationTypeOffset = 45;
    private const int ObstructionIdOffset = 46;
    private const int InstructionOffset = 66;
    public static readonly byte InformationType = 0x96;

    public new static CCSetPredefinedObstructionCommand FromBytes(byte[] message) {
        var MessageType = (CCSetPredefinedObstructionCommandMessageType)message[MessageTypeOffset];
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var TanBytes = new byte[] { message[TanOffset], message[TanOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        var Tan = BitConverter.ToUInt16(TanBytes);
        var ObstructionId = Encoding.Latin1.GetString(message, ObstructionIdOffset, 20);
        var Instruction = (CCSetPredefinedObstructionCommandInstruction)message[InstructionOffset];
        return new CCSetPredefinedObstructionCommand(MessageType, SenderIdentifier, ReceiverIdentifier, Tan, ObstructionId, Instruction);
    }

    public override byte[] ToByteArray() {
        var result = new byte[67];
        result[0] = (byte)ProtocolType.CC;
        result[InformationTypeOffset] = (byte)InformationType;
        {
            var bytes = BitConverter.GetBytes((ushort)MessageType);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            bytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        }
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        var TanBytes = BitConverter.GetBytes(Tan);
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        TanBytes.Take(2).ToArray().CopyTo(result, TanOffset);
        Encoding.Latin1.GetBytes(ObstructionId.PadRight(20, ' ')).CopyTo(result, ObstructionIdOffset);
        result[InstructionOffset] = (byte)Instruction;
        return result;
    }
}

public enum CCSetPredefinedObstructionCommandMessageType : ushort {
    Regular = 0x0050,
    Confirmation = 0x0055
}

public enum CCSetPredefinedObstructionCommandInstruction : byte {
    Clear = 0x01,
    ObstructionSetMovementAuthorityAssociatedWithDegradedClassRouteAllowed = 0x02,
    ObstructionSetNoMovementAuthorityAllowed = 0x03
}

public record CCPredefinedObstructionStatusMessage (string SenderIdentifier, string ReceiverIdentifier, string ObstructionId, CCPredefinedObstructionStatusMessageStatus Status) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int InformationTypeOffset = 43;
    private const int ObstructionIdOffset = 44;
    private const int StatusOffset = 64;
    public static readonly ushort MessageType = 0x0040;
    public static readonly byte InformationType = 0x27;

    public new static CCPredefinedObstructionStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ObstructionId = Encoding.Latin1.GetString(message, ObstructionIdOffset, 20);
        var Status = (CCPredefinedObstructionStatusMessageStatus)message[StatusOffset];
        return new CCPredefinedObstructionStatusMessage(SenderIdentifier, ReceiverIdentifier, ObstructionId, Status);
    }

    public override byte[] ToByteArray() {
        var result = new byte[65];
        result[0] = (byte)ProtocolType.CC;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        result[InformationTypeOffset] = (byte)InformationType;
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        Encoding.Latin1.GetBytes(ObstructionId.PadRight(20, ' ')).CopyTo(result, ObstructionIdOffset);
        result[StatusOffset] = (byte)Status;
        return result;
    }
}

public enum CCPredefinedObstructionStatusMessageStatus : byte {
    Clear = 0x01,
    ObstructionSetMovementAuthorityAssociatedWithDegradedClassRouteAllowed = 0x02,
    ObstructionSetNoMovementAuthorityAllowed = 0x03
}

public record CCReleaseMovementAuthorityCommand (CCReleaseMovementAuthorityCommandMessageType MessageType, string SenderIdentifier, string ReceiverIdentifier, ushort Tan, string OnboardId) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int TanOffset = 43;
    private const int InformationTypeOffset = 45;
    private const int OnboardIdOffset = 46;
    public static readonly byte InformationType = 0x2C;

    public new static CCReleaseMovementAuthorityCommand FromBytes(byte[] message) {
        var MessageType = (CCReleaseMovementAuthorityCommandMessageType)message[MessageTypeOffset];
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var TanBytes = new byte[] { message[TanOffset], message[TanOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        var Tan = BitConverter.ToUInt16(TanBytes);
        var OnboardId = Encoding.Latin1.GetString(message, OnboardIdOffset, 20);
        return new CCReleaseMovementAuthorityCommand(MessageType, SenderIdentifier, ReceiverIdentifier, Tan, OnboardId);
    }

    public override byte[] ToByteArray() {
        var result = new byte[66];
        result[0] = (byte)ProtocolType.CC;
        result[InformationTypeOffset] = (byte)InformationType;
        {
            var bytes = BitConverter.GetBytes((ushort)MessageType);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            bytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        }
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        var TanBytes = BitConverter.GetBytes(Tan);
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        TanBytes.Take(2).ToArray().CopyTo(result, TanOffset);
        Encoding.Latin1.GetBytes(OnboardId.PadRight(20, '_')).CopyTo(result, OnboardIdOffset);
        return result;
    }
}

public enum CCReleaseMovementAuthorityCommandMessageType : ushort {
    Regular = 0x0050,
    Confirmation = 0x0055
}

public record CCAbortCommandCommand (string SenderIdentifier, string ReceiverIdentifier, ushort ConfirmationTan) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ConfirmationTanOffset = 43;
    public static readonly ushort MessageType = 0x0065;

    public new static CCAbortCommandCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ConfirmationTanBytes = new byte[] { message[ConfirmationTanOffset], message[ConfirmationTanOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(ConfirmationTanBytes);
        var ConfirmationTan = BitConverter.ToUInt16(ConfirmationTanBytes);
        return new CCAbortCommandCommand(SenderIdentifier, ReceiverIdentifier, ConfirmationTan);
    }

    public override byte[] ToByteArray() {
        var result = new byte[45];
        result[0] = (byte)ProtocolType.CC;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        var ConfirmationTanBytes = BitConverter.GetBytes(ConfirmationTan);
        if (!BitConverter.IsLittleEndian) Array.Reverse(ConfirmationTanBytes);
        ConfirmationTanBytes.Take(2).ToArray().CopyTo(result, ConfirmationTanOffset);
        return result;
    }
}



public record CCConfirmationOfACommandWithSafetyCodesCommand (string SenderIdentifier, string ReceiverIdentifier, ushort ConfirmationTan, CCConfirmationOfACommandWithSafetyCodesCommandMessageSafetyCodeApplicability MessageSafetyCodeApplicability, string MessageSafetyCode, CCConfirmationOfACommandWithSafetyCodesCommandConfirmationSafetyCodeApplicability ConfirmationSafetyCodeApplicability, string ConfirmationSafetyCodePart1, string ConfirmationSafetyCodePart2, string ConfirmedElementId) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ConfirmationTanOffset = 43;
    private const int MessageSafetyCodeApplicabilityOffset = 45;
    private const int MessageSafetyCodeOffset = 46;
    private const int ConfirmationSafetyCodeApplicabilityOffset = 62;
    private const int ConfirmationSafetyCodePart1Offset = 63;
    private const int ConfirmationSafetyCodePart2Offset = 67;
    private const int ConfirmedElementIdOffset = 71;
    public static readonly ushort MessageType = 0x0060;

    public new static CCConfirmationOfACommandWithSafetyCodesCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ConfirmationTanBytes = new byte[] { message[ConfirmationTanOffset], message[ConfirmationTanOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(ConfirmationTanBytes);
        var ConfirmationTan = BitConverter.ToUInt16(ConfirmationTanBytes);
        var MessageSafetyCodeApplicability = (CCConfirmationOfACommandWithSafetyCodesCommandMessageSafetyCodeApplicability)message[MessageSafetyCodeApplicabilityOffset];
        var MessageSafetyCode = Encoding.Latin1.GetString(message, MessageSafetyCodeOffset, 16);
        var ConfirmationSafetyCodeApplicability = (CCConfirmationOfACommandWithSafetyCodesCommandConfirmationSafetyCodeApplicability)message[ConfirmationSafetyCodeApplicabilityOffset];
        var ConfirmationSafetyCodePart1 = Encoding.Latin1.GetString(message, ConfirmationSafetyCodePart1Offset, 4);
        var ConfirmationSafetyCodePart2 = Encoding.Latin1.GetString(message, ConfirmationSafetyCodePart2Offset, 4);
        var ConfirmedElementId = Encoding.Latin1.GetString(message, ConfirmedElementIdOffset, 20);
        return new CCConfirmationOfACommandWithSafetyCodesCommand(SenderIdentifier, ReceiverIdentifier, ConfirmationTan, MessageSafetyCodeApplicability, MessageSafetyCode, ConfirmationSafetyCodeApplicability, ConfirmationSafetyCodePart1, ConfirmationSafetyCodePart2, ConfirmedElementId);
    }

    public override byte[] ToByteArray() {
        var result = new byte[91];
        result[0] = (byte)ProtocolType.CC;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        var ConfirmationTanBytes = BitConverter.GetBytes(ConfirmationTan);
        if (!BitConverter.IsLittleEndian) Array.Reverse(ConfirmationTanBytes);
        ConfirmationTanBytes.Take(2).ToArray().CopyTo(result, ConfirmationTanOffset);
        result[MessageSafetyCodeApplicabilityOffset] = (byte)MessageSafetyCodeApplicability;
        Encoding.Latin1.GetBytes(MessageSafetyCode.PadRight(16, '_')).CopyTo(result, MessageSafetyCodeOffset);
        result[ConfirmationSafetyCodeApplicabilityOffset] = (byte)ConfirmationSafetyCodeApplicability;
        Encoding.Latin1.GetBytes(ConfirmationSafetyCodePart1.PadRight(4, '_')).CopyTo(result, ConfirmationSafetyCodePart1Offset);
        Encoding.Latin1.GetBytes(ConfirmationSafetyCodePart2.PadRight(4, '_')).CopyTo(result, ConfirmationSafetyCodePart2Offset);
        Encoding.Latin1.GetBytes(ConfirmedElementId.PadRight(20, ' ')).CopyTo(result, ConfirmedElementIdOffset);
        return result;
    }
}

public enum CCConfirmationOfACommandWithSafetyCodesCommandMessageSafetyCodeApplicability : byte {
    SafetyCodeApplicable = 0x01,
    SafetyCodeNotApplicable = 0xFF
}

public enum CCConfirmationOfACommandWithSafetyCodesCommandConfirmationSafetyCodeApplicability : byte {
    SafetyCodeApplicableCheckPart1And2 = 0x01,
    SafetyCodeApplicableCheckPart1 = 0x02,
    SafetyCodeNotApplicable = 0xFF
}

public record CCCommandAcceptedMessage (string SenderIdentifier, string ReceiverIdentifier, ushort ConfirmationTan) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ConfirmationTanOffset = 43;
    public static readonly ushort MessageType = 0x0044;

    public new static CCCommandAcceptedMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ConfirmationTanBytes = new byte[] { message[ConfirmationTanOffset], message[ConfirmationTanOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(ConfirmationTanBytes);
        var ConfirmationTan = BitConverter.ToUInt16(ConfirmationTanBytes);
        return new CCCommandAcceptedMessage(SenderIdentifier, ReceiverIdentifier, ConfirmationTan);
    }

    public override byte[] ToByteArray() {
        var result = new byte[45];
        result[0] = (byte)ProtocolType.CC;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        var ConfirmationTanBytes = BitConverter.GetBytes(ConfirmationTan);
        if (!BitConverter.IsLittleEndian) Array.Reverse(ConfirmationTanBytes);
        ConfirmationTanBytes.Take(2).ToArray().CopyTo(result, ConfirmationTanOffset);
        return result;
    }
}



public record CCReleaseForNormalOperationCommand (CCReleaseForNormalOperationCommandMessageType MessageType, string SenderIdentifier, string ReceiverIdentifier, ushort Tan) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int TanOffset = 43;
    private const int InformationTypeOffset = 45;
    public static readonly byte InformationType = 0xA0;

    public new static CCReleaseForNormalOperationCommand FromBytes(byte[] message) {
        var MessageType = (CCReleaseForNormalOperationCommandMessageType)message[MessageTypeOffset];
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var TanBytes = new byte[] { message[TanOffset], message[TanOffset + 1] };
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        var Tan = BitConverter.ToUInt16(TanBytes);
        return new CCReleaseForNormalOperationCommand(MessageType, SenderIdentifier, ReceiverIdentifier, Tan);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46];
        result[0] = (byte)ProtocolType.CC;
        result[InformationTypeOffset] = (byte)InformationType;
        {
            var bytes = BitConverter.GetBytes((ushort)MessageType);
            if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
            bytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        }
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        var TanBytes = BitConverter.GetBytes(Tan);
        if (!BitConverter.IsLittleEndian) Array.Reverse(TanBytes);
        TanBytes.Take(2).ToArray().CopyTo(result, TanOffset);
        return result;
    }
}

public enum CCReleaseForNormalOperationCommandMessageType : ushort {
    Regular = 0x0050,
    Confirmation = 0x0055
}
