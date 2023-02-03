using Raspite.Library;

var tag = new StringTag()
{
    Name = "Username",
    Value = "Raspite"
};

var bytes = BinaryTagSerializer.Serialize(tag);