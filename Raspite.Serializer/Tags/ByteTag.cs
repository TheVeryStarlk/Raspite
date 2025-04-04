namespace Raspite.Serializer.Tags;

public sealed record ByteTag(byte Value, string Name = "") : Tag<byte>
{
    public override byte Identifier => 1;
}