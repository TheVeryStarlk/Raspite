using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Raspite.Tests")]

namespace Raspite.Serializer.Streams;

/// <summary>
/// Wraps a <see cref="Stream"/> and provides methods to write common used types.
/// </summary>
internal sealed class WriteableBinaryStream
{
    private readonly Stream stream;
    private readonly bool needSwap;

    public WriteableBinaryStream(Stream stream, bool littleEndian)
    {
        this.stream = stream.CanWrite
            ? stream
            : throw new BinaryTagSerializationException("Stream does not support writing.");

        needSwap = BitConverter.IsLittleEndian != littleEndian;
    }

    public void WriteByte(byte value)
    {
        stream.WriteByte(value);
    }

    public void WriteSignedByte(sbyte value)
    {
        stream.WriteByte((byte) value);
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

    public async Task WriteStringAsync(string value)
    {
        var buffer = Encoding.UTF8.GetBytes(value);

        await WriteUnsignedShortAsync((ushort) buffer.Length);
        await WriteBytesAsync(buffer);
    }
}