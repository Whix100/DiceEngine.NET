using DiceEngine.Expressions;
using DiceEngine.Expressions.Interfaces;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.FunctionAttributes.PreprocessAttributes;

public class AsBooleanAttribute : PreprocessAttribute
{
    public override IExpression Preprocess(IExpression expression)
    {
        return (IExpression?)ILogicalConvertible.ConvertToLogical(expression) ?? Undefined.UNDEFINED;
    }
}
