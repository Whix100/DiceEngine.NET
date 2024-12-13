using DiceEngine.Expressions;

namespace DiceEngine.Exceptions;

/// <summary>
/// Initializes a new instance of the <see cref="InvalidAssignmentException"/> class using the specified invalid
/// expression.
/// </summary>
/// <param name="expression">The invalide expression.</param>
public class InvalidAssignmentException(IExpression expression)
    : Exception($"'{expression}' is not a variable and therefore cannot use the assignment operator.")
{ }
