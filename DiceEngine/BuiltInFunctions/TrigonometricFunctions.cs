using DiceEngine.Attributes;
using DiceEngine.FunctionAttributes.ConditionalAttributes;

namespace DiceEngine.NativeFunctions;

public static class TrigonometricFunctions
{
    [NativeFunction("sin")]
    [Elementwise]
    public static double Sin(double x)
        => Math.Sin(x);

    [NativeFunction("cos")]
    [Elementwise]
    public static double Cos(double x)
        => Math.Cos(x);

    [NativeFunction("tan")]
    [Elementwise]
    public static double Tan(double x)
    {        
        return (x / Math.PI - 0.5) % 1 == 0
            ? Double.NaN
            : Math.Tan(x);
    }

    [NativeFunction("asin", "arcsin", "arsin")]
    [Elementwise]
    public static double Asin(double x)
        => Math.Asin(x);

    [NativeFunction("acos", "arccos", "arcos")]
    [Elementwise]
    public static double Acos(double x)
        => Math.Acos(x);

    [NativeFunction("atan", "arctan", "artan")]
    [Elementwise]
    public static double Atan(double x)
        => Double.IsFinite(x)
            ? Math.Atan(x)
            : Double.IsPositiveInfinity(x)
                ? Math.PI / 2
                : Double.IsNegativeInfinity(x)
                    ? -Math.PI / 2
                    : Double.NaN;

    [NativeFunction("csc")]
    [Elementwise]
    public static double Csc(double x)
    {
        return x / Math.PI % 1 == 0
            ? Double.NaN
            : 1 / Math.Sin(x);
    }

    [NativeFunction("sec")]
    [Elementwise]
    public static double Sec(double x)
    {
        return (x / Math.PI - 0.5) % 1 == 0
            ? Double.NaN
            : 1 / Math.Cos(x);
    }

    [NativeFunction("cot")]
    [Elementwise]
    public static double Cot(double x)
    {
        return x / Math.PI % 1 == 0
            ? Double.NaN
            : 1 / Math.Tan(x);
    }

    [NativeFunction("acsc", "arccsc", "arcsc")]
    [Elementwise]
    public static double Acsc([Gap(-1, 1)] double x)
        => Double.IsFinite(x)
            ? Math.Asin(1 / x)
            : Double.IsInfinity(x)
                ? 0
                : Double.NaN;

    [NativeFunction("asec", "arcsec", "arsec")]
    [Elementwise]
    public static double Asec([Gap(-1, 1)] double x)
        => Double.IsFinite(x)
            ? Math.Acos(1 / x)
            : Double.IsInfinity(x)
                ? Math.PI / 2
                : Double.NaN;

    [NativeFunction("acot", "arccot", "arcot")]
    [Elementwise]
    public static double Acot(double x)
        => Double.IsFinite(x)
            ? Math.PI / 2 - Math.Atan(x)
            : Double.IsPositiveInfinity(x)
                ? 0
                : Double.IsNegativeInfinity(x)
                    ? Math.PI
                    : Double.NaN;

    [NativeFunction("sinh")]
    [Elementwise]
    public static double Sinh(double x)
        => Double.IsFinite(x)
            ? Math.Sinh(x)
            : Double.IsInfinity(x)
                ? x
                : Double.NaN;

    [NativeFunction("cosh")]
    [Elementwise]
    public static double Cosh(double x)
        => Double.IsFinite(x)
            ? Math.Cosh(x)
            : Double.IsInfinity(x)
                ? Double.PositiveInfinity
                : Double.NaN;

    [NativeFunction("tanh")]
    [Elementwise]
    public static double Tanh(double x)
        => Double.IsFinite(x)
            ? Math.Tanh(x)
            : Double.IsPositiveInfinity(x)
                ? 1
                : Double.IsNegativeInfinity(x)
                    ? -1
                    : Double.NaN;

    [NativeFunction("asinh", "arcsinh", "arsinh")]
    [Elementwise]
    public static double Asinh(double x)
        => Double.IsFinite(x)
            ? Math.Asinh(x)
            : Double.IsPositiveInfinity(x)
                ? Double.PositiveInfinity
                : Double.IsNegativeInfinity(x)
                    ? Double.NegativeInfinity
                    : Double.NaN;

    [NativeFunction("acosh", "arccosh", "arcosh")]
    [Elementwise]
    public static double Acosh([Minimum(1)] double x)
        => Double.IsFinite(x)
            ? Math.Acosh(x)
            : Double.IsPositiveInfinity(x)
                ? Double.PositiveInfinity
                : Double.NaN;

    [NativeFunction("atanh", "arctanh", "artanh")]
    [Elementwise]
    public static double Atanh(double x)
        => Math.Atanh(x);

    [NativeFunction("csch")]
    [Elementwise]
    public static double Csch([Gap(0, 0, inclusive: true)] double x)
        => Double.IsFinite(x)
            ? 1 / Math.Sinh(x)
            : Double.IsInfinity(x)
                ? 0
                : Double.NaN;

    [NativeFunction("sech")]
    [Elementwise]
    public static double Sech(double x)
        => Double.IsFinite(x)
            ? 1 / Math.Cosh(x)
            : Double.IsInfinity(x)
                ? 0
                : Double.NaN;

    [NativeFunction("coth")]
    [Elementwise]
    public static double Coth([Gap(0, 0, inclusive: true)] double x)
        => Double.IsFinite(x)
            ? Math.Cosh(x) / Math.Sinh(x)
            : Double.IsPositiveInfinity(x)
                ? 1
                : Double.IsNegativeInfinity(x)
                    ? -1
                    : Double.NaN;

    [NativeFunction("acsch", "arccsch", "arcsch")]
    [Elementwise]
    public static double Acsch([Gap(0, 0, inclusive: true)] double x)
        => Double.IsFinite(x)
            ? Asinh(1 / x)
            : Double.IsInfinity(x)
                ? 0
                : Double.NaN;

    [NativeFunction("asech", "arcsech", "arsech")]
    [Elementwise]
    public static double Asech([Range(0, 1)] double x)
        => Acosh(1 / x);

    [NativeFunction("acoth", "arccoth", "arcoth")]
    [Elementwise]
    public static double Acoth([Gap(-1, 1)] double x)
        => Double.IsFinite(x)
            ? x switch
            {
                1 => Double.PositiveInfinity,
                -1 => Double.NegativeInfinity,
                _ => Atanh(1 / x)
            }
            : Double.IsInfinity(x)
                ? 0
                : Double.NaN;
}
