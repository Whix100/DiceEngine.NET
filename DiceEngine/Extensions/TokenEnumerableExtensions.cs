using DiceEngine.Exceptions;
using DiceEngine.Parsing;
using DiceEngine.Parsing.Tokens;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using System.Text;

namespace DiceEngine.Extensions;

internal static class TokenEnumerableExtensions
{
    internal static string JoinTokens(this IEnumerable<IToken> tokens)
    {
        int offset = tokens.FirstOrDefault()?.Index ?? 0;
        StringBuilder builder = new();

        foreach (IToken token in tokens)
        {
            builder.Append(string.Join("", new int[token.Index - offset - builder.Length].Select(_ => ' ')));
            builder.Append(token.Value);
        }

        return builder.ToString();
    }

    internal static ImmutableArray<IToken>[] Split(this ImmutableArray<IToken> tokens, string separator)
    {
        List<ImmutableArray<IToken>> result = [];
        List<IToken> subset = [];

        foreach (IToken token in tokens)
        {
            if (token.Value == separator)
            {
                result.Add([.. subset]);
                subset = [];
            }
            else
            {
                subset.Add(token);
            }
        }

        if (subset.Count > 0)
            result.Add([.. subset]);

        return [.. result];
    }

    internal static ImmutableArray<IToken>[] Split(this ImmutableArray<IToken> tokens, char separator)
        => Split(tokens, separator.ToString());

    internal static ImmutableArray<IToken>[] Split(this ImmutableArray<IToken> tokens, params string[] separator)
    {
        List<ImmutableArray<IToken>> result = [];
        List<IToken> subset = [];

        for (int i = 0; i < tokens.Length - separator.Length; i++)
        {
            if (tokens[i..(i + separator.Length)].Select(t => t.Value).SequenceEqual(separator))
            {
                result.Add([.. subset]);
                subset = [];
                i += separator.Length - 1;
            }
            else
            {
                subset.Add(tokens[i]);
            }
        }

        if (subset.Count > 0)
            result.Add([.. subset]);

        return [.. result];
    }
    /// <summary>
    /// Checks if the input has balanced brackets.
    /// </summary>
    /// <param name="input">The input to check.</param>
    /// <param name="brackets">Type of brackets to check.</param>
    /// <returns>
    /// <see langword="true"/> if the input string has balanced brackets, <see langword="false"/> otherwise.
    /// </returns>
    public static bool CheckBalancedBrackets(this ImmutableArray<IToken> input, Brackets brackets = Brackets.All)
    {
        if (brackets == Brackets.None)
            return true;

        Dictionary<Bracket, int> openCounts = Enum.GetValues<Bracket>().Where(b => brackets.HasFlag((Brackets)b)).ToDictionary(c => c, c => 0);
        Dictionary<Bracket, int> closeCounts = Enum.GetValues<Bracket>().Where(b => brackets.HasFlag((Brackets)b)).ToDictionary(c => c, c => 0);

        for (int i = 0; i < input.Length; i++)
        {
            IToken current = input[i];

            if (current is OpenBracketToken open)
            {
                openCounts[open.BracketType]++;
            }
            else if (current is CloseBracketToken close)
            {
                if (openCounts[close.BracketType] == 0)
                    return false;

                closeCounts[close.BracketType]++;
            }
        }

        return !openCounts.Select(kvp => kvp.Value == closeCounts[kvp.Key]).Any(x => !x);
    }

    /// <summary>
    /// Condenses the input by replacing all bracketed subsets with a <see cref="CondensedToken"/>.
    /// </summary>
    /// <param name="input">The input to condense.</param>
    /// <param name="brackets">Type of brackets to condense.</param>
    /// <returns>The condensed list of tokens.</returns>
    /// <exception cref="UnbalancedParenthesesException">
    /// Thrown when the input has unbalanced brackets.
    /// </exception>
    public static ImmutableArray<IToken> Condense(this ImmutableArray<IToken> input, Brackets brackets = Brackets.All)
    {
        if (brackets == Brackets.None)
            return input;

        int toks = 0;
        List<IToken> output = [];
        int start = -1;
        int offset = 0;
        int localOffset = 0;
        Stack<Bracket> depth = [];

        for (int i = 0; i < input.Length; i++)
        {
            IToken current = input[i];

            if (current is OpenBracketToken openBracket && brackets.HasFlag((Brackets)openBracket.BracketType))
            {
                if (start == -1)
                {
                    start = i;
                    localOffset = 0;
                }

                depth.Push(openBracket.BracketType);
            }
            else if (current is CloseBracketToken closeBracket && brackets.HasFlag((Brackets)closeBracket.BracketType))
            {
                if (start < 0 || closeBracket.BracketType != depth.Pop())
                {
                    throw new UnbalancedParenthesesException(string.Join(' ', input.Select(x => x.Value)), i);
                }
                else if (depth.Count == 0)
                {
                    output.Add(new CondensedToken(input[start..(i + 1)], start - (offset - localOffset), toks++));
                    start = -1;
                    depth.Clear();
                }
            }
            else
            {
                if (start < 0)
                    output.Add(current);
                else if (i == input.Length - 1)
                    throw new UnbalancedParenthesesException(input.JoinTokens(), i);
            }
        }

        return [.. output];
    }

    /// <summary>
    /// Condenses the input by replacing all bracketed subsets with a <see cref="CondensedToken"/>.
    /// </summary>
    /// <param name="input">The input to condense.</param>
    /// <param name="condensed">The condensed list of tokens.</param>
    /// <param name="brackets">Type of brackets to condense.</param>
    /// <returns>The condensed list of tokens.</returns>
    /// <exception cref="UnbalancedParenthesesException">
    /// Thrown when the input has unbalanced brackets.
    /// </exception>
    public static bool TryCondense(this ImmutableArray<IToken> input, out ImmutableArray<IToken>? condensed,
        Brackets brackets = Brackets.All)
    {
        try
        {
            condensed = input.Condense(brackets);
            return true;
        }
        catch
        {
            condensed = null;
            return false;
        }
    }

    /// <summary>
    /// Uncondenses a list of tokens.
    /// </summary>
    /// <param name="input">The condensed list of tokens.</param>
    /// <returns>The uncondensed form of the tokens.</returns>
    public static ImmutableArray<IToken> Uncondense(this ImmutableArray<IToken> input)
    {
        List<IToken> result = [];

        foreach (IToken token in input)
        {
            if (token is CondensedToken condensed)
            {
                result.AddRange(condensed.Tokens);
            }
            else
            {
                result.Add(token);
            }
        }

        return [.. result];
    }

    /// <summary>
    /// Converts an index in a list that might contain <see cref="CondensedToken"/>s to the corresponding index in the non-condensed list.
    /// </summary>
    /// <param name="input">The list of tokens.</param>
    /// <param name="index">The index in the condensed list.</param>
    /// <returns>The corresponding index from the uncondensed list of tokens.</returns>
    public static int UncondenseIndex(this ImmutableArray<IToken> input, int index)
        => index + input[..index].Where(t => t is CondensedToken).Cast<CondensedToken>().Select(t => t.Tokens.Length - 1).Sum();
}
