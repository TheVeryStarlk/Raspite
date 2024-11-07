namespace Raspite.Serializer.Tags;

public sealed record ShortTag : Tag<short>
{
	public override byte Identifier => 2;

	private ShortTag()
	{
	}

	public static ShortTag Create(short value, string name = "")
	{
		return new ShortTag
		{
			Name = name,
			Value = value
		};
	}
}