using Raspite.Serializer;
using Raspite.Serializer.Tags;

await using var file = File.OpenRead("bigtest.nbt");
var tag = await BinaryTagSerializer.DeserializeAsync<CompoundTag>(file);

// await BinaryTagSerializer.SerializeAsync(tag, file);