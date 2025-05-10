using System.Buffers;

namespace Raspite.Tests;

internal sealed class BinaryTagWriterTests
{
    [Test]
    public void Writing_StringTag_IsCorrect()
    {
        ReadOnlySpan<byte> correct =
        [
            8, 0, 4, 83, 101, 101, 110, 0, 3, 72, 101, 121
        ];

        var buffer = new ArrayBufferWriter<byte>();
        var writer = new BinaryTagWriter(buffer, false);

        writer.WriteStringTag("Hey", "Seen");

        Assert.That(buffer.WrittenSpan.ToArray(), Is.EqualTo(correct.ToArray()).AsCollection);
    }
}