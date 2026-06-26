using System.Collections.Immutable;

namespace Raspite.Tags;

public sealed class BytesTag(ImmutableArray<byte> value, string name = "") : Tag<ImmutableArray<byte>>(value, name)
{
    public override byte Identifier => Bytes;

    public static BytesTag Create(IEnumerable<byte> bytes, string name = "")
    {
        return new BytesTag([.. bytes], name);
    }
}