using System.Collections.Immutable;

using Raspite.Tags.Building;

namespace Raspite.Tags;

public interface IListTag : ITag
{
    int Length { get; }

    ImmutableArray<ITag> RawTags { get; }
}

public static class ListTag
{
    public static IListTag Create(IReadOnlyList<ITag> tags, string name = "", bool validate = true)
    {
        if (tags.Count == 0)
        {
            return new ListTag<EndTag>([], name);
        }

        var identifier = tags[0].Identifier;

        if (validate)
        {
            for (int i = 1; i < tags.Count; i++)
            {
                if (identifier != tags[i].Identifier)
                {
                    throw new ArgumentException("All tags in the list must be of the same type.", nameof(tags));
                }
            }
        }

        return identifier switch
        {
            Tag.Byte => Create(tags.Cast<ByteTag>(), name),
            Tag.Short => Create(tags.Cast<ShortTag>(), name),
            Tag.Integer => Create(tags.Cast<IntegerTag>(), name),
            Tag.Long => Create(tags.Cast<LongTag>(), name),
            Tag.Float => Create(tags.Cast<FloatTag>(), name),
            Tag.Double => Create(tags.Cast<DoubleTag>(), name),
            Tag.Bytes => Create(tags.Cast<BytesTag>(), name),
            Tag.String => Create(tags.Cast<StringTag>(), name),
            Tag.List => Create(tags.Cast<IListTag>(), name),
            Tag.Compound => Create(tags.Cast<CompoundTag>(), name),
            Tag.Integers => Create(tags.Cast<IntegersTag>(), name),
            Tag.Longs => Create(tags.Cast<LongsTag>(), name),
            _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, "Invalid tag identifier.")
        };
    }

    public static ListTag<TTag> Create<TTag>(IEnumerable<TTag> tags, string name = "") where TTag : class, ITag
    {
        return new ListTag<TTag>([.. tags], name);
    }
}

public sealed class ListTag<TTag>(ImmutableArray<TTag> value, string name = "") : Tag<ImmutableArray<TTag>>(value, name), IListTag where TTag : class, ITag
{
    public override byte Identifier => List;

    public TTag this[int index] => Value[index];

    public int Length { get; } = value.Length;

    public ImmutableArray<ITag> RawTags { get; } = ImmutableArray<ITag>.CastUp(value);

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