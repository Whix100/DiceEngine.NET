using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;
using DiceEngine.Expressions.Terminals;
using System.Collections;
using System.Reflection;

namespace DiceEngine.Expressions.Collections;

public static class TerminalCollection
{
    public static Terminal TerminateCollection(IEnumerableExpression? collection)
    {
        if (collection is null)
            return Undefined.UNDEFINED;

        Type collectionType = collection.GetType();

        if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(TerminalCollection<>))
            return (Terminal)collection;

        if (collection.All(x => x is Terminal))
        {
            MethodInfo method = typeof(TerminalCollection<>).MakeGenericType(collection.GetType())
                .GetMethod(nameof(IEnumerableExpression.ConvertIEnumerable))!;

            return (Terminal)method.Invoke(null, [collection])!;
        }

        throw new ArgumentException("All elements must be terminals.");
    }

    public static IEnumerableExpression ConvertIEnumerable(IEnumerableExpression collection)
    {
        if (collection.All(x => x is Terminal))
            return (IEnumerableExpression)TerminateCollection(collection);

        return collection;
    }
}

public class TerminalCollection<T>(T values) : Terminal, IEnumerableExpression, IPrefixOperable, IPostfixOperable, IBinaryOperable
    where T : IEnumerableExpression
{
    public T Values { get; set; } = values;

    public TerminalCollection(IEnumerable<Terminal> elements) : this((T)T.ConvertIEnumerable(elements)) { }

    public Terminal PrefixOperate(string identifier, ExpressionContext context)
        => IPrefixOperable.Operate(identifier, Values, context);

    public Terminal PostfixOperate(string identifier, ExpressionContext context)
        => IPostfixOperable.Operate(identifier, Values, context);

    public Terminal? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext context)
        => IBinaryOperable.Operate(identifier, Values, right, context);

    public Terminal? BinaryRightOperate(string identifier, IExpression left, ExpressionContext context)
        => IBinaryOperable.Operate(identifier, left, Values, context);

    public static IEnumerableExpression ConvertIEnumerable(IEnumerable<IExpression> expressions)
    {
        if (expressions.All(x => x is Terminal))
            return new TerminalCollection<T>((T)T.ConvertIEnumerable(expressions));

        return T.ConvertIEnumerable(expressions);
    }

    public IEnumerableExpression Map(Func<IExpression, IExpression> map)
        => Values.Map(map);

    public IEnumerableExpression Combine(IEnumerable<IExpression> expressions, Func<IExpression, IExpression, IExpression> combine)
        => Values.Combine(expressions, combine);

    public override bool Equals(object? obj)
        => Values.Equals(obj);

    public override int GetHashCode()
        => Values.GetHashCode();

    public override string ToString(string? format)
        => Values.ToString(format);

    public IEnumerator<IExpression> GetEnumerator()
        => Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => ((IEnumerable)Values).GetEnumerator();
}
