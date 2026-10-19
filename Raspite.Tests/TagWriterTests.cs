using System.Buffers;

namespace Raspite.Tests;

internal sealed class TagWriterTests
{
    [Test]
    public void Writing_StringTag_IsCorrect()
    {
        ReadOnlySpan<byte> correct =
        [
            8, 0, 4, 83, 101, 101, 110, 0, 3, 72, 101, 121
        ];

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new TagWriter(buffer, false, false, false);

        writer.WriteStringTag("Hey", "Seen");

        Assert.That(buffer.WrittenSpan.ToArray(), Is.EqualTo(correct.ToArray()).AsCollection);
    }

    [Test]
    public void Writing_VariableLengthIntegerTag_IsCorrect()
    {
        ReadOnlySpan<byte> correct =
        [
            3, 4, 83, 101, 101, 110, 215, 4
        ];

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new TagWriter(buffer, true, false, true);

        writer.WriteIntegerTag(-300, "Seen");

        Assert.That(buffer.WrittenSpan.ToArray(), Is.EqualTo(correct.ToArray()).AsCollection);
    }
}