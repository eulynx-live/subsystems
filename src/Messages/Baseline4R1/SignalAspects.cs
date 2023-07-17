namespace EulynxLive.Messages.Baseline4R1;

public class BasicAspectType
{
    /// Eu.SAT.209
    /// FTIA: Po0
    /// NR: Red/Stop/On
    /// SZ: SZ1
    /// DB: Hp0
    /// CFL: SFA 1
    /// PR: HS01
    /// SNCF: Carré / Carré violet / Guidon d'arrêt fermé
    public static byte Stop_Danger_1 => 0x1;

    /// Eu.SAT.210
    public static byte Stop_Danger_2 => 0x21;

    /// Eu.SAT.211
    public static byte Proceed_Clear_1 => 0x4;

    /// Eu.SAT.213
    public static byte Flashing_Clear_1 => 0x5;

    /// Eu.SAT.214
    public static byte Flashing_Clear_2 => 0x6;

    /// Eu.SAT.215
    public static byte Approach_Caution => 0x7;

    /// Eu.SAT.316
    public static byte StaffResponsible => 0x17;

    /// Eu.SAT.216
    public static byte FlashingYellow => 0x18;

    /// Eu.SAT.217
    public static byte PreliminaryCaution => 0x28;

    /// Eu.SAT.218
    public static byte FlashingDoubleYellow => 0x47;

    /// Eu.SAT.219
    public static byte ExpectStop => 0x08;

    /// Eu.SAT.340
    public static byte ExpectLimitedSpeed => 0x0B;

    /// Eu.SAT.220
    public static byte ShuntingAllowed => 0x02;
        /// Eu.SAT.222
    public static byte IgnoreSignal => 0x0A;
        /// Eu.SAT.320
    /// Eu.SAT.223
    /// Eu.SAT.226
    /// Eu.SAT.227
    /// Eu.SAT.228
    /// Eu.SAT.375
    /// Eu.SAT.235
    public static byte IntendedDark => 0xFF;
        /// Eu.SAT.237
    /// Eu.SAT.238
    /// Eu.SAT.239
    /// Eu.SAT.240
    /// Eu.SAT.242
    /// Eu.SAT.365
}

public class BasicAspectTypeExtension
{
    /// Eu.SAT.229
    public static byte SubstitutionSignal => 0x01;

    /// Eu.SAT.230
    public static byte DriveOnSight => 0x02;

    /// Eu.SAT.231
    public static byte PassSignalAtStopToOppositeTrack => 0x03;

    /// Eu.SAT.232
    public static byte RouteToOppositeTrack => 0x04;

    /// Eu.SAT.233
    public static byte ExpectEarlyStop => 0x05;

    /// Eu.SAT.346
    public static byte IntendedDark => 0xFF;
}

/// Eu.SAT.354
public class SpeedIndicators {
    /// Eu.SAT.355
    public static byte Indication10 => 0x01;
        public static byte Indication20 => 0x02;
        public static byte Indication30 => 0x03;
        public static byte Indication40 => 0x04;
        public static byte Indication50 => 0x05;
        public static byte Indication60 => 0x06;
        public static byte Indication70 => 0x07;
        public static byte Indication80 => 0x08;
        public static byte Indication90 => 0x09;
        public static byte Indication100 => 0x0A;
        public static byte Indication110 => 0x0B;
        public static byte Indication120 => 0x0C;
        public static byte Indication130 => 0x0D;
        public static byte Indication140 => 0x0E;
        /// Eu.SAT.356

    public static byte Indication150 => 0x0F;
        /// Eu.SAT.369
    public static byte Indication160 => 0x10;
        /// Eu.SAT.357
    /// Eu.SAT.358
    public static byte IndicationDark => 0xFF;
}

public class SpeedIndicatorsAnnouncements {
    public static byte Announcement10 => 0x01;
        public static byte Announcement20 => 0x02;
        public static byte Announcement30 => 0x03;
        public static byte Announcement40 => 0x04;
        public static byte Announcement50 => 0x05;
        public static byte Announcement60 => 0x06;
        public static byte Announcement70 => 0x07;
        public static byte Announcement80 => 0x08;
        public static byte Announcement90 => 0x09;
        public static byte Announcement100 => 0x0A;
        public static byte Announcement110 => 0x0B;
        public static byte Announcement120 => 0x0C;
        public static byte Announcement130 => 0x0D;
        public static byte Announcement140 => 0x0E;
        public static byte Announcement150 => 0x0F;
        public static byte Announcement160 => 0x10;
        public static byte AnnouncementDark => 0xFF;
    }

/// Eu.SAT.251
public class DirectionIndicators {
    /// Eu.SAT.252
    public static byte IndicationA => 0x01;
    public static byte IndicationB => 0x02;
    public static byte IndicationC => 0x03;
    public static byte IndicationD => 0x04;
    public static byte IndicationE => 0x05;
    public static byte IndicationF => 0x06;
    public static byte IndicationG => 0x07;
    public static byte IndicationH => 0x08;
    public static byte IndicationI => 0x09;
    public static byte IndicationJ => 0x0A;
    public static byte IndicationK => 0x0B;
    public static byte IndicationL => 0x0C;
    public static byte IndicationM => 0x0D;
    public static byte IndicationN => 0x0E;
    public static byte IndicationO => 0x0F;
    public static byte IndicationP => 0x10;
    public static byte IndicationQ => 0x11;
    public static byte IndicationR => 0x12;
    public static byte IndicationS => 0x13;
    public static byte IndicationT => 0x14;
    public static byte IndicationU => 0x15;
    public static byte IndicationV => 0x16;
    public static byte IndicationW => 0x17;
    public static byte IndicationX => 0x18;
    public static byte IndicationY => 0x19;
    /// Eu.SAT.254
    public static byte IndicationZ => 0x1A;

    // Missing additional indicators here (Eu.SAT.306, Eu.SAT.304, Eu.SAT.256)

    /// Eu.SAT.255
    /// Eu.SAT.353
    public static byte IndicationDark => 0xFF;
}

public class DirectionIndicatorsAnnouncements {
    public static byte AnnouncementDark => 0xFF;
}

public class DowngradeInformation {
    public static byte NotApplicable => 0xFF;
}

public class RouteInformation {
    public static byte NotApplicable => 0xFF;
}

public class IntentionallyDark {
    public static byte NotApplicable => 0xFF;
}
