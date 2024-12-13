using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;
using DiceEngine.Parsing;
using DiceEngine.Parsing.Rules;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using TestDiceEngine.TestData;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.Parsing;

[TestClass]
public class TestParser
{
    private const string PREFIX = @"((\+{2})|(\-{2})|[\+\-!~¬])";
    private const string POSTFIX = @"((\+{2})|(\-{2})|((?<![^!](!!)*!)!!)|[!%#])";
    private const string OPERAND = @$"({PREFIX}*(\d|\w|[\[\{{\]\}}\u001A]){POSTFIX}*)";
    private const string ATTRIBUTE = @"(\w(\(\d(,\d)*\))?)";
    private const string PARAMETER = @$"((\[{ATTRIBUTE}(,{ATTRIBUTE})*\])?\w)";
    private static readonly RegexRule CUSTOM_RULE = new RegexRule("Char", "[A-Z]",
        (_, match, _) => new Number(match.Value[0] - 65), RegexOptions.None);

    /// <summary>
    /// Tests that the Parser can be initialized properly from either constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        (string Name, string[]? Value)[] defaultRules =
        [
            ("Collection", null),
            ("FunctionCall", null),
            ("LambdaFunction", [@$"^({PARAMETER}|(\({PARAMETER}?\))|(\({PARAMETER}(,{PARAMETER})*\)))=>"]),
            ("Parentheses", null),
            ("WithParentheses", [@"[\(\)]"]),
            ("AssignBinOp", [@$"(?<={OPERAND})(?<!!)(=)(?={OPERAND})"]),
            ("DiceNotation", null),
            ("OrBinOp", [@$"(?<={OPERAND})(\|\||∨)(?={OPERAND})"]),
            ("XorBinOp", [@$"(?<={OPERAND})(⊕)(?={OPERAND})"]),
            ("AndBinOp", [@$"(?<={OPERAND})(&&|∧)(?={OPERAND})"]),
            ("EqBinOp", [@$"(?<={OPERAND})(==|!=|<>|≠)(?={OPERAND})"]),
            ("IneqBinOp", [@$"(?<={OPERAND})(>=|<=|<(?!>)|(?<!<)>|[≤≥])(?={OPERAND})"]),
            ("AddBinOp", [@$"(?<={OPERAND})([\+\-])(?={OPERAND})"]),
            ("MultBinOp", [@$"(?<={OPERAND})(%%|//|[*×/÷%])(?={OPERAND})"]),
            ("ExpBinOp", [@"(?<=.)(\^)(?=.)"]),
            ("Prefix", [$"^{PREFIX}"]),
            ("Postfix", [$"{POSTFIX}$"]),
            ("Indexer", null),
            ("Undefined", ["undefined", "dne"]),
            ("Logical", ["true", "false"]),
            ("Infinity", ["∞", "inf", "infinity"]),
            ("Constant", ["π", "pi", "τ", "tau", "empty_set", "empty", "∅", "e"]),
            ("Variable", ["WordToken"]),
            ("Number", ["NumberToken"]),
        ];

        Parser parser = new Parser();

        for (int i = 0; i < defaultRules.Length; i++)
        {
            IParserRule rule = parser.Grammar[i];

            Assert.AreEqual(defaultRules[i].Name, rule.Name);

            if (rule is RegexRule regexRule)
                Assert.AreEqual(defaultRules[i].Value?.Single(), regexRule.Regex);
            else if (rule is OptionRule optionRule)
                UtilFunctions.AreEqual(defaultRules[i]!.Value ?? [], optionRule.Options);
            else if (rule.GetType().IsGenericType && rule.GetType().GetGenericTypeDefinition() == typeof(TypeRule<>))
                Assert.AreEqual(defaultRules[i].Value?.Single(), rule.GetType().GenericTypeArguments.Single().Name);
            else
                Assert.AreEqual(defaultRules[i].Value, null);

            Assert.AreEqual(rule, parser.GetGrammarRule(rule.Name));
            Assert.AreEqual(rule, parser.GetGrammarRule(i));
        }

        parser = new Parser([CUSTOM_RULE]);

        Assert.IsTrue(parser.Grammar.Length == 1);
        Assert.IsTrue((RegexRule)parser.Grammar[0] == CUSTOM_RULE);
    }

    /// <summary>
    /// Tests that the Parser is able to properly parse strings into IExpressions.
    /// </summary>
    [TestMethod]
    public void TestParse()
    {
        ParseTestCases(TestCases.Expressions);
    }

    /// <summary>
    /// Tests that the Parser is able to properly parse strings into IExpressions.
    /// </summary>
    [TestMethod]
    public void TestParseCollections()
    {
        ParseTestCases(TestCases.Collections);
    }

    /// <summary>
    /// Tests that the Parser is able to properly parse dice notation strings into IExpressions.
    /// </summary>
    [TestMethod]
    public void TestParseDiceNotation()
    {
        ParseTestCases(TestCases.DiceNotation);
    }

    private static void ParseTestCases(IEnumerable<TestCase> testCases)
    {
        Parser parser = new Parser();

        foreach (TestCase testCase in testCases)
        {
            IExpression parsed = parser.Parse(testCase.Tokenized);

            Assert.AreEqual(testCase.Parsed, parsed);
            parser.ClearCache();
        }
    }

    /// <summary>
    /// Tests that previously parsed expressions get added to the Parser's cache, expressions can be manually added and
    /// removed from the cache, a string can be found in the cache, and the cache can be cleared.
    /// </summary>
    [TestMethod]
    public void TestCache()
    {
        Parser parser = new Parser();

        foreach (TestCase test_case in TestCases.Expressions)
        {
            parser.Parse(test_case.ExpressionString);
            Assert.IsTrue(parser.ContainsCache(test_case.ExpressionString));
            Assert.IsTrue(parser.ContainsCache(test_case.Tokenized));
            parser.RemoveCache(test_case.Tokenized);
            Assert.IsFalse(parser.ContainsCache(test_case.ExpressionString));
            Assert.IsFalse(parser.ContainsCache(test_case.Tokenized));
        }

        (string, IExpression) pi = ("pi", new Number(3.1415926535));

        parser.AddCache(pi.Item1, pi.Item2);
        Assert.IsTrue(parser.ContainsCache(pi.Item1));
        parser.ClearCache();
        Assert.IsFalse(parser.ContainsCache(pi.Item1));
    }

    /// <summary>
    /// Tests that Rules can be added and removed from the Parser's grammar along with being able to change the priority
    /// of Rules.
    /// </summary>
    [TestMethod]
    public void TestGrammar()
    {
        Parser parser = new Parser();
        RegexRule tau = new RegexRule("tau", "tau", (_, _, _) => new Number(6.28), RegexOptions.None);

        Assert.IsFalse(parser.Grammar.Contains(CUSTOM_RULE));
        Assert.IsFalse(parser.Grammar.Contains(tau));
        Assert.IsTrue(parser.AddGrammarRule(CUSTOM_RULE, 0));
        Assert.IsTrue((RegexRule)parser.Grammar[0] == CUSTOM_RULE);
        Assert.IsTrue(parser.AddGrammarRule(tau, -1));
        Assert.IsTrue((RegexRule)parser.Grammar.Last() == tau);
        Assert.IsTrue(parser.GrammarContains(tau.Name));
        Assert.AreNotEqual(-1, parser.ReplaceGrammarRule(new ParserRule("tau", null!, null!)));
        Assert.IsNotInstanceOfType(parser.GetGrammarRule("tau"), typeof(RegexRule));
        Assert.AreNotEqual(-1, parser.RemoveGrammarRule(CUSTOM_RULE.Name));
        Assert.IsFalse(parser.GrammarContains(CUSTOM_RULE.Name));
        Assert.IsTrue(parser.RemoveGrammarRuleAt(parser.Grammar.Length - 1));
        Assert.IsFalse(parser.GrammarContains(tau.Name));
    }

    /// <summary>
    /// Tests the tokenizing and detokenizing of varying types of brackets.
    /// </summary>
    //[TestMethod]
    //public void TestTokenizer()
    //{
    //    string input = "(0) [1234] {12{cs}34} <dfsg{dfg}dsa[234]54>";
    //    Dictionary<Brackets, string> expected = new Dictionary<Brackets, string>
    //    {
    //        { Brackets.None, input },
    //        { Brackets.Parenthesis, @"[0] \[1234\] {12{cs}34} <dfsg{dfg}dsa\[234\]54>" },
    //        { Brackets.Square, @"(0) [0] {12{cs}34} <dfsg{dfg}dsa[1]54>" },
    //        { Brackets.Curly, @"(0) \[1234\] [0] <dfsg[1]dsa\[234\]54>" },
    //        { Brackets.All, @"[0] [1] [2] [3]" },
    //    };

    //    foreach (Brackets bracket in expected.Keys)
    //    {
    //        string tokenized = input.TokenizeInput(out Token[] tokens, bracket);
    //        Assert.AreEqual(expected[bracket], tokenized);

    //        if (tokens.Length > 0)
    //        {
    //            MatchCollection matches = Regex.Matches(tokenized, @"\[\d+\]");
    //            for (int i = 0; i < matches.Count; i++)
    //            {
    //                int index = matches[i].Index;
    //                int dindex = ContextFreeUtils.DetokenizeIndex(index, tokenized, tokens);

    //                Assert.AreEqual(tokens[i].Index, dindex);
    //            }

    //            int[] dindexes = ContextFreeUtils.DetokenizeIndexes(matches.Select(m => m.Index), tokenized, tokens)
    //                .ToArray();

    //            for (int i = 0; i < tokens.Length; i++)
    //                Assert.AreEqual(tokens[i].Index, dindexes[i]);
    //        }
    //    }
    //}
}
