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

    public static short ToShort(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(buffer)
            : BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    public static ushort ToUnsignedShort(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(buffer)
            : BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    public static int ToInteger(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(buffer)
            : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    public static long ToLong(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(buffer)
            : BinaryPrimitives.ReadInt64LittleEndian(buffer);
    }

    public static float ToFloat(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadSingleBigEndian(buffer)
            : BinaryPrimitives.ReadSingleLittleEndian(buffer);
    }

    public static double ToDouble(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadDoubleBigEndian(buffer)
            : BinaryPrimitives.ReadDoubleLittleEndian(buffer);
    }
}