<p align="center">
  <img width="100" height="100" align="center" src="raspite.png">
</p>

# Raspite
A fast, lightweight, and easy-to-use [NBT](https://minecraft.wiki/w/NBT_format) serialization library.

## Usage

Raspite is quite similar to STJ in terms of using.

## Reading

```cs
var tag = BinaryTagSerializer.Deserialize<CompoundTag>(source);
```

Deserializing expects a complete buffer, otherwise it'll throw an exception.

## Writing

```cs
var tag = CompoundTagBuilder
    .Create()
    .AddStringTag("Raspite", "Name")
    .Build();

BinaryTagSerializer.Serialize(buffer, tag);
```