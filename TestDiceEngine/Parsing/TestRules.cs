using DiceEngine.Expressions;
using DiceEngine.Parsing;
using DiceEngine.Parsing.Rules;

namespace TestDiceEngine.Parsing;

[TestClass]
public class TestRules
{
    /// <summary>
    /// Test Rules for initialization, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestRule()
    {
        string name = "RuleName";
        IExpression parsed = Logical.TRUE;
        Token matched = new Token("Rule", 0);
        Rule rule = new Rule(name, (_, _, _) => parsed, (_, _) => matched);

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(parsed, rule.Parse("", matched, null!));
        Assert.AreEqual(matched, rule.Match("", []));
    }

    /// <summary>
    /// Test RegexRules for initialization, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestRegexRule()
    {
        string name = "RegexRuleName";
        string regex = "[A-Z][a-z]+";
        RegexRuleOptions options = RegexRuleOptions.Trim;
        Token[] matches = [new Token("Input", 0), new Token("String", 5)];
        IExpression parsed = Logical.TRUE;
        RegexRule rule = new RegexRule(name, regex, options, (_, _, _) => parsed);
        string input = "InputString";

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(regex, rule.RegularExpression);
        Assert.AreEqual(options, rule.Options);
        Assert.AreEqual(parsed, rule.Parse("", matches[0], null!));
        Assert.AreEqual(matches[0], rule.Match(input, []));

        int i = 0;

        foreach (Token match in rule.Matches(input, []))
            Assert.AreEqual(matches[i++], match);
    }

    /// <summary>
    /// Test NestedRegexRules for initialization, building the regular expression, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestNestedRegexRule()
    {
        string name = "NestedRegexRuleName";
        string regex_template = "[A-Z][a-z]+{String}";
        RegexRule string_rule = new RegexRule("String", "String", RegexRuleOptions.None, null!);
        string regex = @"[A-Z][a-z]+(\s*String\s*)";
        RegexRuleOptions options = RegexRuleOptions.Trim | RegexRuleOptions.PadReferences;
        Token[] matches = [new Token("New String ", 0), new Token("Input String", 11)];
        IExpression parsed = Logical.TRUE;
        NestedRegexRule rule = new NestedRegexRule(name, regex_template, options, (_, _, _) => parsed);
        string input = "New String Input String";

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(regex_template, rule.RegularExpressionTemplate);
        Assert.IsNull(rule.RegularExpression);
        Assert.AreEqual(options, rule.Options);
        Assert.AreEqual(parsed, rule.Parse("", matches[0], null!));
        Assert.AreEqual(matches[0].Value, rule.Match(input, [string_rule])?.Value);
        Assert.AreEqual(regex, rule.RegularExpression);

        int i = 0;

        foreach (Token match in rule.Matches(input, []))
            Assert.AreEqual(matches[i++].Value, match.Value);
    }

    /// <summary>
    /// Test NestedRegexRules for initialization, building the regular expression, parsing, and matching.
    /// </summary>
    [TestMethod]
    public void TestReferenceRegexRule()
    {
        string name = "ReferenceRegexRuleName";
        string regex_template = "[A-Z][a-z]+{String}";
        RegexRule string_rule = new RegexRule("String", "String", RegexRuleOptions.None, null!);
        string regex = @"[A-Z][a-z]+(\s*String\s*)";
        RegexRuleOptions options = RegexRuleOptions.Trim | RegexRuleOptions.PadReferences;
        Token[] matches = [new Token("New String ", 0), new Token("Input String", 11)];
        IExpression parsed = Undefined.UNDEFINED;
        ReferenceRegexRule rule = new ReferenceRegexRule(name, regex_template, options);
        string input = "New String Input String";

        Assert.AreEqual(name, rule.Name);
        Assert.AreEqual(regex_template, rule.RegularExpressionTemplate);
        Assert.IsNull(rule.RegularExpression);
        Assert.AreEqual(options, rule.Options);
        Assert.AreEqual(parsed, rule.Parse("", matches[0], null!));
        Assert.AreEqual(matches[0], rule.Match(input, [string_rule]));
        Assert.AreEqual(regex, rule.RegularExpression);

        int i = 0;

        foreach (Token match in rule.Matches(input, [string_rule]))
            Assert.AreEqual(matches[i++], match);
    }
}
