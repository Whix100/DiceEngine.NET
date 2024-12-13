using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions.Interfaces;

public interface IPostfixOperable
{
    Terminal PostfixOperate(string identifier, ExpressionContext context);

    public static Terminal Operate(string identifier, IExpression inside, ExpressionContext? context = null)
    {
        context ??= new ExpressionContext();

        if (inside is IPostfixOperable operable)
        {
            Terminal result = operable.PostfixOperate(identifier, context);

            return result;
        }
        else if (inside is IEnumerableExpression enumExpr)
        {
            return TerminalCollection.TerminateCollection(enumExpr.Map(e => Operate(identifier, e, context)));
        }

        return Undefined.UNDEFINED;
    }
}
