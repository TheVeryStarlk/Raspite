namespace Raspite.Serializer;

internal sealed class BinaryStream
{
    private readonly Stream stream;
    private readonly bool needSwap;
    private readonly bool bigEndian;

    public BinaryStream(Stream stream, bool littleEndian)
    {
        this.stream = stream;

        needSwap = BitConverter.IsLittleEndian != littleEndian;
        bigEndian = !littleEndian;
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
}