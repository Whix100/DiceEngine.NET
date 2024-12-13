using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DiceEngine.Tokenization.Tokens;

public partial class NumberToken(string value, int index) : IToken
{
    public string Value { get; } = value;

    /// <summary>
    /// The numeric value parsed from the captured string.
    /// </summary>
    public double ParsedValue { get; } = double.Parse(value);

    public int Index { get; } = index;

    public char RegexAlias => '0';

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is NumberToken token && token.ParsedValue == ParsedValue;

    public override int GetHashCode()
        => ParsedValue.GetHashCode();

    public static bool operator ==(NumberToken a, IToken b)
        => a.Equals(b);

    public static bool operator !=(NumberToken a, IToken b)
        => !a.Equals(b);

    public static implicit operator NumberToken(Match match)
        => new NumberToken(match.Value, match.Index);

    public static implicit operator string(NumberToken token)
        => token.Value;
}
