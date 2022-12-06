```csharp
using Raspite.Library;

await NbtSerializer.SerializeAsync(await File.ReadAllTextAsync("level.dat"));
```