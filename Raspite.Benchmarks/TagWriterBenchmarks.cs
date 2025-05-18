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
}