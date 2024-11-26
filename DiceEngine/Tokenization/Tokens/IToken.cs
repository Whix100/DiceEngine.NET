namespace DiceEngine.Tokenization.Tokens;

public interface IToken
{
    /// <summary>
    /// The value of the token.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The index of the token in the larger string.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The length of <see cref="Value"/>.
    /// </summary>
    public int Length => Value.Length;

    /// <summary>
    /// The value to use in a regular expression.
    /// </summary>
    public char RegexAlias { get; }

    public char this[int index] => Value[index];

    public string this[Range range] => Value[range];
}
