using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record CCAbortCommandCommand (string SenderIdentifier, string ReceiverIdentifier, byte ConfirmationTan) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ConfirmationTanOffset = 43;

    public new static CCAbortCommandCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ConfirmationTan = (byte)message[ConfirmationTanOffset];
        return new CCAbortCommandCommand(SenderIdentifier, ReceiverIdentifier, ConfirmationTan);
    }

    public override byte[] ToByteArray() {
        var result = new byte[45];
        result[0] = (byte)ProtocolType.CC;
        BitConverter.GetBytes(0x0065).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ConfirmationTanOffset] = (byte)ConfirmationTan;
        return result;
    }
}




public record CCReleaseForNormalOperationCommand (string SenderIdentifier, string ReceiverIdentifier, byte Tan) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int TanOffset = 43;
    private const int InformationTypeOffset = 45;

    public new static CCReleaseForNormalOperationCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var Tan = (byte)message[TanOffset];
        return new CCReleaseForNormalOperationCommand(SenderIdentifier, ReceiverIdentifier, Tan);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46];
        result[0] = (byte)ProtocolType.CC;
        BitConverter.GetBytes(0x0050).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[TanOffset] = (byte)Tan;
        return result;
    }
}





public record CCCommandAcceptedMessage (string SenderIdentifier, string ReceiverIdentifier, byte ConfirmationTan) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ConfirmationTanOffset = 43;

    public new static CCCommandAcceptedMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ConfirmationTan = (byte)message[ConfirmationTanOffset];
        return new CCCommandAcceptedMessage(SenderIdentifier, ReceiverIdentifier, ConfirmationTan);
    }

    public override byte[] ToByteArray() {
        var result = new byte[45];
        result[0] = (byte)ProtocolType.CC;
        BitConverter.GetBytes(0x0044).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ConfirmationTanOffset] = (byte)ConfirmationTan;
        return result;
    }
}



