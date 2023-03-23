using System;
using System.Linq;
using System.Text;

namespace EulynxLive.Messages.Baseline4R1;

public record PointPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, PointPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : IByteSerializable {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public static PointPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (PointPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new PointPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public byte[] ToByteArray() {
        var result = new byte[44 + ChecksumLength];
        result[0] = (byte)ProtocolType.Point;
        BitConverter.GetBytes(0x0024).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum PointPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}
