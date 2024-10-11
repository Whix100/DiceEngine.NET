using DiceEngine.Attributes;
using DiceEngine.Expressions;
using System.Text.RegularExpressions;

namespace DiceEngine.Parsing.Rules;

/// <summary>
/// A rule to be used as a reference for a <see cref="NestedRegexRule"/>.
/// </summary>
/// <param name="name">The name of the <see cref="ReferenceRegexRule"/>.</param>
/// <param name="regex">The regex <see cref="string"/> to match an expression <see cref="string"/> to.</param>
/// <param name="options">
/// The options for the regular expression along with additional options finding a match.
/// </param>
[ReferenceRule]
public class ReferenceRegexRule(string name, string regex, RegexRuleOptions options = RegexRuleOptions.None)
    : NestedRegexRule(name, regex, options, (_, _, _) => Undefined.UNDEFINED)
{
    /// <summary>
    /// A rule to be used as a reference for a <see cref="NestedRegexRule"/>.
    /// </summary>
    /// <param name="name">The name of the <see cref="ReferenceRegexRule"/>.</param>
    /// <param name="regex">The regex <see cref="string"/> to match an expression <see cref="string"/> to.</param>
    /// <param name="options">
    /// The options for the regular expression along with additional options finding a match.
    /// </param>
    public ReferenceRegexRule(string name, string regex, RegexOptions options)
        : this(name, regex, (RegexRuleOptions)options)
    { }

    public override bool Equals(object? obj)
        => obj is not null && obj is RegexRule a && RegularExpression == a.RegularExpression;

    public override int GetHashCode()
        => RegularExpressionTemplate.GetHashCode();

    public static bool operator ==(ReferenceRegexRule a, ReferenceRegexRule b)
        => a.Equals(b);

    public static bool operator !=(ReferenceRegexRule a, ReferenceRegexRule b)
        => !a.Equals(b);
}
