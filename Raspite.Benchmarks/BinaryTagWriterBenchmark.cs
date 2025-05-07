using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace Raspite.Benchmarks;

public class BinaryTagWriterBenchmark
{
    [Benchmark]
    public int Simple()
    {
        var buffer = new ArrayBufferWriter<byte>(16);
        var writer = new BinaryTagWriter(buffer, false, false);

        writer.WriteByteTag(byte.MaxValue);
        writer.WriteShortTag(short.MaxValue);
        writer.WriteIntegerTag(int.MaxValue);

        return buffer.WrittenSpan.Length;
    }
}