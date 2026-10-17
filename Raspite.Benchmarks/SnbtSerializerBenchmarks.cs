using System.Buffers;
using BenchmarkDotNet.Attributes;
using Raspite.Tags;
using Raspite.Tags.Building;

namespace Raspite.Benchmarks;

[MemoryDiagnoser]
public class SnbtSerializerBenchmarks
{
    private const string Small = "{id:\"minecraft:stone\",Count:1b}";

    private const string Medium = """
                                  {
                                      Name: "Steve",
                                      Health: 20.0f,
                                      XpLevel: 30,
                                      Pos: [12.5d, 64.0d, -3.25d],
                                      Rotation: [90.0f, 0.0f],
                                      OnGround: true,
                                      UUID: [I; 1, 2, 3, 4],
                                      abilities: { flying: false, mayfly: true, walkSpeed: 0.1f }
                                  }
                                  """;

    private readonly CompoundTag tag;

    private readonly string complex;

    private readonly ArrayBufferWriter<char> buffer = new();

    private readonly SnbtSerializerOptions indented = new() { Indented = true };

    public SnbtSerializerBenchmarks()
    {
        var items = new CompoundTag[64];

        for (var index = 0; index < items.Length; index++)
        {
            items[index] = CompoundTagBuilder
                .Create()
                .AddString($"minecraft:item_{index}", "id")
                .AddByte((byte) index, "Slot")
                .AddByte(1, "Count")
                .AddCompound(CompoundTagBuilder
                    .Create("tag")
                    .AddString($"Item \"{index}\"\nwith escapes", "display")
                    .AddIntegerList([index, index * 2, index * 3], "Damage")
                    .Build())
                .Build();
        }

        tag = CompoundTagBuilder
            .Create()
            .AddCompoundList(items, "Inventory")
            .AddLongs([.. Enumerable.Range(0, 256).Select(index => (long) index * int.MaxValue)], "BlockStates")
            .AddBytes([.. Enumerable.Range(0, 256).Select(index => (byte) index)], "Biomes")
            .AddDoubleList([Math.PI, Math.E, 1e-10], "Motion")
            .AddString("A string with spaces, 'quotes' and \"more quotes\"", "Description")
            .Build();

        complex = SnbtSerializer.Serialize(tag);
    }

    [Benchmark]
    public bool ParsingSmall()
    {
        return SnbtSerializer.TryParse(Small, out _);
    }

    [Benchmark]
    public bool ParsingMedium()
    {
        return SnbtSerializer.TryParse(Medium, out _);
    }

    [Benchmark]
    public bool ParsingComplex()
    {
        return SnbtSerializer.TryParse(complex, out _);
    }

    [Benchmark]
    public int SerializingCompact()
    {
        buffer.ResetWrittenCount();
        SnbtSerializer.Serialize(buffer, tag);

        return buffer.WrittenCount;
    }

    [Benchmark]
    public int SerializingIndented()
    {
        buffer.ResetWrittenCount();
        SnbtSerializer.Serialize(buffer, tag, indented);

        return buffer.WrittenCount;
    }

    [Benchmark]
    public string SerializingCompactString()
    {
        return SnbtSerializer.Serialize(tag);
    }

    [Benchmark]
    public string SerializingIndentedString()
    {
        return SnbtSerializer.Serialize(tag, indented);
    }
}
