using Raspite.Library.Scanning;

namespace Raspite.Tests;

public sealed class ScannerTests
{
    [Fact]
    public void ScannerBedrockHeader_Outputs_CorrectTokens()
    {
        // Arrange
        var source = new byte[]
        {
            8, 0, 0, 0, 1, 0, 0, 0, 0
        };

        var expected = new Token(Tag.End);

        // Act
        var actual = new Scanner(source, Edition.Bedrock).Run();

        // Assert
        Assert.Equal(expected.Tag.Type, actual.Tag.Type);
    }

    [Fact]
    public void ScannerJava_Outputs_CorrectTokens()
    {
        // Arrange
        var source = new byte[]
        {
            8, 0, 4, 78, 97, 109, 101, 0, 7, 82, 97, 115, 112, 105, 116, 101
        };

        var expected = new Token.Value(Tag.String, "Name", "Raspite");

        // Act
        var actual = new Scanner(source, Edition.Java).Run() as Token.Value;

        // Assert
        Assert.Equal(expected.Tag.Type, actual?.Tag.Type);
        Assert.Equal(expected.Name, actual?.Name);
        Assert.Equal(expected.Content, actual?.Content);
    }

    [Fact]
    public void ScannerBedrock_Outputs_CorrectTokens()
    {
        // Arrange
        var source = new byte[]
        {
            8, 0, 0, 0, 16, 0, 0, 0, 8, 4, 0, 78, 97, 109, 101, 7, 0, 82, 97, 115, 112, 105, 116, 101
        };

        var expected = new Token.Value(Tag.String, "Name", "Raspite");

        // Act
        var actual = new Scanner(source, Edition.Bedrock).Run() as Token.Value;

        // Assert
        Assert.Equal(expected.Tag.Type, actual?.Tag.Type);
        Assert.Equal(expected.Name, actual?.Name);
        Assert.Equal(expected.Content, actual?.Content);
    }
}