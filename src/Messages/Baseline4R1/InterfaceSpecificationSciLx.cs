using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record ExternalLevelCrossingSystemCrossingClearCommand (string SenderIdentifier, string ReceiverIdentifier) : IByteSerializable {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;

    public static ExternalLevelCrossingSystemCrossingClearCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        
        return new ExternalLevelCrossingSystemCrossingClearCommand(SenderIdentifier, ReceiverIdentifier);
    }

    public byte[] ToByteArray() {
        var result = new byte[43];
        result[0] = (byte)ProtocolType.ExternalLevelCrossingSystem;
        BitConverter.GetBytes(0x0007).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        
        return result;
    }
}





