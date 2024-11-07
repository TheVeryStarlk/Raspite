namespace Raspite.Serializer.Tags;

public sealed record CompoundTag : CollectionTag<Tag>
{
	public override byte Identifier => 10;

	private CompoundTag()
	{
	}

	public static CompoundTag Create(Tag[] children, string name = "")
	{
		return new CompoundTag
		{
			Name = name,
			Children = children
		};
	}

	public static CompoundTagBuilder Create(string name = "")
	{
		return new CompoundTagBuilder(name);
	}

	public T First<T>(string? name = "") where T : Tag
	{
		var tag = Children.First(tag =>
		{
			var typeMatches = typeof(T) == tag.GetType();

			return name is not null
				? typeMatches && tag.Name == name
				: typeMatches;
		});

		return (T) tag;
	}

	internal override int CalculateLength(bool nameless)
	{
		return base.CalculateLength(nameless)
		       + Children.Sum(child => child.CalculateLength(false))
		       + sizeof(byte);
	}
}

public sealed class CompoundTagBuilder
{
	private readonly string name;
	private readonly List<Tag> children = [];

	internal CompoundTagBuilder(string name)
	{
		this.name = name;
	}

	public CompoundTagBuilder Add(Tag tag)
	{
		children.Add(tag);
		return this;
	}

	public CompoundTag Build()
	{
		return CompoundTag.Create(children.ToArray(), name);
	}
}