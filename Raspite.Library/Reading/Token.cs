namespace Raspite.Library.Reading;

public record Token(Tag Tag)
{
    public sealed record Parent(Tag Tag, string? Name, Token[] Tokens) : Token(Tag);

    public sealed record Value(Tag Tag, string? Name, object Content) : Token(Tag);
}