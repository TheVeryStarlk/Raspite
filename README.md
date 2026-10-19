![](https://img.shields.io/nuget/v/Raspite)

<p align="center">
  <img width="150" height="150" align="center" src="Raspite.png">
</p>

# Raspite

A fast, light-weight, and easy-to-use [NBT](https://minecraft.wiki/w/NBT_format) serialization library.

## Usage

Breaking changes are to be expected.

### Building

```cs
var compound = CompoundTagBuilder
    .Create("Compound")
    .AddString("Value", "Name")
    .Build();

// Compile-time safe.
var list = ListTagBuilder<StringTag>
    .Create("List")
    .AddString("Value")
    .Build();
```

### Parsing

Parsing is as simple as a single line of code, especially when the entire buffer is already present.

```cs
var success = TagSerializer.TryParse<CompoundTag>(source, out var tag);
```

### Writing

```cs
var tag = new StringTag("Hello, world!");
TagSerializer.Serialize(buffer, tag);
```

### Bedrock

Bedrock edition's networking uses variable-length integers, longs and length prefixes.

```cs
var options = new TagSerializerOptions
{
    VariableLength = true
};

TagSerializer.Serialize(buffer, tag, options);
```
