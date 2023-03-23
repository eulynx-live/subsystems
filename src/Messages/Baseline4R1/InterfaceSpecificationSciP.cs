using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record PointMovePointCommand (string SenderIdentifier, string ReceiverIdentifier, PointMovePointCommandCommandedPointPosition CommandedPointPosition) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int CommandedPointPositionOffset = 43;

    public static new PointMovePointCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var CommandedPointPosition = (PointMovePointCommandCommandedPointPosition)message[CommandedPointPositionOffset];
        return new PointMovePointCommand(SenderIdentifier, ReceiverIdentifier, CommandedPointPosition);
    }

    public override byte[] ToByteArray() {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.Point;
        BitConverter.GetBytes(0x0001).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[CommandedPointPositionOffset] = (byte)CommandedPointPosition;
        return result;
    }
}

public enum PointMovePointCommandCommandedPointPosition : byte {
    SubsystemElectronicInterlockingRequestsARightHandPointMoving = 0x01,
    SubsystemElectronicInterlockingRequestsALeftHandPointMoving = 0x02
}



public record PointPointPositionMessage (string SenderIdentifier, string ReceiverIdentifier, PointPointPositionMessageReportedPointPosition ReportedPointPosition, PointPointPositionMessageReportedDegradedPointPosition ReportedDegradedPointPosition) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReportedPointPositionOffset = 43;
    private const int ReportedDegradedPointPositionOffset = 44;

    public static new PointPointPositionMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReportedPointPosition = (PointPointPositionMessageReportedPointPosition)message[ReportedPointPositionOffset];
        var ReportedDegradedPointPosition = (PointPointPositionMessageReportedDegradedPointPosition)message[ReportedDegradedPointPositionOffset];
        return new PointPointPositionMessage(SenderIdentifier, ReceiverIdentifier, ReportedPointPosition, ReportedDegradedPointPosition);
    }

    public override byte[] ToByteArray() {
        var result = new byte[45];
        result[0] = (byte)ProtocolType.Point;
        BitConverter.GetBytes(0x000B).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReportedPointPositionOffset] = (byte)ReportedPointPosition;
        result[ReportedDegradedPointPositionOffset] = (byte)ReportedDegradedPointPosition;
        return result;
    }
}

public enum PointPointPositionMessageReportedPointPosition : byte {
    PointIsInARightHandPositionDefinedEndPosition = 0x01,
    PointIsInALeftHandPositionDefinedEndPosition = 0x02,
    PointIsInNoEndPosition = 0x03,
    PointIsTrailed = 0x04
}

public enum PointPointPositionMessageReportedDegradedPointPosition : byte {
    PointIsInADegradedRightHandPosition = 0x01,
    PointIsInADegradedLeftHandPosition = 0x02,
    PointIsNotInADegradedPosition = 0x03,
    DegradedPointPositionIsNotApplicable = 0xFF
}


public record PointTimeoutMessage (string SenderIdentifier, string ReceiverIdentifier) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public static new PointTimeoutMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new PointTimeoutMessage(SenderIdentifier, ReceiverIdentifier);
    }

    public override byte[] ToByteArray() {
        var result = new byte[43];
        result[0] = (byte)ProtocolType.Point;
        BitConverter.GetBytes(0x000C).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record PointAbilityToMovePointMessage (string SenderIdentifier, string ReceiverIdentifier, PointAbilityToMovePointMessageReportedAbilityToMovePointStatus ReportedAbilityToMovePointStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ReportedAbilityToMovePointStatusOffset = 43;

    public static new PointAbilityToMovePointMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ReportedAbilityToMovePointStatus = (PointAbilityToMovePointMessageReportedAbilityToMovePointStatus)message[ReportedAbilityToMovePointStatusOffset];
        return new PointAbilityToMovePointMessage(SenderIdentifier, ReceiverIdentifier, ReportedAbilityToMovePointStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.Point;
        BitConverter.GetBytes(0x000D).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ReportedAbilityToMovePointStatusOffset] = (byte)ReportedAbilityToMovePointStatus;
        return result;
    }
}

public enum PointAbilityToMovePointMessageReportedAbilityToMovePointStatus : byte {
    PointIsAbleToMove = 0x01,
    PointIsUnableToMove = 0x02
}

