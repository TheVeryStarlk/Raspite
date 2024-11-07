namespace Raspite.Serializer.Tags;

public sealed record IntegerTag : Tag<int>
{
	public override byte Identifier => 3;

	private IntegerTag()
	{
	}

	public static IntegerTag Create(int value, string name = "")
	{
		return new IntegerTag
		{
			Name = name,
			Value = value
		};
	}
}