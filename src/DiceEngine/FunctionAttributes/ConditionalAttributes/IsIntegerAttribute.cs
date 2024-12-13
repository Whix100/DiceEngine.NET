using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class IsIntegerAttribute : ConditionAttribute
{
    public override bool CheckCondition(IExpression expression)
        => expression is Number num && num.Value % 0 == 0;
}
