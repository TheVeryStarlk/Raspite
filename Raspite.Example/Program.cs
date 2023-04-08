using Raspite.Serializer;
using Raspite.Serializer.Tags;

var tag = new CompoundTag()
{
    Children = new Tag[]
    {
        new StringTag()
        {
            Name = "Username",
            Value = "Raspite"
        }
    }
};

await using var file = File.OpenWrite("file.dat");

await BinaryTagSerializer.SerializeAsync(tag, file);