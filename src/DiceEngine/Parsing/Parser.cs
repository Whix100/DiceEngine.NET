using DiceEngine.Attributes;
using DiceEngine.Expressions;
using DiceEngine.Extensions;
using DiceEngine.Parsing.Rules;
using DiceEngine.Tokenization;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using System.Reflection;
using static DiceEngine.Parsing.Defaults.ParserDefaults;

namespace DiceEngine.Parsing;

/// <summary>
/// Create a <see cref="Parser"/> using the specified grammar.
/// </summary>
/// <param name="grammar">
/// The specified <see cref="IEnumerable{Rule}"/> to be used as the grammar of the <see cref="Parser"/>.
/// </param>
public partial class Parser(IEnumerable<IParserRule> grammar, Tokenizer? tokenizer = null)
{
    private readonly List<IParserRule> _grammar = grammar.ToList();
    private readonly Tokenizer _tokenizer = tokenizer ?? new();
    
    public IParserRule[] Grammar => [.. _grammar];

    /// <summary>
    /// Creates a <see cref="Parser"/> with the default grammar and default tokenizer.
    /// </summary>
    public Parser() : this(DefaultRules) { }

    /// <summary>
    /// Creates a <see cref="Parser"/> with the default grammar.
    /// </summary>
    public Parser(Tokenizer tokenizer) : this(DefaultRules, tokenizer) { }

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

        ImmutableArray<IToken> tokenizedInput = _tokenizer.Tokenize(input);
        IExpression result = Parse(tokenizedInput);

        AddCache(input, result);
        return result;
    }

    /// <summary>
    /// Parses a tokenized expression into an <see cref="IExpression"/>.
    /// </summary>
    /// <param name="input">The tokenized expression to parse.</param>
    /// <returns>An <see cref="IExpression"/> parsed from the specified expression <see cref="string"/>.</returns>
    /// <exception cref="Exception"></exception>
    public IExpression Parse(ImmutableArray<IToken> input)
    {
        if (ContainsCache(input))
            return _cache[input];

        foreach (IParserRule rule in _grammar)
        {
            IExpression? expression = rule.Parse(input, this);

            if (expression is not null)
                return expression;
        }

        throw new Exception($"The input was not in the correct format: '{input.JoinTokens()}'");
    }

    /// <summary>
    /// Add <see cref="IParserRule"/> to the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="rule">The <see cref="IParserRule"/> to be added to the grammar.</param>
    /// <param name="index">The index to put the <see cref="IParserRule"/> in the grammar.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IParserRule"/> was successfully added to the grammar; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool AddGrammarRule(IParserRule rule, int index = -1)
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

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes the <see cref="IParserRule"/> with the specified name from the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="IParserRule"/> to be removed.</param>
    /// <returns>
    /// The index of the removed rule if the <see cref="IParserRule"/> was successfully removed; otherwise, -1.
    /// </returns>
    public int RemoveGrammarRule(string name)
    {
        for (int i = 0; i < _grammar.Count; i++)
        {
            if (_grammar[i].Name == name)
            {
                if (RemoveGrammarRuleAt(i))
                    return i;
                else
                    break;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes the <see cref="IParserRule"/> at the specified index from the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="index">The index for the <see cref="IParserRule"/> to be removed.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IParserRule"/> was successfully removed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool RemoveGrammarRuleAt(int index)
    {
        try
        {
            _grammar.RemoveAt(index);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Replaces the current <see cref="IParserRule"/> with the specified name with a new <see cref="IParserRule"/>.
    /// </summary>
    /// <param name="newRule">The new rule to replace the old rule.</param>
    /// <returns>
    /// The index of the replaced rule if the <see cref="IParserRule"/> was successfully replaced; otherwise, -1.
    /// </returns>
    public int ReplaceGrammarRule(IParserRule newRule)
    {
        for (int i = 0; i < _grammar.Count; i++)
        {
            if (_grammar[i].Name == newRule.Name)
            {
                _grammar[i] = newRule;
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Determines whether a rule with the specified name is in the grammar of the <see cref="Parser"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="IParserRule"/> to find.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="IParserRule"/> was successfully found; otherwise, <see langword="false"/>.
    /// </returns>
    public bool GrammarContains(string name)
    {
        foreach (IParserRule rule in _grammar)
            if (rule.Name == name)
                return true;

        return false;
    }

    /// <summary>
    /// Gets a <see cref="IParserRule"/> from the grammar based on the specified name.
    /// </summary>
    /// <param name="name">The name of the grammar rule.</param>
    /// <returns>The <see cref="IParserRule"/> in the grammar with the name <paramref name="name"/>.</returns>
    public IParserRule? GetGrammarRule(string name)
    {
        int idx = 0;

        while (idx < _grammar.Count && _grammar[idx].Name != name)
            idx++;

        return GetGrammarRule(idx);
    }

    /// <summary>
    /// Gets a <see cref="IParserRule"/> from the grammar based on the specified index.
    /// </summary>
    /// <param name="index">The index of the grammar rule.</param>
    /// <returns>The <see cref="IParserRule"/> in the grammar at the index of <paramref name="index"/>.</returns>
    public IParserRule? GetGrammarRule(int index)
        => index >= 0 && index < _grammar.Count ? _grammar[index] : null;
}
