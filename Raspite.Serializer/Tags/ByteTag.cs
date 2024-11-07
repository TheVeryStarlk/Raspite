namespace Raspite.Serializer.Tags;

public sealed record ByteTag : Tag<byte>
{
	public override byte Identifier => 1;

	private ByteTag()
	{
	}

	public static ByteTag Create(byte value, string name = "")
	{
		return new ByteTag
		{
			Name = name,
			Value = value
		};
	}
}