namespace Raspite.Serializer.Tags;

public sealed record LongCollectionTag : CollectionTag<long>
{
	public override byte Identifier => 12;

	private LongCollectionTag()
	{
	}

	public static LongCollectionTag Create(long[] children, string name = "")
	{
		return new LongCollectionTag
		{
			Name = name,
			Children = children
		};
	}

	internal override int CalculateLength(bool nameless)
	{
		return base.CalculateLength(nameless)
		       + sizeof(int)
		       + sizeof(long) * Children.Length;
	}
}