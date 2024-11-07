namespace Raspite.Serializer.Tags;

public sealed record LongTag : Tag<long>
{
	public override byte Identifier => 4;

	private LongTag()
	{
	}

	public static LongTag Create(long value, string name = "")
	{
		return new LongTag
		{
			Name = name,
			Value = value
		};
	}
}