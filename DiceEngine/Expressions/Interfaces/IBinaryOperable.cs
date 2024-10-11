using DiceEngine.Context;
using DiceEngine.Expressions.Collections;

namespace DiceEngine.Expressions.Interfaces;

public interface IBinaryOperable
{
    IExpression? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext context);

    IExpression? BinaryRightOperate(string identifier, IExpression left, ExpressionContext context);

    public static IExpression Operate(string identifier, IExpression left, IExpression right,
        ExpressionContext? context = null)
    {
        context ??= new ExpressionContext();
        left = left.Evaluate(context);
        right = right.Evaluate(context);

        if (left is IBinaryOperable leftOperable)
        {
            IExpression? result = leftOperable.BinaryLeftOperate(identifier, right, context);

            if (result is not null)
                return result;
        }

        if (right is IBinaryOperable rightOperable)
        {
            IExpression? result = rightOperable.BinaryRightOperate(identifier, left, context);

            if (result is not null)
                return result;
        }

        if (left is IEnumerableExpression leftEnumExpr)
        {
            if (right is IEnumerableExpression rightEnumExpr)
            {
                if (leftEnumExpr.Count() == rightEnumExpr.Count())
                {
                    return leftEnumExpr.Combine(rightEnumExpr, (x, y) =>
                        new BinaryOperator(identifier, x, y).Evaluate(context));
                }
            }
            else
            {
                return leftEnumExpr.Map(e => new BinaryOperator(identifier, e, right).Evaluate(context));
            }
        }
        else if (right is IEnumerableExpression rightEnumExpr)
        {
            return rightEnumExpr.Map(e => new BinaryOperator(identifier, left, e).Evaluate(context));
        }

        return Undefined.UNDEFINED;
    }
}
