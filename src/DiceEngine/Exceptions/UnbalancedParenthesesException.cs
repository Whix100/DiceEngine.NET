namespace DiceEngine.Exceptions;

/// <summary>
/// Initializes a new instance of the <see cref="UnbalancedParenthesesException"/> class using the specified 
/// unbalanced expression.
/// </summary>
/// <param name="expression">The expression with unbalanced parentheses.</param>
/// <param name="index">The index of where the error is located.</param>
public class UnbalancedParenthesesException(string expression, int? index = null)
    : Exception($"The expression '{expression}' contains unbalanced parentheses" +
       (index.HasValue ? $" at {index}." : "."))
{ }
