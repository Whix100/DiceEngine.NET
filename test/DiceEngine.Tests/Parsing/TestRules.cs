using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;
using DiceEngine.Parsing;
using DiceEngine.Parsing.Rules;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace TestDiceEngine.Parsing;

[TestClass]
public class TestRules
{
    /// <summary>
    /// Test ParserRules for initialization, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestRule()
    {
        string name = "RuleName";
        IExpression parsed = Logical.TRUE;
        TokenMatch matched = new TokenMatch([new WordToken("Rule", 0)], 0);
        ParserRule rule = new ParserRule(name, (_, _) => parsed, (_, _) => matched);

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(parsed, rule.Parse([], matched, null!));
        Assert.AreEqual(matched, rule.Match([], []));
    }

    /// <summary>
    /// Test RegexRules for initialization, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestRegexRule()
    {
        string name = "RegexRuleName";
        string regex = @"\w\d";
        TokenMatch match = new TokenMatch([new WordToken("Input", 0), new NumberToken("123", 5)], 0);
        IExpression parsed = Logical.TRUE;
        RegexRule rule = new RegexRule(name, regex, (_, _, _) => parsed);
        ImmutableArray<IToken> input = [new WordToken("Input", 0), new NumberToken("123", 5)];

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(regex, rule.Regex);
        Assert.AreEqual(RegexOptions.None, rule.Options);
        Assert.AreEqual(parsed, rule.Parse(input, new Parser()));

        int i = 0;

        foreach (IToken token in rule.Match(input, [])?.Match ?? [])
            Assert.AreEqual(match[i++], token);
    }

    /// <summary>
    /// Test TypeRules for initialization, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestTypeRule()
    {
        string name = "TypeRuleName";
        IExpression parsed = Logical.TRUE;
        TokenMatch match = new TokenMatch([new WordToken("Input", 0)], 0);
        TypeRule<WordToken> rule = new TypeRule<WordToken>(name, (_, _) => parsed);
        ImmutableArray<IToken> input = [new WordToken("Input", 0)];

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(parsed, rule.Parse([], match, null!));
        Assert.AreEqual(match.Single(), rule.Match(input, [])?.Single());
    }

    /// <summary>
    /// Test OptionRules for initialization, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestOptionRule()
    {
        string name = "OptionRuleName";
        IExpression parsed = Logical.TRUE;
        TokenMatch match = new TokenMatch([new WordToken("Input", 0)], 0);
        OptionRule rule = new OptionRule(name, ["Input"], (_, _) => parsed);
        ImmutableArray<IToken> input = [new WordToken("Input", 0)];

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(parsed, rule.Parse([], match, null!));
        Assert.AreEqual(match.Single(), rule.Match(input, [])?.Single());
    }
}
