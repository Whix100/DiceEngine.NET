using DiceEngine.Expressions;
using DiceEngine.Parsing;
using DiceEngine.Parsing.Rules;
using System.Text.RegularExpressions;
using TestDiceEngine.TestData;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.Parsing;

[TestClass]
public class TestParser
{
    readonly RegexRule CUSTOM_RULE = new RegexRule("Char", @"[A-Z]", RegexOptions.None,
        (expression, token, _) => new Number(token.Value[0] - 65));

    /// <summary>
    /// Tests that the Parser can be initialized properly from either constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        (string Name, string? Regex)[] default_rules =
        [
            ("DiscreteOperand",
                "({Prefix}*({Variable}|{Undefined}|{Logical}|{Infinity}|{Constant}|{Number}|{Token}){Postfix}*)"),
            ("Operand", @"[\[\{]?({DiscreteOperand}|{Parameter}|{TokenizedParameter})[\]\}]?"),
            ("Token", @"\[\d+\]"),
            ("Attribute", @"([A-Za-z][A-Za-z_0-9]*(\({Number}(,{Number})*\))?)"),
            ("Parameter", @"((\\?\[{Attribute}(,{Attribute})*\\?\])?{Variable})"),
            ("TokenizedAttribute", @"([A-Za-z][A-Za-z_0-9]*({Token})?)"),
            ("TokenizedParameter", @"((\\?\[{TokenizedAttribute}(,{TokenizedAttribute})*\\?\])?{Variable})"),
            ("Collection", null),
            ("FunctionCall", null),
            ("LambdaFunction", @"({Parameter}|\(\s*(({Parameter},)*{Parameter})?\))\s*=>"),
            ("Parentheses", null),
            ("WithParentheses", @"\(|\)"),
            ("AssignBinOp", @"(?<={Operand})(?<!!)(=)(?={Operand})"),
            ("DiceNotation", @"\d*d(\d+|%|F)((((k|d|(r(r|o|a))|e)(|h|l|((<|>)=?)|≤|≥))|mi|ma)\d+)*"),
            ("OrBinOp", @"(?<={Operand})(\|\||∨)(?={Operand})"),
            ("XorBinOp", @"(?<={Operand})(⊕)(?={Operand})"),
            ("AndBinOp", @"(?<={Operand})(&&|∧)(?={Operand})"),
            ("EqBinOp", @"(?<={Operand})(==|!=|<>|≠)(?={Operand})"),
            ("IneqBinOp", @"(?<={Operand})(>=|<=|<(?!>)|(?<!<)>|[≤≥])(?={Operand})"),
            ("AddBinOp", @"(?<={Operand})([\+\-])(?={Operand})"),
            ("MultBinOp", @"(?<={Operand})(%%|//|[*×/÷%])(?={Operand})"),
            ("ExpBinOp", @"(?<={Operand})(\^)(?={Operand})"),
            ("Prefix", @"((\+{2})|(\-{2})|[\+\-!~¬])"),
            ("Postfix", @"((\+{2})|(\-{2})|((?<![A-Za-zΑ-Ωα-ω0-9](!!)*!)!!)|[!%#])"),
            ("Indexer", null),
            ("Undefined", "undefined|dne"),
            ("Logical", "true|false"),
            ("Infinity", "∞|(inf(inity)?)"),
            ("Constant", "(π|pi|τ|tau|(empty(_set)?)|∅|e)"),
            ("Variable", "([A-Za-zΑ-Ωα-ω]+(_[A-Za-zΑ-Ωα-ω0-9]+)*)"),
            ("Number", @"((\d+\.?\d*)|(\d*\.?\d+))"),
        ];

        Parser parser = new Parser();

        for (int i = 0; i < default_rules.Length; i++)
        {
            IRule rule = parser.Grammar[i];

            Assert.AreEqual(default_rules[i].Name, rule.Name);

            if (rule is NestedRegexRule nested_regex_rule)
                Assert.AreEqual(default_rules[i].Regex, nested_regex_rule.RegularExpressionTemplate);
            else if (rule is RegexRule regex_rule)
                Assert.AreEqual(default_rules[i].Regex, regex_rule.RegularExpression);

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

    private static void ParseTestCases(IEnumerable<TestCase> test_cases)
    {
        Parser parser = new Parser();

        foreach (TestCase test_case in test_cases)
        {
            IExpression parsed = parser.Parse(test_case.ExpressionString);

            Assert.AreEqual(test_case.Parsed, parsed);
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
            parser.RemoveCache(test_case.ExpressionString);
            Assert.IsFalse(parser.ContainsCache(test_case.ExpressionString));
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
        RegexRule tau = new RegexRule("tau", "tau", RegexOptions.None, (expression, token, _) => new Number(6.28));

        Assert.IsFalse(parser.Grammar.Contains(CUSTOM_RULE));
        Assert.IsFalse(parser.Grammar.Contains(tau));
        Assert.IsTrue(parser.AddGrammarRule(CUSTOM_RULE, 0));
        Assert.IsTrue((RegexRule)parser.Grammar[0] == CUSTOM_RULE);
        Assert.IsTrue(parser.AddGrammarRule(tau, -1));
        Assert.IsTrue((RegexRule)parser.Grammar.Last() == tau);
        Assert.IsTrue(parser.GrammarContains(tau.Name));
        Assert.AreNotEqual(-1, parser.ReplaceGrammarRule(new Rule("tau", null!, null!)));
        Assert.IsNotInstanceOfType(parser.GetGrammarRule("tau"), typeof(RegexRule));
        Assert.AreNotEqual(-1, parser.RemoveGrammarRule(CUSTOM_RULE.Name));
        Assert.IsFalse(parser.GrammarContains(CUSTOM_RULE.Name));
        Assert.IsTrue(parser.RemoveGrammarRuleAt(parser.Grammar.Length - 1));
        Assert.IsFalse(parser.GrammarContains(tau.Name));
    }

    /// <summary>
    /// Tests the tokenizing and detokenizing of varying types of brackets.
    /// </summary>
    [TestMethod]
    public void TestTokenizer()
    {
        string input = "(0) [1234] {12{cs}34} <dfsg{dfg}dsa[234]54>";
        Dictionary<Brackets, string> expected = new Dictionary<Brackets, string>
        {
            { Brackets.None, input },
            { Brackets.Parenthesis, @"[0] \[1234\] {12{cs}34} <dfsg{dfg}dsa\[234\]54>" },
            { Brackets.Square, @"(0) [0] {12{cs}34} <dfsg{dfg}dsa[1]54>" },
            { Brackets.Curly, @"(0) \[1234\] [0] <dfsg[1]dsa\[234\]54>" },
            { Brackets.Angle, @"(0) \[1234\] {12{cs}34} [0]" },
            { Brackets.All, @"[0] [1] [2] [3]" },
        };

        foreach (Brackets bracket in expected.Keys)
        {
            string tokenized = input.TokenizeInput(out Token[] tokens, bracket);
            Assert.AreEqual(expected[bracket], tokenized);

            if (tokens.Length > 0)
            {
                MatchCollection matches = Regex.Matches(tokenized, @"\[\d+\]");
                for (int i = 0; i < matches.Count; i++)
                {
                    int index = matches[i].Index;
                    int dindex = ContextFreeUtils.DetokenizeIndex(index, tokenized, tokens);

                    Assert.AreEqual(tokens[i].Index, dindex);
                }

                int[] dindexes = ContextFreeUtils.DetokenizeIndexes(matches.Select(m => m.Index), tokenized, tokens)
                    .ToArray();

                for (int i = 0; i < tokens.Length; i++)
                    Assert.AreEqual(tokens[i].Index, dindexes[i]);
            }
        }
    }
}
