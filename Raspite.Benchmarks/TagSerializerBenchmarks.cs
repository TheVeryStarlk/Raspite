using System.Buffers;
using BenchmarkDotNet.Attributes;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace Raspite.Benchmarks;

public class TagSerializerBenchmarks
{
    private readonly CompoundTag tag = CompoundTagBuilder
        .Create("Parent")
        .AddByte(byte.MaxValue, nameof(ByteTag))
        .AddString("Hello, world", nameof(StringTag))
        .AddCompound(CompoundTagBuilder.Create("Container").AddInteger(int.MaxValue, nameof(IntegerTag)).Build(), nameof(ByteTag))
        .AddLongs([Random.Shared.NextInt64()], nameof(ByteTag))
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