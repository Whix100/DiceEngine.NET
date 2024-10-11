using DiceEngine.Expressions;

namespace TestDiceEngine.TestUtils;

public static class TestValues
{
    public static readonly Undefined UNDEFINED = Undefined.UNDEFINED;
    public static readonly Infinity INFINITY = Infinity.POSITIVE;
    public static readonly Infinity NEGATIVE_INFINITY = Infinity.NEGATIVE;
    public static readonly Number ZERO = new Number(0);
    public static readonly Number ONE = new Number(1);
    public static readonly Number NEGATIVE_ONE = new Number(-1);
    public static readonly Number INTEGER = new Number(42);
    public static readonly Number NEGATIVE_INTEGER = new Number(-42);
    public static readonly Number SMALL_DECIMAL = new Number(0.2468);
    public static readonly Number NEGATIVE_SMALL_DECIMAL = new Number(-0.2468);
    public static readonly Number DECIMAL = new Number(42.42);
    public static readonly Number NEGATIVE_DECIMAL = new Number(-42.42);
    public static readonly Number PI = new Number(Math.PI);
    public static readonly Number NEGATIVE_PI = new Number(-Math.PI);
    public static readonly Number PI_OVER_FOUR = new Number(Math.PI / 4);
    public static readonly Number NEGATIVE_PI_OVER_FOUR = new Number(-Math.PI / 4);
    public static readonly Number PI_OVER_TWO = new Number(Math.PI / 2);
    public static readonly Number NEGATIVE_PI_OVER_TWO = new Number(-Math.PI / 2);
    public static readonly Number TAU = new Number(Math.Tau);
    public static readonly Number NEGATIVE_TAU = new Number(-Math.Tau);
    public static readonly Number THREE_PI_OVER_TWO = new Number(3 * Math.PI / 2);
    public static readonly Number NEGATIVE_THREE_PI_OVER_TWO = new Number(-3 * Math.PI / 2);
    public static readonly Number THREE_PI_OVER_FOUR = new Number(3 * Math.PI / 4);
    public static readonly Number NEGATIVE_THREE_PI_OVER_FOUR = new Number(-3 * Math.PI / 4);

    public static IExpression F(IExpression x)
        => x;

    public static IExpression G(IExpression m, IExpression x, IExpression b)
        => new BinaryOperator("+", new BinaryOperator("*", m, x), b).Evaluate();

    public static IExpression P(IExpression x)
        => x is Number n ? new Number(n.Value + 1) : Undefined.UNDEFINED;

    public static IExpression SubFromTen(IExpression x)
        => new BinaryOperator("-", new Number(10), x).Evaluate();

    public static IExpression IsEven(IExpression x)
        => x is Number n ? new Number(n.Value % 2 == 0 ? 1 : 0) : Undefined.UNDEFINED;

    public static IExpression Aggregate(IExpression a, IExpression b)
        => new BinaryOperator("+", a, b).Evaluate();
}
