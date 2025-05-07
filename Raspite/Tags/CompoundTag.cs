namespace Raspite.Tags;

public sealed class CompoundTag(TagBase[] value, string name = "") : TagBase<TagBase[]>(value, name);