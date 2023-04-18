using Raspite.Serializer.Streams;

namespace Raspite.Tests;

public sealed class BinaryStreamWrappersTests
{
    [Fact]
    public async Task WriteRead_BigEndian_SimpleTypes_Correctly()
    {
        using var memory = new MemoryStream();
        var writer = new WriteableBinaryStream(memory, false);

        await writer.WriteIntegerAsync(int.MaxValue);
        await writer.WriteFloatAsync(float.MaxValue);
        await writer.WriteStringAsync("Raspite");

        memory.Position = 0;

        var reader = new ReadableBinaryStream(memory, false);

        Assert.Equal(int.MaxValue, await reader.ReadIntegerAsync());
        Assert.Equal(float.MaxValue, await reader.ReadFloatAsync());
        Assert.Equal("Raspite", await reader.ReadStringAsync());
    }

    [Fact]
    public async Task WriteRead_LittleEndian_SimpleTypes_Correctly()
    {
        using var memory = new MemoryStream();
        var writer = new WriteableBinaryStream(memory, true);

        await writer.WriteIntegerAsync(int.MaxValue);
        await writer.WriteFloatAsync(float.MaxValue);
        await writer.WriteStringAsync("Raspite");

        memory.Position = 0;

        var reader = new ReadableBinaryStream(memory, true);

        Assert.Equal(int.MaxValue, await reader.ReadIntegerAsync());
        Assert.Equal(float.MaxValue, await reader.ReadFloatAsync());
        Assert.Equal("Raspite", await reader.ReadStringAsync());
    }
}