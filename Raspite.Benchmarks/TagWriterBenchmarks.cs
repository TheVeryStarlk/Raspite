using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace Raspite.Benchmarks;

public class TagWriterBenchmarks
{
    [Benchmark]
    public int PrimitiveTag()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new TagWriter(buffer, false);

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
    public int BigEndianIntegersTag()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new TagWriter(buffer, false);

        writer.WriteIntegersTag(Enumerable.Repeat(int.MaxValue, byte.MaxValue).ToArray(), "Parent");

        return buffer.WrittenSpan.Length;
    }

    [Benchmark]
    public int LittleEndianIntegersTag()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new TagWriter(buffer, true);

        writer.WriteIntegersTag(Enumerable.Repeat(int.MaxValue, byte.MaxValue).ToArray(), "Parent");

        return buffer.WrittenSpan.Length;
    }
}