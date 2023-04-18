[![Raspite.Serializer](https://img.shields.io/nuget/v/Raspite.Serializer)](https://www.nuget.org/packages/Raspite.Serializer/1.1.0/)

<p align="center">
  <img width="175" height="175" align="center" src="Raspite.Serializer/raspite.png">
</p>

# Raspite
An up-to-date and easy to use library for serializing/deserializing Minecraft's named binary tag (NBT) format.

# Usage

#### Serializing
Serializing takes a tag and a stream to serialize to:
```cs
var tag = new StringTag()
{
    Value = "Hello, world!"
};

await using var stream = new MemoryStream();
await BinaryTagSerializer.SerializeAsync(tag, stream);
```

Deserializing is pretty much the same, but in a reversed way:
```cs
await using var stream = new MemoryStream();
var tag = await BinaryTagSerializer.DeserializeAsync<StringTag>(file);
```

#### Collection tags

* The compound tag builder provides easy API to build a compound tag:
```cs
var tag = CompoundTagBuilder.Create("Parent")
    .AddStringTag("Hello, world!")
    .AddShortTag(6942)
    .Build();
```

* All collection tags have an indexer for indexing tags/values:
```cs
var stringTag = (StringTag) tag[0];
```

* Compound tag and list tag offer LINQ-like searching methods:
```cs
var stringTag = tag.First<StringTag>(name: "Username");
```