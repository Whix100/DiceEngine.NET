using DiceEngine.Expressions;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class NotUndefinedAttribute : ConditionAttribute
{
    public override bool CheckCondition(IExpression expression)
        => !Undefined.UNDEFINED.Equals(expression);
}
