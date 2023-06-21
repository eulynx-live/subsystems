using System;
using System.Text;
using System.Linq;

namespace EulynxLive.Messages.Baseline4R1;

public record AdjacentInterlockingSystemAccessRestrictionRequestCommand (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemAccessRestrictionRequestCommandAccessRestrictionType AccessRestrictionType) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int AccessRestrictionTypeOffset = 63;

    public new static AdjacentInterlockingSystemAccessRestrictionRequestCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var AccessRestrictionType = (AdjacentInterlockingSystemAccessRestrictionRequestCommandAccessRestrictionType)message[AccessRestrictionTypeOffset];
        return new AdjacentInterlockingSystemAccessRestrictionRequestCommand(SenderIdentifier, ReceiverIdentifier, BoundaryId, AccessRestrictionType);
    }

    public override byte[] ToByteArray() {
        var result = new byte[64];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0003).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[AccessRestrictionTypeOffset] = (byte)AccessRestrictionType;
        return result;
    }
}

public enum AdjacentInterlockingSystemAccessRestrictionRequestCommandAccessRestrictionType : byte {
    NoAccess = 0x01,
    WorkTrack = 0x02,
    TrackOutOfService = 0x03,
    EmergencyTrain = 0x04,
    SecondaryVehicle = 0x05,
    WorkTeam = 0x06,
    LevelCrossingInDegradedOperation = 0x07,
    ClearanceCheckRequired = 0x08,
    SectionCheckRequired = 0x09,
    NoElectricTrains = 0x10,
    ExtraordinaryTransport = 0x11,
    CatenaryOffPantographDown = 0x12,
    WrittenOrderRequired = 0x13,
    AccessRestrictionTypeNotApplicable = 0xFF
}


public record AdjacentInterlockingSystemFlankProtectionRequestCommand (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemFlankProtectionRequestCommandFlankProtectionRequestType FlankProtectionRequestType) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int FlankProtectionRequestTypeOffset = 63;

    public new static AdjacentInterlockingSystemFlankProtectionRequestCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var FlankProtectionRequestType = (AdjacentInterlockingSystemFlankProtectionRequestCommandFlankProtectionRequestType)message[FlankProtectionRequestTypeOffset];
        return new AdjacentInterlockingSystemFlankProtectionRequestCommand(SenderIdentifier, ReceiverIdentifier, BoundaryId, FlankProtectionRequestType);
    }

    public override byte[] ToByteArray() {
        var result = new byte[64];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0005).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[FlankProtectionRequestTypeOffset] = (byte)FlankProtectionRequestType;
        return result;
    }
}

public enum AdjacentInterlockingSystemFlankProtectionRequestCommandFlankProtectionRequestType : byte {
    Provision = 0x01,
    Cancellation = 0x02
}


public record AdjacentInterlockingSystemRouteRequestCommand (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte RouteId, AdjacentInterlockingSystemRouteRequestCommandRouteType RouteType) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int RouteIdOffset = 63;
    private const int RouteTypeOffset = 83;

    public new static AdjacentInterlockingSystemRouteRequestCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var RouteId = (byte)message[RouteIdOffset];
        var RouteType = (AdjacentInterlockingSystemRouteRequestCommandRouteType)message[RouteTypeOffset];
        return new AdjacentInterlockingSystemRouteRequestCommand(SenderIdentifier, ReceiverIdentifier, BoundaryId, RouteId, RouteType);
    }

    public override byte[] ToByteArray() {
        var result = new byte[84];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0007).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[RouteIdOffset] = (byte)RouteId;
        result[RouteTypeOffset] = (byte)RouteType;
        return result;
    }
}

public enum AdjacentInterlockingSystemRouteRequestCommandRouteType : byte {
    MainRoute = 0x01,
    ShuntingRoute = 0x02,
    OnSightRoute = 0x03,
    SrTrainRoute = 0x04,
    SpecialTrainRoute = 0x05,
    TemporaryShuntingArea = 0x06
}


public record AdjacentInterlockingSystemRouteCancellationRequestCommand (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte RouteId) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int RouteIdOffset = 63;

    public new static AdjacentInterlockingSystemRouteCancellationRequestCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var RouteId = (byte)message[RouteIdOffset];
        return new AdjacentInterlockingSystemRouteCancellationRequestCommand(SenderIdentifier, ReceiverIdentifier, BoundaryId, RouteId);
    }

    public override byte[] ToByteArray() {
        var result = new byte[83];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x000A).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[RouteIdOffset] = (byte)RouteId;
        return result;
    }
}




public record AdjacentInterlockingSystemRoutePretestRequestCommand (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte RouteId, AdjacentInterlockingSystemRoutePretestRequestCommandRouteType RouteType) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int RouteIdOffset = 63;
    private const int RouteTypeOffset = 83;

    public new static AdjacentInterlockingSystemRoutePretestRequestCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var RouteId = (byte)message[RouteIdOffset];
        var RouteType = (AdjacentInterlockingSystemRoutePretestRequestCommandRouteType)message[RouteTypeOffset];
        return new AdjacentInterlockingSystemRoutePretestRequestCommand(SenderIdentifier, ReceiverIdentifier, BoundaryId, RouteId, RouteType);
    }

    public override byte[] ToByteArray() {
        var result = new byte[84];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x000F).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[RouteIdOffset] = (byte)RouteId;
        result[RouteTypeOffset] = (byte)RouteType;
        return result;
    }
}

public enum AdjacentInterlockingSystemRoutePretestRequestCommandRouteType : byte {
    MainRoute = 0x01,
    ShuntingRoute = 0x02,
    OnSightRoute = 0x03,
    SrTrainRoute = 0x04,
    SpecialTrainRoute = 0x05,
    TemporaryShuntingArea = 0x06
}


public record AdjacentInterlockingSystemRouteReleaseInhibitionActivationRequestCommand (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;

    public new static AdjacentInterlockingSystemRouteReleaseInhibitionActivationRequestCommand FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        return new AdjacentInterlockingSystemRouteReleaseInhibitionActivationRequestCommand(SenderIdentifier, ReceiverIdentifier, BoundaryId);
    }

    public override byte[] ToByteArray() {
        var result = new byte[63];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0011).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        return result;
    }
}





public record AdjacentInterlockingSystemActivationZoneStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte ActivationZoneId, AdjacentInterlockingSystemActivationZoneStatusMessageActivationZoneStatus ActivationZoneStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int ActivationZoneIdOffset = 63;
    private const int ActivationZoneStatusOffset = 83;

    public new static AdjacentInterlockingSystemActivationZoneStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var ActivationZoneId = (byte)message[ActivationZoneIdOffset];
        var ActivationZoneStatus = (AdjacentInterlockingSystemActivationZoneStatusMessageActivationZoneStatus)message[ActivationZoneStatusOffset];
        return new AdjacentInterlockingSystemActivationZoneStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, ActivationZoneId, ActivationZoneStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[84];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0001).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[ActivationZoneIdOffset] = (byte)ActivationZoneId;
        result[ActivationZoneStatusOffset] = (byte)ActivationZoneStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemActivationZoneStatusMessageActivationZoneStatus : byte {
    Active = 0x01,
    NotActive = 0x02
}


public record AdjacentInterlockingSystemApproachZoneStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte ApproachZoneId, AdjacentInterlockingSystemApproachZoneStatusMessageApproachZoneStatus ApproachZoneStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int ApproachZoneIdOffset = 63;
    private const int ApproachZoneStatusOffset = 83;

    public new static AdjacentInterlockingSystemApproachZoneStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var ApproachZoneId = (byte)message[ApproachZoneIdOffset];
        var ApproachZoneStatus = (AdjacentInterlockingSystemApproachZoneStatusMessageApproachZoneStatus)message[ApproachZoneStatusOffset];
        return new AdjacentInterlockingSystemApproachZoneStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, ApproachZoneId, ApproachZoneStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[84];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0002).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[ApproachZoneIdOffset] = (byte)ApproachZoneId;
        result[ApproachZoneStatusOffset] = (byte)ApproachZoneStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemApproachZoneStatusMessageApproachZoneStatus : byte {
    Active = 0x01,
    NotActive = 0x02
}


public record AdjacentInterlockingSystemAccessRestrictionStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemAccessRestrictionStatusMessageAccessRestrictionActivationStatus AccessRestrictionActivationStatus, AdjacentInterlockingSystemAccessRestrictionStatusMessageAccessRestrictionType AccessRestrictionType) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int AccessRestrictionActivationStatusOffset = 63;
    private const int AccessRestrictionTypeOffset = 64;

    public new static AdjacentInterlockingSystemAccessRestrictionStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var AccessRestrictionActivationStatus = (AdjacentInterlockingSystemAccessRestrictionStatusMessageAccessRestrictionActivationStatus)message[AccessRestrictionActivationStatusOffset];
        var AccessRestrictionType = (AdjacentInterlockingSystemAccessRestrictionStatusMessageAccessRestrictionType)message[AccessRestrictionTypeOffset];
        return new AdjacentInterlockingSystemAccessRestrictionStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, AccessRestrictionActivationStatus, AccessRestrictionType);
    }

    public override byte[] ToByteArray() {
        var result = new byte[65];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0012).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[AccessRestrictionActivationStatusOffset] = (byte)AccessRestrictionActivationStatus;
        result[AccessRestrictionTypeOffset] = (byte)AccessRestrictionType;
        return result;
    }
}

public enum AdjacentInterlockingSystemAccessRestrictionStatusMessageAccessRestrictionActivationStatus : byte {
    Active = 0x01,
    NotActive = 0x02
}

public enum AdjacentInterlockingSystemAccessRestrictionStatusMessageAccessRestrictionType : byte {
    NoAccess = 0x01,
    WorkTrack = 0x02,
    TrackOutOfService = 0x03,
    EmergencyTrain = 0x04,
    SecondaryVehicle = 0x05,
    WorkTeam = 0x06,
    LevelCrossingInDegradedOperation = 0x07,
    ClearanceCheckRequired = 0x08,
    SectionCheckRequired = 0x09,
    NoElectricTrains = 0x10,
    ExtraordinaryTransport = 0x11,
    CatenaryOffPantographDown = 0x12,
    WrittenOrderRequired = 0x13,
    ManualRouteCondition = 0x14,
    DoNotUseOppositeDirection = 0x15,
    UseOppositeDirection = 0x16,
    NoLxRemoteSupervision = 0x17,
    LxRemoteSupervisionTimeout = 0x18,
    AccessRestrictionTypeNotApplicable = 0xFF
}


public record AdjacentInterlockingSystemLineStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemLineStatusMessageLineStatus LineStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int LineStatusOffset = 63;

    public new static AdjacentInterlockingSystemLineStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var LineStatus = (AdjacentInterlockingSystemLineStatusMessageLineStatus)message[LineStatusOffset];
        return new AdjacentInterlockingSystemLineStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, LineStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[64];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0004).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[LineStatusOffset] = (byte)LineStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemLineStatusMessageLineStatus : byte {
    Vacant = 0x01,
    Occupied = 0x02,
    RequestForLineBlockReset = 0x03
}


public record AdjacentInterlockingSystemFlankProtectionStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemFlankProtectionStatusMessageFlankProtectionStatus FlankProtectionStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int FlankProtectionStatusOffset = 63;

    public new static AdjacentInterlockingSystemFlankProtectionStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var FlankProtectionStatus = (AdjacentInterlockingSystemFlankProtectionStatusMessageFlankProtectionStatus)message[FlankProtectionStatusOffset];
        return new AdjacentInterlockingSystemFlankProtectionStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, FlankProtectionStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[64];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0013).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[FlankProtectionStatusOffset] = (byte)FlankProtectionStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemFlankProtectionStatusMessageFlankProtectionStatus : byte {
    Provided = 0x01,
    NotProvided = 0x02
}


public record AdjacentInterlockingSystemLineDirectionControlMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation LineDirectionControlInformation, AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus LineDirectionStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int LineDirectionControlInformationOffset = 63;
    private const int LineDirectionStatusOffset = 64;

    public new static AdjacentInterlockingSystemLineDirectionControlMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var LineDirectionControlInformation = (AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation)message[LineDirectionControlInformationOffset];
        var LineDirectionStatus = (AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus)message[LineDirectionStatusOffset];
        return new AdjacentInterlockingSystemLineDirectionControlMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, LineDirectionControlInformation, LineDirectionStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[65];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0006).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[LineDirectionControlInformationOffset] = (byte)LineDirectionControlInformation;
        result[LineDirectionStatusOffset] = (byte)LineDirectionStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionControlInformation : byte {
    NoDirection = 0x01,
    Entry = 0x02,
    Exit = 0x03,
    DirectionRequest = 0x04,
    DirectionHandover = 0x05,
    DirectionHandoverAborted = 0x06,
    DisableLineBlockDirection = 0x07,
    EnableLineBlockDirection = 0x08
}

public enum AdjacentInterlockingSystemLineDirectionControlMessageLineDirectionStatus : byte {
    Released = 0x01,
    Locked = 0x02,
    LineBlockDirectionDisabled = 0x03,
    LineDirectionStatusNotApplicable = 0xFF
}


public record AdjacentInterlockingSystemRouteStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte RouteId, AdjacentInterlockingSystemRouteStatusMessageRouteType RouteType, AdjacentInterlockingSystemRouteStatusMessageRouteStatus RouteStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int RouteIdOffset = 63;
    private const int RouteTypeOffset = 83;
    private const int RouteStatusOffset = 84;

    public new static AdjacentInterlockingSystemRouteStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var RouteId = (byte)message[RouteIdOffset];
        var RouteType = (AdjacentInterlockingSystemRouteStatusMessageRouteType)message[RouteTypeOffset];
        var RouteStatus = (AdjacentInterlockingSystemRouteStatusMessageRouteStatus)message[RouteStatusOffset];
        return new AdjacentInterlockingSystemRouteStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, RouteId, RouteType, RouteStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[85];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0008).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[RouteIdOffset] = (byte)RouteId;
        result[RouteTypeOffset] = (byte)RouteType;
        result[RouteStatusOffset] = (byte)RouteStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemRouteStatusMessageRouteType : byte {
    MainRoute = 0x01,
    ShuntingRoute = 0x02,
    OnSightRoute = 0x03,
    SrTrainRoute = 0x04,
    SpecialTrainRoute = 0x05,
    TemporaryShuntingArea = 0x06
}

public enum AdjacentInterlockingSystemRouteStatusMessageRouteStatus : byte {
    Initiated = 0x01,
    Locked = 0x02,
    NoRoute = 0x03
}


public record AdjacentInterlockingSystemRouteMonitoringStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte RouteId, AdjacentInterlockingSystemRouteMonitoringStatusMessageRouteType RouteType, byte OverlapId, AdjacentInterlockingSystemRouteMonitoringStatusMessageRouteMonitoring RouteMonitoring, AdjacentInterlockingSystemRouteMonitoringStatusMessageOccupancyMonitoring OccupancyMonitoring, AdjacentInterlockingSystemRouteMonitoringStatusMessageLevelCrossingMonitoring LevelCrossingMonitoring, byte EntranceSpeed, byte TargetSpeed, AdjacentInterlockingSystemRouteMonitoringStatusMessageDynamicOrStaticTargetSpeed DynamicOrStaticTargetSpeed) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int RouteIdOffset = 63;
    private const int RouteTypeOffset = 83;
    private const int OverlapIdOffset = 84;
    private const int RouteMonitoringOffset = 104;
    private const int OccupancyMonitoringOffset = 105;
    private const int LevelCrossingMonitoringOffset = 106;
    private const int EntranceSpeedOffset = 107;
    private const int TargetSpeedOffset = 108;
    private const int DynamicOrStaticTargetSpeedOffset = 109;

    public new static AdjacentInterlockingSystemRouteMonitoringStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var RouteId = (byte)message[RouteIdOffset];
        var RouteType = (AdjacentInterlockingSystemRouteMonitoringStatusMessageRouteType)message[RouteTypeOffset];
        var OverlapId = (byte)message[OverlapIdOffset];
        var RouteMonitoring = (AdjacentInterlockingSystemRouteMonitoringStatusMessageRouteMonitoring)message[RouteMonitoringOffset];
        var OccupancyMonitoring = (AdjacentInterlockingSystemRouteMonitoringStatusMessageOccupancyMonitoring)message[OccupancyMonitoringOffset];
        var LevelCrossingMonitoring = (AdjacentInterlockingSystemRouteMonitoringStatusMessageLevelCrossingMonitoring)message[LevelCrossingMonitoringOffset];
        var EntranceSpeed = (byte)message[EntranceSpeedOffset];
        var TargetSpeed = (byte)message[TargetSpeedOffset];
        var DynamicOrStaticTargetSpeed = (AdjacentInterlockingSystemRouteMonitoringStatusMessageDynamicOrStaticTargetSpeed)message[DynamicOrStaticTargetSpeedOffset];
        return new AdjacentInterlockingSystemRouteMonitoringStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, RouteId, RouteType, OverlapId, RouteMonitoring, OccupancyMonitoring, LevelCrossingMonitoring, EntranceSpeed, TargetSpeed, DynamicOrStaticTargetSpeed);
    }

    public override byte[] ToByteArray() {
        var result = new byte[110];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0009).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[RouteIdOffset] = (byte)RouteId;
        result[RouteTypeOffset] = (byte)RouteType;
        result[OverlapIdOffset] = (byte)OverlapId;
        result[RouteMonitoringOffset] = (byte)RouteMonitoring;
        result[OccupancyMonitoringOffset] = (byte)OccupancyMonitoring;
        result[LevelCrossingMonitoringOffset] = (byte)LevelCrossingMonitoring;
        result[EntranceSpeedOffset] = (byte)EntranceSpeed;
        result[TargetSpeedOffset] = (byte)TargetSpeed;
        result[DynamicOrStaticTargetSpeedOffset] = (byte)DynamicOrStaticTargetSpeed;
        return result;
    }
}

public enum AdjacentInterlockingSystemRouteMonitoringStatusMessageRouteType : byte {
    MainRoute = 0x01,
    ShuntingRoute = 0x02,
    OnSightRoute = 0x03,
    SrTrainRoute = 0x04,
    SpecialTrainRoute = 0x05,
    TemporaryShuntingArea = 0x06
}

public enum AdjacentInterlockingSystemRouteMonitoringStatusMessageRouteMonitoring : byte {
    RouteMonitoringConditionsOfSecondaryRoutePresent = 0x01,
    RouteMonitoringConditionsOfSecondaryRouteNotPresent = 0x02,
    RouteMonitoringConditionsOfSecondaryRoutePresentUpToNextBlockIndicator = 0x03,
    ShuntingRouteMonitoringConditionsOfSecondaryRoutePresent = 0x04
}

public enum AdjacentInterlockingSystemRouteMonitoringStatusMessageOccupancyMonitoring : byte {
    Occupation = 0x01,
    NoOccupation = 0x02,
    OccupancyMonitoringNotApplicable = 0xFF
}

public enum AdjacentInterlockingSystemRouteMonitoringStatusMessageLevelCrossingMonitoring : byte {
    LevelCrossingMonitoringConditionsOfSecondaryRoutePresent = 0x01,
    LevelCrossingMonitoringConditionsOfSecondaryRouteNotPresent = 0x02,
    LevelCrossingMonitoringConditionsPresentUpToNextBlockIndicator = 0x03,
    LevelCrossingMonitoringNotApplicable = 0xFF
}

public enum AdjacentInterlockingSystemRouteMonitoringStatusMessageDynamicOrStaticTargetSpeed : byte {
    Dynamic = 0x01,
    Static = 0x02,
    DynamicOrStaticTargetSpeedNotApplicable = 0xFF
}


public record AdjacentInterlockingSystemTrainOperatedRouteReleaseStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemTrainOperatedRouteReleaseStatusMessageTrainOperatedRouteReleaseStatus TrainOperatedRouteReleaseStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int TrainOperatedRouteReleaseStatusOffset = 63;

    public new static AdjacentInterlockingSystemTrainOperatedRouteReleaseStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var TrainOperatedRouteReleaseStatus = (AdjacentInterlockingSystemTrainOperatedRouteReleaseStatusMessageTrainOperatedRouteReleaseStatus)message[TrainOperatedRouteReleaseStatusOffset];
        return new AdjacentInterlockingSystemTrainOperatedRouteReleaseStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, TrainOperatedRouteReleaseStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[64];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x000B).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[TrainOperatedRouteReleaseStatusOffset] = (byte)TrainOperatedRouteReleaseStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemTrainOperatedRouteReleaseStatusMessageTrainOperatedRouteReleaseStatus : byte {
    TvpsAdjacentToTheBoundaryIsInACorrectOccupancySequence = 0x01,
    TvpsAdjacentToTheBoundaryIsReleasedByTrain = 0x02,
    TvpsAdjacentToTheBoundaryIsNotInACorrectOccupancySequenceAndNotReleasedByTrain = 0x03
}


public record AdjacentInterlockingSystemSignalStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte AspectLampCombinations, byte AspectExtensionLampCombinations, byte SpeedIndicator, byte SpeedIndicatorAnnouncement, byte DirectionIndicator, byte DirectionIndicatorAnnouncement, AdjacentInterlockingSystemSignalStatusMessageIntentionallyDark IntentionallyDark) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int AspectLampCombinationsOffset = 63;
    private const int AspectExtensionLampCombinationsOffset = 64;
    private const int SpeedIndicatorOffset = 65;
    private const int SpeedIndicatorAnnouncementOffset = 66;
    private const int DirectionIndicatorOffset = 67;
    private const int DirectionIndicatorAnnouncementOffset = 68;
    private const int IntentionallyDarkOffset = 69;

    public new static AdjacentInterlockingSystemSignalStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var AspectLampCombinations = (byte)message[AspectLampCombinationsOffset];
        var AspectExtensionLampCombinations = (byte)message[AspectExtensionLampCombinationsOffset];
        var SpeedIndicator = (byte)message[SpeedIndicatorOffset];
        var SpeedIndicatorAnnouncement = (byte)message[SpeedIndicatorAnnouncementOffset];
        var DirectionIndicator = (byte)message[DirectionIndicatorOffset];
        var DirectionIndicatorAnnouncement = (byte)message[DirectionIndicatorAnnouncementOffset];
        var IntentionallyDark = (AdjacentInterlockingSystemSignalStatusMessageIntentionallyDark)message[IntentionallyDarkOffset];
        return new AdjacentInterlockingSystemSignalStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, AspectLampCombinations, AspectExtensionLampCombinations, SpeedIndicator, SpeedIndicatorAnnouncement, DirectionIndicator, DirectionIndicatorAnnouncement, IntentionallyDark);
    }

    public override byte[] ToByteArray() {
        var result = new byte[70];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x000C).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[AspectLampCombinationsOffset] = (byte)AspectLampCombinations;
        result[AspectExtensionLampCombinationsOffset] = (byte)AspectExtensionLampCombinations;
        result[SpeedIndicatorOffset] = (byte)SpeedIndicator;
        result[SpeedIndicatorAnnouncementOffset] = (byte)SpeedIndicatorAnnouncement;
        result[DirectionIndicatorOffset] = (byte)DirectionIndicator;
        result[DirectionIndicatorAnnouncementOffset] = (byte)DirectionIndicatorAnnouncement;
        result[IntentionallyDarkOffset] = (byte)IntentionallyDark;
        return result;
    }
}

public enum AdjacentInterlockingSystemSignalStatusMessageIntentionallyDark : byte {
    TheCommandedSignalAspectIsIndicatedInTheSetLuminosity = 0x01,
    TheCommandedSignalAspectIsIndicatedDark = 0x0F,
    IntentionallyDarkNotApplicable = 0xFF
}


public record AdjacentInterlockingSystemTvpsStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemTvpsStatusMessageOccupancyStatus OccupancyStatus, AdjacentInterlockingSystemTvpsStatusMessageFoulingStatus FoulingStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int OccupancyStatusOffset = 63;
    private const int FoulingStatusOffset = 64;

    public new static AdjacentInterlockingSystemTvpsStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var OccupancyStatus = (AdjacentInterlockingSystemTvpsStatusMessageOccupancyStatus)message[OccupancyStatusOffset];
        var FoulingStatus = (AdjacentInterlockingSystemTvpsStatusMessageFoulingStatus)message[FoulingStatusOffset];
        return new AdjacentInterlockingSystemTvpsStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, OccupancyStatus, FoulingStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[65];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x000D).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[OccupancyStatusOffset] = (byte)OccupancyStatus;
        result[FoulingStatusOffset] = (byte)FoulingStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemTvpsStatusMessageOccupancyStatus : byte {
    Vacant = 0x01,
    Occupied = 0x02,
    Disturbed = 0x03
}

public enum AdjacentInterlockingSystemTvpsStatusMessageFoulingStatus : byte {
    Fouling = 0x01,
    NotFouling = 0x02,
    FoulingStatusNotApplicable = 0xFF
}


public record AdjacentInterlockingSystemOppositeMainSignalStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;

    public new static AdjacentInterlockingSystemOppositeMainSignalStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        return new AdjacentInterlockingSystemOppositeMainSignalStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId);
    }

    public override byte[] ToByteArray() {
        var result = new byte[63];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x000E).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        return result;
    }
}




public record AdjacentInterlockingSystemRoutePretestStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, byte RouteId, AdjacentInterlockingSystemRoutePretestStatusMessageRouteType RouteType, AdjacentInterlockingSystemRoutePretestStatusMessageRouteStatus RouteStatus, AdjacentInterlockingSystemRoutePretestStatusMessagePretestResponse PretestResponse) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int RouteIdOffset = 63;
    private const int RouteTypeOffset = 83;
    private const int RouteStatusOffset = 84;
    private const int PretestResponseOffset = 85;

    public new static AdjacentInterlockingSystemRoutePretestStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var RouteId = (byte)message[RouteIdOffset];
        var RouteType = (AdjacentInterlockingSystemRoutePretestStatusMessageRouteType)message[RouteTypeOffset];
        var RouteStatus = (AdjacentInterlockingSystemRoutePretestStatusMessageRouteStatus)message[RouteStatusOffset];
        var PretestResponse = (AdjacentInterlockingSystemRoutePretestStatusMessagePretestResponse)message[PretestResponseOffset];
        return new AdjacentInterlockingSystemRoutePretestStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, RouteId, RouteType, RouteStatus, PretestResponse);
    }

    public override byte[] ToByteArray() {
        var result = new byte[86];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0010).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[RouteIdOffset] = (byte)RouteId;
        result[RouteTypeOffset] = (byte)RouteType;
        result[RouteStatusOffset] = (byte)RouteStatus;
        result[PretestResponseOffset] = (byte)PretestResponse;
        return result;
    }
}

public enum AdjacentInterlockingSystemRoutePretestStatusMessageRouteType : byte {
    MainRoute = 0x01,
    ShuntingRoute = 0x02,
    OnSightRoute = 0x03,
    SrTrainRoute = 0x04,
    SpecialTrainRoute = 0x05,
    TemporaryShuntingArea = 0x06
}

public enum AdjacentInterlockingSystemRoutePretestStatusMessageRouteStatus : byte {
    Initiated = 0x01,
    Locked = 0x02,
    NoRoute = 0x03
}

public enum AdjacentInterlockingSystemRoutePretestStatusMessagePretestResponse : byte {
    PossibleAndVacant = 0x01,
    PossibleAndOccupied = 0x02,
    Queue = 0x03,
    Rejected = 0x04
}


public record AdjacentInterlockingSystemRouteReleaseInhibitionStatusMessage (string SenderIdentifier, string ReceiverIdentifier, byte BoundaryId, AdjacentInterlockingSystemRouteReleaseInhibitionStatusMessageRouteReleaseInhibitionStatus RouteReleaseInhibitionStatus) : Message {
    private const int MessageTypeOffset = 1;
    private const int SenderIdentifierOffset = 3;
    private const int ReceiverIdentifierOffset = 23;
    private const int BoundaryIdOffset = 43;
    private const int RouteReleaseInhibitionStatusOffset = 63;

    public new static AdjacentInterlockingSystemRouteReleaseInhibitionStatusMessage FromBytes(byte[] message) {
        var SenderIdentifier = Encoding.Latin1.GetString(message, SenderIdentifierOffset, 20);
        var ReceiverIdentifier = Encoding.Latin1.GetString(message, ReceiverIdentifierOffset, 20);
        var BoundaryId = (byte)message[BoundaryIdOffset];
        var RouteReleaseInhibitionStatus = (AdjacentInterlockingSystemRouteReleaseInhibitionStatusMessageRouteReleaseInhibitionStatus)message[RouteReleaseInhibitionStatusOffset];
        return new AdjacentInterlockingSystemRouteReleaseInhibitionStatusMessage(SenderIdentifier, ReceiverIdentifier, BoundaryId, RouteReleaseInhibitionStatus);
    }

    public override byte[] ToByteArray() {
        var result = new byte[64];
        result[0] = (byte)ProtocolType.AdjacentInterlockingSystem;
        BitConverter.GetBytes(0x0014).Take(2).ToArray().CopyTo(result, MessageTypeOffset);
        Encoding.Latin1.GetBytes(SenderIdentifier.PadRight(20, '_')).CopyTo(result, SenderIdentifierOffset);
        Encoding.Latin1.GetBytes(ReceiverIdentifier.PadRight(20, '_')).CopyTo(result, ReceiverIdentifierOffset);
        result[BoundaryIdOffset] = (byte)BoundaryId;
        result[RouteReleaseInhibitionStatusOffset] = (byte)RouteReleaseInhibitionStatus;
        return result;
    }
}

public enum AdjacentInterlockingSystemRouteReleaseInhibitionStatusMessageRouteReleaseInhibitionStatus : byte {
    Activated = 0x01,
    Deactivated = 0x02
}

