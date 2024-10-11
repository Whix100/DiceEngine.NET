using DiceEngine.BuiltInFunctions;
using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the the <see cref="PostfixOperator"/> class.
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

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
    {
        IExpression evaluated = Inside.Evaluate(context);

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

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => new PostfixOperator(Identifier, Inside.EvaluateDice(context));

    public override string ToString()
        => ToString(null);

    public override bool Equals(object? obj)
        => obj is not null && obj is PostfixOperator uo && uo.Identifier == Identifier &&
            uo.Inside.Equals(Inside);

    public override int GetHashCode()
        => Identifier.GetHashCode();

    public string ToString(string? format)
        => $"{Inside.ToString(format)}{Identifier}";

    private static IExpression Factorial(IExpression x, ExpressionContext context)
        => IFunction.ForEach(new Function(FactorialFunctions.Factorial, true), [x], context);

    private static IExpression Percent(IExpression x, ExpressionContext context)
    {
        IExpression x_eval = x.Evaluate(context);

        if (x_eval is Number n)
        {
            return new Number(n.Value / 100);
        }
        else if (x_eval is Constant c && Infinity.POSITIVE.Equals(c))
        {
            return x_eval;
        }
        else if (x_eval is PostfixOperator uo && Infinity.POSITIVE.Equals(uo.Inside))
        {
            return uo;
        }
        else if (x_eval is IEnumerableExpression enum_expr)
        {
            return enum_expr.Map(e => Percent(e, context));
        }

        return Undefined.UNDEFINED;
    }

    private static IExpression DoubleFactorial(IExpression x, ExpressionContext context)
        => IFunction.ForEach(new Function(FactorialFunctions.DoubleFactorial, true), [x], context);

    private static IExpression Primorial(IExpression x, ExpressionContext context)
        => IFunction.ForEach(new Function(FactorialFunctions.Primorial, true), [x], context);

    private static IExpression PostDecrement(IExpression x, ExpressionContext context)
    {
        IExpression x_eval = x.Evaluate(context);

        if (x is IEnumerableExpression enum_expr)
        {
            return enum_expr.Map(e => PostDecrement(e, context));
        }
        else if (x_eval is IEnumerableExpression eval_enum_expr)
        {
            return eval_enum_expr.Map(e => PostDecrement(e, context));
        }

        IExpression new_val = new BinaryOperator("-", x_eval, new Number(1)).Evaluate(context);

        context ??= new ExpressionContext();

        if (x is Variable v)
            context[v.Name] = new_val;

        return x_eval;
    }

    private static IExpression PostIncrement(IExpression x, ExpressionContext context)
    {
        IExpression x_eval = x.Evaluate(context);

        if (x is IEnumerableExpression enum_expr)
        {
            return enum_expr.Map(e => PostIncrement(e, context));
        }
        else if (x_eval is IEnumerableExpression eval_enum_expr)
        {
            return eval_enum_expr.Map(e => PostIncrement(e, context));
        }

        IExpression new_val = new BinaryOperator("+", x_eval, new Number(1)).Evaluate(context);

        context ??= new ExpressionContext();

        if (x is Variable v)
            context[v.Name] = new_val;

        return x_eval;
    }
}
