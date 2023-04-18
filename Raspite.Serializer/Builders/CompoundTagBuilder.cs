using Raspite.Serializer.Tags;

namespace Raspite.Serializer.Builders;

/// <summary>
/// Provides methods to build a <see cref="CompoundTag"/> in builder style.
/// </summary>
public sealed class CompoundTagBuilder
{
    private readonly string tagName;
    private readonly List<Tag> children;

    public CompoundTagBuilder(string tagName)
    {
        this.tagName = tagName;
        children = new List<Tag>();
    }

    public static CompoundTagBuilder Create(string name = "")
    {
        return new CompoundTagBuilder(name);
    }

    public CompoundTag Build()
    {
        return new CompoundTag()
        {
            Name = tagName,
            Children = children.ToArray()
        };
    }

    public CompoundTagBuilder AddSignedByteTag(sbyte value, string name = "")
    {
        children.Add(new SignedByteTag()
        {
            Name = name,
            Value = value
        });

        return this;
    }

    public CompoundTagBuilder AddShortTag(short value, string name = "")
    {
        children.Add(new ShortTag()
        {
            Name = name,
            Value = value
        });

        return this;
    }

    public CompoundTagBuilder AddIntegerTag(int value, string name = "")
    {
        children.Add(new IntegerTag()
        {
            Name = name,
            Value = value
        });

        return this;
    }

    public CompoundTagBuilder AddLongTag(long value, string name = "")
    {
        children.Add(new LongTag()
        {
            Name = name,
            Value = value
        });

        return this;
    }

    public CompoundTagBuilder AddFloatTag(float value, string name = "")
    {
        children.Add(new FloatTag()
        {
            Name = name,
            Value = value
        });

        return this;
    }

    public CompoundTagBuilder AddDoubleTag(double value, string name = "")
    {
        children.Add(new DoubleTag()
        {
            Name = name,
            Value = value
        });

        return this;
    }

    public CompoundTagBuilder AddSignedByteCollectionTag(sbyte[] value, string name = "")
    {
        children.Add(new SignedByteCollectionTag()
        {
            Name = name,
            Children = value
        });

        return this;
    }

    public CompoundTagBuilder AddStringTag(string value, string name = "")
    {
        children.Add(new StringTag()
        {
            Name = name,
            Value = value
        });

        return this;
    }

    public CompoundTagBuilder AddListTag(Tag[] value, string name = "")
    {
        children.Add(new ListTag()
        {
            Name = name,
            Children = value
        });

        return this;
    }

    public CompoundTagBuilder AddCompoundTag(Tag[] value, string name = "")
    {
        children.Add(new CompoundTag()
        {
            Name = name,
            Children = value
        });

        return this;
    }

    public CompoundTagBuilder AddIntegerCollectionTag(int[] value, string name = "")
    {
        children.Add(new IntegerCollectionTag()
        {
            Name = name,
            Children = value
        });

        return this;
    }

    public CompoundTagBuilder AddLongCollectionTag(long[] value, string name = "")
    {
        children.Add(new LongCollectionTag()
        {
            Name = name,
            Children = value
        });

        return this;
    }
}