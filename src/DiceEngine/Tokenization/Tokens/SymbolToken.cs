using System.Diagnostics.CodeAnalysis;

namespace DiceEngine.Tokenization.Tokens;

/// <summary>
/// A character that represents a symbol in an expression.
/// </summary>
/// <param name="value">The <see cref="char"/> contained within the <see cref="SymbolToken"/>.</param>
/// <param name="index">The starting index within the containing <see cref="string"/>.</param>
public class SymbolToken(char value, int index) : IToken
{
    public string Value { get; } = value.ToString();

    /// <summary>
    /// The character for the captured symbol.
    /// </summary>
    public char Character { get; } = value;

    public int Index { get; } = index;

    public char RegexAlias => Character;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is SymbolToken token && token.Character == Character;

    public override int GetHashCode()
        => Character.GetHashCode();

    public static bool operator ==(SymbolToken a, IToken b)
        => a.Equals(b);

    public static bool operator !=(SymbolToken a, IToken b)
        => !a.Equals(b);

    public static implicit operator string(SymbolToken token)
        => token.Value;
}
