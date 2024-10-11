using DiceEngine.Attributes;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.FunctionAttributes.ConditionalAttributes;

namespace DiceEngine.BuiltInFunctions;

public static class StatisticalFunctions
{
    [BuiltInFunction("count", "length", "len")]
    public static int Count(IExpression expressions)
    {
        return expressions is IEnumerableExpression enum_expr
            ? enum_expr.Count()
            : 1;
    }

    [BuiltInFunction("max", "maximum")]
    public static IExpression Max([AreNumbers] IExpression expressions)
    {
        return expressions is IEnumerableExpression enum_expr
            ? enum_expr.MaxBy(x => ((Number)x).Value) ?? Undefined.UNDEFINED
            : expressions;
    }

    [BuiltInFunction("average", "mean")]
    public static IExpression Mean([AreNumbers] IExpression expressions)
    {
        if (expressions is IEnumerableExpression enum_expr)
        {
            int count = enum_expr.Count();
            
            return count == 0
                ? Undefined.UNDEFINED
                : (Number)(enum_expr.Select(x => ((Number)x).Value).Aggregate((a, b) => a + b) / count);
        }

        return expressions;
    }

    [BuiltInFunction("median")]
    public static IExpression Median([AreNumbers] IExpression expressions)
    {
        if (expressions is IEnumerableExpression enum_expr)
        {
            IEnumerable<IExpression> sorted = enum_expr.OrderBy(x => ((Number)x).Value);
            int half = sorted.Count() / 2;

            return sorted.Count() % 2 == 0
                ? (Number)((((Number)enum_expr.ElementAt(half - 1)).Value +
                    ((Number)enum_expr.ElementAt(half)).Value) / 2)
                : enum_expr.ElementAt(half);
        }

        return expressions;
    }

    [BuiltInFunction("min", "minimum")]
    public static IExpression Min([AreNumbers] IExpression expressions)
    {
        return expressions is IEnumerableExpression enum_expr
            ? enum_expr.MinBy(x => ((Number)x).Value) ?? Undefined.UNDEFINED
            : expressions;
    }

    [BuiltInFunction("mode")]
    public static IExpression Mode([AreNumbers] IExpression expressions)
    {
        if (expressions is IEnumerableExpression enum_expr)
        {
            Dictionary<IExpression, ushort> counts = [];

            foreach (IExpression expression in enum_expr)
                if (!counts.TryAdd(expression, 1))
                    counts[expression]++;

            int max = counts.MaxBy(kvp => kvp.Value).Value;
            IEnumerable<IExpression> modes = counts.OrderBy(kvp => ((Number)kvp.Value).Value)
                .Where(kvp => kvp.Value == max)
                .Select(kvp => kvp.Key);

            return modes.Count() == 1
                ? modes.Single()
                : new Set(modes);
        }

        return expressions;
    }

    [BuiltInFunction("percentile")]
    public static IExpression Percentile([AreNumbers] IExpression expressions, double p)
    {
        if (expressions is IEnumerableExpression enum_expr)
        {
            int count = enum_expr.Count();

            if (count == 0 || p < 0 || p > 1)
                return Undefined.UNDEFINED;
            else if (count == 1)
                return enum_expr.Single();

            IEnumerable<Number> values = enum_expr.Cast<Number>().OrderBy(x => x.Value);
            double i = (count - 1) * p;
            int prev_i = (int)Math.Floor(i);

            if (prev_i == count - 1)
                return values.Last();

            double prev = values.ElementAt(prev_i).Value;
            double next = values.ElementAt(prev_i + 1).Value;

            return (Number)(prev + (i % 1) * (next - prev));
        }
        else
        {
            return expressions;
        }
    }

    [BuiltInFunction("quartile")]
    public static IExpression Quartile([AreNumbers] IExpression expressions, [Range(0, 4)] double q)
    {
        return Percentile(expressions, q / 4);
    }

    [BuiltInFunction("stdev", "stdevs")]
    public static IExpression SampleStandardDeviation([AreNumbers] IExpression expressions)
    {
        if (expressions is IEnumerableExpression enum_expr)
        {
            double mean = ((Number)Mean(expressions)).Value;

            return (Number)Math.Sqrt(((double)1 / (enum_expr.Count() - 1)) *
                ((Number)Sum(enum_expr.Map(x => (Number)Math.Pow(((Number)x).Value - mean, 2)))).Value);
        }

        return Undefined.UNDEFINED;
    }

    [BuiltInFunction("stdevp")]
    public static IExpression PopulationStandardDeviation([AreNumbers] IExpression expressions)
    {
        if (expressions is IEnumerableExpression enum_expr)
        {
            double mean = ((Number)Mean(expressions)).Value;

            return (Number)Math.Sqrt(((double)1 / enum_expr.Count()) *
                ((Number)Sum(enum_expr.Map(x => (Number)Math.Pow(((Number)x).Value - mean, 2)))).Value);
        }

        return (Number)0;
    }

    [BuiltInFunction("sum", "total")]
    public static IExpression Sum([AreNumbers] IExpression expressions)
    {
        return expressions is IEnumerableExpression enum_expr
            ? (Number)enum_expr.Select(x => ((Number)x).Value).Aggregate((a, b) => a + b)
            : expressions;
    }
}
