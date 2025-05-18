<p align="center">
  <img width="100" height="100" align="center" src="raspite.png">
</p>

# Raspite

A fast, lightweight, and easy-to-use [NBT](https://minecraft.wiki/w/NBT_format) serialization library.

## Usage

Breaking changes are to be expected.

## Reading

```cs
var success = TagSerializer.TryParse(source, out var tag);
```

Reading returns true if the whole buffer was read, otherwise false.

## Writing

```cs
var tag = CompoundTagBuilder
    .Create()
    .AddStringTag("Raspite", "Name")
    .Build();

TagSerializer.Serialize(buffer, tag);
```

## Plans

* Possibly implement SNBT.
* Implement variable-prefix types.