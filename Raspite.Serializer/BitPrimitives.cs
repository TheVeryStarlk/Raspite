using System.Buffers.Binary;
using Raspite.Serializer.Extensions;

namespace Raspite.Serializer;

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

    public static byte[] GetBytes(float value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(value.ReverseEndianness())
            : BitConverter.GetBytes(value);
    }

    public static byte[] GetBytes(double value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(value.ReverseEndianness())
            : BitConverter.GetBytes(value);
    }
}