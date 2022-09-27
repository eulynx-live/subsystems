export enum MainAspect
{
    /// NeuPro.77.137
    Disallowed = 0x0,
    /// NeuPro.77.116
    Hp0 = 0x1,
    /// NeuPro.77.121
    Hp0Sh1 = 0x2,
    /// NeuPro.77.437
    Hp0Fahrtanzeiger = 0x3,
    /// NeuPro.77.117
    Ks1 = 0x4,
    /// NeuPro.77.118
    Ks1Blinking = 0x5,
    /// NeuPro.77.439
    Ks1BlinkingAndAdditionalLight = 0x6,
    /// NeuPro.77.119
    Ks2 = 0x7,
    /// NeuPro.77.230
    Ks2Additional = 0x8,
    /// NeuPro.77.120
    Sh1 = 0x9,
    /// Kennlicht, NeuPro.77.231
    MarkerLight = 0xA,
    /// NeuPro.77.122
    Off = 0xFF,
}

/// Zusatzbegriffe
export enum SecondaryAspect
{
    /// NeuPro.77.124
    Disallowed = 0x0,
    /// NeuPro.77.123
    Zs1 = 0x01,
    /// NeuPro.77.125
    Zs7 = 0x02,
    /// NeuPro.77.127
    Zs8 = 0x03,
    /// NeuPro.77.232
    Zs6 = 0x04,
    /// NeuPro.77.233
    Zs13 = 0x05,
    /// NeuPro.77.138
    Off = 0xFF,
}

/// Zs3
export enum Zs3v
{
    /// NeuPro.77.140
    Disallowed = 0x0,
    /// NeuPro.77.141
    Number1 = 0x01,
    /// NeuPro.77.142
    Number2 = 0x02,
    /// NeuPro.77.143
    Number3 = 0x03,
    /// NeuPro.77.144
    Number4 = 0x04,
    /// NeuPro.77.145
    Number5 = 0x05,
    /// NeuPro.77.146
    Number6 = 0x06,
    /// NeuPro.77.147
    Number7 = 0x07,
    /// NeuPro.77.148
    Number8 = 0x08,
    /// NeuPro.77.149
    Number9 = 0x09,
    /// NeuPro.77.150
    Number10 = 0x0A,
    /// NeuPro.77.151
    Number11 = 0x0B,
    /// NeuPro.77.152
    Number12 = 0x0C,
    /// NeuPro.77.153
    Number13 = 0x0E,
    /// NeuPro.77.154
    Number14 = 0x0D,
    /// NeuPro.77.155
    Number15 = 0x0F,
    /// NeuPro.77.156
    Off = 0xFF,
}

/// Richtungsanzeiger
/// Route Indicator
export enum Zs2v
{
    /// NeuPro.77.158
    Disallowed = 0x0,
    /// NeuPro.77.159
    A = 0x01,
    B = 0x02,
    C = 0x03,
    D = 0x04,
    E = 0x05,
    F = 0x06,
    G = 0x07,
    H = 0x08,
    I = 0x09,
    J = 0x0A,
    K = 0x0B,
    L = 0x0C,
    M = 0x0D,
    N = 0x0E,
    O = 0x0F,
    P = 0x10,
    Q = 0x11,
    R = 0x12,
    S = 0x13,
    T = 0x14,
    U = 0x15,
    V = 0x16,
    W = 0x17,
    X = 0x18,
    Y = 0x19,
    Z = 0x1A,
    /// NeuPro.77.160
    Off = 0xFF,
}

/// Don't know a good translation here.
export enum Abwertungsinformation
{
    /// NeuPro.77.162
    Disallowed = 0x00,
    /// NeuPro.77.163
    /// Hp0 instead of Ks2
    Type1 = 0x01,
    /// NeuPro.77.234
    /// Additional Light on/off with Ks2
    Type2 = 0x02,
    /// NeuPro.77.235
    /// Zs3v visible
    Type3 = 0x03,
    /// NeuPro.77.168
    NoInfo = 0xFF,
}

/// NeuPro.77.181
export enum Luminosity
{
    /// NeuPro.77.182
    Disallowed = 0x00,
    /// NeuPro.77.183
    Daytime = 0x01,
    /// NeuPro.77.184
    Nighttime = 0x02,
    /// NeuPro.77.185
    Undefined = 0xFF
}

export interface SimulatorState
{
    /// Whether the light signal was set up (aufgerüstet).
    setup: boolean;
    id: string;
    suspended: boolean;
    mainAspect: MainAspect;
    secondaryAspect: SecondaryAspect;
    zs3: Zs3v;
    zs3v: Zs3v;
    zs2: Zs2v;
    zs2v: Zs2v;
    luminosity: Luminosity;
};
