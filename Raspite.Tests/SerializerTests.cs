using Raspite.Serializer;
using Raspite.Serializer.Tags;

namespace Raspite.Tests;

public sealed class SerializerTests
{
    [Fact]
    public async Task SimpleTag_SerializesCorrectly()
    {
        var source = new StringTag()
        {
            Name = "Raspite",
            Value = "Serializer"
        };

        var actual = new byte[]
        {
            0x08,
            0x00, 0x07,
            0x52, 0x61, 0x73, 0x70, 0x69, 0x74, 0x65,
            0x00, 0x0A,
            0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x69, 0x7a, 0x65, 0x72
        };

        using var expected = new MemoryStream();
        await BinaryTagSerializer.SerializeAsync(source, expected);

        Assert.Equal(expected.ToArray(), actual);
    }

    [Fact]
    public async Task SimpleTag_DeserializesCorrectly()
    {
        var source = new MemoryStream(new byte[]
        {
            0x08,
            0x00, 0x07,
            0x52, 0x61, 0x73, 0x70, 0x69, 0x74, 0x65,
            0x00, 0x0A,
            0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x69, 0x7a, 0x65, 0x72
        });

        var actual = await BinaryTagSerializer.DeserializeAsync<StringTag>(source);

        var expected = new StringTag()
        {
            Name = "Raspite",
            Value = "Serializer"
        };

        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Value, actual.Value);
    }
}