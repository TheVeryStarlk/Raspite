using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace Raspite.Benchmarks;

public class BinaryTagWriterBenchmarks
{
    [Benchmark]
    public int PrimitiveTag()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new BinaryTagWriter(buffer, false);

        writer.WriteByteTag(byte.MaxValue);
        writer.WriteShortTag(short.MaxValue);
        writer.WriteIntegerTag(int.MaxValue);
        writer.WriteLongTag(long.MaxValue);
        writer.WriteFloatTag(float.MaxValue);
        writer.WriteDoubleTag(double.MaxValue);
        writer.WriteStringTag("Hey", "Seen");

        return buffer.WrittenSpan.Length;
    }


    [Benchmark]
    public int BigEndianIntegerCollectionTag()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new BinaryTagWriter(buffer, false);

        writer.WriteIntegerCollectionTag(Enumerable.Repeat(int.MaxValue, byte.MaxValue).ToArray(), "Parent");

        return buffer.WrittenSpan.Length;
    }

    [Benchmark]
    public int LittleEndianIntegerCollectionTag()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new BinaryTagWriter(buffer, true);

        writer.WriteIntegerCollectionTag(Enumerable.Repeat(int.MaxValue, byte.MaxValue).ToArray(), "Parent");

        return buffer.WrittenSpan.Length;
    }
}