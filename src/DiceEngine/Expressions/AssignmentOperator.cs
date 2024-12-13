using DiceEngine.Context;
using DiceEngine.Exceptions;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the <see cref="AssignmentOperator"/> class.
/// </summary>
/// <param name="assignedVariable">
/// The <see cref="IExpression"/> variable that's getting assigned to. If <paramref name="assignedVariable"/> is
/// not a <see cref="Variable"/>, an exception is thrown.
/// </param>
/// <param name="value">
/// The <see cref="IExpression"/> value being assigned to <paramref name="assignedVariable"/>.
/// </param>
public class AssignmentOperator(IExpression assignedVariable, IExpression value) : IExpression
{
    public readonly Variable AssignedVariable = assignedVariable is Variable v
        ? v
        : throw new InvalidAssignmentException(assignedVariable);
    public readonly IExpression Value = value;

    public Terminal Evaluate()
        => Evaluate(new ExpressionContext());

    public Terminal Evaluate(ExpressionContext context)
    {
        Terminal eval = Value.Evaluate(context);

        context[AssignedVariable.Name] = eval;
        return eval;
    }

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        IExpression eval = Value.StepEvaluate(context);

        if (eval.Equals(Value) && eval is Terminal term)
        {
            context[AssignedVariable.Name] = term;
            return eval;
        }

        return new AssignmentOperator(AssignedVariable, eval);
    }

    public override bool Equals(object? obj)
        => obj is not null && obj is AssignmentOperator assn_op &&
            assn_op.AssignedVariable.Equals(AssignedVariable) && assn_op.Value.Equals(Value);

    public override int GetHashCode()
        => HashCode.Combine(AssignedVariable, Value);

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{AssignedVariable.ToString(format)}={Value.ToString(format)}";
}
