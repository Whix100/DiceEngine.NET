using DiceEngine.Expressions;

namespace DiceEngine.FunctionAttributes.PreprocessAttributes;

public class AsNumberAttribute : PreprocessAttribute
{
    public override IExpression Preprocess(IExpression expression)
        => expression is Number
            ? expression
            : Logical.TRUE.Equals(expression) || Logical.FALSE.Equals(expression)
                ? expression.Evaluate()
                : Undefined.UNDEFINED;
}
