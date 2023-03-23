using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record MovePointCommand (string SenderIdentifier, string ReceiverIdentifier, MovePointCommandCommandedPointPosition CommandedPointPosition) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int CommandedPointPositionOffset = 43;

    public MovePointCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var CommandedPointPosition = (MovePointCommandCommandedPointPosition)message[CommandedPointPositionOffset];
        return new MovePointCommand(SenderIdentifier, ReceiverIdentifier, CommandedPointPosition);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0001).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[CommandedPointPositionOffset] = (byte)CommandedPointPosition;
        return result;
    }
}

public enum MovePointCommandCommandedPointPosition : byte {
    SubsystemElectronicInterlockingRequestsARightHandPointMoving = 0x01,
    SubsystemElectronicInterlockingRequestsALeftHandPointMoving = 0x02
}



public record PointPositionMessage (string SenderIdentifier, string ReceiverIdentifier, PointPositionMessageReportedPointPosition ReportedPointPosition, PointPositionMessageReportedDegradedPointPosition ReportedDegradedPointPosition) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReportedPointPositionOffset = 43;
    private const int ReportedDegradedPointPositionOffset = 44;

    public PointPositionMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReportedPointPosition = (PointPositionMessageReportedPointPosition)message[ReportedPointPositionOffset];
        var ReportedDegradedPointPosition = (PointPositionMessageReportedDegradedPointPosition)message[ReportedDegradedPointPositionOffset];
        return new PointPositionMessage(SenderIdentifier, ReceiverIdentifier, ReportedPointPosition, ReportedDegradedPointPosition);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[45];
        result[0] = protocolType;
        BitConverter.GetBytes(0x000B).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReportedPointPositionOffset] = (byte)ReportedPointPosition;
        result[ReportedDegradedPointPositionOffset] = (byte)ReportedDegradedPointPosition;
        return result;
    }
}

public enum PointPositionMessageReportedPointPosition : byte {
    PointIsInARightHandPositionDefinedEndPosition = 0x01,
    PointIsInALeftHandPositionDefinedEndPosition = 0x02,
    PointIsInNoEndPosition = 0x03,
    PointIsTrailed = 0x04
}

public enum PointPositionMessageReportedDegradedPointPosition : byte {
    PointIsInADegradedRightHandPosition = 0x01,
    PointIsInADegradedLeftHandPosition = 0x02,
    PointIsNotInADegradedPosition = 0x03,
    DegradedPointPositionIsNotApplicable = 0xFF
}


public record TimeoutMessage (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public TimeoutMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new TimeoutMessage(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x000C).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record AbilityToMovePointMessage (string SenderIdentifier, string ReceiverIdentifier, AbilityToMovePointMessageReportedAbilityToMovePointStatus ReportedAbilityToMovePointStatus) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReportedAbilityToMovePointStatusOffset = 43;

    public AbilityToMovePointMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReportedAbilityToMovePointStatus = (AbilityToMovePointMessageReportedAbilityToMovePointStatus)message[ReportedAbilityToMovePointStatusOffset];
        return new AbilityToMovePointMessage(SenderIdentifier, ReceiverIdentifier, ReportedAbilityToMovePointStatus);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x000D).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReportedAbilityToMovePointStatusOffset] = (byte)ReportedAbilityToMovePointStatus;
        return result;
    }
}

public enum AbilityToMovePointMessageReportedAbilityToMovePointStatus : byte {
    PointIsAbleToMove = 0x01,
    PointIsUnableToMove = 0x02
}

