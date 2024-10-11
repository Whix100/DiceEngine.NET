using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the the <see cref="PrefixOperator"/> class.
/// </summary>
/// <param name="op">The identifier for the operator.</param>
/// <param name="expression">The <see cref="IExpression"/> operand for this operator.</param>
public class PrefixOperator(string op, IExpression expression) : IExpression
{
    public const string POSITIVE = "+";
    public const string NEGATIVE = "-";
    public const string NOT = "~";
    public const string NOT_ALT = "¬";
    public const string SUBFACTORIAL = "!";
    public const string PRE_DECREMENT = "--";
    public const string PRE_INCREMENT = "++";

    public readonly string Identifier = op;
    public readonly IExpression Inside = expression;

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
    {
        IExpression evaluated = Inside.Evaluate(context);

        return IPrefixOperable.Operate(Identifier, evaluated, context);
    }

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        IExpression enum_eval = Inside.StepEvaluate(context);

        if (!enum_eval.Equals(Inside))
            return new PrefixOperator(Identifier, enum_eval);
        else if (enum_eval is IPrefixOperable operable)
            return operable.PrefixOperate(Identifier, context);
        else if (enum_eval is IEnumerableExpression enum_expr)
            return enum_expr.Map(e => e is IPrefixOperable e_operable
                ? e_operable.PrefixOperate(Identifier, context)
                : e);

        return Undefined.UNDEFINED;
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => new PrefixOperator(Identifier, Inside.EvaluateDice(context));

    public override string ToString()
        => ToString(null);

    public override bool Equals(object? obj)
        => obj is not null && obj is PrefixOperator uo && uo.Identifier == Identifier &&
            uo.Inside.Equals(Inside);

    public override int GetHashCode()
        => Identifier.GetHashCode();

    public string ToString(string? format)
        => $"{Identifier}{Inside.ToString(format)}";
}
