using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class GapAttribute(double start, double end, bool allowUndefined = false, bool inclusive = false)
    : ConditionAttribute
{
    public readonly IExpression Start = ((Number)start).Evaluate();
    public readonly IExpression End = ((Number)end).Evaluate();
    public readonly bool AllowUndefined = allowUndefined;
    public readonly bool Inclusive = inclusive;

    public override bool CheckCondition(IExpression expression)
    {
        IExpression lowerConditionResult = IBinaryOperable.Operate(Inclusive ? "<" : "<=", expression, Start);

        if (lowerConditionResult is Logical isLower)
        {
            if (isLower.Value)
            {
                return true;
            }
        }
        else if (!AllowUndefined)
        {
            return false;
        }

        IExpression upperConditionResult = IBinaryOperable.Operate(Inclusive ? ">" : ">=", expression, End);

        return upperConditionResult is Logical isHigher ? isHigher.Value : AllowUndefined;
    }
}
