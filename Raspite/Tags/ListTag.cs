using System.Collections.Immutable;

using Raspite.Tags.Building;

namespace Raspite.Tags;

public interface IListTag : ITag
{
    int Length { get; }

    ImmutableArray<Tag> RawTags { get; }
}

public sealed class ListTag<TTag>(ImmutableArray<TTag> value, string name = "") : Tag<ImmutableArray<TTag>>(value, name), IListTag where TTag : ITag
{
    public override byte Identifier => List;

    public TTag this[int index] => Value[index];

    public int Length { get; } = value.Length;

    public ImmutableArray<Tag> RawTags { get; } = value.CastArray<Tag>();

    /// <summary>
    /// Converts this tag to a builder containing all the values this tag contained.
    /// It is the inverse operation of <code>ListTagBuilder{TTag}.Build()</code>
    /// </summary>
    /// <param name="name">
    /// The name of the tag about to be built.
    /// Can be used for renaming it.
    /// <code>null</code> to leave the name as it was.
    /// </param>
    /// <returns></returns>
    public ListTagBuilder<TTag> ToBuilder(string? name = null)
    {
        return new ListTagBuilder<TTag>([.. Value], name ?? Name);
    }
}

public static class ListTagExtensions
{
    public static byte GetByte(this ListTag<ByteTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static bool GetBoolean(this ListTag<ByteTag> listTag, int index)
    {
        return listTag[index].Value != 0; 
    }

    public static short GetShort(this ListTag<ShortTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static int GetInteger(this ListTag<IntegerTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static long GetLong(this ListTag<LongTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static float GetFloat(this ListTag<FloatTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static double GetDouble(this ListTag<DoubleTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static string GetString(this ListTag<StringTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static ImmutableArray<byte> GetBytes(this ListTag<BytesTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static ImmutableArray<int> GetIntegers(this ListTag<IntegersTag> listTag, int index)
    {
        return listTag[index].Value; 
    }

    public static ImmutableArray<long> GetLongs(this ListTag<LongsTag> listTag, int index)
    {
        return listTag[index].Value; 
    }
}