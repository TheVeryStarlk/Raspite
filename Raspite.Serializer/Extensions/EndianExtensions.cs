using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Raspite.Serializer.Extensions;

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