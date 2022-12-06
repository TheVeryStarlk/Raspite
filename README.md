```csharp
using Raspite.Library;

var _ = await NbtSerializer.SerializeAsync(await File.ReadAllTextAsync("level.dat"));
```