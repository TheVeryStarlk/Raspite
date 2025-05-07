namespace Raspite.Tags;

public sealed class IntegerCollectionTag(int[] value, string name = "") : TagBase<int[]>(value, name);