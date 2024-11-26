using DiceEngine.Extensions;
using DiceEngine.Parsing.Rules;
using DiceEngine.Parsing.Tokens;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using System.Security;
using System.Text.RegularExpressions;

namespace DiceEngine.Parsing.Defaults;

internal static partial class MatchFunctions
{
    internal static TokenMatch? MatchCollection(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        if (input.Length >= 2 && input.First() is OpenBracketToken open && input.Last() is CloseBracketToken close &&
            open.BracketType == close.BracketType && (Bracket.Square | Bracket.Curly).HasFlag(open.BracketType))
        {
            return new TokenMatch(input, 0);
        }

        return null;
    }

    internal static TokenMatch? MatchFunctionCall(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        if (input.Length >= 3 && input.First() is WordToken && input[1] is OpenBracketToken { BracketType: Bracket.Parenthesis } &&
            input.Last() is CloseBracketToken { BracketType: Bracket.Parenthesis })
        {
            return new TokenMatch(input, 0);
        }

        return null;
    }

    internal static TokenMatch? MatchParentheses(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        if (input.Length >= 2 && input.First() is OpenBracketToken { BracketType: Bracket.Parenthesis } &&
            input.Last() is CloseBracketToken { BracketType: Bracket.Parenthesis })
        {
            int depth = 0;

            for (int i = 1; i < input.Length - 1; i++)
            {
                IToken current = input[i];

                if (current is OpenBracketToken { BracketType: Bracket.Parenthesis })
                {
                    depth++;
                }
                else if (current is CloseBracketToken { BracketType: Bracket.Parenthesis })
                {
                    if (depth == 0)
                        return null;

                    depth--;
                }
            }

            if (depth == 0)
                return new TokenMatch(input[1..^1], 1);
        }

        return null;
    }

    internal static TokenMatch? MatchDiceNotation(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        int currentIndex = 0;

        if (input.First() is NumberToken { ParsedValue: >= 0 } count && double.IsInteger(count.ParsedValue))
        {
            currentIndex++;
        }

        if (currentIndex < input.Length && input[currentIndex] is WordToken { Value: "d" or "dF" } die)
        {
            currentIndex++;

            if (die.Value == "dF" ||
                (input[currentIndex] is NumberToken { ParsedValue: >= 0 } size && double.IsInteger(size.ParsedValue)) ||
                input[currentIndex] is SymbolToken { Character: '%' })
            {
                if (die.Value != "dF")
                    currentIndex++;

                Match match = DiceOperatorRegex().Match(input[currentIndex..].JoinTokens());

                if (match.Success)
                    return new TokenMatch(input, 0);
            }
        }

        return null;
    }

    internal static TokenMatch? MatchIndexer(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        IEnumerable<CondensedToken> tokens = input.Condense(Brackets.Square)
            .Where(token => token is CondensedToken condensed &&
                condensed.Tokens.First() is OpenBracketToken { BracketType: Bracket.Square })
            .Cast<CondensedToken>();

        if (tokens.Any())
        {
            CondensedToken match = tokens.Last();

            if (match.Tokens.Length == 3 && match.Tokens[1] is NumberToken)
                return new TokenMatch(match.Tokens, match.Index);
        }

        return null;
    }

    [GeneratedRegex(@"^(((([kde]|(r[roa]))(|[hl≤≥]|((<|>)=?)))|mi|ma)\d+)*$")]
    private static partial Regex DiceOperatorRegex();
}
