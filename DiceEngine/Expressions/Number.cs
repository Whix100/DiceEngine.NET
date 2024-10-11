using DiceEngine.BuiltInFunctions;
using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;
using DiceEngine.TypeConverters;
using System;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the the <see cref="Number"/> class.
/// </summary>
/// <param name="value">The numeric value.</param>
public class Number(double value) : IExpression, ILogicalConvertible, IPrefixOperable, IPostfixOperable, IBinaryOperable
{
    public static readonly Number ZERO = (Number)0;
    public static readonly Number ONE = (Number)1;

    public double Value { get; set; } = value;

    public bool IsInteger { get; set; } = value % 1 == 0;

    public bool IsEven { get; set; } = value % 2 == 0;

    public bool IsOdd { get; set; } = value % 2 != 0;

    public bool IsPositive { get; set; } = value > 0;

    public bool IsNegative { get; set; } = value < 0;

    public IExpression Evaluate()
        => Evaluate(null!);

    public IExpression Evaluate(ExpressionContext variables)
        => Value switch
        {
            Double.PositiveInfinity => Infinity.POSITIVE,
            Double.NegativeInfinity => Infinity.NEGATIVE,
            Double.NaN => Undefined.UNDEFINED,
            _ => this
        };

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext variables)
        => Evaluate(variables);

    public Logical ToLogical()
        => Value switch
        {
            0 => Logical.FALSE,
            _ => Logical.TRUE
        };

    public IExpression PrefixOperate(string identifier, ExpressionContext _)
    {
        return identifier switch
        {
            PrefixOperator.POSITIVE => this,
            PrefixOperator.NEGATIVE => new Number(-Value),
            PrefixOperator.NOT or PrefixOperator.NOT_ALT => Value == 0 ? Logical.TRUE : Logical.FALSE,
            PrefixOperator.SUBFACTORIAL => new Number(FactorialFunctions.Subfactorial(Value)),
            PrefixOperator.PRE_DECREMENT => new Number(Value - 1),
            PrefixOperator.PRE_INCREMENT => new Number(Value + 1),
            _ => Undefined.UNDEFINED,
        };
    }

    public IExpression PostfixOperate(string identifier, ExpressionContext _)
    {
        return identifier switch
        {
            PostfixOperator.FACTORIAL => new Number(FactorialFunctions.Factorial(Value)),
            PostfixOperator.PERCENT => new Number(Value / 100),
            PostfixOperator.DOUBLE_FACTORIAL => new Number(FactorialFunctions.DoubleFactorial(Value)),
            PostfixOperator.PRIMORIAL => new Number(FactorialFunctions.Primorial(Value)),
            PostfixOperator.POST_DECREMENT or PostfixOperator.POST_INCREMENT => this,
            _ => Undefined.UNDEFINED,
        };
    }

    public IExpression? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext context)
    {
        if (right is Number num)
        {
            switch (identifier)
            {
                case BinaryOperator.ADDITION:
                    return (Number)(Value + num.Value);
                case BinaryOperator.SUBTRACTION:
                    return (Number)(Value - num.Value);
                case BinaryOperator.MULTIPLICATION:
                case BinaryOperator.CROSS_MULTIPLICATION:
                    return (Number)(Value * num.Value);
                case BinaryOperator.SLASH_DIVISION:
                case BinaryOperator.DIVISION:
                    return (Number)(Value / num.Value);
                case BinaryOperator.EXPONENT:
                    return (Number)Math.Pow(Value, num.Value);
                case BinaryOperator.EUCLIDEAN_MODULUS:
                    return (Number)(Value - Math.Abs(num.Value) * Math.Floor(Value / Math.Abs(num.Value)));
                case BinaryOperator.TRUC_MODULUS:
                    return (Number)(Value % num.Value);
                case BinaryOperator.INT_DIVISION:
                    return (Number)(Math.Sign(num.Value) * Math.Floor(Value / Math.Abs(num.Value)));
                case BinaryOperator.AND:
                case BinaryOperator.AND_ALT:
                case BinaryOperator.OR:
                case BinaryOperator.OR_ALT:
                case BinaryOperator.XOR:
                    return ToLogical().BinaryLeftOperate(identifier, num.ToLogical(), context);
                case BinaryOperator.IS_EQUAL:
                    return (Logical)(Value == num.Value);
                case BinaryOperator.NOT_EQUAL:
                case BinaryOperator.NOT_EQUAL_ALT:
                case BinaryOperator.GREATER_OR_LESS_THAN:
                    return (Logical)(Value != num.Value);
                case BinaryOperator.LESS_THAN:
                    return (Logical)(Value < num.Value);
                case BinaryOperator.LESS_THAN_OR_EQUAL:
                case BinaryOperator.LESS_THAN_OR_EQUAL_ALT:
                    return (Logical)(Value <= num.Value);
                case BinaryOperator.GREATER_THAN:
                    return (Logical)(Value > num.Value);
                case BinaryOperator.GREATER_THAN_OR_EQUAL:
                case BinaryOperator.GREATER_THAN_OR_EQUAL_ALT:
                    return (Logical)(Value >= num.Value);
            }

            return null;
        }

        return null;
    }

    public IExpression? BinaryRightOperate(string identifier, IExpression left, ExpressionContext context)
    {
        switch (identifier)
        {
            case BinaryOperator.ADDITION:
            case BinaryOperator.SUBTRACTION:
            case BinaryOperator.MULTIPLICATION:
            case BinaryOperator.CROSS_MULTIPLICATION:
            case BinaryOperator.SLASH_DIVISION:
            case BinaryOperator.DIVISION:
            case BinaryOperator.EXPONENT:
            case BinaryOperator.EUCLIDEAN_MODULUS:
            case BinaryOperator.TRUC_MODULUS:
            case BinaryOperator.INT_DIVISION:
            case BinaryOperator.AND:
            case BinaryOperator.AND_ALT:
            case BinaryOperator.OR:
            case BinaryOperator.OR_ALT:
            case BinaryOperator.XOR:
            case BinaryOperator.IS_EQUAL:
            case BinaryOperator.NOT_EQUAL:
            case BinaryOperator.NOT_EQUAL_ALT:
            case BinaryOperator.GREATER_OR_LESS_THAN:
            case BinaryOperator.LESS_THAN:
            case BinaryOperator.LESS_THAN_OR_EQUAL:
            case BinaryOperator.LESS_THAN_OR_EQUAL_ALT:
            case BinaryOperator.GREATER_THAN:
            case BinaryOperator.GREATER_THAN_OR_EQUAL:
            case BinaryOperator.GREATER_THAN_OR_EQUAL_ALT:
                return null;
        }

        return null;
    }

    public IExpression EvaluateDice()
        => this;

    public IExpression EvaluateDice(ExpressionContext _)
        => this;

    public override bool Equals(object? obj)
        => obj is not null && obj is Number n && n.Value == Value;

    public override int GetHashCode()
        => Value.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => Value.ToString(format);

    public static explicit operator decimal(Number n)
        => (decimal)n.Value;

    public static explicit operator double(Number n)
        => n.Value;

    public static explicit operator float(Number n)
        => (float)n.Value;

    public static explicit operator long(Number n)
        => (long)n.Value;

    public static explicit operator int(Number n)
        => (int)n.Value;

    public static explicit operator short(Number n)
        => (short)n.Value;

    public static explicit operator sbyte(Number n)
        => (sbyte)n.Value;

    public static explicit operator ulong(Number n)
        => (ulong)n.Value;

    public static explicit operator uint(Number n)
        => (uint)n.Value;

    public static explicit operator ushort(Number n)
        => (ushort)n.Value;

    public static explicit operator byte(Number n)
        => (byte)n.Value;

    public static explicit operator Number(decimal n)
        => new Number((double)n);

    public static explicit operator Number(double n)
        => new Number(n);

    public static explicit operator Number(float n)
        => new Number(n);

    public static explicit operator Number(long n)
        => new Number(n);

    public static explicit operator Number(int n)
        => new Number(n);

    public static explicit operator Number(short n)
        => new Number(n);

    public static explicit operator Number(sbyte n)
        => new Number(n);

    public static explicit operator Number(ulong n)
        => new Number(n);

    public static explicit operator Number(uint n)
        => new Number(n);

    public static explicit operator Number(ushort n)
        => new Number(n);

    public static explicit operator Number(byte n)
        => new Number(n);
}
