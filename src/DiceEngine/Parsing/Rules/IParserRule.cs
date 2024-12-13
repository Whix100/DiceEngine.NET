using DiceEngine.Expressions;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;

namespace DiceEngine.Parsing.Rules;

public interface IParserRule
{
    public string Name { get; }

    /// <summary>
    /// The function that gets used to find a match in an input.
    /// </summary>
    /// <param name="input">The input <see cref="ImmutableArray{IToken}"/> to find the match in.</param>
    /// <param name="rules">Other rules from the calling <see cref="Parser"/>.</param>
    /// <returns>
    /// A new <see cref="IEnumerable{IToken}"/> containing the value of the matching input and the index of where it
    /// was found; <see langword="null"/> if no match was found.
    /// </returns>
    TokenMatch? Match(ImmutableArray<IToken> input, IEnumerable<IParserRule> rules);

    /// <summary>
    /// Tries to parse the input into an <see cref="IExpression"/>.
    /// </summary>
    /// <param name="input">The input <see cref="ImmutableArray{IToken}"/> to parse.</param>
    /// <param name="parser">The <see cref="Parser"/> to use to parse any sub-expressions.</param>
    /// <returns>
    /// The parsed <see cref="IExpression"/>; <see langword="null"/> if the input could not be parsed.
    /// </returns>
    IExpression? Parse(ImmutableArray<IToken> input, Parser parser);

    /// <summary>
    /// Tries to parse the input <see cref="ImmutableArray{IToken}"/> into an <see cref="IExpression"/> using the given token.
    /// </summary>
    /// <param name="input">The input <see cref="ImmutableArray{IToken}"/> to parse.</param>
    /// <param name="match">The matching <see cref="IEnumerable{IToken}"/> with its index to use to parse the input.</param>
    /// <param name="parser">The <see cref="Parser"/> to use to parse any sub-expressions.</param>
    /// <returns>
    /// The parsed <see cref="IExpression"/>; <see langword="null"/> if the input could not be parsed.
    /// </returns>
    IExpression? Parse(ImmutableArray<IToken> input, TokenMatch match, Parser parser);
}