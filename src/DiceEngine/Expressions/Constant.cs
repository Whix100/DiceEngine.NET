using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the <see cref="Constant"/> class.
/// </summary>
/// <param name="identifier">The identifier <see cref="string"/> for this <see cref="Constant"/>.</param>
public class Constant(string identifier) : IExpression
{
    private static readonly Dictionary<string, Terminal> _values = new Dictionary<string, Terminal>()
    {
        { "π", new Number(Math.PI) },
        { "pi", new Number(Math.PI) },
        { "τ", new Number(Math.Tau) },
        { "tau", new Number(Math.Tau) },
        { "e", new Number(Math.E) },
        { "∅", new TerminalCollection<Set>(new Set()) },
        { "empty", new TerminalCollection<Set>(new Set()) },
        { "empty_set", new TerminalCollection<Set>(new Set()) },
    };

    public readonly string Identifier = identifier;

    public static Constant PI { get; } = new Constant("π");
    public static Constant TAU { get; } = new Constant("τ");
    public static Constant E { get; } = new Constant("e");

    public Terminal Evaluate()
        => _values[Identifier];

    public Terminal Evaluate(ExpressionContext _)
        => Evaluate();

    public IExpression StepEvaluate()
        => Evaluate();

    public IExpression StepEvaluate(ExpressionContext _)
        => Evaluate();

    public override bool Equals(object? obj)
        => obj is not null && obj is Constant c && 
            (c.Identifier == Identifier || Identifier switch
            {
                "π" or "pi" => c.Identifier == "π" || c.Identifier == "pi",
                "τ" or "tau" => c.Identifier == "τ" || c.Identifier == "tau",
                "e" => c.Identifier == Identifier,
                "∅" or "empty" or "empty_set" => c.Identifier == "∅" || c.Identifier == "empty" ||
                    c.Identifier == "empty_set",
                _ => false,
            });

    public override int GetHashCode()
        => Identifier.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => Identifier;
}
