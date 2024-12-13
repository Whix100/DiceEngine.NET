using DiceEngine.Expressions;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public abstract class ConditionAttribute : FunctionAttribute
{
    public abstract bool CheckCondition(IExpression expression);
}
