using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R2;

public record LightSignalSetLuminosityCommand (string SenderIdentifier, string ReceiverIdentifier, LightSignalSetLuminosityCommandByteNr43:Luminosity ByteNr43:Luminosity) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ByteNr43:LuminosityOffset = 43;
    public static readonly ushort MessageType = 0x0002;

    public new static LightSignalSetLuminosityCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ByteNr43:Luminosity = (LightSignalSetLuminosityCommandByteNr43:Luminosity)message[ByteNr43:LuminosityOffset];
        return new LightSignalSetLuminosityCommand(SenderIdentifier, ReceiverIdentifier, ByteNr43:Luminosity);
    }

    public override byte[] ToByteArray() {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.LightSignal;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ByteNr43:LuminosityOffset] = (byte)ByteNr43:Luminosity;
        return result;
    }
}

public enum LightSignalSetLuminosityCommandByteNr43:Luminosity : byte {
    LuminosityForDay = 0x01,
    LuminosityForNight = 0x02,
    IntentionallyDeleted = 0xFE
}

public record LightSignalSetLuminosityMessage (string SenderIdentifier, string ReceiverIdentifier, LightSignalSetLuminosityMessageByteNr43:Luminosity ByteNr43:Luminosity) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ByteNr43:LuminosityOffset = 43;
    public static readonly ushort MessageType = 0x0004;

    public new static LightSignalSetLuminosityMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ByteNr43:Luminosity = (LightSignalSetLuminosityMessageByteNr43:Luminosity)message[ByteNr43:LuminosityOffset];
        return new LightSignalSetLuminosityMessage(SenderIdentifier, ReceiverIdentifier, ByteNr43:Luminosity);
    }

    public override byte[] ToByteArray() {
        var result = new byte[44];
        result[0] = (byte)ProtocolType.LightSignal;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ByteNr43:LuminosityOffset] = (byte)ByteNr43:Luminosity;
        return result;
    }
}

public enum LightSignalSetLuminosityMessageByteNr43:Luminosity : byte {
    LuminosityForDay = 0x01,
    LuminosityForNight = 0x02,
    IntentionallyDeleted = 0xFE
}
