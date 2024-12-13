using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Interfaces;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the <see cref="PostfixOperator"/> class.
/// </summary>
/// <param name="op">The identifier for the operator.</param>
/// <param name="expression">The <see cref="IExpression"/> operand for this operator.</param>
public class PostfixOperator(string op, IExpression expression) : IExpression
{
    public const string FACTORIAL = "!";
    public const string PERCENT = "%";
    public const string DOUBLE_FACTORIAL = "!!";
    public const string PRIMORIAL = "#";
    public const string POST_DECREMENT = "--";
    public const string POST_INCREMENT = "++";

    public readonly string Identifier = op;
    public readonly IExpression Inside = expression;

    public Terminal Evaluate()
        => Evaluate(new ExpressionContext());

    public Terminal Evaluate(ExpressionContext context)
    {
        Terminal evaluated = Inside.Evaluate(context);

        return IPostfixOperable.Operate(Identifier, evaluated, context);
    }

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        IExpression enum_eval = Inside.StepEvaluate(context);

        if (!enum_eval.Equals(Inside))
            return new PostfixOperator(Identifier, enum_eval);
        else if (enum_eval is IPostfixOperable operable)
            return operable.PostfixOperate(Identifier, context);
        else if (enum_eval is IEnumerableExpression enum_expr)
            return enum_expr.Map(e => e is IPostfixOperable e_operable
                ? e_operable.PostfixOperate(Identifier, context)
                : e);

        return Undefined.UNDEFINED;
    }

public override string ToString()
        => ToString(null);

    public override bool Equals(object? obj)
        => obj is not null && obj is PostfixOperator uo && uo.Identifier == Identifier &&
            uo.Inside.Equals(Inside);

    public override int GetHashCode()
        => Identifier.GetHashCode();

    public string ToString(string? format)
        => $"{Inside.ToString(format)}{Identifier}";
}
