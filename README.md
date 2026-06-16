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

However, for streaming scenarios it can be easily used like in the example below.

```cs
async ValueTask<Tag?> ReadTagAsync(PipeReader reader, CancellationToken cancellationToken)
{
    while (true)
    {
        var result = await reader.ReadAsync(cancellationToken);
        var buffer = result.Buffer;

        // In the event that no tag is parsed successfully, mark consumed as nothing and examined as the entire buffer.
        var consumed = buffer.Start;
        var examined = buffer.End;

        try
        {
            var span = result.Buffer.IsSingleSegment ? result.Buffer.FirstSpan : result.Buffer.ToArray();

            if (TagSerializer.TryParse(span, out var tag))
            {
                consumed = buffer.Start;
                examined = consumed;

                return tag;
            }

            if (result.IsCompleted)
            {
                if (buffer.Length > 0)
                {
                    throw new InvalidDataException("Incomplete message.");
                }

                break;
            }
        }
        finally
        {
            reader.AdvanceTo(consumed, examined);
        }
    }

    return null;
}
```

### Writing

```cs
var tag = new StringTag("Hello, world!");
TagSerializer.Serialize(buffer, tag);
```

## Plans

* Implement variable-prefix types.
