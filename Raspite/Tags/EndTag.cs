namespace Raspite.Tags;

public sealed class EndTag : Tag
{
    public static EndTag Instance { get; } = new();

    public override byte Identifier => End;

    private EndTag()
    {
        // Bad.
    }
}