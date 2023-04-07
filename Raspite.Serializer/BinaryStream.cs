using Raspite.Serializer.Extensions;

namespace Raspite.Serializer;

internal sealed class BinaryStream
{
    private readonly Stream stream;
    private readonly bool needSwap;

    public BinaryStream(Stream stream, bool littleEndian)
    {
        this.stream = stream;
        needSwap = BitConverter.IsLittleEndian != littleEndian;
    }

    public async Task WriteBytesAsync(params byte[] value)
    {
        await stream.WriteAsync(value);
    }

    public async Task WriteSignedBytesAsync(params sbyte[] value)
    {
        await stream.WriteAsync(Array.ConvertAll(value, input => (byte) input));
    }

    public async Task WriteShortAsync(short value)
    {
        var buffer = BitPrimitives.GetBytes(value, needSwap);
        await stream.WriteAsync(buffer);
    }

    public async Task WriteUnsignedShortAsync(ushort value)
    {
        var buffer = BitPrimitives.GetBytes(value, needSwap);
        await stream.WriteAsync(buffer);
    }

    public async Task WriteIntegerAsync(int value)
    {
        var buffer = BitPrimitives.GetBytes(value, needSwap);
        await stream.WriteAsync(buffer);
    }

    public async Task WriteLongAsync(long value)
    {
        var buffer = BitPrimitives.GetBytes(value, needSwap);
        await stream.WriteAsync(buffer);
    }

    public async Task WriteFloatAsync(float value)
    {
        var buffer = BitPrimitives.GetBytes(value, needSwap);
        await stream.WriteAsync(buffer);
    }

    public async Task WriteDoubleAsync(double value)
    {
        var buffer = BitPrimitives.GetBytes(value, needSwap);
        await stream.WriteAsync(buffer);
    }
}