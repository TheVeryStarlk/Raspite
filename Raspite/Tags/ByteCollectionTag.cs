namespace Raspite.Tags;

public sealed class ByteCollectionTag(byte[] value, string name = "") : TagBase<byte[]>(value, name);