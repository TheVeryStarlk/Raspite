namespace Raspite.Tags;

public sealed class IntegerTag(int value, string name = "") : TagBase<int>(value, name);