[![Raspite.Serializer](https://img.shields.io/nuget/v/Raspite.Serializer)](https://www.nuget.org/packages/Raspite.Serializer/)

<p align="center">
  <img width="175" height="175" align="center" src="raspite.png">
</p>

# Raspite
An up-to-date and easy to use library for serializing/deserializing Minecraft's named binary tag (NBT) format.

# Usage
Raspite's API is similar to STJ's.

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