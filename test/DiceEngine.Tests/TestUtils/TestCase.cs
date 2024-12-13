using DiceEngine.Expressions;
using DiceEngine.Tokenization.Tokens;
using System.Collections.Immutable;

namespace TestDiceEngine.TestUtils;

public readonly struct TestCase
{
    public readonly string ExpressionString;
    public readonly ImmutableArray<IToken> Tokenized;
    public readonly IExpression Parsed;
    public readonly IExpression Evaluated;
    public readonly IExpression[] StepEvaluated;

    public TestCase(string expressionString, IEnumerable<IToken> tokenized, IExpression parsed, IExpression? evaluated = null,
        params IExpression[] stepEvaluated)
    {
        ExpressionString = expressionString;
        Tokenized = [.. tokenized];
        Parsed = parsed;
        Evaluated = evaluated ?? Parsed;
        StepEvaluated = [..stepEvaluated, Evaluated];
    }
}
