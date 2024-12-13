using DiceEngine.Extensions;
using DiceEngine.Tokenization.Tokens;
using System.Collections;
using System.Collections.Immutable;

namespace DiceEngine.Parsing;

/// <summary>
/// A subset of tokens that match a rule.
/// </summary>
/// <param name="match">The matching subset of tokens.</param>
/// <param name="index">The index in the list of tokens where the match was found.</param>
public class TokenMatch(IEnumerable<IToken> match, int index) : IEnumerable<IToken>
{
    private readonly IToken[] _match = [.. match];

    public IReadOnlyList<IToken> Match => _match;

    public int Index { get; } = index >= 0 ? index : throw new ArgumentException(nameof(index), "Index cannot be negative.");

    public int Length => Match.Count;

    public string Value => _match.ToList().JoinTokens();

    public IToken this[int index] => _match[index];

    public ImmutableArray<IToken> this[Range range] => [.. _match[range]];

    public IEnumerator<IToken> GetEnumerator() => ((IEnumerable<IToken>)_match).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _match.GetEnumerator();
}
