namespace EulynxLive.FieldElementSubsystems.Extensions;


public static class Utils
{
    public static byte[] HexToByteArray(this string hexString)
    {
        hexString = hexString.Replace("0x", "");
        if (hexString.Length % 2 != 0)
        {
            throw new ArgumentException("Invalid hex string length. The length must be even.", $"${hexString}");
        }

        byte[] byteArray = new byte[hexString.Length / 2];

        for (int i = 0; i < hexString.Length; i += 2)
        {
            byteArray[i / 2] = (byte)(Convert.ToInt16(hexString.Substring(i, 2), 16) & 0xFF);
        }

        return byteArray;
    }
}
