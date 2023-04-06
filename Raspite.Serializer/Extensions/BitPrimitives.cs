using System.Buffers.Binary;

namespace Raspite.Serializer.Extensions;

internal static class BitPrimitives
{
    public static byte[] GetBytes(short value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }

    public static byte[] GetBytes(ushort value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }

    public static byte[] GetBytes(int value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }

    public static byte[] GetBytes(long value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }
}