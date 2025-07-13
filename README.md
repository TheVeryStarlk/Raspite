# Raspite

A fast, lightweight, and easy-to-use [NBT](https://minecraft.wiki/w/NBT_format) serialization library.

## Usage

Breaking changes are to be expected.

## Parsing

```cs
var success = TagSerializer.TryParse(source, out var tag);
```

## Writing

```cs
var tag = new StringTag("Hello, world!");
TagSerializer.Serialize(buffer, tag);
```

## Plans

* Implement variable-prefix types.