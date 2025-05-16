using System.Buffers;
using Raspite.Tags;

namespace Raspite.Tests;

internal sealed class BinaryTagSerializerTests
{
    [Test]
    public void Serializing_StringTag_IsCorrect()
    {
        ReadOnlySpan<byte> correct =
        [
            8, 0, 4, 83, 101, 101, 110, 0, 3, 72, 101, 121
        ];

        var buffer = new ArrayBufferWriter<byte>();
        var tag = new StringTag("Hey", "Seen");

        var options = new BinaryTagSerializerOptions();
        BinaryTagSerializer.Serialize(buffer, tag, options);

        Assert.That(buffer.WrittenSpan.ToArray(), Is.EqualTo(correct.ToArray()).AsCollection);
    }

    [Test]
    public void Deserializing_StringTag_IsCorrect()
    {
        Assert.Multiple(() =>
        {
            ReadOnlySpan<byte> correct =
            [
                8, 0, 4, 83, 101, 101, 110, 0, 3, 72, 101, 121
            ];

            var options = new BinaryTagSerializerOptions();
            var result = BinaryTagSerializer.TryRead(correct, out var tag, options);

            Assert.That(result, Is.True);
            Assert.That(tag, Is.AssignableTo<StringTag>());

            var instance = (StringTag) tag;

            Assert.That(instance.Value, Is.EqualTo("Hey"));
            Assert.That(instance.Name, Is.EqualTo("Seen"));
        });
    }
}