using DiceEngine.Context;

namespace DiceEngine.Expressions;

public class Undefined : IExpression
{
    public static readonly Undefined UNDEFINED = new Undefined("undefined");
    public static readonly Undefined DNE = new Undefined("dne");

    public readonly string Identifier;

    private Undefined(string identifier)
        => Identifier = identifier;

    public IExpression Evaluate()
        => this;

    public IExpression Evaluate(ExpressionContext _)
        => this;

    public IExpression StepEvaluate()
        => this;

    public IExpression StepEvaluate(ExpressionContext _)
        => this;

    public IExpression EvaluateDice()
        => this;

    public IExpression EvaluateDice(ExpressionContext _)
        => this;

    public override bool Equals(object? obj)
        => obj is not null && obj is Undefined;

    public override int GetHashCode()
        => Identifier.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? _)
        => Identifier;
}
