namespace Raspite.Tags;

public sealed class StringTag(string value, string name = "") : TagBase<string>(value, name);