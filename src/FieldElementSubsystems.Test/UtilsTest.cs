using EulynxLive.FieldElementSubsystems.Extensions;

public class UtilsTest
{
    [Fact]
    public static void TestHexToByteArray()
    {
        string hexString = "0A0B0C0D0E0F";
        byte[] byteArray = hexString.HexToByteArray();
        Assert.Equal(byteArray, new byte[] { 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F });
    }

    [Fact]
    public static void TestHexToByteArrayUneven()
    {
        string hexString = "F";
        Assert.Throws<ArgumentException>(() => hexString.HexToByteArray());
    }
}
