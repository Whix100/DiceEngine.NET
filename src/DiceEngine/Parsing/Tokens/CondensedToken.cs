using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace DiceEngine.Parsing.Tokens;

/// <summary>
/// A token consisting of a sequence of tokens.
/// </summary>
/// <param name="value">The sequence of tokens.</param>
/// <param name="index">The index of the starting token.</param>
/// <param name="tokenIndex">The number of the <see cref="CondensedToken"/> in the sequence.</param>
public class CondensedToken(ImmutableArray<IToken> value, int index, int tokenIndex) : IToken
{
    private readonly ImmutableArray<IToken> _value = value;

    public string Value { get; } = $"[{tokenIndex}]";

    public int Index { get; } = index;

    /// <summary>
    /// The number of the <see cref="CondensedToken"/> in the sequence.
    /// </summary>
    public int TokenIndex { get; } = tokenIndex;

    /// <summary>
    /// The captured sequence of tokens.
    /// </summary>
    public ImmutableArray<IToken> Tokens => [.. _value];

    public char RegexAlias => '\u001A';

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is CondensedToken token && token._value.SequenceEqual(_value);

    public override int GetHashCode()
        => _value.GetHashCode();

    public static bool operator ==(CondensedToken a, IToken b)
        => a.Equals(b);

    public static bool operator !=(CondensedToken a, IToken b)
        => !a.Equals(b);
}
