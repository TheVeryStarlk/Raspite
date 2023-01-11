using Raspite.Library;

var compound = new CompoundTag
{
    Name = "World",
    Value = new TagBase[]
    {
        new IntTag
        {
            Name = "ID",
            Value = Random.Shared.Next()
        },
        new ListTag
        {
            Name = "Players",
            Value = new StringTag[]
            {
                new StringTag
                {
                    Value = "Starlk"
                }
            }
        }
    }
};

_ = BinaryTagSerializer.Serialize(compound);