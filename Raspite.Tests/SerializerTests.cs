namespace Raspite.Tests;

public sealed class SerializerTests
{
    [Fact]
    public async Task Serializing_Little_None_Outputs_CorrectTags()
    {
        // Arrange
        var source = new byte[]
        {
            8, 4, 0, 78, 97, 109, 101, 7, 0, 82, 97, 115, 112, 105, 116, 101
        };

        var expected = new TagBase.String("Raspite", "Name");

        // Act
        var actual = await NbtSerializer.SerializeAsync(source, new NbtSerializerOptions()
        {
            Endianness = Endianness.Little,
            Compression = Compression.None
        }) as TagBase.String;

        // Assert
        Assert.Equal(expected.Value, actual?.Value);
        Assert.Equal(expected.Name, actual?.Name);
    }

    [Fact]
    public async Task Deserializing_Little_None_Outputs_CorrectBytes()
    {
        // Arrange
        var source = new TagBase.String("Raspite", "Name");

        var expected = new byte[]
        {
            8, 4, 0, 78, 97, 109, 101, 7, 0, 82, 97, 115, 112, 105, 116, 101
        };

        // Act
        var actual = await NbtSerializer.DeserializeAsync(source, new NbtSerializerOptions()
        {
            Endianness = Endianness.Little,
            Compression = Compression.None
        });

        // Assert
        Assert.Equal(expected, actual);
    }
}