using DiceEngine.BuiltInFunctions;
using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the the <see cref="Infinity"/> class.
/// </summary>
/// <param name="identifier">The identifier <see cref="string"/> for this <see cref="Infinity"/>.</param>
/// <param name="positive">Boolean value indicating whether this <see cref="Infinity"/> is positive.</param>
public class Infinity(string identifier, bool positive = true) : IExpression, ILogicalConvertible, IPrefixOperable,
    IPostfixOperable, IBinaryOperable
{
    public static readonly Infinity POSITIVE = new Infinity("∞", true);
    public static readonly Infinity NEGATIVE = new Infinity("∞", false);
    public static readonly Infinity POSITIVE_INFINITY = new Infinity("infinity", true);
    public static readonly Infinity NEGATIVE_INFINITY = new Infinity("infinity", false);
    public static readonly Infinity POSITIVE_INF = new Infinity("inf", true);
    public static readonly Infinity NEGATIVE_INF = new Infinity("inf", false);

    public readonly string Identifier = identifier;
    public readonly bool IsPositive = positive;
    public readonly bool IsNegative = !positive;

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

    public Logical ToLogical()
        => Logical.TRUE;

    public IExpression PrefixOperate(string identifier, ExpressionContext _)
    {
        return identifier switch
        {
            PrefixOperator.POSITIVE => this,
            PrefixOperator.NEGATIVE => new Infinity(Identifier, IsNegative),
            PrefixOperator.NOT or PrefixOperator.NOT_ALT => Logical.FALSE,
            PrefixOperator.SUBFACTORIAL => ((Number)FactorialFunctions.Subfactorial(this)).Evaluate(),
            PrefixOperator.PRE_DECREMENT or PrefixOperator.PRE_INCREMENT => this,
            _ => Undefined.UNDEFINED,
        };
    }

    public IExpression PostfixOperate(string identifier, ExpressionContext _)
    {
        return identifier switch
        {
            PostfixOperator.FACTORIAL => ((Number)FactorialFunctions.Factorial(this)).Evaluate(),
            PostfixOperator.DOUBLE_FACTORIAL => ((Number)FactorialFunctions.DoubleFactorial(this)).Evaluate(),
            PostfixOperator.PRIMORIAL => ((Number)FactorialFunctions.Primorial(this)).Evaluate(),
            PostfixOperator.PERCENT or PostfixOperator.POST_DECREMENT or PostfixOperator.POST_INCREMENT => this,
            _ => Undefined.UNDEFINED,
        };
    }

    public IExpression? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext context)
    {
        if (right is Undefined)
            return null;

        switch (identifier)
        {
            case BinaryOperator.ADDITION:
                if (right is Number || right is Logical || right is Infinity addInfinity &&
                    addInfinity.IsPositive == IsPositive)
                    return this;
                break;
            case BinaryOperator.SUBTRACTION:
                if (right is Number || right is Logical || right is Infinity subInfinity &&
                    subInfinity.IsPositive != IsPositive)
                    return this;
                break;
            case BinaryOperator.MULTIPLICATION:
            case BinaryOperator.CROSS_MULTIPLICATION:
                if (right is Infinity infinity)
                    return infinity.IsPositive == IsPositive ? POSITIVE : NEGATIVE;
                else if (right is Number multNum && multNum.Value != 0)
                    return multNum.IsNegative ^ IsPositive ? POSITIVE : NEGATIVE;
                break;
            case BinaryOperator.SLASH_DIVISION:
            case BinaryOperator.DIVISION:
                if (right is Number divNum && divNum.Value != 0)
                    return this;
                break;
            case BinaryOperator.EXPONENT:
                if (right is Number expNum && expNum.Value != 0)
                {
                    if (expNum.IsNegative)
                        return Number.ZERO;
                    else if (IsPositive)
                        return POSITIVE;
                    else if (expNum.IsInteger)
                        return expNum.IsEven ? POSITIVE : NEGATIVE;
                }
                break;
            case BinaryOperator.AND:
            case BinaryOperator.AND_ALT:
            case BinaryOperator.OR:
            case BinaryOperator.OR_ALT:
            case BinaryOperator.XOR:
                if (right is ILogicalConvertible boolConvertible)
                    return ToLogical().BinaryLeftOperate(identifier, boolConvertible.ToLogical(), context);
                break;
            case BinaryOperator.IS_EQUAL:
                return (Logical)Equals(right);
            case BinaryOperator.NOT_EQUAL:
            case BinaryOperator.NOT_EQUAL_ALT:
            case BinaryOperator.GREATER_OR_LESS_THAN:
                return (Logical)!Equals(right);
            case BinaryOperator.LESS_THAN:
                if (IsPositive)
                    return Logical.FALSE;
                else if (!NEGATIVE.Equals(right))
                    return Logical.TRUE;
                else
                    return Logical.FALSE;
            case BinaryOperator.LESS_THAN_OR_EQUAL:
            case BinaryOperator.LESS_THAN_OR_EQUAL_ALT:
                if (Equals(right))
                    return Logical.TRUE;
                else
                    return BinaryLeftOperate(BinaryOperator.LESS_THAN, right, context);
            case BinaryOperator.GREATER_THAN:
                if (IsNegative)
                    return Logical.FALSE;
                else if (!POSITIVE.Equals(right))
                    return Logical.TRUE;
                else
                    return Logical.FALSE;
            case BinaryOperator.GREATER_THAN_OR_EQUAL:
            case BinaryOperator.GREATER_THAN_OR_EQUAL_ALT:
                if (Equals(right))
                    return Logical.TRUE;
                else
                    return BinaryLeftOperate(BinaryOperator.GREATER_THAN, right, context);
        }

        return null;
    }

    public IExpression? BinaryRightOperate(string identifier, IExpression left, ExpressionContext context)
    {
        if (left is Undefined)
            return null;

        switch (identifier)
        {
            case BinaryOperator.ADDITION:
                if (left is Number || left is Logical || left is Infinity addInfinity &&
                    addInfinity.IsPositive == IsPositive)
                    return this;
                break;
            case BinaryOperator.SUBTRACTION:
                if (left is Number || left is Logical || left is Infinity subInfinity &&
                    subInfinity.IsPositive != IsPositive)
                    return IsPositive ? NEGATIVE : POSITIVE;
                break;
            case BinaryOperator.MULTIPLICATION:
            case BinaryOperator.CROSS_MULTIPLICATION:
                if (left is Infinity infinity)
                    return infinity.IsPositive == IsPositive ? POSITIVE : NEGATIVE;
                else if (left is Number multNum && multNum.Value != 0)
                    return multNum.IsNegative ^ IsPositive ? POSITIVE : NEGATIVE;
                break;
            case BinaryOperator.SLASH_DIVISION:
            case BinaryOperator.DIVISION:
                if (left is Number divNum && divNum.Value != 0)
                    return Number.ZERO;
                break;
            case BinaryOperator.EXPONENT:
                if (IsNegative)
                {
                    if ((left is Number || left is Infinity || left is Logical) && IsNegative)
                        return Number.ZERO;
                }
                else if (left is Number || left is Logical)
                {
                    return POSITIVE;
                }
                break;
            case BinaryOperator.AND:
            case BinaryOperator.AND_ALT:
            case BinaryOperator.OR:
            case BinaryOperator.OR_ALT:
            case BinaryOperator.XOR:
                if (left is ILogicalConvertible boolConvertible)
                    return ToLogical().BinaryRightOperate(identifier, boolConvertible.ToLogical(), context);
                break;
            case BinaryOperator.IS_EQUAL:
                return (Logical)Equals(left);
            case BinaryOperator.NOT_EQUAL:
            case BinaryOperator.NOT_EQUAL_ALT:
            case BinaryOperator.GREATER_OR_LESS_THAN:
                return (Logical)!Equals(left);
            case BinaryOperator.LESS_THAN:
                if (IsNegative)
                    return Logical.FALSE;
                else if (!POSITIVE.Equals(left))
                    return Logical.TRUE;
                else
                    return Logical.FALSE;
            case BinaryOperator.LESS_THAN_OR_EQUAL:
            case BinaryOperator.LESS_THAN_OR_EQUAL_ALT:
                if (Equals(left))
                    return Logical.TRUE;
                else
                    return BinaryRightOperate(BinaryOperator.LESS_THAN, left, context);
            case BinaryOperator.GREATER_THAN:
                if (IsPositive)
                    return Logical.FALSE;
                else if (!NEGATIVE.Equals(left))
                    return Logical.TRUE;
                else
                    return Logical.FALSE;
            case BinaryOperator.GREATER_THAN_OR_EQUAL:
            case BinaryOperator.GREATER_THAN_OR_EQUAL_ALT:
                if (Equals(left))
                    return Logical.TRUE;
                else
                    return BinaryRightOperate(BinaryOperator.GREATER_THAN, left, context);
        }

        return null;
    }

    public override bool Equals(object? obj)
        => obj is not null && obj is Infinity i && i.IsPositive == IsPositive;

    public override int GetHashCode()
        => IsPositive.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{(IsPositive ? "" : '-')}{Identifier}";

    public static implicit operator double(Infinity infinity)
        => infinity.IsPositive ? Double.PositiveInfinity : Double.NegativeInfinity;
}
