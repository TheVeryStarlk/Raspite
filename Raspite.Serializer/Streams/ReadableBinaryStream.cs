using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Raspite.Tests")]

namespace Raspite.Serializer.Streams;

/// <summary>
/// Wraps a <see cref="Stream"/> and provides methods to read common used types.
/// </summary>
internal sealed class ReadableBinaryStream
{
    private readonly Stream stream;
    private readonly bool bigEndian;

    public ReadableBinaryStream(Stream stream, bool littleEndian)
    {
        this.stream = stream.CanRead
            ? stream
            : throw new BinaryTagSerializationException("Stream does not support reading.");

        bigEndian = !littleEndian;
    }

    public int ReadByte()
    {
        return stream.ReadByte();
    }

    public sbyte ReadSignedByte()
    {
        return (sbyte) stream.ReadByte();
    }

    public async Task<byte[]> ReadBytesAsync(int size)
    {
        var buffer = new byte[size];
        await stream.ReadExactlyAsync(buffer);

        return buffer;
    }

    public async Task<sbyte[]> ReadSignedBytesAsync(int size)
    {
        var buffer = new byte[size];
        await stream.ReadExactlyAsync(buffer);

        return Array.ConvertAll(buffer, input => (sbyte) input);
    }

    public async Task<short> ReadShortAsync()
    {
        var buffer = new byte[sizeof(short)];
        await stream.ReadExactlyAsync(buffer);

        return BitPrimitives.ToShort(buffer, bigEndian);
    }

    public async Task<ushort> ReadUnsignedShortAsync()
    {
        var buffer = new byte[sizeof(ushort)];
        await stream.ReadExactlyAsync(buffer);

        return BitPrimitives.ToUnsignedShort(buffer, bigEndian);
    }

    public async Task<int> ReadIntegerAsync()
    {
        var buffer = new byte[sizeof(int)];
        await stream.ReadExactlyAsync(buffer);

        return BitPrimitives.ToInteger(buffer, bigEndian);
    }

    public async Task<long> ReadLongAsync()
    {
        var buffer = new byte[sizeof(long)];
        await stream.ReadExactlyAsync(buffer);

        return BitPrimitives.ToLong(buffer, bigEndian);
    }

    public async Task<float> ReadFloatAsync()
    {
        var buffer = new byte[sizeof(float)];
        await stream.ReadExactlyAsync(buffer);

        return BitPrimitives.ToFloat(buffer, bigEndian);
    }

    public async Task<double> ReadDoubleAsync()
    {
        var buffer = new byte[sizeof(double)];
        await stream.ReadExactlyAsync(buffer);

        return BitPrimitives.ToDouble(buffer, bigEndian);
    }

    public async Task<string> ReadStringAsync()
    {
        var size = await ReadUnsignedShortAsync();
        var buffer = await ReadBytesAsync(size);

        return Encoding.UTF8.GetString(buffer);
    }
}