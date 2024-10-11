using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class MinimumAttribute(double minimum, bool allowUndefined = false, bool inclusive = true) : ConditionAttribute
{
    public readonly IExpression Minimum = ((Number)minimum).Evaluate();
    public readonly bool AllowUndefined = allowUndefined;
    public readonly bool Inclusive = inclusive;

    public override bool CheckCondition(IExpression expression)
    {
        IExpression conditionResult = IBinaryOperable.Operate(Inclusive ? ">=" : ">", expression, Minimum);

        return conditionResult is Logical isHigher ? isHigher.Value : AllowUndefined;
    }
}
