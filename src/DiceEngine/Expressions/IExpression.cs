using DiceEngine.Context;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions;

public interface IExpression
{
    /// <summary>
    /// Evaluates and calculates the value of the <see cref="IExpression"/>.
    /// </summary>
    /// <returns>
    /// A new <see cref="Terminal"/> containing the value of the current <see cref="IExpression"/>.
    /// </returns>
    public Terminal Evaluate();

    /// <summary>
    /// Evaluates and calculates the value of the <see cref="IExpression"/>.
    /// </summary>
    /// <param name="context">The values for each variable that could appear in the <see cref="IExpression"/>.</param>
    /// <returns>
    /// A new <see cref="Terminal"/> containing the value of the current <see cref="IExpression"/>.
    /// </returns>
    public Terminal Evaluate(ExpressionContext context);

    /// <summary>
    /// Evaluates and calculates one step towards the value of the <see cref="IExpression"/>.
    /// </summary>
    /// <returns>
    /// A new <see cref="IExpression"/> containing the partially evaluated value of the current 
    /// <see cref="IExpression"/>.
    /// </returns>
    public IExpression StepEvaluate();

    /// <summary>
    /// Evaluates and calculates one step towards the value of the <see cref="IExpression"/>.
    /// </summary>
    /// <param name="context">The values for each variable that could appear in the <see cref="IExpression"/>.</param>
    /// <returns>
    /// A new <see cref="IExpression"/> containing the partially evaluated value of the current 
    /// <see cref="IExpression"/>.
    /// </returns>
    public IExpression StepEvaluate(ExpressionContext context);

    /// <summary>
    /// Evaluates all dice in the <see cref="IExpression"/>.
    /// </summary>
    /// <returns>
    /// A new <see cref="IExpression"/> containing the value of the current <see cref="IExpression"/> with all dice
    /// evaluated.
    /// </returns>
    public IExpression EvaluateDice();

    /// <summary>
    /// Evaluates all dice in the <see cref="IExpression"/>.
    /// </summary>
    /// <param name="context">The values for each variable that could appear in the <see cref="IExpression"/>.</param>
    /// <returns>
    /// A new <see cref="IExpression"/> containing the value of the current <see cref="IExpression"/> with all dice
    /// evaluated.
    /// </returns>
    public IExpression EvaluateDice(ExpressionContext context);

    /// <summary>
    /// Returns a <see cref="string"/> that represents the current <see cref="IExpression"/>.
    /// </summary>
    /// <returns>A <see cref="string"/> that represents the current <see cref="IExpression"/>.</returns>
    public string ToString();

    /// <summary>
    /// Returns a <see cref="string"/> that represents the current <see cref="IExpression"/> using the specified 
    /// format <see cref="string"/> for number formats.
    /// </summary>
    /// <param name="format"></param>
    /// <returns>
    /// A <see cref="string"/> that represents the current <see cref="IExpression"/> using the specified format 
    /// <see cref="string"/>.
    /// </returns>
    public string ToString(string? format);

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates
    /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the other
    /// object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <para>Value – Meaning</para>
    /// <para>Less than zero – This instance precedes obj in the sort order.</para>
    /// <para>Zero – This instance occurs in the same position in the sort order as obj.</para>
    /// <para>Greater than zero – This instance follows obj in the sort order.</para>
    /// </returns>
    /// <exception cref="ArgumentException">obj is not the same type as this instance.</exception>
    public int CompareTo(object? obj)
    {
        if (obj is IExpression expression)
            return GetHashCode().CompareTo(expression.GetHashCode());

        return 0;
    }
}
