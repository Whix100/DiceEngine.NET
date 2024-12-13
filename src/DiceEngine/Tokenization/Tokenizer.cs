using DiceEngine.Tokenization.Rules;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;
using static DiceEngine.Tokenization.Tokens.Defaults.TokenizerDefaults;

namespace DiceEngine.Tokenization;

/// <summary>
/// A <see cref="Tokenizer"/> to convert a <see langword="string"/> into a list of tokens.
/// </summary>
/// <param name="rules">
/// A list of types that implement <see cref="IToken"/>s define the behavior of the <see cref="Tokenizer"/>.
/// </param>
/// <param name="trimWhitespace">Whether to trim the whitespace in between tokens.</param>
public class Tokenizer(IEnumerable<ITokenizerRule> rules, bool trimWhitespace = true)
{
    private readonly List<ITokenizerRule> _rules = rules.ToList();

    public ITokenizerRule[] Rules => [.. _rules];

    public bool TrimWhitespace { get; } = trimWhitespace;

    /// <summary>
    /// Creates a <see cref="Tokenizer"/> with the default rules list.
    /// </summary>
    public Tokenizer() : this(DefaultRules) { }

    /// <summary>
    /// Parses an expression <see cref="string"/> into an <see cref="IToken"/>.
    /// </summary>
    /// <param name="input">The expression <see cref="string"/> to parse.</param>
    /// <returns>An <see cref="IToken"/> parsed from the specified expression <see cref="string"/>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public ImmutableArray<IToken> Tokenize(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        List<IToken> tokens = [];
        int i = 0;

        if (TrimWhitespace)
        {
            string trimmedInput = input.TrimStart();

            i += input.Length - trimmedInput.Length;
            input = trimmedInput;
        }

        while (input.Length != 0)
        {
            bool matched = false;

            foreach (ITokenizerRule rule in _rules)
            {
                IToken? token = rule.Tokenize(ref input, i);

                if (token is not null)
                {
                    matched = true;
                    tokens.Add(token);
                    i += token.Value.Length;
                    break;
                }
            }

            if (!matched)
            {
                tokens.Add(new Token(input.First().ToString(), 0));
                input = input[1..];
                i++;
            }

            if (TrimWhitespace)
            {
                string trimmedInput = input.TrimStart();

                i += input.Length - trimmedInput.Length;
                input = trimmedInput;
            }
        }

        return [.. tokens];
    }

    /// <summary>
    /// Add <see cref="ITokenizerRule"/> to the grammar of the <see cref="Tokenizer"/>.
    /// </summary>
    /// <param name="rule">The <see cref="ITokenizerRule"/> to be added to the grammar.</param>
    /// <param name="index">The index to put the <see cref="ITokenizerRule"/> in the grammar.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="ITokenizerRule"/> was successfully added to the grammar; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool AddRule(ITokenizerRule rule, int index = -1)
    {
        if (index < 0)
            index += _rules.Count + 1;

        try
        {
            if (ContainsRule(rule.Name))
                return false;

            if (index <= 0)
                _rules.Insert(0, rule);
            else if (index >= _rules.Count)
                _rules.Insert(_rules.Count, rule);
            else
                _rules.Insert(index, rule);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Removes the <see cref="ITokenizerRule"/> with the specified name from the grammar of the <see cref="Tokenizer"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ITokenizerRule"/> to be removed.</param>
    /// <returns>
    /// The index of the removed rule if the <see cref="ITokenizerRule"/> was successfully removed; otherwise, -1.
    /// </returns>
    public int RemoveRule(string name)
    {
        for (int i = 0; i < _rules.Count; i++)
        {
            if (_rules[i].Name == name)
            {
                if (RemoveRuleAt(i))
                    return i;
                else
                    break;
            }
        }

        return -1;
    }

    /// <summary>
    /// Removes the <see cref="ITokenizerRule"/> at the specified index from the grammar of the <see cref="Tokenizer"/>.
    /// </summary>
    /// <param name="index">The index for the <see cref="ITokenizerRule"/> to be removed.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="ITokenizerRule"/> was successfully removed; otherwise, <see langword="false"/>.
    /// </returns>
    public bool RemoveRuleAt(int index)
    {
        try
        {
            _rules.RemoveAt(index);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Replaces the current <see cref="ITokenizerRule"/> with the specified name with a new <see cref="ITokenizerRule"/>.
    /// </summary>
    /// <param name="newRule">The new rule to replace the old rule.</param>
    /// <returns>
    /// The index of the replaced rule if the <see cref="ITokenizerRule"/> was successfully replaced; otherwise, -1.
    /// </returns>
    public int ReplaceRule(ITokenizerRule newRule)
    {
        for (int i = 0; i < _rules.Count; i++)
        {
            if (_rules[i].Name == newRule.Name)
            {
                _rules[i] = newRule;
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Determines whether a rule with the specified name is in the grammar of the <see cref="Tokenizer"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ITokenizerRule"/> to find.</param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="ITokenizerRule"/> was successfully found; otherwise, <see langword="false"/>.
    /// </returns>
    public bool ContainsRule(string name)
    {
        foreach (ITokenizerRule rule in _rules)
            if (rule.Name == name)
                return true;

        return false;
    }

    /// <summary>
    /// Gets a <see cref="ITokenizerRule"/> from the grammar based on the specified name.
    /// </summary>
    /// <param name="name">The name of the grammar rule.</param>
    /// <returns>The <see cref="ITokenizerRule"/> in the grammar with the name <paramref name="name"/>.</returns>
    public ITokenizerRule? GetRule(string name)
    {
        int idx = 0;

        while (idx < _rules.Count && _rules[idx].Name != name)
            idx++;

        return GetRule(idx);
    }

    /// <summary>
    /// Gets a <see cref="ITokenizerRule"/> from the grammar based on the specified index.
    /// </summary>
    /// <param name="index">The index of the grammar rule.</param>
    /// <returns>The <see cref="ITokenizerRule"/> in the grammar at the index of <paramref name="index"/>.</returns>
    public ITokenizerRule? GetRule(int index)
        => index >= 0 && index < _rules.Count ? _rules[index] : null;
}
