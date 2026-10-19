using System.Buffers;
using Raspite.Tags;
using Raspite.Tags.Building;

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

    [Test]
    public void ParsingSerializing_VariableLength_IsCorrect()
    {
        using (Assert.EnterMultipleScope())
        {
            var options = new TagSerializerOptions { LittleEndian = true, VariableLength = true };

            var correct = CompoundTagBuilder
                .Create("Compound")
                .AddLong(long.MinValue, "Long")
                .AddString("Value", "String")
                .AddIntegers([int.MinValue, -1, 0, int.MaxValue], "Integers")
                .AddLongs([long.MinValue, -1, 0, long.MaxValue], "Longs")
                .Build();

            var writer = new ArrayBufferWriter<byte>();

            TagSerializer.Serialize(writer, correct, options);

            var result = TagSerializer.TryParse<CompoundTag>(writer.WrittenSpan, out var tag, options);

            Assert.That(result, Is.True);
            Assert.That(tag!.Name, Is.EqualTo("Compound"));
            Assert.That(tag.GetLong("Long"), Is.EqualTo(long.MinValue));
            Assert.That(tag.GetString("String"), Is.EqualTo("Value"));
            Assert.That(tag.GetIntegers("Integers"), Is.EqualTo(correct.GetIntegers("Integers")).AsCollection);
            Assert.That(tag.GetLongs("Longs"), Is.EqualTo(correct.GetLongs("Longs")).AsCollection);
        }
    }
}