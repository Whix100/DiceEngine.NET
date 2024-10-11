using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class MaximumAttribute(double maximum, bool allowUndefined = false, bool inclusive = true) : ConditionAttribute
{
    public readonly IExpression Maximum = ((Number)maximum).Evaluate();
    public readonly bool AllowUndefined = allowUndefined;
    public readonly bool Inclusive = inclusive;

    public override bool CheckCondition(IExpression expression)
    {
        IExpression conditionResult = IBinaryOperable.Operate(Inclusive ? "<=" : "<", expression, Maximum);

        return conditionResult is Logical isLower ? isLower.Value : AllowUndefined;
    }
}
