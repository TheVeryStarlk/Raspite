namespace Raspite.Serializer.Tags;

public sealed record ByteTag(byte Value, string Name = "") : Tag<byte>(Value, Name)
{
    public override byte Identifier => 1;
}