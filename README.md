<p align="center">
  <img width="100" height="100" align="center" src="raspite.png">
</p>

# Raspite

A fast, lightweight, and easy-to-use [NBT](https://minecraft.wiki/w/NBT_format) serialization library.

## Usage

Breaking changes are to be expected.

## Parsing

```cs
var success = TagSerializer.TryParse(source, out var tag);
```

Returns true if the buffer was parsed successfully; otherwise, false.

## Writing

```cs
var tag = new StringTag("Hello, world!");

TagSerializer.Serialize(buffer, tag);
```

## Plans

* Implement variable-prefix types.