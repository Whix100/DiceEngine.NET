using DiceEngine.Expressions.Dice;
using DiceEngine.Extensions;
using System.Collections.ObjectModel;

namespace DiceEngine.Expressions.Components;

/// <summary>
/// The selector for selecting results from a roll.
/// </summary>
/// <param name="selector">The symbol representing the method of selecting results.</param>
/// <param name="value">The value to use in the selection.</param>
public readonly struct ResultSelector(string selector, int value)
{
    private static readonly ReadOnlyDictionary<string, Func<int, Dictionary<int, RollValue>, IEnumerable<int>>> SELECTORS =
        new Dictionary<string, Func<int, Dictionary<int, RollValue>, IEnumerable<int>>>
        {
            { "", SelectLiteral },
            { "h", SelectHighest },
            { "l", SelectLowest },
            { "<", SelectLessThan },
            { "<=", SelectLessThanOrEqual },
            { ">", SelectGreaterThan },
            { ">=", SelectGreaterThanOrEqual },
        }.AsReadOnly();

    public readonly string Selector = selector;
    public readonly int Value = value;

    /// <summary>
    /// Creates a new instance of <see cref="ResultSelector"/> using the literal selector.
    /// </summary>
    /// <param name="value">The value to use in the selection.</param>
    public ResultSelector(int value) : this("", value) { }

    public int[] Select(IEnumerable<RollValue> result, bool excludeExploded = false)
    {
        if (SELECTORS.TryGetValue(Selector, out Func<int, Dictionary<int, RollValue>, IEnumerable<int>>? select))
            return select(Value, result.Select((r, i) => (i, r))
                    .Where(kvp => !kvp.r.IsDropped && (!excludeExploded || kvp.r.IsExploded))
                    .ToDictionary())
                .ToArray();

        return [];
    }

    public override int GetHashCode()
        => HashCode.Combine(Selector, Value);

    public override bool Equals(object? obj)
    {
        if (obj is ResultSelector resultSelector)
        {
            return Selector == resultSelector.Selector && Value == resultSelector.Value;
        }

        return false;
    }

    public override string? ToString()
        => $"{Selector}{Value}";

    private static IEnumerable<int> SelectLiteral(int value, Dictionary<int, RollValue> results)
        => results.KeysWhere((k, v) => v == value);

    private static IEnumerable<int> SelectHighest(int value, Dictionary<int, RollValue> results)
    {
        return results
            .OrderByDescending(pair => pair.Value)
            .Take(value)
            .Select(pair => pair.Key);
    }

    private static IEnumerable<int> SelectLowest(int value, Dictionary<int, RollValue> results)
    {
        return results
            .OrderBy(pair => pair.Value)
            .Take(value)
            .Select(pair => pair.Key);
    }

    private static IEnumerable<int> SelectLessThan(int value, Dictionary<int, RollValue> results)
        => results.KeysWhere((k, v) => v < value);

    private static IEnumerable<int> SelectLessThanOrEqual(int value, Dictionary<int, RollValue> results)
        => results.KeysWhere((k, v) => v <= value);

    private static IEnumerable<int> SelectGreaterThan(int value, Dictionary<int, RollValue> results)
        => results.KeysWhere((k, v) => v > value);

    private static IEnumerable<int> SelectGreaterThanOrEqual(int value, Dictionary<int, RollValue> results)
        => results.KeysWhere((k, v) => v >= value);
}
