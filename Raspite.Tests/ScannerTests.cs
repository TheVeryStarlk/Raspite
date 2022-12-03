using Raspite.Library.Scanning;

namespace Raspite.Tests;

public sealed class ScannerTests
{
    [Fact]
    public void ScannerBigEndian_Outputs_CorrectTokens()
    {
        // Arrange
        var source = new byte[]
        {
            8, 0, 4, 78, 97, 109, 101, 0, 7, 82, 97, 115, 112, 105, 116, 101
        };

        var expected = new Token.Value(Tag.String, "Name", "Raspite");

        // Act
        var actual = new Scanner(source, Endian.Big).Run() as Token.Value;

        // Assert
        Assert.Equal(expected.Tag.Type, actual?.Tag.Type);
        Assert.Equal(expected.Name, actual?.Name);
        Assert.Equal(expected.Content, actual?.Content);
    }

    [Fact]
    public void ScannerLittleEndian_Outputs_CorrectTokens()
    {
        // Arrange
        var source = new byte[]
        {
            8, 4, 0, 78, 97, 109, 101, 7, 0, 82, 97, 115, 112, 105, 116, 101
        };

        var expected = new Token.Value(Tag.String, "Name", "Raspite");

        // Act
        var actual = new Scanner(source, Endian.Little).Run() as Token.Value;

        // Assert
        Assert.Equal(expected.Tag.Type, actual?.Tag.Type);
        Assert.Equal(expected.Name, actual?.Name);
        Assert.Equal(expected.Content, actual?.Content);
    }
}