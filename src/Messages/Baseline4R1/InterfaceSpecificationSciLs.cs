using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record SetLuminosityCommand (string SenderIdentifier, string ReceiverIdentifier, SetLuminosityCommandLuminosity Luminosity) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int LuminosityOffset = 43;

    public SetLuminosityCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var Luminosity = (SetLuminosityCommandLuminosity)message[LuminosityOffset];
        return new SetLuminosityCommand(SenderIdentifier, ReceiverIdentifier, Luminosity);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0002).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[LuminosityOffset] = (byte)Luminosity;
        return result;
    }
}

public enum SetLuminosityCommandLuminosity : byte {
    LuminosityForDay = 0x01,
    LuminosityForNight = 0x02,
    IntentionallyDeleted = 0xFE
}



public record SetLuminosityMessage (string SenderIdentifier, string ReceiverIdentifier, SetLuminosityMessageLuminosity Luminosity) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int LuminosityOffset = 43;

    public SetLuminosityMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var SeceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var Luminosity = (SetLuminosityMessageLuminosity)message[LuminosityOffset];
        return new SetLuminosityMessage(SenderIdentifier, ReceiverIdentifier, Luminosity);
    }

    public byte[] ToByteArray(byte protocolType) {
        var result = new byte[44];
        result[0] = protocolType;
        BitConverter.GetBytes(0x0004).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[LuminosityOffset] = (byte)Luminosity;
        return result;
    }
}

public enum SetLuminosityMessageLuminosity : byte {
    LuminosityForDay = 0x01,
    LuminosityForNight = 0x02,
    IntentionallyDeleted = 0xFE
}

