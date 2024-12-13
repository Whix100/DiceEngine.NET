namespace DiceEngine.Exceptions;

public class TooManyDiceRollsException() : Exception($"Limit of {MAX_ROLLS:n0} dice rolls reached!")
{
    public const int MAX_ROLLS = 1_000;
}
