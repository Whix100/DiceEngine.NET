using DiceEngine.Context;

namespace DiceEngine.Expressions.Terminals;

public abstract class Terminal : IExpression
{
    public Terminal Evaluate()
        => this;

    public Terminal Evaluate(ExpressionContext _)
        => this;

    public IExpression StepEvaluate()
        => this;

    public IExpression StepEvaluate(ExpressionContext _)
        => this;

    public override string ToString()
        => ToString(null);

    public abstract string ToString(string? format);

    public static explicit operator Terminal(decimal n)
        => new Number((double)n);

    public static explicit operator Terminal(double n)
    {
        if (double.IsFinite(n))
        {
            return new Number(n);
        }
        else if (double.IsInfinity(n))
        {
            return double.IsPositive(n) ? Infinity.POSITIVE : Infinity.NEGATIVE;
        }

        return Undefined.UNDEFINED;
    }

    public static explicit operator Terminal(float n)
    {
        if (float.IsFinite(n))
        {
            return new Number(n);
        }
        else if (float.IsInfinity(n))
        {
            return float.IsPositive(n) ? Infinity.POSITIVE : Infinity.NEGATIVE;
        }

        return Undefined.UNDEFINED;
    }

    public static explicit operator Terminal(long n)
        => new Number(n);

    public static explicit operator Terminal(int n)
        => new Number(n);

    public static explicit operator Terminal(short n)
        => new Number(n);

    public static explicit operator Terminal(sbyte n)
        => new Number(n);

    public static explicit operator Terminal(ulong n)
        => new Number(n);

    public static explicit operator Terminal(uint n)
        => new Number(n);

    public static explicit operator Terminal(ushort n)
        => new Number(n);

    public static explicit operator Terminal(byte n)
        => new Number(n);
}
