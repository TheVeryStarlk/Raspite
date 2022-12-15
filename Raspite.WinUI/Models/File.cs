using Raspite.Library;
using System.IO;

namespace Raspite.WinUI.Models;

internal sealed record File(string Path, NbtTag Tag)
{
    public string Name => System.IO.Path.GetFileName(Path);
}