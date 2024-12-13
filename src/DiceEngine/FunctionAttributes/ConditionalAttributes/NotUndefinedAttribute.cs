using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class NotUndefinedAttribute : ConditionAttribute
{
    public override bool CheckCondition(IExpression expression)
        => !Undefined.UNDEFINED.Equals(expression);
}
