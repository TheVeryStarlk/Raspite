namespace Raspite.Tags;

public sealed class ShortTag(short value, string name = "") : TagBase<short>(value, name);