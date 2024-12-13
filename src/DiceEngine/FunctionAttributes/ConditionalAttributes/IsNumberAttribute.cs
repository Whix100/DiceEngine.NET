using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class IsNumberAttribute : ConditionAttribute
{
    public override bool CheckCondition(IExpression expression)
        => expression is Number;
}
