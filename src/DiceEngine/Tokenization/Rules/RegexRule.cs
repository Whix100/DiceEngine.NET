using DiceEngine.Tokenization.Tokens;
using System.Text.RegularExpressions;

namespace DiceEngine.Tokenization.Rules;

/// <summary>
/// A rule to be used to tokenize a <see cref="string"/> into an <see cref="IToken"/>.
/// </summary>
/// <param name="name">The name of the <see cref="RegexRule"/>.</param>
/// <param name="regex">The regex <see cref="string"/> to match an expression <see cref="string"/> to.</param>
/// <param name="tokenize">
/// The function to use to tokenize a <see cref="string"/> into an <see cref="IToken"/>.
/// </param>
/// <param name="options">The options for the regular expression.</param>
public class RegexRule(string name, string regex, Func<Match, int, IToken> tokenize, RegexOptions options = RegexOptions.None) : ITokenizerRule
{
    private readonly Func<Match, int, IToken> _tokenize = tokenize;

    public readonly string RegularExpression = regex;
    public readonly RegexOptions Options = options;

    public string Name { get; } = name;

    public IToken? Tokenize(ref string input, int index)
    {
        Match match = Regex.Match(input, RegularExpression, Options);

        if (match.Success)
        {
            input = input[(match.Index + match.Length)..];
            return _tokenize(match, index);
        }

        return null;
    }

    public override bool Equals(object? obj)
        => obj is RegexRule a && RegularExpression == a.RegularExpression && Options == a.Options;

    public override int GetHashCode()
        => HashCode.Combine(RegularExpression, Options);

    public static bool operator ==(RegexRule a, RegexRule b)
        => a.Equals(b);

    public static bool operator !=(RegexRule a, RegexRule b)
        => !a.Equals(b);
}
