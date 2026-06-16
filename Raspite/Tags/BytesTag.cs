using System.Collections.Immutable;

namespace Raspite.Tags;

public sealed class BytesTag(ImmutableArray<byte> value, string name = "") : Tag<ImmutableArray<byte>>(value, name)
{
    public override byte Identifier => Bytes;
}