namespace Raspite.Tags;

public sealed class LongTag(long value, string name = "") : TagBase<long>(value, name);