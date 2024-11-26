using DiceEngine.Tokenization.Tokens;

namespace DiceEngine.Tokenization.Rules;

public interface ITokenizerRule
{
    public string Name { get; }

    /// <summary>
    /// Tries to tokenize the input <see cref="string"/> into an <see cref="IToken"/>.
    /// </summary>
    /// <param name="input">The input <see cref="string"/> to tokenize.</param>
    /// <param name="index">The index of the start of the input string.</param>
    /// <returns>
    /// The tokenized <see cref="IToken"/>; <see langword="null"/> if the input does not match the rule.
    /// </returns>
    public IToken? Tokenize(ref string input, int index);
}
