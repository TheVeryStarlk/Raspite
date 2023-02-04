using Raspite.Serializer;
using Raspite.Serializer.Tags;

var tag = new StringTag()
{
    Name = "Username",
    Value = "Raspite"
};

var bytes = BinaryTagSerializer.Serialize(tag);
// BinaryTagSerializer.Deserialize<StringTag>(bytes);