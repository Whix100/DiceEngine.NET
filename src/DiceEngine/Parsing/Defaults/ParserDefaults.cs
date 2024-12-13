using DiceEngine.Parsing.Rules;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using static DiceEngine.Parsing.Defaults.MatchFunctions;
using static DiceEngine.Parsing.Defaults.ParseFunctions;
using static DiceEngine.Parsing.Defaults.ParseMatchFunctions;

namespace DiceEngine.Parsing.Defaults;

public static class ParserDefaults
{
    private const string PREFIX = @"((\+{2})|(\-{2})|[\+\-!~¬])";
    private const string POSTFIX = @"((\+{2})|(\-{2})|((?<![^!](!!)*!)!!)|[!%#])";
    private const string OPERAND = @$"({PREFIX}*(\d|\w|[\[\{{\]\}}\u001A]){POSTFIX}*)";
    private const string ATTRIBUTE = @"(\w(\(\d(,\d)*\))?)";
    private const string PARAMETER = @$"((\[{ATTRIBUTE}(,{ATTRIBUTE})*\])?\w)";

    public static readonly ImmutableArray<IParserRule> DefaultRules = [
        new ParserRule("Collection", ParseCollection, MatchCollection, ParseMatchCollection),
        new ParserRule("FunctionCall", ParseFunctionCall, MatchFunctionCall, ParseMatchFunctionCall),
        new RegexRule("LambdaFunction", @$"^({PARAMETER}|(\({PARAMETER}?\))|(\({PARAMETER}(,{PARAMETER})*\)))=>", ParseMatchLambdaFunction),
        new ParserRule("Parentheses", ParseMatchParentheses, MatchParentheses),
        new RegexRule("WithParentheses", @"[\(\)]", ParseMatchWithParentheses),
        new RegexRule("AssignBinOp", @$"(?<={OPERAND})(?<!!)(=)(?={OPERAND})", ParseMatchAssignmentOperator, RegexOptions.RightToLeft),
        new ParserRule("DiceNotation", ParseMatchDice, MatchDiceNotation),
        new RegexRule("OrBinOp", @$"(?<={OPERAND})(\|\||∨)(?={OPERAND})", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("XorBinOp", @$"(?<={OPERAND})(⊕)(?={OPERAND})", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("AndBinOp", @$"(?<={OPERAND})(&&|∧)(?={OPERAND})", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("EqBinOp", @$"(?<={OPERAND})(==|!=|<>|≠)(?={OPERAND})", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("IneqBinOp", @$"(?<={OPERAND})(>=|<=|<(?!>)|(?<!<)>|[≤≥])(?={OPERAND})", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("AddBinOp", @$"(?<={OPERAND})([\+\-])(?={OPERAND})", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("MultBinOp", @$"(?<={OPERAND})(%%|//|[*×/÷%])(?={OPERAND})", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("ExpBinOp", @"(?<=.)(\^)(?=.)", ParseMatchBinaryOperator, RegexOptions.RightToLeft),
        new RegexRule("Prefix", $"^{PREFIX}", ParseMatchPrefix),
        new RegexRule("Postfix", $"{POSTFIX}$", ParseMatchPostfix),
        new ParserRule("Indexer", ParseMatchIndexer, MatchIndexer),
        new OptionRule("Undefined", ["undefined", "dne"], ParseMatchUndefined),
        new OptionRule("Logical", ["true", "false"], ParseMatchLogical),
        new OptionRule("Infinity", ["∞", "inf", "infinity"], ParseMatchInfinity),
        new OptionRule("Constant", ["π", "pi", "τ", "tau", "empty_set", "empty", "∅", "e"], ParseMatchConstant),
        new TypeRule<WordToken>("Variable", ParseMatchVariable),
        new TypeRule<NumberToken>("Number", ParseMatchNumber),
    ];
}
