<p align="center">
  <img width="100" height="100" align="center" src="raspite.png">
</p>

# Raspite

A fast, lightweight, and easy-to-use [NBT](https://minecraft.wiki/w/NBT_format) serialization library.

## Usage

Breaking changes are to be expected.

## Reading

```cs
var success = BinaryTagSerializer.TryRead(
    source, 
    out var tag, 
    BinaryTagSerializerOptions.Default);
```

Reading returns true if the whole buffer was read, otherwise false.

## Writing

```cs
var tag = CompoundTagBuilder
    .Create()
    .AddStringTag("Raspite", "Name")
    .Build();

BinaryTagSerializer.Serialize(
    buffer, 
    tag, 
    BinaryTagSerializerOptions.Default);
```