using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace EulynxLive.Messages.Baseline4R1;

/// <summary>
/// Eu.SCI-IO.PDI.158
///
/// </summary>
/// <value></value>
public record GenericIOSetOutputChannelsCommand(string SenderIdentifier, string ReceiverIdentifier, byte NumberOfChannels, byte[] ChannelStates): Message(SenderIdentifier, ReceiverIdentifier) 
{
    public static readonly ushort MessageType = 0x0001; // Eu.SCI-ILS.PDI.162

    public new static GenericIOSetOutputChannelsCommand FromBytes(byte[] data)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(data, 3, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(data, 23, 20);
        var NumberOfChannels = data[43];
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        var ChannelStates = data.Skip(44).Take(NumberOfChannels).ToArray();

        return new GenericIOSetOutputChannelsCommand(SenderIdentifier, ReceiverIdentifier, NumberOfChannels, ChannelStates);
    }

    public override byte[] ToByteArray()
    {
        List<byte> data = new List<byte>();

        data.Add((byte)ProtocolType.GenericIO);
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        data.AddRange(MessageTypeBytes);
        data.AddRange(Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')));
        data.AddRange(Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')));
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        data.Add(NumberOfChannels);
        data.AddRange(ChannelStates);

        return data.ToArray();
    }
}

/// <summary>
/// Eu.SCI-IO.PDI.170
///
/// </summary>
/// <value></value>
public record GenericIOStateOfOutputChannelsMessage(string SenderIdentifier, string ReceiverIdentifier, byte NumberOfChannels, byte[] ChannelStates): Message(SenderIdentifier, ReceiverIdentifier) 
{
    public static readonly ushort MessageType = 0x0002; // Eu.SCI-IO.PDI.175

    public new static GenericIOStateOfOutputChannelsMessage FromBytes(byte[] data)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(data, 3, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(data, 23, 20);
        var NumberOfChannels = data[43];
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        var ChannelStates = data.Skip(44).Take(NumberOfChannels).ToArray();

        return new GenericIOStateOfOutputChannelsMessage(SenderIdentifier, ReceiverIdentifier, NumberOfChannels, ChannelStates);
    }

    public override byte[] ToByteArray()
    {
        List<byte> data = new List<byte>();

        data.Add((byte)ProtocolType.GenericIO);
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        data.AddRange(MessageTypeBytes);
        data.AddRange(Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')));
        data.AddRange(Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')));
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        data.Add(NumberOfChannels);
        data.AddRange(ChannelStates);

        return data.ToArray();
    }
}

/// <summary>
/// Eu.SCI-IO.PDI.184
///
/// </summary>
/// <value></value>
public record GenericIOStateOfInputChannelsMessage(string SenderIdentifier, string ReceiverIdentifier, byte NumberOfChannels, byte[] ChannelStates): Message(SenderIdentifier, ReceiverIdentifier) 
{
    public static readonly ushort MessageType = 0x0003; // Eu.SCI-IO.PDI.189

    public new static GenericIOStateOfInputChannelsMessage FromBytes(byte[] data)
    {
        var SenderIdentifier = Encoding.Latin1.GetString(data, 3, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(data, 23, 20);
        var NumberOfChannels = data[43];
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        var ChannelStates = data.Skip(44).Take(NumberOfChannels).ToArray();

        return new GenericIOStateOfInputChannelsMessage(SenderIdentifier, ReceiverIdentifier, NumberOfChannels, ChannelStates);
    }

    public override byte[] ToByteArray()
    {
        List<byte> data = new List<byte>();

        data.Add((byte)ProtocolType.GenericIO);
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        data.AddRange(MessageTypeBytes);
        data.AddRange(Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')));
        data.AddRange(Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')));
        if (NumberOfChannels > 51) throw new ArgumentOutOfRangeException("NumberOfChannels", "The maximum number of channels is 51");
        data.Add(NumberOfChannels);
        data.AddRange(ChannelStates);

        return data.ToArray();
    }
}
