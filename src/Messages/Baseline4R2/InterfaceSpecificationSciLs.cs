using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R2;

public record LightSignalSetLuminosityCommand (string SenderIdentifier, string ReceiverIdentifier, LightSignalSetLuminosityCommandLuminosity Luminosity) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int LuminosityOffset = 43;
    public static readonly ushort MessageType = 0x0002;

    public new static LightSignalSetLuminosityCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var Luminosity = (LightSignalSetLuminosityCommandLuminosity)message[LuminosityOffset];
        return new LightSignalSetLuminosityCommand(SenderIdentifier, ReceiverIdentifier, Luminosity);
    }

    public override byte[] ToByteArray() {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.LightSignal;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[LuminosityOffset] = (byte)Luminosity;
        return result;
    }
}

public enum LightSignalSetLuminosityCommandLuminosity : byte {
    LuminosityForDay = 0x01,
    LuminosityForNight = 0x02,
    IntentionallyDeleted = 0xFE
}

public record LightSignalSetLuminosityMessage (string SenderIdentifier, string ReceiverIdentifier, LightSignalSetLuminosityMessageLuminosity Luminosity) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int LuminosityOffset = 43;
    public static readonly ushort MessageType = 0x0004;

    public new static LightSignalSetLuminosityMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var Luminosity = (LightSignalSetLuminosityMessageLuminosity)message[LuminosityOffset];
        return new LightSignalSetLuminosityMessage(SenderIdentifier, ReceiverIdentifier, Luminosity);
    }

    public override byte[] ToByteArray() {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.LightSignal;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[LuminosityOffset] = (byte)Luminosity;
        return result;
    }
}

public enum LightSignalSetLuminosityMessageLuminosity : byte {
    LuminosityForDay = 0x01,
    LuminosityForNight = 0x02,
    IntentionallyDeleted = 0xFE
}
