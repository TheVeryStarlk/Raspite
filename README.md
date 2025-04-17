<p align="center">
  <img width="175" height="175" align="center" src="raspite.png">
</p>

# Raspite
An up-to-date and easy to use library for serializing/deserializing Minecraft's [named binary tag (NBT)](https://minecraft.fandom.com/wiki/NBT_format) format.

# Usage
The API is similar to System.Text.Json's:

### Deserializing
```cs
await using var stream = File.OpenRead("test.nbt");
var tag = await BinaryTagSerializer.DeserializeAsync<CompoundTag>(stream);
```

### Serializing
```cs
var tag = CompoundTag.Create("Parent")
	.Add(StringTag.Create("Hello, world!", "Message"))
	.Build();

await using var stream = File.Create("test.nbt");
await BinaryTagSerializer.SerializeAsync(tag, stream);
```

There are also synchronous versions for deserializing and serializing.

### Tags
You can easily build tags using existing tag builders:

```csharp
var tag = CompoundTag.Create("Parent")
	.Add(StringTag.Create("Hello, world!", "Message"))
	.Add(CompoundTag.Create("Numbers")
		.Add(IntegerTag.Create(2048, "First"))
		.Add(ByteTag.Create(255, "Big"))
		.Add(ByteTag.Create(69, "Nice"))
		.Add(FloatTag.Create(3.14F, "Wow"))
		.Build())
	.Build();
```

Querying children:
```cs
var answer = tag.First<StringTag>("Message").Value;
```