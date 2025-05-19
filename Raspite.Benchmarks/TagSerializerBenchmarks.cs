using System.Buffers;
using BenchmarkDotNet.Attributes;
using Raspite.Tags;
using Raspite.Tags.Builders;

namespace Raspite.Benchmarks;

public class TagSerializerBenchmarks
{
    private readonly CompoundTag tag = CompoundTagBuilder
        .Create()
        .AddByteTag(byte.MaxValue, nameof(ByteTag))
        .AddStringTag("Hello, world!", nameof(StringTag))
        .AddCompoundTag(CompoundTagBuilder
            .Create()
            .AddIntegerTag(int.MaxValue, "Age")
            .Build())
        .AddLongsTag([long.MaxValue, long.MaxValue], "Identifiers")
        .Build();

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