using DiceEngine.Context;
using DiceEngine.Expressions;
using System.Text;

namespace TestDiceEngine.TestModels;

internal class RandomValue(double? min, double? max,
    Func<IExpression, bool>? validate = null)
    : IExpression
{
    public readonly double? Min = min;
    public readonly double? Max = max;
    public readonly Func<IExpression, bool>? Validate = validate;

    public IExpression Evaluate()
        => throw new NotImplementedException();

    public IExpression Evaluate(ExpressionContext context)
        => throw new NotImplementedException();

    public IExpression StepEvaluate()
        => throw new NotImplementedException();

    public IExpression StepEvaluate(ExpressionContext context)
        => throw new NotImplementedException();

    public IExpression EvaluateDice()
        => throw new NotImplementedException();

    public IExpression EvaluateDice(ExpressionContext context)
        => throw new NotImplementedException();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
    {
        StringBuilder builder = new StringBuilder();

        if (Min.HasValue)
            builder.Append($"Min: {Min}. ");

        if (Max.HasValue)
            builder.Append($"Max: {Max}. ");

        if (Validate is not null)
            builder.Append($"Has Validation. ");

        if (builder.Length > 0)
            return $"RandomValue: ({builder})";
        else
            return "RandomValue";
    }

    public override int GetHashCode()
        => HashCode.Combine(Min, Max, Validate);

    public override bool Equals(object? obj)
        => obj is not null &&
        ((obj is RandomValue value && value.Min == Min && value.Max == Max && value.Validate == Validate) ||
        (obj is IExpression expr &&
            (!Min.HasValue || !Logical.FALSE.Equals(new BinaryOperator(">=", expr, (Number)Min.Value).Evaluate())) &&
            (!Max.HasValue || !Logical.FALSE.Equals(new BinaryOperator("<=", expr, (Number)Max.Value).Evaluate())) &&
            (Validate is null || Validate(expr))));
}
