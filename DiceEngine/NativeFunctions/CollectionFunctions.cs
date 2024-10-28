using DiceEngine.Attributes;
using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Functions;
using DiceEngine.Expressions.Terminals;
using DiceEngine.Extensions;
using DiceEngine.FunctionAttributes.PreprocessAttributes;

namespace DiceEngine.NativeFunctions;

public static class CollectionFunctions
{
    [NativeFunction("map")]
    public static Terminal Map(IEnumerableExpression collection, Function operation, ExpressionContext context)
    {
        IEnumerable<Terminal> expressions = collection.Select(x => operation.Invoke([x.Evaluate(context)], context));
        IEnumerableExpression? result = (IEnumerableExpression?)Activator.CreateInstance(collection.GetType(), expressions);

        return TerminalCollection.TerminateCollection(result);
    }

    [NativeFunction("filter", "where")]
    public static Terminal Filter(IEnumerableExpression collection, Function selector, ExpressionContext context)
    {
        IEnumerable<Terminal> expressions = collection
            .Select(x => x.Evaluate(context))
            .Where(x => !Logical.FALSE.Equals(new AsBooleanAttribute().Preprocess(selector.Invoke([x], context))));
        IEnumerableExpression? result = (IEnumerableExpression?)Activator.CreateInstance(collection.GetType(), expressions);

        return TerminalCollection.TerminateCollection(result);
    }

    [NativeFunction("aggregate")]
    public static Terminal Aggregate(IEnumerableExpression collection, Function aggregator, ExpressionContext context)
    {
        if (!collection.Any())
            return Undefined.UNDEFINED;

        IExpression? result = collection
            .Select(x => x.Evaluate(context))
            .Aggregate((a, b) => aggregator.Invoke([a, b], context));

        return result is Terminal term
            ? term
            : Undefined.UNDEFINED;
    }

    [NativeFunction("range")]
    public static Terminal Range(int start, int count, int step)
    {
        if (count < 0)
            return Undefined.UNDEFINED;

        return TerminalCollection.TerminateCollection(new Vector(Enumerable.Range(0, count).Select(i => (Terminal)(start + i * step))));
    }

    [NativeFunction("random")]
    public static Terminal Random(Number count, Number min, Number max, bool intOnly)
    {
        Random random = new Random();

        if (intOnly)
        {
            long minimum = Convert.ToInt64(Math.Truncate(min.Value + (min.Value <= 0 ? 0 : 1)));
            long maximum = Convert.ToInt64(Math.Truncate(max.Value - (max.Value >= 0 ? 0 : 1)));

            return TerminalCollection.TerminateCollection(new Vector(Enumerable.Range(1, Convert.ToInt32(count.Value))
                .Select(x => maximum < minimum
                    ? (Terminal)Undefined.UNDEFINED
                    : (Number)random.NextInt64(minimum, Math.Max(maximum, minimum)))));
        }
        else
        {
            double range = max.Value - min.Value;

            return TerminalCollection.TerminateCollection(new Vector(Enumerable.Range(1, Convert.ToInt32(count.Value))
                .Select(x => (Terminal)(random.NextDouble() * range + min.Value))));
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

    [NativeFunction("sort", "order")]
    public static Terminal Sort(IEnumerableExpression collection)
    {
        List<IExpression> values = [.. collection];

        values.Sort(new ExpressionComparer());

        return TerminalCollection.TerminateCollection(IEnumerableExpression.ConvertIEnumerable(collection.GetType(), values));
    }

    [NativeFunction("concat", "concatenate")]
    public static Terminal Concat(IEnumerableExpression a, IEnumerableExpression b)
    {
        return TerminalCollection.TerminateCollection(IEnumerableExpression.ConvertIEnumerable(a.GetType(), a.Concat(b)));
    }

    [NativeFunction("append")]
    public static Terminal Append(IEnumerableExpression collection, IExpression element)
    {
        return TerminalCollection.TerminateCollection(IEnumerableExpression.ConvertIEnumerable(collection.GetType(), collection.Append(element)));
    }

    [NativeFunction("prepend")]
    public static Terminal Prepend(IEnumerableExpression collection, IExpression element)
    {
        return TerminalCollection.TerminateCollection(IEnumerableExpression.ConvertIEnumerable(collection.GetType(), collection.Prepend(element)));
    }

    [NativeFunction("insert")]
    public static Terminal Insert(IEnumerableExpression collection, IExpression element, int index)
    {
        if (index < -collection.Count() || index > collection.Count())
            return Undefined.UNDEFINED;

        return TerminalCollection.TerminateCollection(
            IEnumerableExpression.ConvertIEnumerable(collection.GetType(), collection.Insert(index, element)));
    }

    [NativeFunction("remove")]
    public static Terminal Remove(IEnumerableExpression collection, int index)
    {
        if (!collection.Any() || index < -collection.Count() || index >= collection.Count())
            return Undefined.UNDEFINED;

        return TerminalCollection.TerminateCollection(IEnumerableExpression.ConvertIEnumerable(collection.GetType(), collection.Remove(index)));
    }

    [NativeFunction("any", "some")]
    public static bool Any(IEnumerableExpression collection, Function condition, ExpressionContext context)
    {
        return ((IEnumerableExpression)collection.Evaluate(context))
            .Any(x => Logical.TRUE.Equals(new AsBooleanAttribute().Preprocess(condition.Invoke([x], context))));
    }

    [NativeFunction("all")]
    public static bool All(IEnumerableExpression collection, Function condition, ExpressionContext context)
    {
        return ((IEnumerableExpression)collection.Evaluate(context))
            .All(x => Logical.TRUE.Equals(new AsBooleanAttribute().Preprocess(condition.Invoke([x], context))));
    }

    [NativeFunction("find")]
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

    [NativeFunction("find_last")]
    public static Terminal FindLast(IEnumerableExpression collection, IExpression item)
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

    [NativeFunction("reverse")]
    public static Terminal Reverse(IEnumerableExpression collection)
    {
        return TerminalCollection.TerminateCollection(IEnumerableExpression.ConvertIEnumerable(collection.GetType(), collection.Reverse()));
    }

    [NativeFunction("zip")]
    public static Terminal Zip(IEnumerableExpression a, IEnumerableExpression b, Function combiner, ExpressionContext context)
    {
        return TerminalCollection.TerminateCollection(
            IEnumerableExpression.ConvertIEnumerable(a.GetType(), a.Zip(b, (x, y) => combiner.Invoke([x, y], context) ?? Undefined.UNDEFINED)));
    }
}
