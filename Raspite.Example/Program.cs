using Raspite.Serializer;
using Raspite.Serializer.Tags;

var tag = CompoundTag.Create("Parent")
	.Add(StringTag.Create("Hello, world!", "Message"))
	.Add(CompoundTag.Create("Numbers")
		.Add(IntegerTag.Create(2048, "First"))
		.Add(ByteTag.Create(255, "Big"))
		.Add(ByteTag.Create(69, "Nice"))
		.Add(FloatTag.Create(3.14F, "Wow"))
		.Build())
	.Build();

var buffer = new byte[byte.MaxValue];

BinaryTagSerializer.Serialize(buffer, tag);

var result = BinaryTagSerializer.Deserialize<CompoundTag>(buffer);

var answer = result
	.First<CompoundTag>()
	.First<ByteTag>("Nice").Value;

Console.WriteLine(answer);