﻿using CalcExpr.Attributes;
using CalcExpr.Expressions;
using CalcExpr.Parsing.Rules;
using System.Reflection;
using System.Text.RegularExpressions;
using static CalcExpr.Parsing.ParseFunctions;
using static CalcExpr.Parsing.MatchFunctions;
using static CalcExpr.Parsing.ParseMatchFunctions;

namespace CalcExpr.Parsing;

public class Parser
{
    private readonly List<Rule> _grammar = [];
    private readonly Dictionary<string, IExpression> _cache = [];

    public Rule[] Grammar
        => _grammar.ToArray();

    public string[] Cache
        => _cache.Keys.ToArray();

    /// <summary>
    /// Creates a <see cref="Parser"/> with the default grammar.
    /// </summary>
    /// <param name="build_rules">Whether or not grammar rules should be prebuilt.</param>
    public Parser(bool build_rules = true)
        : this ([
            new ReferenceRegexRule("DiscreteOperand", "({Prefix}*({Variable}|{Constant}|{Number}|{Token}){Postfix}*)",
                RegexRuleOptions.PadReferences),
            new ReferenceRegexRule("Operand", @"[\[\{]?({DiscreteOperand}|{Parameter}|{TokenizedParameter})[\]\}]?"),
            new ReferenceRegexRule("Token", @"\[\d+\]"),
            new ReferenceRegexRule("Attribute", @"([A-Za-z][A-Za-z_0-9]*(\({Number}(,{Number})*\))?)",
                RegexRuleOptions.PadReferences),
            new ReferenceRegexRule("Parameter", @"((\\?\[{Attribute}(,{Attribute})*\\?\])?{Variable})",
                RegexRuleOptions.PadReferences),
            new ReferenceRegexRule("TokenizedAttribute", @"([A-Za-z][A-Za-z_0-9]*({Token})?)",
                RegexRuleOptions.PadReferences),
            new ReferenceRegexRule("TokenizedParameter",
                @"((\\?\[{TokenizedAttribute}(,{TokenizedAttribute})*\\?\])?{Variable})",
                RegexRuleOptions.PadReferences),
            new Rule("Collection", ParseMatchCollection, MatchCollection),
            new Rule("FunctionCall", ParseFunctionCall, MatchFunctionCall, ParseMatchFunctionCall),
            new NestedRegexRule("LambdaFunction", @"({Parameter}|\(\s*(({Parameter},)*{Parameter})?\))\s*=>",
                RegexRuleOptions.Left | RegexRuleOptions.PadReferences | RegexRuleOptions.Trim,
                ParseMatchLambdaFunction),
            new Rule("Parentheses", ParseMatchParentheses, MatchParentheses),
            new RegexRule("WithParentheses", @"\(|\)", RegexOptions.None, ParseMatchWithParentheses),
            new NestedRegexRule("AssignBinOp", @"(?<={Operand})(?<!!)(=)(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchAssignmentOperator),
            new NestedRegexRule("OrBinOp", @"(?<={Operand})(\|\||∨)(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new NestedRegexRule("XorBinOp", @"(?<={Operand})(⊕)(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new NestedRegexRule("AndBinOp", @"(?<={Operand})(&&|∧)(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new NestedRegexRule("EqBinOp", @"(?<={Operand})(==|!=|<>|≠)(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new NestedRegexRule("IneqBinOp", @"(?<={Operand})(>=|<=|<(?!>)|(?<!<)>|[≤≥])(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new NestedRegexRule("AddBinOp", @"(?<={Operand})([\+\-])(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new NestedRegexRule("MultBinOp", @"(?<={Operand})(%%|//|[*×/÷%])(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new NestedRegexRule("ExpBinOp", @"(?<={Operand})(\^)(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchBinaryOperator),
            new RegexRule("Prefix", @"((\+{2})|(\-{2})|[\+\-!~¬])",
                RegexRuleOptions.Left | RegexRuleOptions.TrimLeft, ParseMatchPrefix),
            new RegexRule("Postfix", @"((\+{2})|(\-{2})|((?<![A-Za-zΑ-Ωα-ω0-9](!!)*!)!!)|[!%#])",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.Right | RegexRuleOptions.TrimRight, ParseMatchPostfix),
            new Rule("Indexer", ParseMatchIndexer, MatchIndexer),
            new RegexRule("Constant", "(∞|(inf(inity)?)|π|pi|τ|tau|true|false|undefined|dne|(empty(_set)?)|∅|e)",
                RegexRuleOptions.Only | RegexRuleOptions.Trim, ParseMatchConstant),
            new RegexRule("Variable", "([A-Za-zΑ-Ωα-ω]+(_[A-Za-zΑ-Ωα-ω0-9]+)*)",
                RegexRuleOptions.Only | RegexRuleOptions.Trim, ParseMatchVariable),
            new RegexRule("Number", @"((\d+\.?\d*)|(\d*\.?\d+))", RegexRuleOptions.Only | RegexRuleOptions.Trim,
                ParseMatchNumber)
        ],
        build_rules)
    { }

    /// <summary>
    /// Create a <see cref="Parser"/> using the specified grammar.
    /// </summary>
    /// <param name="grammar">
    /// The specified <see cref="IEnumerable{Rule}"/> to be used as the grammar of the <see cref="Parser"/>.
    /// </param>
    /// <param name="build_rules">Whether or not grammar rules should be prebuilt.</param>
    public Parser(IEnumerable<Rule> grammar, bool build_rules = true)
    {
        _grammar = grammar.ToList();

        if (build_rules)
            BuildGrammarRules(_grammar);
    }

    /// <summary>
    /// Parses an expression <see cref="string"/> into an <see cref="IExpression"/>.
    /// </summary>
    /// <param name="input">The expression <see cref="string"/> to parse.</param>
    /// <returns>An <see cref="IExpression"/> parsed from the specified expression <see cref="string"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public IExpression Parse(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (ContainsCache(input))
            return _cache[CleanExpressionString(input)];

        foreach (Rule rule in _grammar)
        {
            if (rule.GetType().GetCustomAttribute<ReferenceRuleAttribute>() is not null)
                continue;

            Token? match = rule.Match(input, Grammar);

            if (match.HasValue)
            {
                IExpression? expression = rule.Parse(input, match.Value, this);

                if (expression is not null)
                {
                    AddCache(input, expression);
                    return expression;
                }
            }
        }

        throw new Exception($"The input was not in the correct format: '{input}'");
    }

    private static string CleanExpressionString(string expression)
        => Regex.Replace(expression, @"\s+", "");

    /// <summary>
    /// Determines whether the cache of the <see cref="Parser"/> contains a specified expression <see cref="string"/>.
    /// </summary>
    /// <param name="expression">The expression to locate in the cache.</param>
    /// <returns>
    /// <see langword="true"/> if the cache contains an entry with the specified expression; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool ContainsCache(string expression)
        => _cache.ContainsKey(CleanExpressionString(expression));

    /// <summary>
    /// Add expression <see cref="string"/> to the cache of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="key">The expression <see cref="string"/> of the cached <see cref="IExpression"/>.</param>
    /// <param name="value">The cached <see cref="IExpression"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IExpression"/> was successfully added to the cache; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool AddCache(string key, IExpression value)
    {
        try
        {
            _cache[CleanExpressionString(key)] = value;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes a cached <see cref="IExpression"/> based on the specified expression <see cref="string"/>.
    /// </summary>
    /// <param name="expression">The expression <see cref="string"/> of the cached <see cref="IExpression"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IExpression"/> was successfully removed from the cache; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool RemoveCache(string expression)
    {
        string clean_expression = CleanExpressionString(expression);

        return _cache.ContainsKey(clean_expression) && _cache.Remove(clean_expression);
    }

    /// <summary>
    /// Clears the cache of the parser.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the cache was successfully cleared; otherwise <see langword="false"/>.
    /// </returns>
    public bool ClearCache()
    {
        _cache.Clear();
        return _cache.Count == 0;
    }

    /// <summary>
    /// Add <see cref="Rule"/> to the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="rule">The <see cref="Rule"/> to be added to the grammar.</param>
    /// <param name="index">The index to put the <see cref="Rule"/> in the grammar.</param>
    /// <param name="build_rules">Whether or not grammar rules should be rebuilt.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Rule"/> was successfully added to the grammar; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool AddGrammarRule(Rule rule, int index = -1, bool build_rules = true)
    {
        if (index < 0)
            index += _grammar.Count + 1;

        try
        {
            if (index <= 0)
                _grammar.Insert(0, rule);
            else if (index >= _grammar.Count)
                _grammar.Insert(_grammar.Count, rule);
            else
                _grammar.Insert(index, rule);

            if (build_rules)
                RebuildGrammarRules();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes the <see cref="Rule"/> with the specified name from the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="Rule"/> to be removed.</param>
    /// <param name="build_rules">Whether or not grammar rules should be rebuilt.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Rule"/> was successfully removed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool RemoveGrammarRule(string name, bool build_rules = true)
    {
        for (int i = 0; i < _grammar.Count; i++)
            if (_grammar[i].Name == name)
                return RemoveGrammarRuleAt(i, build_rules);

        return false;
    }

    /// <summary>
    /// Removes the <see cref="Rule"/> at the specified index from the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="index">The index for the <see cref="Rule"/> to be removed.</param>
    /// <param name="build_rules">Whether or not grammar rules should be rebuilt.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Rule"/> was successfully removed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool RemoveGrammarRuleAt(int index, bool build_rules = true)
    {
        try
        {
            _grammar.RemoveAt(index);

            if (build_rules)
                RebuildGrammarRules();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether a rule with the specified name is in the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="Rule"/> to find.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="Rule"/> was successfully found; otherwise, <see langword="false"/>.
    /// </returns>
    public bool GrammarContains(string name)
    {
        foreach (Rule rule in _grammar)
            if (rule.Name == name)
                return true;

        return false;
    }

    /// <summary>
    /// Gets a <see cref="Rule"/> from the grammar based on the specified name.
    /// </summary>
    /// <param name="name">The name of the grammar rule.</param>
    /// <returns>The <see cref="Rule"/> in the grammar with the name <paramref name="name"/>.</returns>
    public Rule? GetGrammarRule(string name)
    {
        int idx = 0;

        while (idx < _grammar.Count && _grammar[idx].Name != name)
            idx++;

        return GetGrammarRule(idx);
    }

    /// <summary>
    /// Gets a <see cref="Rule"/> from the grammar based on the specified index.
    /// </summary>
    /// <param name="index">The index of the grammar rule.</param>
    /// <returns>The <see cref="Rule"/> in the grammar at the index of <paramref name="index"/>.</returns>
    public Rule? GetGrammarRule(int index)
        => index >= 0 && index < _grammar.Count ? _grammar[index] : null;

    /// <summary>
    /// Rebuilds all of the grammar rules in the <see cref="Parser"/>.
    /// </summary>
    public void RebuildGrammarRules()
        => BuildGrammarRules(_grammar);

    private static void BuildGrammarRules(IEnumerable<Rule> rules)
    {
        foreach (Rule rule in rules)
            if (rule is NestedRegexRule regex_rule)
                regex_rule.Build(rules);
    }
}
