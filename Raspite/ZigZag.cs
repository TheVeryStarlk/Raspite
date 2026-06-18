namespace Raspite;

internal static class ZigZag
{
    public static int Encode(int value)
    {
        return (value << 1) ^ (value >> 31);
    }

    public static long Encode(long value)
    {
        return (value << 1) ^ (value >> 63);
    }
}