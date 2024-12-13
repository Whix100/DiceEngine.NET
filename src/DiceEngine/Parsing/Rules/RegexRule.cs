using DiceEngine.Expressions;
using DiceEngine.Tokenization.Tokens;
using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace DiceEngine.Parsing.Rules;

/// <summary>
/// A parser rule that matches a sequence of tokens using a regular expression.
/// </summary>
/// <param name="name">The name of the rule.</param>
/// <param name="regex">The regular expression to match a sequence of tokens.</param>
/// <param name="parse">The function to parse the matched token.</param>
/// <param name="options">The regex options to use to match.</param>
public class RegexRule(string name, [StringSyntax(StringSyntaxAttribute.Regex)] string regex,
    Func<ImmutableArray<IToken>, TokenMatch, Parser, IExpression> parse, RegexOptions options = RegexOptions.None)
    : IParserRule
{
    private readonly Regex _regex = new Regex(regex, options);
    private readonly Func<ImmutableArray<IToken>, TokenMatch, Parser, IExpression> _parse = parse;

    public string Name { get; } = name;

    public string Regex { get; } = regex;

    public RegexOptions Options { get; } = options;

    public TokenMatch? Match(ImmutableArray<IToken> input, IEnumerable<IParserRule> _)
    {
        string inputString = string.Join("", input.Select(x => x.RegexAlias));
        Match match = _regex.Match(inputString);

        if (match.Success)
            return new TokenMatch(input[match.Index..(match.Index + match.Length)], match.Index);

        return null;
    }

    public IExpression? Parse(ImmutableArray<IToken> input, Parser parser)
    {
        TokenMatch? match = Match(input, parser.Grammar);

        if (match is not null)
            return Parse(input, match, parser);

        return null;
    }

    public IExpression? Parse(ImmutableArray<IToken> input, TokenMatch match, Parser parser)
        => _parse(input, match, parser);

    public override bool Equals(object? obj)
        => obj is RegexRule r && r._parse == _parse && r.Regex == Regex && r.Options == Options;

    public override int GetHashCode()
        => HashCode.Combine(Name, _parse, Regex, Options);

    public static bool operator ==(RegexRule a, RegexRule b)
        => a.Equals(b);

    public static bool operator !=(RegexRule a, RegexRule b)
        => !a.Equals(b);
}
