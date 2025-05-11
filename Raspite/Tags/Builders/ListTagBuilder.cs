namespace Raspite.Tags.Builders;

public sealed class ListTagBuilder<T> where T : Tag
{
    private readonly string parent;

    // Arbitrary length;
    private readonly T[] children = new T[byte.MaxValue];

    private int index;

    private ListTagBuilder(string parent)
    {
        this.parent = parent;
    }

    public static ListTagBuilder<T> Create(string parent = "")
    {
        return new ListTagBuilder<T>(parent);
    }

    public ListTagBuilder<T> Add(T tag)
    {
        children[index++] = tag;
        return this;
    }

    public ListTag<T> Build()
    {
        return new ListTag<T>(children[..index], parent);
    }
}