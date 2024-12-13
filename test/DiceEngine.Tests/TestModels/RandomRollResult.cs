using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Dice;
using DiceEngine.Expressions.Terminals;
using System.Collections;
using System.Text;

namespace TestDiceEngine.TestModels;

internal class RandomRollResult(int? minCount, int? maxCount, RollValue? min, RollValue? max,
    Func<RollValue, bool>? elementValidator = null, Func<IEnumerable<RollValue>, bool>? enumerableValidator = null)
    : IEnumerable<RollValue>, IExpression
{
    public readonly int? MinCount = minCount;
    public readonly int? MaxCount = maxCount;
    public readonly RollValue? Min = min;
    public readonly RollValue? Max = max;
    public readonly Func<RollValue, bool>? ElementValidator = elementValidator;
    public readonly Func<IEnumerable<RollValue>, bool>? EnumerableValidator = enumerableValidator;

    public RandomRollResult(int? minCount, int? maxCount, int? min, int? max,
        Func<RollValue, bool>? elementValidator = null,
        Func<IEnumerable<RollValue>, bool>? enumerableValidator = null)
        : this(minCount, maxCount, (RollValue?)min, (RollValue?)max, elementValidator, enumerableValidator)
    {
    }

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

    public IEnumerator<RollValue> GetEnumerator()
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

        if (EnumerableValidator is not null)
            builder.Append($"Has Enumerable Validation. ");

        if (builder.Length > 0)
            return $"{GetType().Name}: ({builder})";
        else
            return GetType().Name;
    }

    public override int GetHashCode()
        => HashCode.Combine(MinCount, MaxCount, Min, Max, ElementValidator, EnumerableValidator);

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is RandomRollResult randResult && randResult.MinCount == MinCount && randResult.MaxCount == MaxCount &&
            randResult.Min.Equals(Min) && randResult.Max.Equals(Max) &&
            randResult.ElementValidator == ElementValidator && randResult.EnumerableValidator == EnumerableValidator)
            return true;

        if (obj is not IEnumerable<RollValue> enumerable)
            return false;

        if ((MinCount.HasValue && enumerable.Count(x => !x.IsDropped) < MinCount) ||
            (MaxCount.HasValue && enumerable.Count(x => !x.IsDropped) > MaxCount))
            return false;

        if ((Min.HasValue && enumerable.Any(x => !x.IsDropped && x < Min.Value)) ||
            (Max.HasValue && enumerable.Any(x => !x.IsDropped && x > Max.Value)))
            return false;

        if (ElementValidator is not null && !enumerable.All(ElementValidator))
            return false;

        if (EnumerableValidator is not null && !EnumerableValidator(enumerable))
            return false;

        return true;
    }
}
