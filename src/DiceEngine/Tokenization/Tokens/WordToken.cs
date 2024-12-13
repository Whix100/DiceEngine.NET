using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DiceEngine.Tokenization.Tokens;

/// <summary>
/// A substring that represents a word such as a variable, constant, or function name.
/// </summary>
/// <param name="value">The <see cref="string"/> contained within the <see cref="Token"/>.</param>
/// <param name="index">The starting index within the containing <see cref="string"/>.</param>
public partial class WordToken(string value, int index) : IToken
{
    public string Value { get; } = value;

    public int Index { get; } = index;

    public char RegexAlias => 'a';

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is WordToken token && token.Value == Value;

    public override int GetHashCode()
        => Value.GetHashCode();

    public static bool operator ==(WordToken a, IToken b)
        => a.Equals(b);

    public static bool operator !=(WordToken a, IToken b)
        => !a.Equals(b);

    public static implicit operator WordToken(Match match)
        => new WordToken(match.Value, match.Index);

    public static implicit operator string(WordToken token)
        => token.Value;
}
