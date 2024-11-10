# Raspite
An up-to-date and easy to use library for serializing/deserializing Minecraft's named binary tag (NBT) format.

# Usage
The API is similar to STJ's. Except that it only accepts streams to deserialize from and serialize to.

### Deserializing
```csharp
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