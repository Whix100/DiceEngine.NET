using DiceEngine.Expressions;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;

namespace DiceEngine.Parsing.Rules;

/// <summary>
/// A parser rule that matches a token of type <see cref="T"/>.
/// </summary>
/// <typeparam name="T">The type of token to match.</typeparam>
/// <param name="name">The name of the rule.</param>
/// <param name="parse">The function to parse the matched token.</param>
public class TypeRule<T>(string name, Func<T, Parser, IExpression> parse) : IParserRule
    where T : IToken
{
    private readonly Func<T, Parser, IExpression> _parse = parse;

    public string Name { get; } = name;

    public TokenMatch? Match(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        if (input.Length == 1 && input.First() is T)
            return new TokenMatch([input.First()], 0);

        return null;
    }

    public IExpression? Parse(ImmutableArray<IToken> input, Parser parser)
    {
        if (input.Length == 1 && input.First() is T token)
            return _parse(token, parser);

        return null;
    }

    public IExpression? Parse(ImmutableArray<IToken> _, TokenMatch match, Parser parser)
        => Parse([.. match.Match], parser);

    public override bool Equals(object? obj)
        => obj is TypeRule<T> r && r._parse == _parse;

    public override int GetHashCode()
        => HashCode.Combine(Name, _parse, typeof(T));

    public static bool operator ==(TypeRule<T> a, IParserRule b)
        => a.Equals(b);

    public static bool operator !=(TypeRule<T> a, IParserRule b)
        => !a.Equals(b);
}
