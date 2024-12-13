using DiceEngine.Expressions;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;

namespace DiceEngine.Parsing.Rules;

/// <summary>
/// A parser rule to match a token from a list of string values.
/// </summary>
/// <param name="name">The name of the rule.</param>
/// <param name="options">The options for a matching token value.</param>
/// <param name="parse">The function to parse the matching token.</param>
/// <param name="stringComparison">String comparison options to checking if a token is a match.</param>
public class OptionRule(string name, string[] options, Func<IToken, Parser, IExpression> parse,
    StringComparison stringComparison = StringComparison.CurrentCulture)
    : IParserRule
{
    private readonly Func<IToken, Parser, IExpression> _parse = parse;

    public string Name { get; } = name;

    public ImmutableHashSet<string> Options { get; } = options.Distinct().ToImmutableHashSet();

    public StringComparison Comparison { get; } = stringComparison;

    public TokenMatch? Match(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        if (input.Length == 1 && Options.Contains(input.First().Value))
            return new TokenMatch([input.First()], 0);

        return null;
    }

    public IExpression? Parse(ImmutableArray<IToken> input, Parser parser)
    {
        if (input.Length == 1 && Options.Contains(input.First().Value))
            return _parse(input.First(), parser);

        return null;
    }

    public IExpression? Parse(ImmutableArray<IToken> _, TokenMatch match, Parser parser)
        => Parse([.. match.Match], parser);

    public override bool Equals(object? obj)
        => obj is OptionRule r && r._parse == _parse && r.Options.SetEquals(Options) && r.Comparison == Comparison;

    public override int GetHashCode()
        => HashCode.Combine(Name, _parse, Options, Comparison);

    public static bool operator ==(OptionRule a, IParserRule b)
        => a.Equals(b);

    public static bool operator !=(OptionRule a, IParserRule b)
        => !a.Equals(b);
}
