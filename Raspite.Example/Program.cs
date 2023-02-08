using Raspite.Serializer;
using Raspite.Serializer.Tags;

TagBase tag = new StringTag()
{
    Name = "Username",
    Value = string.Empty
};

var bytes = BinaryTagSerializer.Serialize(tag);
// BinaryTagSerializer.Deserialize<StringTag>(bytes);

tag.SetValue("Raspite");
Console.WriteLine(tag.GetValue<string>());