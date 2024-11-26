using DiceEngine.Attributes;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Components;
using DiceEngine.Expressions.Dice;
using DiceEngine.Expressions.Functions;
using DiceEngine.Expressions.Terminals;
using DiceEngine.Extensions;
using DiceEngine.Parsing.Rules;
using DiceEngine.Parsing.Tokens;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DiceEngine.Parsing.Defaults;

internal static partial class ParseMatchFunctions
{
    internal static IEnumerableExpression ParseMatchCollection(ImmutableArray<IToken> _, TokenMatch match, Parser parser)
    {
        ImmutableArray<IToken> condensed = match[1..^1].Condense(Brackets.Square | Brackets.Curly);
        IEnumerable<IExpression> enumerable = condensed.Split(',').Select(element => parser.Parse(element.Uncondense()));

        return (match.First() as OpenBracketToken)!.BracketType == Bracket.Square
            ? new Vector(enumerable)
            : new Set(enumerable);
    }

    internal static FunctionCall ParseMatchFunctionCall(ImmutableArray<IToken> _, TokenMatch match, Parser parser)
    {
        string functionName = match.First().Value;
        ImmutableArray<IToken> condensedArgs = match[2..^1].Condense();

        IEnumerable<IExpression> args = condensedArgs
            .Split(',')
            .Select(arg => parser.Parse(arg.Uncondense()));

        return new FunctionCall(functionName, args);
    }

    internal static LambdaFunction ParseMatchLambdaFunction(ImmutableArray<IToken> input, TokenMatch match, Parser parser)
    {
        ImmutableArray<IToken> parameterTokens = match.First() is OpenBracketToken { BracketType: Bracket.Parenthesis }
            ? match[1..^3]
            : match[..^2];
        ImmutableArray<IToken> condensedParameters = parameterTokens.Condense(Brackets.Square);
        IEnumerable<Parameter> parameters = condensedParameters.Split(',')
            .Select(p =>
            {
                ImmutableArray<IToken>[] attributes = p.First() is CondensedToken attribute
                    ? attribute.Tokens[1..^1].Split(',')
                    : [];

                return new Parameter(p.Last().Value, attributes);
            }).ToArray();

        return new LambdaFunction(parameters, parser.Parse(input[match.Length..]));
    }

    internal static Parentheses ParseMatchParentheses(ImmutableArray<IToken> _, TokenMatch match, Parser parser)
        => new Parentheses(parser.Parse(match[..]));

    internal static IExpression ParseMatchWithParentheses(ImmutableArray<IToken> input, TokenMatch _, Parser parser)
    {
        ImmutableArray<IToken> condensed = input.Condense(Brackets.Parenthesis);

        foreach (IParserRule rule in parser.Grammar)
        {
            if (rule.GetType().GetCustomAttribute<ReferenceRuleAttribute>() is not null)
                continue;

            TokenMatch? subMatch = rule.Match(condensed, parser.Grammar);

            if (subMatch is not null)
            {
                IExpression? expression = rule.Parse(input, new TokenMatch(subMatch.Match.ToImmutableArray().Uncondense(),
                    condensed.UncondenseIndex(subMatch.Index)), parser);

                if (expression is not null)
                    return expression;
            }
        }

        throw new Exception($"The input was not in the correct format: '{input.JoinTokens()}'");
    }

    internal static AssignmentOperator ParseMatchAssignmentOperator(ImmutableArray<IToken> input, TokenMatch match, Parser parser)
        => new AssignmentOperator((parser.Parse(input[..match.Index]) as Variable)!, parser.Parse(input[(match.Index + match.Length)..]));

    internal static BinaryOperator ParseMatchBinaryOperator(ImmutableArray<IToken> input, TokenMatch match, Parser parser)
        => new BinaryOperator(match.Value, parser.Parse(input[..match.Index]), parser.Parse(input[(match.Index + match.Length)..]));

    internal static PrefixOperator ParseMatchPrefix(ImmutableArray<IToken> input, TokenMatch match, Parser parser)
        => new PrefixOperator(match.Value, parser.Parse(input[(match.Index + match.Length)..]));

    internal static PostfixOperator ParseMatchPostfix(ImmutableArray<IToken> input, TokenMatch match, Parser parser)
        => new PostfixOperator(match.Value, parser.Parse(input[..match.Index]));

    internal static IExpression ParseMatchDice(ImmutableArray<IToken> input, TokenMatch _, Parser __)
    {
        NumberToken? sizeMatch = input.First() as NumberToken;
        int offset = sizeMatch is null ? 0 : 1;
        string dieString = input[offset] is WordToken { Value: "dF" }
            ? "F"
            : input[++offset].Value;
        IDie die = dieString switch
        {
            "%" => new PercentileDie(),
            "F" => new FateDie(),
            _ => new Die((int)(input[offset] as NumberToken)!.ParsedValue),
        };

        if (sizeMatch is not null)
            die = new DiceSet((int)sizeMatch.ParsedValue, die);

        string operatorString = input[++offset..].JoinTokens();

        offset = 0;

        while (offset < operatorString.Length) 
        {
            string operation = OperatorRegex().Match(operatorString[offset..]).Value;

            offset += operation.Length;

            Match selectorValue = SelectorValueRegex().Match(operatorString[offset..]);
            string selector = operatorString[offset..(offset + selectorValue.Index)];

            die = new DiceOperator(operation, die, new ResultSelector(selector, Convert.ToInt32(selectorValue.Value)));

            offset += selectorValue.Index + selectorValue.Length;
        }

        return die;
    }

    internal static Indexer ParseMatchIndexer(ImmutableArray<IToken> input, TokenMatch match, Parser parser)
        => new Indexer(parser.Parse(input[..match.Index]), parser.Parse(match[1..^1]));

    internal static Undefined ParseMatchUndefined(IToken match, Parser __)
        => match.Value switch
        {
            "undefined" => Undefined.UNDEFINED,
            "dne" => Undefined.DNE,
            _ => throw new Exception($"The input was not in the correct format: '{match.Value}'")
        };

    internal static Logical ParseMatchLogical(IToken match, Parser __)
        => match.Value switch
        {
            "true" => Logical.TRUE,
            "false" => Logical.FALSE,
            _ => throw new Exception($"The input was not in the correct format: '{match.Value}'")
        };

    internal static Infinity ParseMatchInfinity(IToken match, Parser __)
        => match.Value switch
        {
            "∞" => Infinity.POSITIVE,
            "infinity" => Infinity.POSITIVE_INFINITY,
            "inf" => Infinity.POSITIVE_INF,
            _ => throw new Exception($"The input was not in the correct format: '{match.Value}'")
        };

    internal static Constant ParseMatchConstant(IToken match, Parser __)
        => new Constant(match.Value);

    internal static Variable ParseMatchVariable(WordToken match, Parser __)
        => new Variable(match.Value);

    internal static Number ParseMatchNumber(NumberToken match, Parser __)
        => new Number(match.ParsedValue);
    
    [GeneratedRegex(@"^([kde]|(r[roa])|m[ia])")]
    private static partial Regex OperatorRegex();

    [GeneratedRegex(@"\d+")]
    private static partial Regex SelectorValueRegex();
}
