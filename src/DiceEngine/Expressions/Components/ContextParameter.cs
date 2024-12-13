using DiceEngine.Context;
using DiceEngine.FunctionAttributes;
using System.Diagnostics.CodeAnalysis;

namespace DiceEngine.Expressions.Components;

public struct ContextParameter(IEnumerable<FunctionAttribute> attributes) : IParameter
{
    public FunctionAttribute[] Attributes { get; } = attributes.ToArray();

    public bool AllowNull { get; } = false;

    public object? ProcessArgument(IExpression argument, ExpressionContext _)
    {
        throw new NotImplementedException();
    }

    public override int GetHashCode()
        => HashCode.Combine(Attributes);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is not null && obj is ContextParameter;
}
