using DiceEngine.Attributes;
using DiceEngine.Expressions;
using DiceEngine.Parsing.Rules;
using System.Reflection;
using System.Text.RegularExpressions;
using static DiceEngine.Parsing.ParseFunctions;
using static DiceEngine.Parsing.MatchFunctions;
using static DiceEngine.Parsing.ParseMatchFunctions;

namespace DiceEngine.Parsing;

public class Parser
{
    private readonly List<IRule> _grammar = [];
    private readonly Dictionary<string, IExpression> _cache = [];

    public IRule[] Grammar
        => _grammar.ToArray();

    public string[] Cache
        => _cache.Keys.ToArray();

    /// <summary>
    /// Creates a <see cref="Parser"/> with the default grammar.
    /// </summary>
    /// <param name="build_rules">Whether or not grammar rules should be prebuilt.</param>
    public Parser(bool build_rules = true)
        : this ([
            new ReferenceRegexRule("DiscreteOperand",
                "({Prefix}*({Variable}|{Undefined}|{Logical}|{Infinity}|{Constant}|{Number}|{Token}){Postfix}*)",
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
            new Rule("Collection", ParseCollection, MatchCollection, ParseMatchCollection),
            new Rule("FunctionCall", ParseFunctionCall, MatchFunctionCall, ParseMatchFunctionCall),
            new NestedRegexRule("LambdaFunction", @"({Parameter}|\(\s*(({Parameter},)*{Parameter})?\))\s*=>",
                RegexRuleOptions.Left | RegexRuleOptions.PadReferences | RegexRuleOptions.Trim,
                ParseMatchLambdaFunction),
            new Rule("Parentheses", ParseMatchParentheses, MatchParentheses),
            new RegexRule("WithParentheses", @"\(|\)", RegexOptions.None, ParseMatchWithParentheses),
            new NestedRegexRule("AssignBinOp", @"(?<={Operand})(?<!!)(=)(?={Operand})",
                RegexRuleOptions.RightToLeft | RegexRuleOptions.PadReferences, ParseMatchAssignmentOperator),
            new RegexRule("DiceNotation", @"\d*d(\d+|%|F)((((k|d|(r(r|o|a))|e)(|h|l|((<|>)=?)|≤|≥))|mi|ma)\d+)*",
                RegexRuleOptions.Only | RegexRuleOptions.Trim, ParseMatchDice),
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
            new RegexRule("Undefined", "undefined|dne", RegexRuleOptions.Only | RegexRuleOptions.Trim,
                ParseMatchUndefined),
            new RegexRule("Logical", "true|false", RegexRuleOptions.Only | RegexRuleOptions.Trim,
                ParseMatchLogical),
            new RegexRule("Infinity", "∞|(inf(inity)?)", RegexRuleOptions.Only | RegexRuleOptions.Trim,
                ParseMatchInfinity),
            new RegexRule("Constant", "(π|pi|τ|tau|(empty(_set)?)|∅|e)",
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
    public Parser(IEnumerable<IRule> grammar, bool build_rules = true)
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

        foreach (IRule rule in _grammar)
        {
            if (rule.GetType().GetCustomAttribute<ReferenceRuleAttribute>() is not null)
                continue;

            IExpression? expression = rule.Parse(input, this);

            if (expression is not null)
            {
                AddCache(input, expression);
                return expression;
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
    /// Add <see cref="IRule"/> to the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="rule">The <see cref="IRule"/> to be added to the grammar.</param>
    /// <param name="index">The index to put the <see cref="IRule"/> in the grammar.</param>
    /// <param name="build_rules">Whether or not grammar rules should be rebuilt.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IRule"/> was successfully added to the grammar; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool AddGrammarRule(IRule rule, int index = -1, bool build_rules = true)
    {
        if (index < 0)
            index += _grammar.Count + 1;

        try
        {
            if (GrammarContains(rule.Name))
                return false;

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
    /// Removes the <see cref="IRule"/> with the specified name from the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="IRule"/> to be removed.</param>
    /// <param name="buildRules">Whether or not grammar rules should be rebuilt.</param>
    /// <returns>
    /// The index of the removed rule if the <see cref="IRule"/> was successfully removed; otherwise, -1.
    /// </returns>
    public int RemoveGrammarRule(string name, bool buildRules = true)
    {
        for (int i = 0; i < _grammar.Count; i++)
        {
            if (_grammar[i].Name == name)
            {
                if (RemoveGrammarRuleAt(i, buildRules))
                    return i;
                else
                    break;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes the <see cref="IRule"/> at the specified index from the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="index">The index for the <see cref="IRule"/> to be removed.</param>
    /// <param name="build_rules">Whether or not grammar rules should be rebuilt.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IRule"/> was successfully removed; otherwise, <see langword="false"/>.
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
    /// Replaces the current <see cref="IRule"/> with the specified name with a new <see cref="IRule"/>.
    /// </summary>
    /// <param name="newRule">The new rule to replace the old rule.</param>
    /// <param name="buildRules">Whether or not grammar rules should be rebuilt.</param>
    /// <returns>
    /// The index of the replaced rule if the <see cref="IRule"/> was successfully replaced; otherwise, -1.
    /// </returns>
    public int ReplaceGrammarRule(IRule newRule, bool buildRules = true)
    {
        for (int i = 0; i < _grammar.Count; i++)
        {
            if (_grammar[i].Name == newRule.Name)
            {
                _grammar[i] = newRule;
                return i;
            }
        }

        if (buildRules)
            RebuildGrammarRules();

        return -1;
    }

    /// <summary>
    /// Determines whether a rule with the specified name is in the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="IRule"/> to find.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IRule"/> was successfully found; otherwise, <see langword="false"/>.
    /// </returns>
    public bool GrammarContains(string name)
    {
        foreach (IRule rule in _grammar)
            if (rule.Name == name)
                return true;

        return false;
    }

    /// <summary>
    /// Gets a <see cref="IRule"/> from the grammar based on the specified name.
    /// </summary>
    /// <param name="name">The name of the grammar rule.</param>
    /// <returns>The <see cref="IRule"/> in the grammar with the name <paramref name="name"/>.</returns>
    public IRule? GetGrammarRule(string name)
    {
        int idx = 0;

        while (idx < _grammar.Count && _grammar[idx].Name != name)
            idx++;

        return GetGrammarRule(idx);
    }

    /// <summary>
    /// Gets a <see cref="IRule"/> from the grammar based on the specified index.
    /// </summary>
    /// <param name="index">The index of the grammar rule.</param>
    /// <returns>The <see cref="IRule"/> in the grammar at the index of <paramref name="index"/>.</returns>
    public IRule? GetGrammarRule(int index)
        => index >= 0 && index < _grammar.Count ? _grammar[index] : null;

    /// <summary>
    /// Rebuilds all of the grammar rules in the <see cref="Parser"/>.
    /// </summary>
    public void RebuildGrammarRules()
        => BuildGrammarRules(_grammar);

    private static void BuildGrammarRules(IEnumerable<IRule> rules)
    {
        foreach (IRule rule in rules)
            if (rule is NestedRegexRule regex_rule)
                regex_rule.Build(rules);
    }
}
