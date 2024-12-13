using DiceEngine.Expressions;

namespace DiceEngine.FunctionAttributes.PreprocessAttributes;

public abstract class PreprocessAttribute : FunctionAttribute
{
    public abstract IExpression Preprocess(IExpression expression);
}
