namespace Raspite.Serializer.Tags;

public sealed record IntegerCollectionTag : CollectionTag<int>
{
    public override byte Identifier => 11;

    private IntegerCollectionTag()
    {
    }

    public static IntegerCollectionTag Create(int[] children, string name = "")
    {
        return new IntegerCollectionTag
        {
            Name = name,
            Children = children
        };
    }
}