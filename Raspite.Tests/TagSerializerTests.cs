using System.Buffers;
using Raspite.Tags;

namespace Raspite.Tests;

internal sealed class TagSerializerTests
{
    [Test]
    public void Parsing_StringTag_IsCorrect()
    {
        Assert.Multiple(() =>
        {
            ReadOnlySpan<byte> correct =
            [
                8, 0, 4, 83, 101, 101, 110, 0, 3, 72, 101, 121
            ];

            var result = TagSerializer.TryParse(correct, out var tag);

            Assert.That(result, Is.True);
            Assert.That(tag, Is.AssignableTo<StringTag>());

            var instance = (StringTag) tag;

            Assert.That(instance.Value, Is.EqualTo("Hey"));
            Assert.That(instance.Name, Is.EqualTo("Seen"));
        });
    }

    [Test]
    public void Serializing_StringTag_IsCorrect()
    {
        ReadOnlySpan<byte> correct =
        [
            8, 0, 4, 83, 101, 101, 110, 0, 3, 72, 101, 121
        ];

        var buffer = new ArrayBufferWriter<byte>();
        var tag = new StringTag("Hey", "Seen");

        TagSerializer.Serialize(buffer, tag);

        Assert.That(buffer.WrittenSpan.ToArray(), Is.EqualTo(correct.ToArray()).AsCollection);
    }
}