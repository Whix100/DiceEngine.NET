using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Extensions;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;

namespace DiceEngine.Parsing.Defaults;

internal static class ParseFunctions
{
    internal static IExpression? ParseCollection(ImmutableArray<IToken> input, Parser parser)
    {
        if (input.Length >= 2 && input.First() is OpenBracketToken open && input.Last() is CloseBracketToken close &&
            open.BracketType == close.BracketType && (Bracket.Square | Bracket.Curly).HasFlag(open.BracketType) &&
            input[1..^1].TryCondense(out ImmutableArray<IToken>? condensed, Brackets.Square | Brackets.Curly))
        {
            IEnumerable<IExpression> enumerable = condensed!.Value.Split(',').Select(e => parser.Parse(e.Uncondense()));

            return open.BracketType == Bracket.Square
                ? new Vector(enumerable)
                : new Set(enumerable);
        }

        return null;
    }

    internal static IExpression? ParseFunctionCall(ImmutableArray<IToken> input, Parser parser)
    {
        if (input.Length >= 3 && input.First() is WordToken functionName && input[1] is OpenBracketToken { BracketType: Bracket.Parenthesis } &&
            input.Last() is CloseBracketToken { BracketType: Bracket.Parenthesis })
        {
            ImmutableArray<IToken> args = input[2..^1];

            if (args.Length > 0)
            {
                ImmutableArray<IToken> condensedArgs = args.Condense(Brackets.All);
                IEnumerable<IExpression> arguments = condensedArgs.Split(',').Select(arg => parser.Parse(arg.Uncondense()));

                return new FunctionCall(functionName.Value, arguments);
            }
        }

        return null;
    }
}
