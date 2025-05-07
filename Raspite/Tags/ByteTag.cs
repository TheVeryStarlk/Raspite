namespace Raspite.Tags;

public sealed class ByteTag(byte value, string name = "") : TagBase<byte>(value, name);