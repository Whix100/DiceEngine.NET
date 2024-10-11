using DiceEngine.Context;
using DiceEngine.Exceptions;
using DiceEngine.Expressions.Components;
using System.Collections.ObjectModel;

namespace DiceEngine.Expressions.Dice;

/// <summary>
/// Initializes a new instance of the <see cref="DiceOperator"/> class.
/// </summary>
/// <param name="op">The identifier for the operator.</param>
/// <param name="inside">The expression inside the operator.</param>
/// <param name="selector">The selector for the operator.</param>
public class DiceOperator(string op, IExpression inside, ResultSelector selector) : IDie
{
    private static readonly ReadOnlyDictionary<string, Func<RollResult, ResultSelector, Random, IEnumerable<RollValue>>> OPERATORS =
        new Dictionary<string, Func<RollResult, ResultSelector, Random, IEnumerable<RollValue>>>
        {
            { "k",  Keep },
            { "d",  Drop },
            { "rr",  Reroll },
            { "r",  Reroll },
            { "ro", RerollOnce },
            { "ra", RerollAndAdd },
            { "e",  Explode },
            { "mi", Minimum },
            { "ma", Maximum },
        }.AsReadOnly();

    public readonly string Identifier = op;
    public readonly IExpression Inside = inside;
    public readonly ResultSelector Selector = selector;

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
    {
        IExpression result = Inside.Evaluate(context);

        if (result is RollResult rollResult &&
            OPERATORS.TryGetValue(Identifier, out Func<RollResult, ResultSelector, Random, IEnumerable<RollValue>>? operate) &&
            !((Identifier == "ma" || Identifier == "mi") && Selector.Selector != ""))
            return new RollResult(operate(rollResult, Selector, context.Random), rollResult.Die);

        return Undefined.UNDEFINED;
    }

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        if (Inside is RollResult rollResult &&
            OPERATORS.TryGetValue(Identifier, out Func<RollResult, ResultSelector, Random, IEnumerable<RollValue>>? operate) &&
            !((Identifier == "ma" || Identifier == "mi") && Selector.Selector != ""))
            return new RollResult(operate(rollResult, Selector, context.Random), rollResult.Die);

        IExpression result = Inside.StepEvaluate(context);

        return new DiceOperator(Identifier, result, Selector);
    }

    public int Roll(Random? random = null)
    {
        if (Inside is RollResult rollResult)
            return rollResult.Die.Roll(random);
        else if (Inside is IDie die)
            return die.Roll(random);
        else
            throw new Exception("Cannot roll a non-die expression.");
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => Evaluate(context);

    public override int GetHashCode()
        => HashCode.Combine(Identifier, Inside, Selector);

    public override bool Equals(object? obj)
        => obj is DiceOperator diceOperator && diceOperator.Identifier == Identifier &&
            Inside.Equals(diceOperator.Inside) && diceOperator.Selector.Equals(Selector);

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{Inside.ToString(format)}{Identifier}{Selector}";

    private static IEnumerable<RollValue> Keep(RollResult result, ResultSelector selector, Random _)
    {
        int[] selected = selector.Select(result);

        return result.Select((r, i) => selected.Contains(i) ? r : r.Drop());
    }

    private static IEnumerable<RollValue> Drop(RollResult result, ResultSelector selector, Random _)
    {
        int[] selected = selector.Select(result);

        return result.Select((r, i) => selected.Contains(i) ? r.Drop() : r);
    }

    private static IEnumerable<RollValue> Reroll(RollResult result, ResultSelector selector, Random random)
    {
        IEnumerable<RollValue> values = result;
        
        for (int[] selected = selector.Select(values); selected.Length > 0; selected = selector.Select(values))
        {
            if (values.Count() >= TooManyDiceRollsException.MAX_ROLLS)
                throw new TooManyDiceRollsException();

            values = values.SelectMany<RollValue, RollValue>(
                (r, i) => selected.Contains(i) ? [r.Drop(), (RollValue)result.Die.Roll(random)] : [r]);

            if (selector.Selector == "l" || selector.Selector == "h")
                break;
        }

        return values;
    }

    private static IEnumerable<RollValue> RerollOnce(RollResult result, ResultSelector selector, Random random)
    {
        int[] selected = selector.Select(result);
        
        return result.SelectMany<RollValue, RollValue>(
            (r, i) => selected.Contains(i) ? [r.Drop(), (RollValue)result.Die.Roll(random)] : [r]);
    }

    private static IEnumerable<RollValue> RerollAndAdd(RollResult result, ResultSelector selector, Random random)
    {
        int[] selected = selector.Select(result);

        return selected.Length == 0
            ? result
            : result.SelectMany<RollValue, RollValue>(
                (r, i) => selected[0] == i ? [r.Drop(), (RollValue)result.Die.Roll(random)] : [r]);
    }

    private static IEnumerable<RollValue> Explode(RollResult result, ResultSelector selector, Random random)
    {
        IEnumerable<RollValue> values = result;
        
        for (int[] selected = selector.Select(values, true); selected.Length > 0; selected = selector.Select(values))
        {
            if (values.Count() >= TooManyDiceRollsException.MAX_ROLLS)
                throw new TooManyDiceRollsException();

            values = values.SelectMany<RollValue, RollValue>(
                (r, i) => selected.Contains(i) ? [r.Explode(), (RollValue)result.Die.Roll(random)] : [r]);

            if (selector.Selector == "l" || selector.Selector == "h")
                break;
        }

        return values;
    }

    private static IEnumerable<RollValue> Minimum(RollResult result, ResultSelector selector, Random _)
    {
        return result.SelectMany<RollValue, RollValue>(
            (r, i) => r < selector.Value ? [r.Drop(), (RollValue)selector.Value] : [r]);
    }

    private static IEnumerable<RollValue> Maximum(RollResult result, ResultSelector selector, Random _)
    {
        return result.SelectMany<RollValue, RollValue>(
            (r, i) => r < selector.Value ? [r.Drop(), (RollValue)selector.Value] : [r]);
    }
}
