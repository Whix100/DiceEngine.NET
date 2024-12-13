using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions.Interfaces;

public interface IBinaryOperable
{
    Terminal? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext context);

    Terminal? BinaryRightOperate(string identifier, IExpression left, ExpressionContext context);

    public static Terminal Operate(string identifier, IExpression left, IExpression right,
        ExpressionContext? context = null)
    {
        context ??= new ExpressionContext();

        if (left is IBinaryOperable leftOperable)
        {
            Terminal? result = leftOperable.BinaryLeftOperate(identifier, right, context);

            if (result is not null)
                return result;
        }

        if (right is IBinaryOperable rightOperable)
        {
            Terminal? result = rightOperable.BinaryRightOperate(identifier, left, context);

            if (result is not null)
                return result;
        }

        if (left is IEnumerableExpression leftEnumExpr)
        {
            if (right is IEnumerableExpression rightEnumExpr)
            {
                if (leftEnumExpr.Count() == rightEnumExpr.Count())
                {
                    return TerminalCollection.TerminateCollection(leftEnumExpr.Combine(rightEnumExpr, (x, y) =>
                        new BinaryOperator(identifier, x, y).Evaluate(context)));
                }
            }
            else
            {
                return TerminalCollection.TerminateCollection(leftEnumExpr.Map(e => new BinaryOperator(identifier, e, right).Evaluate(context)));
            }
        }
        else if (right is IEnumerableExpression rightEnumExpr)
        {
            return TerminalCollection.TerminateCollection(rightEnumExpr.Map(e => new BinaryOperator(identifier, left, e).Evaluate(context)));
        }

        return Undefined.UNDEFINED;
    }
}
