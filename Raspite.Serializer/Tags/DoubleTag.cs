namespace Raspite.Serializer.Tags;

public sealed record DoubleTag : Tag<double>
{
	public override byte Identifier => 6;

	private DoubleTag()
	{
	}

	public static DoubleTag Create(double value, string name = "")
	{
		return new DoubleTag
		{
			Name = name,
			Value = value
		};
	}
}