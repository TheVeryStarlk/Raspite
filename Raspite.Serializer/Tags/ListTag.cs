namespace Raspite.Serializer.Tags;

public sealed record ListTag : CollectionTag<Tag>
{
	public override byte Identifier => 9;

	private ListTag()
	{
	}

	public static ListTag Create(Tag[] children, string name = "")
	{
		return new ListTag
		{
			Name = name,
			Children = children
		};
	}

	public static ListTagBuilder<T> Create<T>(string name = "") where T : Tag
	{
		return new ListTagBuilder<T>(name);
	}

	internal override int CalculateLength(bool nameless)
	{
		return base.CalculateLength(nameless)
		       + sizeof(byte)
		       + sizeof(int)
		       + Children.Sum(child => child.CalculateLength(true));
	}
}

public sealed class ListTagBuilder<T> where T : Tag
{
	private readonly string name;
	private readonly List<T> children = [];

	internal ListTagBuilder(string name)
	{
		this.name = name;
	}

	public ListTagBuilder<T> Add(T tag)
	{
		children.Add(tag);
		return this;
	}

	public ListTag Build()
	{
		// ReSharper disable once CoVariantArrayConversion
		return ListTag.Create(children.ToArray(), name);
	}
}