using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;

namespace DiceEngine.Tokenization.Rules;

/// <summary>
/// A rule to be used to tokenize a <see cref="string"/> into an <see cref="IToken"/>.
/// </summary>
/// <param name="name">The name of the <see cref="CharSetRule"/>.</param>
/// <param name="chars">The characters to match.</param>
/// <param name="tokenize">
/// The function to use to tokenize a <see cref="string"/> into an <see cref="IToken"/>.
/// </param>
/// <param name="options">The options for the regular expression.</param>
public class CharSetRule(string name, IEnumerable<char> chars, Func<char, int, IToken> tokenize) : ITokenizerRule
{
    private readonly Func<char, int, IToken> _tokenize = tokenize;

    public readonly ImmutableHashSet<char> Characters = chars.Distinct().ToImmutableHashSet();

    public string Name { get; } = name;

    public IToken? Tokenize(ref string input, int index)
    {
        foreach (char c in Characters)
        {
            if (input.StartsWith(c))
            {
                input = input[1..];
                return _tokenize(c, index);
            }
        }

        return null;
    }

    public override bool Equals(object? obj)
        => obj is CharSetRule a && a.Characters.SetEquals(Characters);

    public override int GetHashCode()
        => HashCode.Combine(Name, Characters);

    public static bool operator ==(CharSetRule a, CharSetRule b)
        => a.Equals(b);

    public static bool operator !=(CharSetRule a, CharSetRule b)
        => !a.Equals(b);
}
