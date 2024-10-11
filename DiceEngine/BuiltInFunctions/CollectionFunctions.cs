using DiceEngine.Attributes;
using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Extensions;
using DiceEngine.FunctionAttributes.PreprocessAttributes;

namespace DiceEngine.BuiltInFunctions;

public static class CollectionFunctions
{
    [BuiltInFunction("map")]
    public static IExpression Map(IEnumerableExpression collection, IFunction operation, ExpressionContext context)
    {
        IEnumerable<IExpression> expressions = collection.Select(x => operation.Invoke([x.Evaluate(context)], context));
        IExpression? result = (IExpression?)Activator.CreateInstance(collection.GetType(), expressions);

        return result ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("filter", "where")]
    public static IExpression Filter(IEnumerableExpression collection, IFunction selector, ExpressionContext context)
    {
        IEnumerable<IExpression> expressions = collection
            .Select(x => x.Evaluate(context))
            .Where(x => !Logical.FALSE.Equals(new AsBooleanAttribute().Preprocess(selector.Invoke([x], context))));
        IExpression? result = (IExpression?)Activator.CreateInstance(collection.GetType(), expressions);

        return result ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("aggregate")]
    public static IExpression Aggregate(IEnumerableExpression collection, IFunction aggregator,
        ExpressionContext context)
    {
        if (!collection.Any())
            return Undefined.UNDEFINED;

        IExpression? result = collection
            .Select(x => x.Evaluate(context))
            .Aggregate((a, b) => aggregator.Invoke([a, b], context));

        return result ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("range")]
    public static IExpression Range(int start, int count, int step)
    {
        if (start % 1 != 0 || count % 1 != 0 || step % 1 != 0 || count < 0)
            return Undefined.UNDEFINED;

        return Vector.ConvertIEnumerable(Enumerable.Range(0, count).Select(i => (Number)(start + i * step)));
    }

    [BuiltInFunction("random")]
    public static IExpression Random(Number count, Number min, Number max, [AsBoolean] IExpression int_only)
    {
        Random random = new Random();

        if (Logical.TRUE.Equals(int_only))
        {
            long minimum = Convert.ToInt64(Math.Truncate(min.Value + (min.Value <= 0 ? 0 : 1)));
            long maximum = Convert.ToInt64(Math.Truncate(max.Value - (max.Value >= 0 ? 0 : 1)));

            return new Vector(Enumerable.Range(1, Convert.ToInt32(count.Value))
                .Select(x => maximum < minimum
                    ? (IExpression)Undefined.UNDEFINED
                    : (Number)random.NextInt64(minimum, Math.Max(maximum, minimum))));
        }
        else
        {
            double range = max.Value - min.Value;

            return new Vector(Enumerable.Range(1, Convert.ToInt32(count.Value))
                .Select(x => (Number)(random.NextDouble() * range + min.Value)));
        }
    }

    private struct ExpressionComparer : IComparer<IExpression>
    {
        public readonly int Compare(IExpression? a, IExpression? b)
        {
            if (a is null || b is null || a.Equals(b))
                return 0;

            if (Infinity.POSITIVE.Equals(a) || Infinity.NEGATIVE.Equals(b))
                return 1;

            if (Infinity.POSITIVE.Equals(b) || Infinity.NEGATIVE.Equals(a))
                return -1;

            if (a is Number num_a && b is Number num_b)
                return num_a.Value.CompareTo(num_b.Value);

            return 0;
        }
    }

    [BuiltInFunction("sort", "order")]
    public static IExpression Sort(IEnumerableExpression collection)
    {
        List<IExpression> values = [.. collection];

        values.Sort(new ExpressionComparer());

        return (IExpression?)collection.GetType().GetMethod("ConvertIEnumerable")?.Invoke(null, [values])
            ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("concat", "concatenate")]
    public static IExpression Concat(IEnumerableExpression a, IEnumerableExpression b)
    {
        return (IExpression?)a.GetType().GetMethod("ConvertIEnumerable")?.Invoke(null, [a.Concat(b)])
            ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("append")]
    public static IExpression Append(IEnumerableExpression collection, IExpression element)
    {
        return (IExpression?)collection.GetType().GetMethod("ConvertIEnumerable")?
            .Invoke(null, [collection.Append(element)])
            ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("prepend")]
    public static IExpression Prepend(IEnumerableExpression collection, IExpression element)
    {
        return (IExpression?)collection.GetType().GetMethod("ConvertIEnumerable")?
            .Invoke(null, [collection.Prepend(element)])
            ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("insert")]
    public static IExpression Insert(IEnumerableExpression collection, IExpression element, int index)
    {
        if (index % 1 != 0 || index < -collection.Count() || index > collection.Count())
            return Undefined.UNDEFINED;

        return (IExpression?)collection.GetType().GetMethod("ConvertIEnumerable")?
            .Invoke(null, [collection.Insert(index, element)])
            ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("remove")]
    public static IExpression Remove(IEnumerableExpression collection, int index)
    {
        if (!collection.Any() || index % 1 != 0 || index < -collection.Count() || index >= collection.Count())
            return Undefined.UNDEFINED;

        return (IExpression?)collection.GetType().GetMethod("ConvertIEnumerable")?
            .Invoke(null, [collection.Remove(index)])
            ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("any", "some")]
    public static bool Any(IEnumerableExpression collection, IFunction condition, ExpressionContext context)
    {
        return ((IEnumerableExpression)collection.Evaluate(context))
            .Any(x => Logical.TRUE.Equals(new AsBooleanAttribute().Preprocess(condition.Invoke([x], context))));
    }

    [BuiltInFunction("all")]
    public static bool All(IEnumerableExpression collection, IFunction condition, ExpressionContext context)
    {
        return ((IEnumerableExpression)collection.Evaluate(context))
            .All(x => Logical.TRUE.Equals(new AsBooleanAttribute().Preprocess(condition.Invoke([x], context))));
    }

    [BuiltInFunction("find")]
    public static int? Find(IEnumerableExpression collection, IExpression item)
    {
        try
        {
            int index = collection.Select((x, i) => (x, i))
                .First(element => element.x.Equals(item))
                .i;

            return index;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    [BuiltInFunction("find_last")]
    public static IExpression FindLast(IEnumerableExpression collection, IExpression item)
    {
        try
        {
            int index = collection.Select((x, i) => (x, i))
                .Last(element => element.x.Equals(item))
                .i;

            return (Number)index;
        }
        catch (InvalidOperationException)
        {
            return Undefined.UNDEFINED;
        }
    }

    [BuiltInFunction("reverse")]
    public static IExpression Reverse(IEnumerableExpression collection)
    {
        return (IExpression?)collection.GetType().GetMethod("ConvertIEnumerable")?.Invoke(null, [collection.Reverse()])
            ?? Undefined.UNDEFINED;
    }

    [BuiltInFunction("zip")]
    public static IExpression Zip(IEnumerableExpression a, IEnumerableExpression b, IFunction combiner,
        ExpressionContext context)
    {
        return (IExpression?)((IEnumerableExpression)a.Evaluate(context)).GetType().GetMethod("ConvertIEnumerable")?
            .Invoke(null, [a.Zip(b, (x, y) => combiner.Invoke([x, y], context) ?? Undefined.UNDEFINED)])
            ?? Undefined.UNDEFINED;
    }
}
