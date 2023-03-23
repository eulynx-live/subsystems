using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;



public record StartInitialisationMessage (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public StartInitialisationMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new StartInitialisationMessage(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0022).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record StatusReportCompletedMessage (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public StatusReportCompletedMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new StatusReportCompletedMessage(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0026).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record InitialisationCompletedMessage (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public InitialisationCompletedMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new InitialisationCompletedMessage(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0023).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record PdiAvailableMessage (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public PdiAvailableMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new PdiAvailableMessage(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0029).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record PdiNotAvailableMessage (string SenderIdentifier, string ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public PdiNotAvailableMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new PdiNotAvailableMessage(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[43];
        result[0] = protocolType;
        BitConverter.GetBytes(0x002A).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}




public record ResetPdiMessage (string SenderIdentifier, string ReceiverIdentifier, ResetPdiMessageResetReason ResetReason) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResetReasonOffset = 43;

    public ResetPdiMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResetReason = (ResetPdiMessageResetReason)message[ResetReasonOffset];
        return new ResetPdiMessage(SenderIdentifier, ReceiverIdentifier, ResetReason);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x002B).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResetReasonOffset] = (byte)ResetReason;
        return result;
    }
}

public enum ResetPdiMessageResetReason : byte {
    ProtocolError = 0x01,
    FormalTelegramError = 0x02,
    ContentTelegramError = 0x03
}

