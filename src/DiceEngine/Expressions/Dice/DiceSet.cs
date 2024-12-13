using DiceEngine.Context;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions.Dice;

/// <summary>
/// Initializes a new instance of the <see cref="DiceSet"/> class.
/// </summary>
/// <param name="size">The number of dice to roll.</param>
/// <param name="die">The die to roll.</param>
public class DiceSet(int size, IDie die) : IDie
{
    public readonly int Size = size;
    public readonly IDie Die = die;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiceSet"/> class.
    /// </summary>
    /// <param name="size">The number of dice to roll.</param>
    /// <param name="die">The number of sides on the <see cref="Dice.Die"/>.</param>
    public DiceSet(int size, int die) : this(size, new Die(die)) { }

    public Terminal Evaluate()
        => Evaluate(new ExpressionContext());

    public Terminal Evaluate(ExpressionContext context)
        => new RollResult(Enumerable.Range(1, Size).Select(x => Roll(context.Random)), this);

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
        => Evaluate(context);

    public int Roll(Random? random = null)
        => Die.Roll(random);

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => Evaluate(context);

    public override int GetHashCode()
        => Size;

    public override bool Equals(object? obj)
        => obj is DiceSet diceSet && diceSet.Size == Size && diceSet.Die.Equals(Die);

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{Size}{Die.ToString(format)}";
}
