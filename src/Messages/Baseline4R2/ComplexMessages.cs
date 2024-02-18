using System;
using System.Linq;
using System.Text;

namespace EulynxLive.Messages.Baseline4R2;

public record AdjacentInterlockingSystemPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, AdjacentInterlockingSystemPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static AdjacentInterlockingSystemPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (AdjacentInterlockingSystemPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new AdjacentInterlockingSystemPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum AdjacentInterlockingSystemPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}

public record TrainDetectionSystemPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, TrainDetectionSystemPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static TrainDetectionSystemPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (TrainDetectionSystemPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new TrainDetectionSystemPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.TrainDetectionSystem;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum TrainDetectionSystemPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}


public record LightSignalPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, LightSignalPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static LightSignalPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (LightSignalPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new LightSignalPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.LightSignal;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum LightSignalPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}

public record PointPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, PointPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static PointPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (PointPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new PointPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.Point;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
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

public record RadioBlockCenterPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, RadioBlockCenterPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static RadioBlockCenterPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (RadioBlockCenterPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new RadioBlockCenterPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.RadioBlockCenter;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum RadioBlockCenterPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}


public record LevelCrossingPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, LevelCrossingPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static LevelCrossingPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (LevelCrossingPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new LevelCrossingPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.LevelCrossing;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum LevelCrossingPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}


public record CCPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, CCPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static CCPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (CCPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new CCPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.CC;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum CCPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}


public record GenericIOPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, GenericIOPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static GenericIOPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (GenericIOPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new GenericIOPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.GenericIO;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum GenericIOPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}


public record ExternalLevelCrossingSystemPdiVersionCheckMessage (string SenderIdentifier, string ReceiverIdentifier, ExternalLevelCrossingSystemPdiVersionCheckMessageResultPdiVersionCheck ResultPdiVersionCheck, byte SenderPdiVersion, byte ChecksumLength, byte[] Checksum) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int ResultPdiVersionCheckOffset = 43;
    private const int SenderPdiVersionOffset = 44;
    private const int ChecksumLengthOffset = 45;
    private const int ChecksumOffset = 46;

    public new static ExternalLevelCrossingSystemPdiVersionCheckMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var ResultPdiVersionCheck = (ExternalLevelCrossingSystemPdiVersionCheckMessageResultPdiVersionCheck)message[ResultPdiVersionCheckOffset];
        var SenderPdiVersion = (byte)message[SenderPdiVersionOffset];
        var ChecksumLength = (byte)message[ChecksumLengthOffset];
        var Checksum = message.Skip(ChecksumOffset).Take(ChecksumLength).ToArray();
        return new ExternalLevelCrossingSystemPdiVersionCheckMessage(SenderIdentifier, ReceiverIdentifier, ResultPdiVersionCheck, SenderPdiVersion, ChecksumLength, Checksum);
    }

    public override byte[] ToByteArray() {
        var result = new byte[46 + ChecksumLength];
        result[0] = (byte)ProtocolType.ExternalLevelCrossingSystem;
        BitConverter.GetBytes(0x0025).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[ResultPdiVersionCheckOffset] = (byte)ResultPdiVersionCheck;
        result[SenderPdiVersionOffset] = (byte)SenderPdiVersion;
        result[ChecksumLengthOffset] = (byte)ChecksumLength;
        Checksum.CopyTo(result, ChecksumOffset);
        return result;
    }
}

public enum ExternalLevelCrossingSystemPdiVersionCheckMessageResultPdiVersionCheck : byte {
    PDIVersionsFromReceiverAndSenderDoNotMatch = 0x01,
    PDIVersionsFromReceiverAndSenderDoMatch = 0x02,
}


/// <summary>
/// Eu.SCI-IO.PDI.158
/// </summary>
public record GenericIOSetOutputChannelsCommand(string SenderIdentifier, string ReceiverIdentifier, byte NumberOfChannels, GenericIOSetOutputChannelsCommandStateOfChannel[] StateOfChannels): Message(SenderIdentifier, ReceiverIdentifier)
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int NumberOfChannelsOffset = 43;
    private const int StateOfChannelsOffset = 44;
    public static readonly ushort MessageType = 0x0001; // Eu.SCI-IO.PDI.162

    public new static GenericIOSetOutputChannelsCommand FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var NumberOfChannels = (byte)message[NumberOfChannelsOffset];
        var StateOfChannels = message.Skip(StateOfChannelsOffset).Take(NumberOfChannels).Cast<GenericIOSetOutputChannelsCommandStateOfChannel>().ToArray();

        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        return new GenericIOSetOutputChannelsCommand(SenderIdentifier, ReceiverIdentifier, NumberOfChannels, StateOfChannels);
    }

    public override byte[] ToByteArray()
    {
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");

        var result = new byte[44 + NumberOfChannels];
        result[0] = (byte)ProtocolType.GenericIO;
        BitConverter.GetBytes(MessageType).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[NumberOfChannelsOffset] = (byte)NumberOfChannels;
        StateOfChannels.Cast<byte>().ToArray().CopyTo(result, StateOfChannelsOffset);
        return result;
    }
}

public enum GenericIOSetOutputChannelsCommandStateOfChannel : byte {
    // Eu.SCI-IO.PDI.168
    ChannelSwitchedOff = 0x01,
    // Eu.SCI-IO.PDI.169
    ChannelSwitchedOn = 0x02,
    // Eu.SCI-IO.PDI.170
    ChannelFlashing = 0x03,
}

/// <summary>
/// Eu.SCI-IO.PDI.170
/// </summary>
public record GenericIOStateOfOutputChannelsMessage(string SenderIdentifier, string ReceiverIdentifier, byte NumberOfChannels, GenericIOStateOfOutputChannelsMessageDisturbanceOfChannel[] DisturbanceOfChannels): Message(SenderIdentifier, ReceiverIdentifier)
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int NumberOfChannelsOffset = 43;
    private const int DisturbanceOfChannelsOffset = 44;
    public static readonly ushort MessageType = 0x0002; // Eu.SCI-IO.PDI.175

    public new static GenericIOStateOfOutputChannelsMessage FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var NumberOfChannels = (byte)message[NumberOfChannelsOffset];
        var DisturbanceOfChannels = message.Skip(DisturbanceOfChannelsOffset).Take(NumberOfChannels).Cast<GenericIOStateOfOutputChannelsMessageDisturbanceOfChannel>().ToArray();

        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        return new GenericIOStateOfOutputChannelsMessage(SenderIdentifier, ReceiverIdentifier, NumberOfChannels, DisturbanceOfChannels);
    }

    public override byte[] ToByteArray()
    {
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");

        var result = new byte[44 + NumberOfChannels];
        result[0] = (byte)ProtocolType.GenericIO;
        BitConverter.GetBytes(MessageType).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[NumberOfChannelsOffset] = (byte)NumberOfChannels;
        DisturbanceOfChannels.Cast<byte>().ToArray().CopyTo(result, DisturbanceOfChannelsOffset);
        return result;
    }
}

public enum GenericIOStateOfOutputChannelsMessageDisturbanceOfChannel : byte {
    // Eu.SCI-IO.PDI.180
    ChannelNotPhysicallyDisturbed = 0x01,
    // Eu.SCI-IO.PDI.181
    ChannelPhysicallyDisturbed = 0x02,
}

/// <summary>
/// Eu.SCI-IO.PDI.184
/// </summary>
public record GenericIOStateOfInputChannelsMessage(string SenderIdentifier, string ReceiverIdentifier, byte NumberOfChannels, GenericIOStateOfInputChannelsMessageStateOfChannel[] StateOfChannels): Message(SenderIdentifier, ReceiverIdentifier)
{
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int NumberOfChannelsOffset = 43;
    private const int StateOfChannelsOffset = 44;
    public static readonly ushort MessageType = 0x0003; // Eu.SCI-IO.PDI.189

    public new static GenericIOStateOfInputChannelsMessage FromBytes(byte[] message)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var NumberOfChannels = (byte)message[NumberOfChannelsOffset];
        var StateOfChannels = message.Skip(StateOfChannelsOffset).Take(NumberOfChannels).Cast<GenericIOStateOfInputChannelsMessageStateOfChannel>().ToArray();

        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        return new GenericIOStateOfInputChannelsMessage(SenderIdentifier, ReceiverIdentifier, NumberOfChannels, StateOfChannels);
    }

    public override byte[] ToByteArray()
    {
        var result = new byte[44 + NumberOfChannels];
        result[0] = (byte)ProtocolType.GenericIO;
        BitConverter.GetBytes(MessageType).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[NumberOfChannelsOffset] = (byte)NumberOfChannels;
        StateOfChannels.Cast<byte>().ToArray().CopyTo(result, StateOfChannelsOffset);
        return result;
    }
}

public enum GenericIOStateOfInputChannelsMessageStateOfChannel : byte {
    // Eu.SCI-IO.PDI.195
    ChannelSwitchedOff = 0x01,
    // Eu.SCI-IO.PDI.196
    ChannelSwitchedOn = 0x02,
    // Eu.SCI-IO.PDI.197
    ChannelDisturbed = 0x03,
}
