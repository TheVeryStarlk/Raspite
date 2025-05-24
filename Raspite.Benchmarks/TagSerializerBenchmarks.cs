using System.Buffers;
using BenchmarkDotNet.Attributes;
using Raspite.Tags;

namespace Raspite.Benchmarks;

public class TagSerializerBenchmarks
{
    private readonly CompoundTag tag = new(
        [
            new ByteTag(0, nameof(ByteTag)),
            new StringTag("Hello, world!", nameof(StringTag)),
            new CompoundTag([new IntegerTag(0, nameof(IntegerTag))], "Child"),
            new LongsTag([1, 2, 3, 4], nameof(LongsTag))
        ],
        "Parent");

    private readonly ReadOnlyMemory<byte> source;

    public TagSerializerBenchmarks()
    {
        var buffer = new ArrayBufferWriter<byte>();

        TagSerializer.Serialize(buffer, tag);

        source = buffer.WrittenMemory;
    }

    [Benchmark]
    public bool Parsing()
    {
        return TagSerializer.TryParse(source.Span, out _);
    }

    [Benchmark]
    public int Serializing()
    {
        var buffer = new ArrayBufferWriter<byte>();

        TagSerializer.Serialize(buffer, tag);

        return buffer.WrittenSpan.Length;
    }
}