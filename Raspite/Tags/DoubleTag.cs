namespace Raspite.Tags;

public sealed class DoubleTag(double value, string name = "") : TagBase<double>(value, name);