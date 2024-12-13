using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;
using System.Collections;
using System.Text;

namespace TestDiceEngine.TestModels;

internal class RandomCollection(int? minCount, int? maxCount, double? min, double? max,
    Func<IExpression, bool>? elementValidator = null, Func<IEnumerable<IExpression>, bool>? collectionValidator = null)
    : IEnumerable<IExpression>, IExpression
{
    public readonly int? MinCount = minCount;
    public readonly int? MaxCount = maxCount;
    public readonly double? Min = min;
    public readonly double? Max = max;
    public readonly Func<IExpression, bool>? ElementValidator = elementValidator;
    public readonly Func<IEnumerable<IExpression>, bool>? CollectionValidator = collectionValidator;

    public Terminal Evaluate()
        => throw new NotImplementedException();

    public Terminal Evaluate(ExpressionContext context)
        => throw new NotImplementedException();

    public IExpression StepEvaluate()
        => throw new NotImplementedException();

    public IExpression StepEvaluate(ExpressionContext context)
        => throw new NotImplementedException();

    public IExpression EvaluateDice()
        => throw new NotImplementedException();

    public IExpression EvaluateDice(ExpressionContext context)
        => throw new NotImplementedException();

    public IEnumerator<IExpression> GetEnumerator()
        => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator()
        => throw new NotImplementedException();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
    {
        StringBuilder builder = new StringBuilder();

        if (MinCount.HasValue)
            builder.Append($"MinCount: {MinCount}. ");

        if (MaxCount.HasValue)
            builder.Append($"MaxCount: {MaxCount}. ");

        if (Min.HasValue)
            builder.Append($"Min: {Min}. ");

        if (Max.HasValue)
            builder.Append($"Max: {Max}. ");

        if (ElementValidator is not null)
            builder.Append($"Has Element Validation. ");

        if (CollectionValidator is not null)
            builder.Append($"Has Collection Validation. ");

        if (builder.Length > 0)
            return $"RandomCollection: ({builder})";
        else
            return "RandomCollection";
    }

    public override int GetHashCode()
        => HashCode.Combine(MinCount, MaxCount, Min, Max, ElementValidator, CollectionValidator);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is RandomCollection collection && collection.MinCount == MinCount && collection.MaxCount == MaxCount &&
            collection.Min == Min && collection.Max == Max && collection.ElementValidator == ElementValidator &&
            collection.CollectionValidator == CollectionValidator)
            return true;

        if (obj is IEnumerable<IExpression> enumExpr && obj is IExpression expr)
        {
            IEnumerable<double> numbers = enumExpr
                .Where(x => x is Number)
                .Select(x => ((Number)x).Value)
                .ToArray();

            if ((!MinCount.HasValue || enumExpr.Count() >= MinCount) &&
                (!MaxCount.HasValue || enumExpr.Count() <= MaxCount) &&
                (!Min.HasValue || numbers.All(x => x >= Min.Value)) &&
                (!Max.HasValue || numbers.All(x => x <= Max.Value)))
                if (ElementValidator is null || enumExpr.All(ElementValidator))
                    if (CollectionValidator is null || CollectionValidator(enumExpr))
                        return true;
        }

        return false;
    }
}
