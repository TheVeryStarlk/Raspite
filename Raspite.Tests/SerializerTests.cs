using Raspite.Library;

namespace Raspite.Tests;

public sealed class SerializerTests
{
    [Fact]
    public async Task Serializing_Outputs_Tags()
    {
        // Arrange
        var source = new byte[]
        {
            8, 4, 0, 78, 97, 109, 101, 7, 0, 82, 97, 115, 112, 105, 116, 101
        };

        var expected = new Tag.String("Raspite", "Name");

        // Act
        var actual = await NbtSerializer.SerializeAsync(source, new BinaryOptions()
        {
            Endianness = Endianness.Little,
            Compression = Compression.None
        }) as Tag.String;

        // Assert
        Assert.Equal(expected.Value, actual?.Value);
        Assert.Equal(expected.Name, actual?.Name);
    }
}