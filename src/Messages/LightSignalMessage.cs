namespace EulynxLive.Messages
{
    public enum LightSignalMessageType: ushort {
        InitializationRequest = 0x0021,
        StartInitialization = 0x0022,
        InitializationCompleted = 0x0023,
        VersionCheckCommand = 0x0024,
        VersionCheckMessage = 0x0025,
        IndicateSignalAspect = 0x0001,
        IndicatedSignalAspect = 0x0003,
        SetLuminosityCommand = 0x0002,
        SetLuminosityMessage = 0x0004,
    }

    public abstract class LightSignalMessage : EulynxMessage
    {
        public LightSignalMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {}

        public override ProtocolType ProtocolType => ProtocolType.LightSignal;
        public override ushort MessageTypeRaw => (ushort) MessageType;
        public abstract LightSignalMessageType MessageType { get; }
    }
    public class LightSignalVersionCheckCommand : LightSignalMessage
    {
        private const int SENDER_PDI_VERSION_OFFSET = 43;
        public LightSignalVersionCheckCommand(string senderId, string receiverId, byte senderPdiVersion) : base(senderId, receiverId)
        {
            SenderPdiVersion = senderPdiVersion;
        }

        public override LightSignalMessageType MessageType => LightSignalMessageType.VersionCheckCommand;

        public byte SenderPdiVersion { get; }

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) => new LightSignalVersionCheckCommand(senderId, receiverId, message[SENDER_PDI_VERSION_OFFSET]);

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
        }
    }

    public class LightSignalVersionCheckMessage : LightSignalMessage
    {
        private const int RESULT_PDI_VERSION_CHECK_OFFSET = 43;
        private const int SENDER_PDI_VERSION_OFFSET = 44;
        private const int CHECKSUM_LENGTH_OFFSET = 45;
        private const int CHECKSUM_DATA_OFFSET = 46;
        public LightSignalVersionCheckMessage(string senderId, string receiverId, PdiVersionCheckResult resultPdiVersionCheck, byte senderPdiVersion, byte checksumLength) : base(senderId, receiverId)
        {
            ResultPdiVersionCheck = resultPdiVersionCheck;
            SenderPdiVersion = senderPdiVersion;
            ChecksumLength = checksumLength;
        }

        public override LightSignalMessageType MessageType => LightSignalMessageType.VersionCheckMessage;

        public PdiVersionCheckResult ResultPdiVersionCheck { get; }
        public byte SenderPdiVersion { get; }
        public byte ChecksumLength { get; }

        public override int Size => 46 + ChecksumLength;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var resultPdiVersionCheck = (PdiVersionCheckResult) message[RESULT_PDI_VERSION_CHECK_OFFSET];
            var senderPdiVersion = message[SENDER_PDI_VERSION_OFFSET];
            var checksumLength = message[CHECKSUM_LENGTH_OFFSET];
            // var CHECKSUM_DATA_OFFSET = message[CHECKSUM_DATA_OFFSET];
            return new LightSignalVersionCheckMessage(senderId, receiverId, resultPdiVersionCheck, senderPdiVersion, checksumLength);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[RESULT_PDI_VERSION_CHECK_OFFSET] = (byte)ResultPdiVersionCheck;
            bytes[SENDER_PDI_VERSION_OFFSET] = SenderPdiVersion;
            bytes[CHECKSUM_LENGTH_OFFSET] = ChecksumLength;
        }
    }

    public class LightSignalInitializationRequestMessage : LightSignalMessage
    {
        public LightSignalInitializationRequestMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LightSignalMessageType MessageType => LightSignalMessageType.InitializationRequest;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new LightSignalInitializationRequestMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class LightSignalStartInitializationMessage : LightSignalMessage
    {
        public LightSignalStartInitializationMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LightSignalMessageType MessageType => LightSignalMessageType.StartInitialization;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new LightSignalStartInitializationMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    public class LightSignalInitializationCompletedMessage : LightSignalMessage
    {
        public LightSignalInitializationCompletedMessage(string senderId, string receiverId) : base(senderId, receiverId)
        {
        }

        public override LightSignalMessageType MessageType => LightSignalMessageType.InitializationCompleted;

        public override int Size => 43;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            return new LightSignalInitializationCompletedMessage(senderId, receiverId);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
        }
    }

    /// Eu.SCI-LS.PDI.276
    public enum Luminosity : byte {
        /// Eu.SCI-LS.PDI.360
        Day = 0x01,
        /// Eu.SCI-LS.PDI.361
        Night = 0x02,
        /// Eu.SCI-LS.PDI.372
        Undefined = 0xFE,
    }

    public class LightSignalSetLuminosityMessage : LightSignalMessage
    {
        private const int RESULT_SET_LUMINOSITY_OFFSET = 43;
        public LightSignalSetLuminosityMessage(string senderId, string receiverId, Luminosity luminosity) : base(senderId, receiverId)
        {
            Luminosity = luminosity;
        }

        public override LightSignalMessageType MessageType => LightSignalMessageType.SetLuminosityMessage;

        public Luminosity Luminosity { get; }

        public override int Size => 44;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message) {
            var luminosity = (Luminosity) message[RESULT_SET_LUMINOSITY_OFFSET];
            return new LightSignalSetLuminosityMessage(senderId, receiverId, luminosity);
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[RESULT_SET_LUMINOSITY_OFFSET] = (byte)Luminosity;
        }
    }

    // Work in progress:
    // public class LightSignalSetLuminosityCommand : LightSignalSetLuminosityMessage
    // {
    //     public LightSignalSetLuminosityCommand(string senderId, string receiverId, Luminosity luminosity) : base(senderId, receiverId, luminosity) { }
    // }

    public enum BasicAspectType : byte
    {
        /// Eu.SAT.209
        /// FTIA: Po0
        /// NR: Red/Stop/On
        /// SZ: SZ1
        /// DB: Hp0
        /// CFL: SFA 1
        /// PR: HS01
        /// SNCF: Carré / Carré violet / Guidon d'arrêt fermé
        Stop_Danger_1 = 0x1,

        /// Eu.SAT.210
        Stop_Danger_2 = 0x21,

        /// Eu.SAT.211
        Proceed_Clear_1 = 0x4,

        /// Eu.SAT.213
        Flashing_Clear_1 = 0x5,

        /// Eu.SAT.214
        Flashing_Clear_2 = 0x6,

        /// Eu.SAT.215
        Approach_Caution = 0x7,

        /// Eu.SAT.316
        StaffResponsible = 0x17,

        /// Eu.SAT.216
        FlashingYellow = 0x18,

        /// Eu.SAT.217
        PreliminaryCaution = 0x28,

        /// Eu.SAT.218
        FlashingDoubleYellow = 0x47,

        /// Eu.SAT.219
        ExpectStop = 0x08,

        /// Eu.SAT.340
        ExpectLimitedSpeed = 0x0B,

        /// Eu.SAT.220
        ShuntingAllowed = 0x02,
        /// Eu.SAT.222
        IgnoreSignal = 0x0A,
        /// Eu.SAT.320
        /// Eu.SAT.223
        /// Eu.SAT.226
        /// Eu.SAT.227
        /// Eu.SAT.228
        /// Eu.SAT.375
        /// Eu.SAT.235
        IntendedDark = 0xFF,
        /// Eu.SAT.237
        /// Eu.SAT.238
        /// Eu.SAT.239
        /// Eu.SAT.240
        /// Eu.SAT.242
        /// Eu.SAT.365
    }

    public enum BasicAspectTypeExtension : byte
    {

        /// Eu.SAT.229
        SubstitutionSignal = 0x01,

        /// Eu.SAT.230
        DriveOnSight = 0x02,

        /// Eu.SAT.231
        PassSignalAtStopToOppositeTrack = 0x03,

        /// Eu.SAT.232
        RouteToOppositeTrack = 0x04,

        /// Eu.SAT.233
        ExpectEarlyStop = 0x05,

        /// Eu.SAT.346
        IntendedDark = 0xFF,
    }

    /// Eu.SAT.354
    public enum SpeedIndicators : byte {
        /// Eu.SAT.355
        Indication10 = 0x01,
        Indication20 = 0x02,
        Indication30 = 0x03,
        Indication40 = 0x04,
        Indication50 = 0x05,
        Indication60 = 0x06,
        Indication70 = 0x07,
        Indication80 = 0x08,
        Indication90 = 0x09,
        Indication100 = 0x0A,
        Indication110 = 0x0B,
        Indication120 = 0x0C,
        Indication130 = 0x0D,
        Indication140 = 0x0E,
        /// Eu.SAT.356

        Indication150 = 0x0F,
        /// Eu.SAT.369
        Indication160 = 0x10,
        /// Eu.SAT.357
        /// Eu.SAT.358
        IndicationDark = 0xFF,
    }

    public enum SpeedIndicatorsAnnouncements : byte {
        Announcement10 = 0x01,
        Announcement20 = 0x02,
        Announcement30 = 0x03,
        Announcement40 = 0x04,
        Announcement50 = 0x05,
        Announcement60 = 0x06,
        Announcement70 = 0x07,
        Announcement80 = 0x08,
        Announcement90 = 0x09,
        Announcement100 = 0x0A,
        Announcement110 = 0x0B,
        Announcement120 = 0x0C,
        Announcement130 = 0x0D,
        Announcement140 = 0x0E,
        Announcement150 = 0x0F,
        Announcement160 = 0x10,
        AnnouncementDark = 0xFF,
    }

    /// Eu.SAT.251
    public enum DirectionIndicators : byte {
        /// Eu.SAT.252
        IndicationA = 0x01,
        IndicationB = 0x02,
        IndicationC = 0x03,
        IndicationD = 0x04,
        IndicationE = 0x05,
        IndicationF = 0x06,
        IndicationG = 0x07,
        IndicationH = 0x08,
        IndicationI = 0x09,
        IndicationJ = 0x0A,
        IndicationK = 0x0B,
        IndicationL = 0x0C,
        IndicationM = 0x0D,
        IndicationN = 0x0E,
        IndicationO = 0x0F,
        IndicationP = 0x10,
        IndicationQ = 0x11,
        IndicationR = 0x12,
        IndicationS = 0x13,
        IndicationT = 0x14,
        IndicationU = 0x15,
        IndicationV = 0x16,
        IndicationW = 0x17,
        IndicationX = 0x18,
        IndicationY = 0x19,
        /// Eu.SAT.254
        IndicationZ = 0x1A,

        // Missing additional indicators here (Eu.SAT.306, Eu.SAT.304, Eu.SAT.256)

        /// Eu.SAT.255
        /// Eu.SAT.353
        IndicationDark = 0xFF,
    }

    public enum DirectionIndicatorsAnnouncements : byte {
        AnnouncementDark = 0xFF,
    }

    public enum DowngradeInformation : byte {
        NotApplicable = 0xFF,
    }

    public enum RouteInformation : byte {
        NotApplicable = 0xFF,
    }

    public enum IntentionallyDark : byte {
        NotApplicable = 0xFF,
    }


    public class LightSignalIndicateSignalAspectCommand : LightSignalMessage
    {
        private const int BASIC_ASPECT_TYPE_OFFSET = 43;
        private const int BASIC_ASPECT_TYPE_EXTENSION_OFFSET = 44;
        private const int SPEED_INDICATORS_OFFSET = 45;
        private const int SPEED_INDICATORS_ANNOUNCEMENTS_OFFSET = 46;
        private const int DIRECTION_INDICATORS_OFFSET = 47;
        private const int DIRECTION_INDICATORS_ANNOUNCEMENTS_OFFSET = 48;
        private const int DOWNGRADE_INFORMATION_OFFSET = 49;
        private const int ROUTE_INFORMATION_OFFSET = 50;
        private const int INTENTIONALLY_DARK_OFFSET = 51;
        private const int NATIONAL_SPECIFIED_OFFSET = 52;
        public override LightSignalMessageType MessageType => LightSignalMessageType.IndicateSignalAspect;

        public LightSignalIndicateSignalAspectCommand(string senderId, string receiverId, BasicAspectType basicAspectType, BasicAspectTypeExtension basicAspectTypeExtension, SpeedIndicators speedIndicators, SpeedIndicatorsAnnouncements speedIndicatorsAnnouncements, DirectionIndicators directionIndicators, DirectionIndicatorsAnnouncements directionIndicatorsAnnouncements, DowngradeInformation downgradeInformation, RouteInformation routeInformation, IntentionallyDark intentionallyDark) : base(senderId, receiverId)
        {
            BasicAspectType = basicAspectType;
            BasicAspectTypeExtension = basicAspectTypeExtension;
            SpeedIndicators = speedIndicators;
            SpeedIndicatorsAnnouncements = speedIndicatorsAnnouncements;
            DirectionIndicators = directionIndicators;
            DirectionIndicatorsAnnouncements = directionIndicatorsAnnouncements;
            DowngradeInformation = downgradeInformation;
            RouteInformation = routeInformation;
            IntentionallyDark = intentionallyDark;
        }

        public BasicAspectType BasicAspectType { get; }
        public BasicAspectTypeExtension BasicAspectTypeExtension { get; }
        public SpeedIndicators SpeedIndicators { get; }
        public SpeedIndicatorsAnnouncements SpeedIndicatorsAnnouncements { get; }
        public DirectionIndicators DirectionIndicators { get; }
        public DirectionIndicatorsAnnouncements DirectionIndicatorsAnnouncements { get; }
        public DowngradeInformation DowngradeInformation { get; }
        public RouteInformation RouteInformation { get; }
        public IntentionallyDark IntentionallyDark { get; }

        public override int Size => 61;

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var basicAspectType = (BasicAspectType) message[BASIC_ASPECT_TYPE_OFFSET];
            var basicAspectTypeExtension = (BasicAspectTypeExtension) message[BASIC_ASPECT_TYPE_EXTENSION_OFFSET];
            var speedIndicators = (SpeedIndicators) message[SPEED_INDICATORS_OFFSET];
            var speedIndicatorsAnnouncements = (SpeedIndicatorsAnnouncements) message[SPEED_INDICATORS_ANNOUNCEMENTS_OFFSET];
            var directionIndicators = (DirectionIndicators) message[DIRECTION_INDICATORS_OFFSET];
            var directionIndicatorsAnnouncements = (DirectionIndicatorsAnnouncements) message[DIRECTION_INDICATORS_ANNOUNCEMENTS_OFFSET];
            var downgradeInformation = (DowngradeInformation) message[DOWNGRADE_INFORMATION_OFFSET];
            var routeInformation = (RouteInformation) message[ROUTE_INFORMATION_OFFSET];
            var intentionallyDark = (IntentionallyDark) message[INTENTIONALLY_DARK_OFFSET];

            return new LightSignalIndicateSignalAspectCommand(
                senderId, receiverId,
                basicAspectType,
                basicAspectTypeExtension,
                speedIndicators,
                speedIndicatorsAnnouncements,
                directionIndicators,
                directionIndicatorsAnnouncements,
                downgradeInformation,
                routeInformation,
                intentionallyDark
            );
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[BASIC_ASPECT_TYPE_OFFSET] = (byte)BasicAspectType;
            bytes[BASIC_ASPECT_TYPE_EXTENSION_OFFSET] = (byte)BasicAspectTypeExtension;
            bytes[SPEED_INDICATORS_OFFSET] = (byte)SpeedIndicators;
            bytes[SPEED_INDICATORS_ANNOUNCEMENTS_OFFSET] = (byte)SpeedIndicatorsAnnouncements;
            bytes[DIRECTION_INDICATORS_OFFSET] = (byte)DirectionIndicators;
            bytes[DIRECTION_INDICATORS_ANNOUNCEMENTS_OFFSET] = (byte)DirectionIndicatorsAnnouncements;
            bytes[DOWNGRADE_INFORMATION_OFFSET] = (byte)DowngradeInformation;
            bytes[ROUTE_INFORMATION_OFFSET] = (byte)RouteInformation;
            bytes[INTENTIONALLY_DARK_OFFSET] = (byte)IntentionallyDark;
        }
    }

    public class LightSignalIndicatedSignalAspectMessage : LightSignalMessage
    {
        private const int BASIC_ASPECT_TYPE_OFFSET = 43;
        private const int BASIC_ASPECT_TYPE_EXTENSION_OFFSET = 44;
        private const int SPEED_INDICATORS_OFFSET = 45;
        private const int SPEED_INDICATORS_ANNOUNCEMENTS_OFFSET = 46;
        private const int DIRECTION_INDICATORS_OFFSET = 47;
        private const int DIRECTION_INDICATORS_ANNOUNCEMENTS_OFFSET = 48;
        private const int DOWNGRADE_INFORMATION_OFFSET = 49;
        private const int ROUTE_INFORMATION_OFFSET = 50;
        private const int INTENTIONALLY_DARK_OFFSET = 51;
        private const int NATIONAL_SPECIFIED_OFFSET = 52;

        public LightSignalIndicatedSignalAspectMessage(string senderId, string receiverId, BasicAspectType basicAspectType, BasicAspectTypeExtension basicAspectTypeExtension, SpeedIndicators speedIndicators, SpeedIndicatorsAnnouncements speedIndicatorsAnnouncements, DirectionIndicators directionIndicators, DirectionIndicatorsAnnouncements directionIndicatorsAnnouncements, DowngradeInformation downgradeInformation, RouteInformation routeInformation, IntentionallyDark intentionallyDark) : base(senderId, receiverId)
        {
            BasicAspectType = basicAspectType;
            BasicAspectTypeExtension = basicAspectTypeExtension;
            SpeedIndicators = speedIndicators;
            SpeedIndicatorsAnnouncements = speedIndicatorsAnnouncements;
            DirectionIndicators = directionIndicators;
            DirectionIndicatorsAnnouncements = directionIndicatorsAnnouncements;
            DowngradeInformation = downgradeInformation;
            RouteInformation = routeInformation;
            IntentionallyDark = intentionallyDark;
        }

        public override LightSignalMessageType MessageType => LightSignalMessageType.IndicatedSignalAspect;

        public override int Size => 61;

        public BasicAspectType BasicAspectType { get; }
        public BasicAspectTypeExtension BasicAspectTypeExtension { get; }
        public SpeedIndicators SpeedIndicators { get; }
        public SpeedIndicatorsAnnouncements SpeedIndicatorsAnnouncements { get; }
        public DirectionIndicators DirectionIndicators { get; }
        public DirectionIndicatorsAnnouncements DirectionIndicatorsAnnouncements { get; }
        public DowngradeInformation DowngradeInformation { get; }
        public RouteInformation RouteInformation { get; }
        public IntentionallyDark IntentionallyDark { get; }

        internal static EulynxMessage Parse(string senderId, string receiverId, byte[] message)
        {
            var basicAspectType = (BasicAspectType) message[BASIC_ASPECT_TYPE_OFFSET];
            var basicAspectTypeExtension = (BasicAspectTypeExtension) message[BASIC_ASPECT_TYPE_EXTENSION_OFFSET];
            var speedIndicators = (SpeedIndicators) message[SPEED_INDICATORS_OFFSET];
            var speedIndicatorsAnnouncements = (SpeedIndicatorsAnnouncements) message[SPEED_INDICATORS_ANNOUNCEMENTS_OFFSET];
            var directionIndicators = (DirectionIndicators) message[DIRECTION_INDICATORS_OFFSET];
            var directionIndicatorsAnnouncements = (DirectionIndicatorsAnnouncements) message[DIRECTION_INDICATORS_ANNOUNCEMENTS_OFFSET];
            var downgradeInformation = (DowngradeInformation) message[DOWNGRADE_INFORMATION_OFFSET];
            var routeInformation = (RouteInformation) message[ROUTE_INFORMATION_OFFSET];
            var intentionallyDark = (IntentionallyDark) message[INTENTIONALLY_DARK_OFFSET];

            return new LightSignalIndicatedSignalAspectMessage(
                senderId, receiverId,
                basicAspectType,
                basicAspectTypeExtension,
                speedIndicators,
                speedIndicatorsAnnouncements,
                directionIndicators,
                directionIndicatorsAnnouncements,
                downgradeInformation,
                routeInformation,
                intentionallyDark
            );
        }

        protected override void WritePayloadToByteArray(byte[] bytes)
        {
            bytes[BASIC_ASPECT_TYPE_OFFSET] = (byte)BasicAspectType;
            bytes[BASIC_ASPECT_TYPE_EXTENSION_OFFSET] = (byte)BasicAspectTypeExtension;
            bytes[SPEED_INDICATORS_OFFSET] = (byte)SpeedIndicators;
            bytes[SPEED_INDICATORS_ANNOUNCEMENTS_OFFSET] = (byte)SpeedIndicatorsAnnouncements;
            bytes[DIRECTION_INDICATORS_OFFSET] = (byte)DirectionIndicators;
            bytes[DIRECTION_INDICATORS_ANNOUNCEMENTS_OFFSET] = (byte)DirectionIndicatorsAnnouncements;
            bytes[DOWNGRADE_INFORMATION_OFFSET] = (byte)DowngradeInformation;
            bytes[ROUTE_INFORMATION_OFFSET] = (byte)RouteInformation;
            bytes[INTENTIONALLY_DARK_OFFSET] = (byte)IntentionallyDark;
        }
    }
}
