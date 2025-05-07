namespace Raspite.Tags;

public sealed class LongCollectionTag(long[] value, string name = "") : TagBase<long[]>(value, name);