using DiceEngine.Context;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions.Dice;

public class Die(int size) : IDie
{
    public readonly int Size = size;

    public Die(uint size) : this((int)size) { }

    public Terminal Evaluate()
        => Evaluate(new ExpressionContext());

    public Terminal Evaluate(ExpressionContext context)
        => new RollResult(Roll(context.Random), this);

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
        => Evaluate(context);

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => Evaluate(context);

    public int Roll(Random? random = null)
        => (random ?? new Random()).Next(Size) + 1;

    public override int GetHashCode()
        => Size;

    public override bool Equals(object? obj)
        => obj is Die die && die.Size == Size;

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"d{Size}";

    public static explicit operator Die(long n)
        => new Die((int)n);

    public static explicit operator Die(int n)
        => new Die(n);

    public static explicit operator Die(short n)
        => new Die(n);

    public static explicit operator Die(sbyte n)
        => new Die(n);

    public static explicit operator Die(ulong n)
        => new Die((int)n);

    public static explicit operator Die(uint n)
        => new Die(n);

    public static explicit operator Die(ushort n)
        => new Die(n);

    public static explicit operator Die(byte n)
        => new Die(n);
}

public interface IDie : IExpression
{
    int Roll(Random? random = null);
}
