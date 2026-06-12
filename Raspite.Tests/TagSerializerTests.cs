using System.Buffers;

namespace Raspite.Tests;

internal sealed class TagSerializerTests
{
    [Test]
    public void ParsingSerializing_BigTest_IsCorrect()
    {
        using (Assert.EnterMultipleScope())
        {
            var correct = File.ReadAllBytes("bigtest.nbt");

            var result = TagSerializer.TryParse(correct, out var tag);

            Assert.That(result, Is.True);

            var writer = new ArrayBufferWriter<byte>();

            TagSerializer.Serialize(writer, tag);

            Assert.That(writer.WrittenSpan.ToArray(), Is.EqualTo(correct.ToArray()).AsCollection);
        }
    }
}