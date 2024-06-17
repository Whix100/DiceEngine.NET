﻿using DiceEngine.Context;
using System.Collections;
using System.Numerics;

namespace DiceEngine.Expressions.Dice;

public class RollResult(IEnumerable<RollValue> results, IDie die) : Number(double.NaN), IEnumerable<RollValue>
{
    private readonly RollValue[] _results = results.ToArray();

    public readonly IDie Die = die;

    public new int Value => _results.Where(r => !r.IsDropped).Select(r => (int)r).Sum();

    public RollValue this[int index]
        => _results[index];

    public RollValue[] this[Range range]
        => _results[range];

    public int Length
        => _results.Length;

    public RollResult(RollValue result, IDie die) : this(new RollValue[] { result }, die)
    {
    }

    public RollResult(int result, IDie die) : this((RollValue)result, die)
    {
    }

    public RollResult(IEnumerable<int> results, IDie die) : this(results.Select(x => (RollValue)x), die)
    {
    }

    public new IExpression Evaluate(ExpressionContext _)
        => (Number)Value;

    public new IExpression StepEvaluate(ExpressionContext _)
        => (Number)Value;

    public override int GetHashCode()
        => HashCode.Combine(Die, _results);

    public override bool Equals(object? obj)
        => obj is RollResult result && Die.Equals(result.Die) && _results.SequenceEqual(result._results);

    public override string ToString()
        => ToString(null);

    public new string ToString(string? format)
        => $"{Die} ({String.Join(", ", _results.Select(r => r.ToString()))})";

    public IEnumerator<RollValue> GetEnumerator()
        => ((IEnumerable<RollValue>)_results).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _results.GetEnumerator();
}

public readonly struct RollValue(int value, bool isFocused, bool isDropped, bool isExploded)
    : IComparisonOperators<RollValue, RollValue, bool>, IEquatable<RollValue>
{
    public readonly int Value = value;
    public readonly bool IsFocused = isFocused;
    public readonly bool IsDropped = isDropped;
    public readonly bool IsExploded = isExploded;

    public RollValue Drop()
        => new RollValue(Value, IsFocused, true, IsExploded);

    public RollValue Explode()
        => new RollValue(Value, IsFocused, IsDropped, true);

    public override string ToString()
    {
        string isDropped = IsDropped ? "~~" : "";
        string isFocused = IsFocused ? "**" : "";
        string isExploded = IsExploded ? "!" : "";

        return $"{isDropped}{isFocused}{Value}{isExploded}{isFocused}{isDropped}";
    }

    public override int GetHashCode()
        => Value.GetHashCode();

    public override bool Equals(object? obj)
        => obj is RollValue value && Equals(value);
    
    public bool Equals(RollValue value)
        => value.Value == Value && value.IsFocused == IsFocused && value.IsDropped == IsDropped &&
            value.IsExploded == IsExploded;

    public static bool operator >(RollValue left, RollValue right)
        => left.Value > right.Value;

    public static bool operator <(RollValue left, RollValue right)
        => left.Value < right.Value;

    public static bool operator >=(RollValue left, RollValue right)
        => left.Value >= right.Value;

    public static bool operator <=(RollValue left, RollValue right)
        => left.Value <= right.Value;

    public static bool operator ==(RollValue left, RollValue right)
        => left.Value == right.Value;

    public static bool operator !=(RollValue left, RollValue right)
        => left.Value != right.Value;

    public static implicit operator int(RollValue value)
        => value.Value;

    public static explicit operator RollValue(int value)
        => new RollValue(value, false, false, false);
}
