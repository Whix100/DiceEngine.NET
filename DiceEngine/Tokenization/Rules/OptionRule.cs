using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;

namespace DiceEngine.Tokenization.Rules;

/// <summary>
/// A rule to be used to tokenize a <see cref="string"/> into an <see cref="IToken"/>.
/// </summary>
/// <param name="name">The name of the <see cref="OptionRule"/>.</param>
/// <param name="options">The options to match.</param>
/// <param name="tokenize">
/// The function to use to tokenize a <see cref="string"/> into an <see cref="IToken"/>.
/// </param>
/// <param name="stringComparison">String comparison options.</param>
public class OptionRule(string name, IEnumerable<string> options, Func<string, int, IToken> tokenize,
    StringComparison stringComparison = StringComparison.CurrentCulture)
    : ITokenizerRule
{
    private readonly Func<string, int, IToken> _tokenize = tokenize;

    public readonly ImmutableHashSet<string> Options = options.Distinct().ToImmutableHashSet();

    public readonly StringComparison Comparison = stringComparison;

    public string Name { get; } = name;

    public IToken? Tokenize(ref string input, int index)
    {
        foreach (string option in Options)
        {
            if (input.StartsWith(option, Comparison))
            {
                input = input[option.Length..];
                return _tokenize(option, index);
            }
        }

        return null;
    }

    public override bool Equals(object? obj)
        => obj is OptionRule a && a.Options.SetEquals(Options) && a.Comparison == Comparison;

    public override int GetHashCode()
        => Options.GetHashCode();

    public static bool operator ==(OptionRule a, OptionRule b)
        => a.Equals(b);

    public static bool operator !=(OptionRule a, OptionRule b)
        => !a.Equals(b);
}
