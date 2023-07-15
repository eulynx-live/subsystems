using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record LightSignalIndicateSignalAspectCommand (string SenderIdentifier, string ReceiverIdentifier, byte CodeForBasicAspectTypes, byte CodeForExtensionOfBasicAspectTypes, byte SpeedIndicators, byte SpeedIndicatorAnnouncements, byte DirectionIndicators, byte DirectionIndicatorAnnouncements, byte DowngradeInformation, LightSignalIndicateSignalAspectCommandRouteInformation RouteInformation, LightSignalIndicateSignalAspectCommandSignalAspectIntentionallyDark SignalAspectIntentionallyDark, byte[] SpecifiedByNationalRequirements) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int CodeForBasicAspectTypesOffset = 43;
    private const int CodeForExtensionOfBasicAspectTypesOffset = 44;
    private const int SpeedIndicatorsOffset = 45;
    private const int SpeedIndicatorAnnouncementsOffset = 46;
    private const int DirectionIndicatorsOffset = 47;
    private const int DirectionIndicatorAnnouncementsOffset = 48;
    private const int DowngradeInformationOffset = 49;
    private const int RouteInformationOffset = 50;
    private const int SignalAspectIntentionallyDarkOffset = 51;
    private const int SpecifiedByNationalRequirementsOffset = 52;
    public static readonly ushort MessageType = 0x0001;

    public new static LightSignalIndicateSignalAspectCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var CodeForBasicAspectTypes = (byte)message[CodeForBasicAspectTypesOffset];
        var CodeForExtensionOfBasicAspectTypes = (byte)message[CodeForExtensionOfBasicAspectTypesOffset];
        var SpeedIndicators = (byte)message[SpeedIndicatorsOffset];
        var SpeedIndicatorAnnouncements = (byte)message[SpeedIndicatorAnnouncementsOffset];
        var DirectionIndicators = (byte)message[DirectionIndicatorsOffset];
        var DirectionIndicatorAnnouncements = (byte)message[DirectionIndicatorAnnouncementsOffset];
        var DowngradeInformation = (byte)message[DowngradeInformationOffset];
        var RouteInformation = (LightSignalIndicateSignalAspectCommandRouteInformation)message[RouteInformationOffset];
        var SignalAspectIntentionallyDark = (LightSignalIndicateSignalAspectCommandSignalAspectIntentionallyDark)message[SignalAspectIntentionallyDarkOffset];
        var SpecifiedByNationalRequirements = new byte[9];
        Buffer.BlockCopy(message, SpecifiedByNationalRequirementsOffset, SpecifiedByNationalRequirements, 0, 9);
        return new LightSignalIndicateSignalAspectCommand(SenderIdentifier, ReceiverIdentifier, CodeForBasicAspectTypes, CodeForExtensionOfBasicAspectTypes, SpeedIndicators, SpeedIndicatorAnnouncements, DirectionIndicators, DirectionIndicatorAnnouncements, DowngradeInformation, RouteInformation, SignalAspectIntentionallyDark, SpecifiedByNationalRequirements);
    }

    public override byte[] ToByteArray() {
        var result = new byte[61];
        result[0] = (byte)ProtocolType.LightSignal;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[CodeForBasicAspectTypesOffset] = (byte)CodeForBasicAspectTypes;
        result[CodeForExtensionOfBasicAspectTypesOffset] = (byte)CodeForExtensionOfBasicAspectTypes;
        result[SpeedIndicatorsOffset] = (byte)SpeedIndicators;
        result[SpeedIndicatorAnnouncementsOffset] = (byte)SpeedIndicatorAnnouncements;
        result[DirectionIndicatorsOffset] = (byte)DirectionIndicators;
        result[DirectionIndicatorAnnouncementsOffset] = (byte)DirectionIndicatorAnnouncements;
        result[DowngradeInformationOffset] = (byte)DowngradeInformation;
        result[RouteInformationOffset] = (byte)RouteInformation;
        result[SignalAspectIntentionallyDarkOffset] = (byte)SignalAspectIntentionallyDark;
        SpecifiedByNationalRequirements.CopyTo(result, SpecifiedByNationalRequirementsOffset);
        return result;
    }
}

public enum LightSignalIndicateSignalAspectCommandRouteInformation : byte {
    SignalInRearOfRoute1InReferenceDirection = 0x1,
    SignalInRearOfRoute2InReferenceDirection = 0x2,
    SignalInRearOfRoute3InReferenceDirection = 0x3,
    SignalInRearOfRoute4InReferenceDirection = 0x4,
    NoInformationSignalInRear = 0xE,
    RouteInformationNotApplicable = 0xF
}

public enum LightSignalIndicateSignalAspectCommandSignalAspectIntentionallyDark : byte {
    CommandedSignalAspectOrTheRelatedDowngradedAspectShallBeIndicatedInTheSetLuminosity = 0x01,
    CommandedSignalAspectOrTheRelatedDowngradedAspectShallBeIndicatedDark = 0x0F,
    IntentionallyDarkNotApplicable = 0xFF
}


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


public record LightSignalIndicatedSignalAspectMessage (string SenderIdentifier, string ReceiverIdentifier, byte CodeForBasicAspectTypes, byte CodeForExtensionOfBasicAspectTypes, byte SpeedIndicators, byte SpeedIndicatorAnnouncements, byte DirectionIndicators, byte DirectionIndicatorAnnouncements, byte DowngradeInformation, LightSignalIndicatedSignalAspectMessageRouteInformation RouteInformation, LightSignalIndicatedSignalAspectMessageSignalAspectIntentionallyDark SignalAspectIntentionallyDark, byte[] SpecifiedByNationalRequirements) : Message(SenderIdentifier, ReceiverIdentifier) {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int CodeForBasicAspectTypesOffset = 43;
    private const int CodeForExtensionOfBasicAspectTypesOffset = 44;
    private const int SpeedIndicatorsOffset = 45;
    private const int SpeedIndicatorAnnouncementsOffset = 46;
    private const int DirectionIndicatorsOffset = 47;
    private const int DirectionIndicatorAnnouncementsOffset = 48;
    private const int DowngradeInformationOffset = 49;
    private const int RouteInformationOffset = 50;
    private const int SignalAspectIntentionallyDarkOffset = 51;
    private const int SpecifiedByNationalRequirementsOffset = 52;
    public static readonly ushort MessageType = 0x0003;

    public new static LightSignalIndicatedSignalAspectMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var CodeForBasicAspectTypes = (byte)message[CodeForBasicAspectTypesOffset];
        var CodeForExtensionOfBasicAspectTypes = (byte)message[CodeForExtensionOfBasicAspectTypesOffset];
        var SpeedIndicators = (byte)message[SpeedIndicatorsOffset];
        var SpeedIndicatorAnnouncements = (byte)message[SpeedIndicatorAnnouncementsOffset];
        var DirectionIndicators = (byte)message[DirectionIndicatorsOffset];
        var DirectionIndicatorAnnouncements = (byte)message[DirectionIndicatorAnnouncementsOffset];
        var DowngradeInformation = (byte)message[DowngradeInformationOffset];
        var RouteInformation = (LightSignalIndicatedSignalAspectMessageRouteInformation)message[RouteInformationOffset];
        var SignalAspectIntentionallyDark = (LightSignalIndicatedSignalAspectMessageSignalAspectIntentionallyDark)message[SignalAspectIntentionallyDarkOffset];
        var SpecifiedByNationalRequirements = new byte[9];
        Buffer.BlockCopy(message, SpecifiedByNationalRequirementsOffset, SpecifiedByNationalRequirements, 0, 9);
        return new LightSignalIndicatedSignalAspectMessage(SenderIdentifier, ReceiverIdentifier, CodeForBasicAspectTypes, CodeForExtensionOfBasicAspectTypes, SpeedIndicators, SpeedIndicatorAnnouncements, DirectionIndicators, DirectionIndicatorAnnouncements, DowngradeInformation, RouteInformation, SignalAspectIntentionallyDark, SpecifiedByNationalRequirements);
    }

    public override byte[] ToByteArray() {
        var result = new byte[61];
        result[0] = (byte)ProtocolType.LightSignal;
        var MessageTypeBytes = BitConverter.GetBytes(MessageType);
        if (!BitConverter.IsLittleEndian) Array.Reverse(MessageTypeBytes);
        MessageTypeBytes.Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[CodeForBasicAspectTypesOffset] = (byte)CodeForBasicAspectTypes;
        result[CodeForExtensionOfBasicAspectTypesOffset] = (byte)CodeForExtensionOfBasicAspectTypes;
        result[SpeedIndicatorsOffset] = (byte)SpeedIndicators;
        result[SpeedIndicatorAnnouncementsOffset] = (byte)SpeedIndicatorAnnouncements;
        result[DirectionIndicatorsOffset] = (byte)DirectionIndicators;
        result[DirectionIndicatorAnnouncementsOffset] = (byte)DirectionIndicatorAnnouncements;
        result[DowngradeInformationOffset] = (byte)DowngradeInformation;
        result[RouteInformationOffset] = (byte)RouteInformation;
        result[SignalAspectIntentionallyDarkOffset] = (byte)SignalAspectIntentionallyDark;
        SpecifiedByNationalRequirements.CopyTo(result, SpecifiedByNationalRequirementsOffset);
        return result;
    }
}

public enum LightSignalIndicatedSignalAspectMessageRouteInformation : byte {
    SignalInRearOfRoute1InReferenceDirection = 0x1,
    SignalInRearOfRoute2InReferenceDirection = 0x2,
    SignalInRearOfRoute3InReferenceDirection = 0x3,
    SignalInRearOfRoute4InReferenceDirection = 0x4,
    NoInformationSignalInRear = 0xE,
    RouteInformationNotApplicable = 0xF
}

public enum LightSignalIndicatedSignalAspectMessageSignalAspectIntentionallyDark : byte {
    CommandedSignalAspectOrTheRelatedDowngradedAspectIsIndicatedInTheSetLuminosity = 0x01,
    CommandedSignalAspectOrTheRelatedDowngradedAspectIsIndicatedDark = 0x0F,
    IntentionallyDarkNotApplicable = 0xFF
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

