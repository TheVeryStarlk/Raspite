namespace Raspite.Library;

internal sealed class BinaryWriter
{
    private readonly Tag source;
    private readonly bool needSwap;

    public BinaryWriter(Tag source, BinaryOptions options)
    {
        this.source = source;
        needSwap = BitConverter.IsLittleEndian == options.Endianness is Endianness.Big;
    }

    public byte[] Run()
    {
        return Scan(source);
    }

    private byte[] Scan(Tag parent)
    {
        return parent switch
        {
            Tag.Byte tag => tag.Deserialize(needSwap),
            Tag.Short tag => tag.Deserialize(needSwap),
            Tag.Int tag => tag.Deserialize(needSwap),
            Tag.Long tag => tag.Deserialize(needSwap),
            Tag.Float tag => tag.Deserialize(needSwap),
            Tag.Double tag => tag.Deserialize(needSwap),
            Tag.ByteArray tag => tag.Deserialize(needSwap),
            Tag.String tag => tag.Deserialize(needSwap),
            Tag.List tag => tag.Deserialize(needSwap),
            Tag.Compound tag => tag.Deserialize(needSwap),
            Tag.IntArray tag => tag.Deserialize(needSwap),
            Tag.LongArray tag => tag.Deserialize(needSwap),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}