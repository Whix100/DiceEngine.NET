namespace DiceEngine.Expressions.Terminals;

public class Undefined : Terminal
{
    public static readonly Undefined UNDEFINED = new Undefined("undefined");
    public static readonly Undefined DNE = new Undefined("dne");

    public readonly string Identifier;

    private Undefined(string identifier)
        => Identifier = identifier;

    public override bool Equals(object? obj)
        => obj is not null && obj is Undefined;

    public override int GetHashCode()
        => Identifier.GetHashCode();

    public override string ToString(string? _)
        => Identifier;
}
