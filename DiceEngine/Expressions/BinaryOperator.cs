using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the <see cref="BinaryOperator"/> class.
/// </summary>
/// <param name="op">The identifier for the operator.</param>
/// <param name="left">The <see cref="IExpression"/> left operand for this operator.</param>
/// <param name="right">The <see cref="IExpression"/> right operand for this operator.</param>
public class BinaryOperator(string op, IExpression left, IExpression right) : IExpression
{
    public const string ADDITION = "+";
    public const string SUBTRACTION = "-";
    public const string MULTIPLICATION = "*";
    public const string CROSS_MULTIPLICATION = "×";
    public const string SLASH_DIVISION = "/";
    public const string DIVISION = "÷";
    public const string EXPONENT = "^";
    public const string EUCLIDEAN_MODULUS = "%";
    public const string TRUC_MODULUS = "%%";
    public const string INT_DIVISION = "//";
    public const string AND = "&&";
    public const string AND_ALT = "∧";
    public const string OR = "||";
    public const string OR_ALT = "∨";
    public const string XOR = "⊕";
    public const string IS_EQUAL = "==";
    public const string NOT_EQUAL = "!=";
    public const string NOT_EQUAL_ALT = "≠";
    public const string GREATER_OR_LESS_THAN = "<>";
    public const string LESS_THAN = "<";
    public const string LESS_THAN_OR_EQUAL = "<=";
    public const string LESS_THAN_OR_EQUAL_ALT = "≤";
    public const string GREATER_THAN = ">";
    public const string GREATER_THAN_OR_EQUAL = ">=";
    public const string GREATER_THAN_OR_EQUAL_ALT = "≥";

    public readonly string Identifier = op;
    public readonly IExpression Left = left;
    public readonly IExpression Right = right;

    public Terminal Evaluate()
        => Evaluate(new ExpressionContext());

    public Terminal Evaluate(ExpressionContext context)
    {
        Terminal leftEvaluated = Left.Evaluate(context);
        Terminal rightEvaluated = Right.Evaluate(context);

        return IBinaryOperable.Operate(Identifier, leftEvaluated, rightEvaluated, context);
    }

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        IExpression l = Left.StepEvaluate(context);

        if (l.Equals(Left))
        {
            IExpression r = Right.StepEvaluate(context);

            return r.Equals(Right) ? Evaluate(context) : new BinaryOperator(Identifier, Left, r);
        }

        return new BinaryOperator(Identifier, l, Right);
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => new BinaryOperator(Identifier, Left.EvaluateDice(context), Right.EvaluateDice(context));

    public override bool Equals(object? obj)
        => obj is not null && obj is BinaryOperator bin_op && bin_op.Identifier == Identifier &&
            Left.Equals(bin_op.Left) && Right.Equals(bin_op.Right);

    public override int GetHashCode()
        => Identifier.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{Left.ToString(format)}{Identifier}{Right.ToString(format)}";
}
