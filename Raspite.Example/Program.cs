using Raspite.Library;

var tag = new StringTag()
{
    Name = "Username",
    Value = "Raspite"
};

_ = BinaryTagSerializer.Serialize<StringTag>(tag);