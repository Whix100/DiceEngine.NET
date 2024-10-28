using DiceEngine.NativeFunctions;
using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.Expressions.Terminals;

/// <summary>
/// Initializes a new instance of the <see cref="Number"/> class.
/// </summary>
/// <param name="value">The numeric value.</param>
public class Number(double value) : Terminal, ILogicalConvertible, IPrefixOperable, IPostfixOperable, IBinaryOperable
{
    public static readonly Number ZERO = (Number)0;
    public static readonly Number ONE = (Number)1;

    public double Value { get; set; } = double.IsFinite(value) ? value : throw new ArgumentException($"{nameof(value)} must be a number.");

    public bool IsInteger { get; set; } = value % 1 == 0;

    public bool IsEven { get; set; } = value % 2 == 0;

    public bool IsOdd { get; set; } = value % 2 != 0;

    public bool IsPositive { get; set; } = value > 0;

    public bool IsNegative { get; set; } = value < 0;

    public Logical ToLogical()
        => Value switch
        {
            0 => Logical.FALSE,
            _ => Logical.TRUE
        };

    public Terminal PrefixOperate(string identifier, ExpressionContext _)
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

    public Terminal PostfixOperate(string identifier, ExpressionContext _)
    {
        return identifier switch
        {
            PostfixOperator.FACTORIAL => (Terminal)FactorialFunctions.Factorial(Value),
            PostfixOperator.PERCENT => (Terminal)(Value / 100),
            PostfixOperator.DOUBLE_FACTORIAL => (Terminal)FactorialFunctions.DoubleFactorial(Value),
            PostfixOperator.PRIMORIAL => (Terminal)FactorialFunctions.Primorial(Value),
            PostfixOperator.POST_DECREMENT or PostfixOperator.POST_INCREMENT => this,
            _ => Undefined.UNDEFINED,
        };
    }

    public Terminal? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext context)
    {
        if (right is Number num)
        {
            return identifier switch
            {
                BinaryOperator.ADDITION => (Terminal)(Value + num.Value),
                BinaryOperator.SUBTRACTION => (Terminal)(Value - num.Value),
                BinaryOperator.MULTIPLICATION or BinaryOperator.CROSS_MULTIPLICATION => (Terminal)(Value * num.Value),
                BinaryOperator.SLASH_DIVISION or BinaryOperator.DIVISION => (Terminal)(Value / num.Value),
                BinaryOperator.EXPONENT => (Terminal)Math.Pow(Value, num.Value),
                BinaryOperator.EUCLIDEAN_MODULUS => (Terminal)(Value - Math.Abs(num.Value) * Math.Floor(Value / Math.Abs(num.Value))),
                BinaryOperator.TRUC_MODULUS => (Terminal)(Value % num.Value),
                BinaryOperator.INT_DIVISION => (Terminal)(Math.Sign(num.Value) * Math.Floor(Value / Math.Abs(num.Value))),
                BinaryOperator.AND or BinaryOperator.AND_ALT or BinaryOperator.OR or BinaryOperator.OR_ALT or BinaryOperator.XOR
                    => ToLogical().BinaryLeftOperate(identifier, num.ToLogical(), context),
                BinaryOperator.IS_EQUAL => (Logical)(Value == num.Value),
                BinaryOperator.NOT_EQUAL or BinaryOperator.NOT_EQUAL_ALT or BinaryOperator.GREATER_OR_LESS_THAN
                    => (Logical)(Value != num.Value),
                BinaryOperator.LESS_THAN => (Logical)(Value < num.Value),
                BinaryOperator.LESS_THAN_OR_EQUAL or BinaryOperator.LESS_THAN_OR_EQUAL_ALT => (Logical)(Value <= num.Value),
                BinaryOperator.GREATER_THAN => (Logical)(Value > num.Value),
                BinaryOperator.GREATER_THAN_OR_EQUAL or BinaryOperator.GREATER_THAN_OR_EQUAL_ALT => (Logical)(Value >= num.Value),
                _ => null,
            };
        }

        return null;
    }

    public Terminal? BinaryRightOperate(string _, IExpression __, ExpressionContext ___)
    {
        return null;
    }

    public override bool Equals(object? obj)
        => obj is not null && obj is Number n && n.Value == Value;

    public override int GetHashCode()
        => Value.GetHashCode();

    public override string ToString(string? format)
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
