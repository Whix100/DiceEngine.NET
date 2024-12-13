using DiceEngine.Attributes;
using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.NativeFunctions;

public static class LogicalFunctions
{
    [NativeFunction("and")]
    [Elementwise]
    public static bool And(bool a, bool b)
        => a && b;

    [NativeFunction("or")]
    [Elementwise]
    public static bool Or(bool a, bool b)
        => a || b;

    [NativeFunction("xor")]
    [Elementwise]
    public static bool Xor(bool a, bool b)
        => (a || b) && !(a && b);

    [NativeFunction("not")]
    [Elementwise]
    public static bool Not(bool x)
        => !x;

    [NativeFunction("bool")]
    [Elementwise]
    public static bool Bool(bool x)
        => x;

    [NativeFunction("if")]
    [Elementwise]
    public static Terminal If(bool condition, Terminal isTrue, Terminal isFalse)
        => condition ? isTrue : isFalse;

    [NativeFunction("is_na")]
    public static bool IsNa(IExpression x, ExpressionContext _)
        => Undefined.UNDEFINED.Equals(x);

    [NativeFunction("is_num", "is_number")]
    public static bool IsNum(IExpression x, ExpressionContext _)
        => x is Number;

    [NativeFunction("is_int", "is_integer")]
    public static bool IsInt(IExpression x, ExpressionContext _)
        => x is Number num && num.Value % 1 == 0;

    [NativeFunction("is_logical")]
    public static bool IsLogical(IExpression x, ExpressionContext _)
        => Logical.TRUE.Equals(x) || Logical.FALSE.Equals(x);

    [NativeFunction("is_even")]
    public static bool? IsEven(double x)
        => Double.IsFinite(x)
            ? x % 2 == 0
            : null;

    [NativeFunction("is_odd")]
    public static bool? IsOdd(double x)
        => Double.IsFinite(x)
            ? Math.Abs(x) % 2 == 1
            : null;

    [NativeFunction("is_positive")]
    public static bool? IsPositive(double x)
        => x > 0
            ? true
            : !Double.IsNaN(x)
                ? false
                : null;

    [NativeFunction("is_negative")]
    public static bool? IsNegative(double x, ExpressionContext _)
        => x < 0
            ? true
            : !Double.IsNaN(x)
                ? false
                : null;
}
