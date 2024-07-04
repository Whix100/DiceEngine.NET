﻿using DiceEngine.Context;

namespace DiceEngine.Expressions.Dice;

public class FateDie : IDie
{
    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
        => new RollResult(Roll(context.Random), this);

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
        => Evaluate(context);

    public int Roll(Random? random = null)
        => (random ?? new Random()).Next(3) - 1;

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => Evaluate(context);

    public override int GetHashCode()
        => 0;

    public override bool Equals(object? obj)
        => obj is FateDie;

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => "dF";
}
