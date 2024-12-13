using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.TypeConverters;

public interface ITypeConverter<T> : ITypeConverter
{
    /// <summary>
    /// Converts from a value to an expression.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>An <see cref="Terminal"/> representing the value.</returns>
    Terminal ConvertToExpression(T value);

    /// <summary>
    /// Converts from an expression to a value.
    /// </summary>
    /// <param name="expression">The expression to convert.</param>
    /// <returns>A value representing the expression.</returns>
    T? ConvertFromExpression(IExpression? expression);
}

public interface ITypeConverter
{
}
