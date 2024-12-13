using DiceEngine.Tokenization.Tokens;

namespace DiceEngine.Tokenization.Rules;

/// <summary>
/// A rule to be used to tokenize a <see cref="string"/> into an <see cref="IToken"/>.
/// </summary>
/// <param name="name">The name of the <see cref="ITokenizerRule"/>.</param>
/// <param name="tokenize">The function to use to tokenize a input <see cref="string"/>.</param>
public class TokenizerRule(string name, Func<string, int, (string, IToken)?> tokenize) : ITokenizerRule
{
    private readonly Func<string, int, (string, IToken)?> _tokenize = tokenize;

    public string Name { get; } = name;

    public IToken? Tokenize(ref string input, int index)
    {
        (string Output, IToken Token)? result = _tokenize(input, index);

        if (result is null)
        {
            return null;
        }
        else
        {
            input = result.Value.Output;
            return result.Value.Token;
        }
    }

    public override bool Equals(object? obj)
        => obj is TokenizerRule r && r._tokenize == _tokenize;

    public override int GetHashCode()
        => HashCode.Combine(Name, _tokenize);

    public static bool operator ==(TokenizerRule a, TokenizerRule b)
        => a.Equals(b);

    public static bool operator !=(TokenizerRule a, TokenizerRule b)
        => !a.Equals(b);
}
