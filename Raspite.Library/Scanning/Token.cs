namespace Raspite.Library.Scanning;

// Name and value can be null because of tags like the ending tag don't have any name and value.
public sealed record Token(Tag Tag, string? Name = null, object? Value = null);