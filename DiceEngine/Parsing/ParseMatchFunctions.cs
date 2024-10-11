using DiceEngine.Attributes;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Components;
using DiceEngine.Parsing.Rules;
using System.Text.RegularExpressions;
using System.Reflection;
using DiceEngine.Expressions.Dice;

namespace DiceEngine.Parsing;

internal static class ParseMatchFunctions
{
    internal static IEnumerableExpression ParseMatchCollection(string _, Token token, Parser parser)
    {
        string tokenized = ContextFreeUtils.TokenizeInput(token.Value[1..^1], out Token[] tokens,
            Brackets.Square | Brackets.Curly);
        IEnumerable<IExpression> enumerable = tokenized.Split(',')
            .Select(e => parser.Parse(Regex.Replace(e, @"(?<!(^|[^\\])\\(\\\\)*)\[\d+\]", m => m.Value[1..^1])));

        return token.Value[0] == '['
            ? new Vector(enumerable)
            : new Set(enumerable);
    }

    internal static FunctionCall ParseMatchFunctionCall(string input, Token token, Parser parser)
    {
        Match function_name = Regex.Match(input, @"(?<=^\s*)([A-Za-zΑ-Ωα-ω]+(_[A-Za-zΑ-Ωα-ω0-9]+)*)");
        string tokenized_args = token.Value[(function_name.Length + 1)..^1].TokenizeInput(out Token[] tokens);

        string[] args = tokenized_args
            .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(arg => !arg.Contains('[')
                ? arg
                : Regex.Replace(arg, @"\[\d+\]", match => tokens[Convert.ToInt32(match.Value[1..^1])]))
            .ToArray();

        return new FunctionCall(function_name.Value, args.Select(arg => parser.Parse(arg)));
    }

    internal static LambdaFunction ParseMatchLambdaFunction(string input, Token token, Parser parser)
    {
        string parameters_string = Regex.Match(token.Value.Trim(), @"(?<=^\(?).*?(?=\)?\s*=>)").Value.TrimStart('(');
        string tokenized_parameters_string = parameters_string.TokenizeInput(out Token[] attribute_tokens,
            Brackets.Square);
        IEnumerable<Parameter> parameters = tokenized_parameters_string
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(p =>
            {
                Match attribute_match = Regex.Match(p, @"(?<=^\s*\[)\d+(?=\])");
                string? attribute_string = attribute_match.Success
                    ? attribute_tokens[Convert.ToInt32(attribute_match.Value)].Value[1..^1]
                    : null;
                IEnumerable<string> attributes = attribute_string is null
                    ? Enumerable.Empty<string>()
                    : Regex.Split(attribute_string, @"(?<!\([^\)]*?),(?![^\(]*?\))");

                return new Parameter(Regex.Match(p,
                    @$"(?<=\]?\s*){((RegexRule?)parser.GetGrammarRule("Variable"))!.RegularExpression}(?=\s*$)").Value,
                    attributes);
            }).ToArray();

        return new LambdaFunction(parameters, parser.Parse(input[(token.Index + token.Length)..]));
    }

    internal static Parentheses ParseMatchParentheses(string _, Token token, Parser parser)
        => new Parentheses(parser.Parse(token));

    internal static IExpression ParseMatchWithParentheses(string input, Token _, Parser parser)
    {
        string tokenized_input = input.TokenizeInput(out Token[] tokens, Brackets.Parenthesis);

        foreach (IRule rule in parser.Grammar)
        {
            if (rule.GetType().GetCustomAttribute<ReferenceRuleAttribute>() is not null)
                continue;

            Token? match = rule.Match(tokenized_input, parser.Grammar);

            if (match.HasValue)
            {
                IExpression? result = rule.Parse(input,
                    new Token(match.Value, ContextFreeUtils.DetokenizeIndex(match.Value.Index, tokenized_input,
                        tokens)),
                    parser);

                if (result is not null)
                    return result;
            }
        }

        throw new Exception($"The input was not in the correct format: '{input}'");
    }

    internal static AssignmentOperator ParseMatchAssignmentOperator(string input, Token match, Parser parser)
        => new AssignmentOperator((parser.Parse(input[..match.Index]) as Variable)!,
            parser.Parse(input[(match.Index + match.Length)..]));

    internal static BinaryOperator ParseMatchBinaryOperator(string input, Token match, Parser parser)
        => new BinaryOperator(match.Value, parser.Parse(input[..match.Index]),
            parser.Parse(input[(match.Index + match.Length)..]));

    internal static PrefixOperator ParseMatchPrefix(string input, Token match, Parser parser)
        => new PrefixOperator(match.Value, parser.Parse(input[(match.Index + match.Length)..]));

    internal static PostfixOperator ParseMatchPostfix(string input, Token match, Parser parser)
        => new PostfixOperator(match.Value, parser.Parse(input[..match.Index]));

    internal static IExpression ParseMatchDice(string _, Token match, Parser __)
    {
        Match sizeMatch = Regex.Match(match, @"^\d+");
        int offset = sizeMatch.Length + 1;
        string dieString = Regex.Match(match.Value[offset..], @"^(\d+|%|F)").Value;
        IDie die = dieString switch
        {
            "%" => new PercentileDie(),
            "F" => new FateDie(),
            _ => new Die(Int32.Parse(dieString)),
        };

        die = sizeMatch.Success
            ? new DiceSet(Int32.Parse(sizeMatch.Value), die)
            : die;
        offset += dieString.Length;

        IExpression result = die;

        while (offset < match.Length)
        {
            string operation = Regex.Match(match.Value[offset..], @"^(k|d|(r(r|o|a))|e|mi|ma)").Value;

            offset += operation.Length;

            Match selectorValue = Regex.Match(match.Value[offset..], @"\d+");
            string selector = match[offset..(offset + selectorValue.Index)];

            result = new DiceOperator(operation, result,
                new ResultSelector(selector, Convert.ToInt32(selectorValue.Value)));

            offset += selectorValue.Index + selectorValue.Length;
        }

        return result;
    }

    internal static Indexer ParseMatchIndexer(string input, Token match, Parser parser)
        => new Indexer(parser.Parse(input[..match.Index]), parser.Parse(match[1..^1]));

    internal static Undefined ParseMatchUndefined(string _, Token match, Parser __)
        => match.Value switch
        {
            "undefined" => Undefined.UNDEFINED,
            "dne" => Undefined.DNE,
            _ => throw new Exception($"The input was not in the correct format: '{match.Value}'")
        };

    internal static Logical ParseMatchLogical(string _, Token match, Parser __)
        => new Logical(Boolean.Parse(match.Value));

    internal static Infinity ParseMatchInfinity(string _, Token match, Parser __)
        => new Infinity(match.Value);

    internal static Constant ParseMatchConstant(string _, Token match, Parser __)
        => new Constant(match.Value);

    internal static Variable ParseMatchVariable(string _, Token match, Parser __)
        => new Variable(match.Value);

    internal static Number ParseMatchNumber(string _, Token match, Parser __)
        => new Number(Convert.ToDouble(match.Value));
}
