using DiceEngine.BuiltInFunctions;
using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.Expressions;

/// <summary>
/// Creates a new instance of the <see cref="Logical"/> class.
/// </summary>
/// <param name="value">The value of the <see cref="Logical"/>.</param>
public class Logical(bool value) : IExpression, IPrefixOperable, IPostfixOperable, IBinaryOperable
{
    public static readonly Logical TRUE = new Logical(true);
    public static readonly Logical FALSE = new Logical(false);

    public readonly bool Value = value;

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

    public IExpression PrefixOperate(string identifier, ExpressionContext _)
    {
        return identifier switch
        {
            PrefixOperator.NOT or PrefixOperator.NOT_ALT => Value ? FALSE : TRUE,
            PrefixOperator.SUBFACTORIAL => (Number)FactorialFunctions.Subfactorial(Value ? 1 : 0),
            PrefixOperator.POSITIVE or PrefixOperator.NEGATIVE => this,
            PrefixOperator.PRE_DECREMENT => Value ? (Number)0 : (Number)(-1),
            PrefixOperator.PRE_INCREMENT => Value ? (Number)2 : (Number)0,
            _ => Undefined.UNDEFINED,
        };
    }

    public IExpression PostfixOperate(string identifier, ExpressionContext _)
    {
        return identifier switch
        {
            PostfixOperator.PERCENT => Value ? (Number)0.01 : (Number)0,
            PostfixOperator.FACTORIAL => (Number)FactorialFunctions.Factorial(this),
            PostfixOperator.DOUBLE_FACTORIAL => (Number)FactorialFunctions.DoubleFactorial(this),
            PostfixOperator.PRIMORIAL => (Number)FactorialFunctions.Primorial(this),
            PostfixOperator.POST_DECREMENT or PostfixOperator.POST_INCREMENT => this,
            _ => Undefined.UNDEFINED,
        };
    }

    public IExpression? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext context)
    {
        Logical? rightLogical = ILogicalConvertible.ConvertToLogical(right);

        if (rightLogical is null)
            return null;

        return identifier switch
        {
            BinaryOperator.AND or BinaryOperator.AND_ALT => Value ? rightLogical : FALSE,
            BinaryOperator.OR or BinaryOperator.OR_ALT => Value ? TRUE : right,
            BinaryOperator.XOR => Value != rightLogical.Value ? TRUE : FALSE,
            _ => (Value ? Number.ONE : Number.ZERO)
                .BinaryLeftOperate(identifier, rightLogical.Value ? Number.ONE : Number.ZERO, context),
        };
    }

    public IExpression? BinaryRightOperate(string identifier, IExpression left, ExpressionContext context)
    {
        Logical? leftLogical = ILogicalConvertible.ConvertToLogical(left);

        if (leftLogical is null)
            return null;

        return identifier switch
        {
            BinaryOperator.AND or BinaryOperator.AND_ALT => leftLogical.Value ? this : FALSE,
            BinaryOperator.OR or BinaryOperator.OR_ALT => leftLogical.Value ? TRUE : this,
            BinaryOperator.XOR => leftLogical.Value != Value ? TRUE : FALSE,
            _ => (Value ? Number.ONE : Number.ZERO)
                .BinaryRightOperate(identifier, leftLogical.Value ? Number.ONE : Number.ZERO, context),
        };
    }

    public override bool Equals(object? obj)
        => obj is not null && obj is Logical logical && logical.Value == Value;

    public override int GetHashCode()
        => Value.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? _)
        => Value ? "true" : "false";

    public static implicit operator bool(Logical logical)
        => logical.Value;

    public static implicit operator int(Logical logical)
        => logical.Value ? 1 : 0;

    public static explicit operator Logical(bool value)
        => value ? TRUE : FALSE;
}
