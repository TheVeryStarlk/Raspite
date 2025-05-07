namespace Raspite.Tags;

public sealed class ListTag(TagBase[] value, string name = "") : TagBase<TagBase[]>(value, name);