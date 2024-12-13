namespace DiceEngine.Exceptions;

/// <summary>
/// Initializes a new instance of the <see cref="ArgumentValueException"/> class using the specified invalid value.
/// </summary>
/// <param name="x">The value that is invalid.</param>
public class ArgumentValueException(double x)
    : Exception($"The value '{x}' is not a valid argument value for this operation.")
{ }
