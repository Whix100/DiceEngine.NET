using DiceEngine.Attributes;
using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.FunctionAttributes.ConditionalAttributes;
using DiceEngine.FunctionAttributes.PreprocessAttributes;

namespace DiceEngine.BuiltInFunctions;

public static class LogicalFunctions
{
    [BuiltInFunction("and")]
    [Elementwise]
    public static bool And(bool a, bool b)
        => a && b;

    [BuiltInFunction("or")]
    [Elementwise]
    public static bool Or(bool a, bool b)
        => a || b;

    [BuiltInFunction("xor")]
    [Elementwise]
    public static bool Xor(bool a, bool b)
        => (a || b) && !(a && b);

    [BuiltInFunction("not")]
    [Elementwise]
    public static bool Not(bool x)
        => !x;

    [BuiltInFunction("bool")]
    [Elementwise]
    public static bool Bool(bool x)
        => x;

    [BuiltInFunction("if")]
    [Elementwise]
    public static IExpression If(bool condition, IExpression is_true, IExpression is_false)
        => condition ? is_true : is_false;

    [BuiltInFunction("is_na")]
    public static bool IsNa(IExpression x, ExpressionContext _)
        => Undefined.UNDEFINED.Equals(x);

    [BuiltInFunction("is_num", "is_number")]
    public static bool IsNum(IExpression x, ExpressionContext _)
        => x is Number;

    [BuiltInFunction("is_int", "is_integer")]
    public static bool IsInt(IExpression x, ExpressionContext _)
        => x is Number num && num.Value % 1 == 0;

    [BuiltInFunction("is_logical")]
    public static bool IsLogical(IExpression x, ExpressionContext _)
        => Logical.TRUE.Equals(x) || Logical.FALSE.Equals(x);

    [BuiltInFunction("is_even")]
    public static bool? IsEven(double x)
        => Double.IsFinite(x)
            ? x % 2 == 0
            : null;

    [BuiltInFunction("is_odd")]
    public static bool? IsOdd(double x)
        => Double.IsFinite(x)
            ? Math.Abs(x) % 2 == 1
            : null;

    [BuiltInFunction("is_positive")]
    public static bool? IsPositive(double x)
        => x > 0
            ? true
            : !Double.IsNaN(x)
                ? false
                : null;

    [BuiltInFunction("is_negative")]
    public static bool? IsNegative(double x, ExpressionContext _)
        => x < 0
            ? true
            : !Double.IsNaN(x)
                ? false
                : null;
}
