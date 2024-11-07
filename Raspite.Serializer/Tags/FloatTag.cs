namespace Raspite.Serializer.Tags;

public sealed record FloatTag : Tag<float>
{
	public override byte Identifier => 5;

	private FloatTag()
	{
	}

	public static FloatTag Create(float value, string name = "")
	{
		return new FloatTag
		{
			Name = name,
			Value = value
		};
	}
}