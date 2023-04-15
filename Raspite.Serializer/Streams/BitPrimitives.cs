using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Raspite.Serializer.Streams;

// https://github.com/ForeverZer0/SharpNBT/blob/master/SharpNBT/EndianExtensions.cs
internal static class EndianExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReverseEndianness(this float value)
    {
        var @int = BitConverter.SingleToInt32Bits(value);
        return BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(@int));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReverseEndianness(this double value)
    {
        var @long = BitConverter.DoubleToInt64Bits(value);
        return BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(@long));
    }
}

/// <summary>
/// Provides helper methods to write/read types in specific endianness.
/// </summary>
internal static class BitPrimitives
{
    /// <summary>
    /// Gets the the bytes of a value.
    /// </summary>
    /// <param name="value">The value to which get the bytes from.</param>
    /// <param name="swap">Whether to reverse endianness or not.</param>
    /// <returns>Bytes representation of the provided value.</returns>
    public static byte[] GetBytes(short value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }

    /// <inheritdoc cref="GetBytes(short, bool)"/>
    public static byte[] GetBytes(ushort value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }

    /// <inheritdoc cref="GetBytes(short, bool)"/>
    public static byte[] GetBytes(int value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }

    /// <inheritdoc cref="GetBytes(short, bool)"/>
    public static byte[] GetBytes(long value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(value))
            : BitConverter.GetBytes(value);
    }

    /// <inheritdoc cref="GetBytes(short, bool)"/>
    public static byte[] GetBytes(float value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(value.ReverseEndianness())
            : BitConverter.GetBytes(value);
    }

    /// <inheritdoc cref="GetBytes(short, bool)"/>
    public static byte[] GetBytes(double value, bool swap)
    {
        return swap
            ? BitConverter.GetBytes(value.ReverseEndianness())
            : BitConverter.GetBytes(value);
    }

    /// <summary>
    /// Gets the value from bytes. 
    /// </summary>
    /// <param name="buffer">The byte representation of the value.</param>
    /// <param name="bigEndian">Whether the provided buffer is in big endian or not.</param>
    /// <returns>The actual value, from bytes.</returns>
    public static short ToShort(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadInt16BigEndian(buffer)
            : BinaryPrimitives.ReadInt16LittleEndian(buffer);
    }

    /// <inheritdoc cref="ToShort(byte[], bool)"/>>
    public static ushort ToUnsignedShort(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(buffer)
            : BinaryPrimitives.ReadUInt16LittleEndian(buffer);
    }

    /// <inheritdoc cref="ToShort(byte[], bool)"/>>
    public static int ToInteger(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadInt32BigEndian(buffer)
            : BinaryPrimitives.ReadInt32LittleEndian(buffer);
    }

    /// <inheritdoc cref="ToShort(byte[], bool)"/>>
    public static long ToLong(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadInt64BigEndian(buffer)
            : BinaryPrimitives.ReadInt64LittleEndian(buffer);
    }

    /// <inheritdoc cref="ToShort(byte[], bool)"/>>
    public static float ToFloat(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadSingleBigEndian(buffer)
            : BinaryPrimitives.ReadSingleLittleEndian(buffer);
    }

    /// <inheritdoc cref="ToShort(byte[], bool)"/>>
    public static double ToDouble(byte[] buffer, bool bigEndian)
    {
        return bigEndian
            ? BinaryPrimitives.ReadDoubleBigEndian(buffer)
            : BinaryPrimitives.ReadDoubleLittleEndian(buffer);
    }
}