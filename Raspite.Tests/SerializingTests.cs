using Raspite.Serializer;
using Raspite.Serializer.Tags;

namespace Raspite.Tests;

internal sealed class SerializingTests
{
	[Test]
	public void SingleTag_Serializes_Correctly()
	{
		var tag = StringTag.Create("Serializer", "Raspite");

		var actual = new byte[]
		{
			0x08,
			0x00, 0x07,
			0x52, 0x61, 0x73, 0x70, 0x69, 0x74, 0x65,
			0x00, 0x0A,
			0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x69, 0x7a, 0x65, 0x72
		};

		var expected = new byte[actual.Length];

		var position = BinaryTagSerializer.Serialize(expected, tag);

		Assert.Multiple(() =>
		{
			Assert.That(position, Is.EqualTo(actual.Length));
			Assert.That(expected.ToArray(), Is.EqualTo(actual));
		});
	}

	[Test]
	public void SingleTag_Deserializes_Correctly()
	{
		var source = new byte[]
		{
			0x08,
			0x00, 0x07,
			0x52, 0x61, 0x73, 0x70, 0x69, 0x74, 0x65,
			0x00, 0x0A,
			0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x69, 0x7a, 0x65, 0x72
		};

		var actual = BinaryTagSerializer.Deserialize<StringTag>(source);

		var expected = StringTag.Create("Serializer", "Raspite");

		Assert.Multiple(() =>
		{
			Assert.That(expected.Name, Is.EqualTo(actual.Name));
			Assert.That(expected.Value, Is.EqualTo(actual.Value));
		});
	}

	[Test]
	public async Task SingleTag_Serializes_Correctly_Async()
	{
		var tag = StringTag.Create("Serializer", "Raspite");

		var actual = new byte[]
		{
			0x08,
			0x00, 0x07,
			0x52, 0x61, 0x73, 0x70, 0x69, 0x74, 0x65,
			0x00, 0x0A,
			0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x69, 0x7a, 0x65, 0x72
		};

		using var expected = new MemoryStream();

		await BinaryTagSerializer.SerializeAsync(expected, tag);

		Assert.That(expected.ToArray(), Is.EqualTo(actual));
	}

	[Test]
	public async Task SingleTag_Deserializes_Correctly_Async()
	{
		var source = new MemoryStream(
		[
			0x08,
			0x00, 0x07,
			0x52, 0x61, 0x73, 0x70, 0x69, 0x74, 0x65,
			0x00, 0x0A,
			0x53, 0x65, 0x72, 0x69, 0x61, 0x6c, 0x69, 0x7a, 0x65, 0x72
		]);

		var actual = await BinaryTagSerializer.DeserializeAsync<StringTag>(source);

		var expected = StringTag.Create("Serializer", "Raspite");

		Assert.Multiple(() =>
		{
			Assert.That(expected.Name, Is.EqualTo(actual.Name));
			Assert.That(expected.Value, Is.EqualTo(actual.Value));
		});
	}
}