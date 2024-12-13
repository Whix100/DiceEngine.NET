using DiceEngine.Expressions;

namespace DiceEngine.FunctionAttributes.ConditionalAttributes;

public class IsExpressionTypeAttribute(Type type) : ConditionAttribute
{
    public readonly Type ParameterType = type;

    public override bool CheckCondition(IExpression expression)
        => ParameterType.IsAssignableFrom(expression.GetType());
}
