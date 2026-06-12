namespace Raspite.Tests;

internal sealed class TagReaderTests
{
    [Test]
    public void Reading_StringTag_IsCorrect()
    {
        using (Assert.EnterMultipleScope())
        {
            ReadOnlySpan<byte> correct =
            [
                8, 0, 4, 83, 101, 101, 110, 0, 3, 72, 101, 121
            ];

            var reader = new TagReader(correct, false, false);

            Assert.That(reader.TryReadStringTag(out var value, out var name), Is.True);
            Assert.That(value, Is.EqualTo("Hey"));
            Assert.That(name, Is.EqualTo("Seen"));
        }
    }
}