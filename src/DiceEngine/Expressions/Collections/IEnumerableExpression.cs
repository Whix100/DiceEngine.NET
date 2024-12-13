namespace DiceEngine.Expressions.Collections;

public interface IEnumerableExpression : IExpression, IEnumerable<IExpression>
{
    /// <summary>
    /// Converts an <see cref="IEnumerable{IExpression}"/> to an <see cref="IEnumerableExpression"/>.
    /// </summary>
    /// <param name="expressions">The <see cref="IEnumerable{IExpression}"/> to convert.</param>
    /// <returns>The new <see cref="IEnumerableExpression"/>.</returns>
    static abstract IEnumerableExpression ConvertIEnumerable(IEnumerable<IExpression> expressions);

    /// <summary>
    /// Maps the <see cref="IEnumerableExpression"/> to a new <see cref="IEnumerableExpression"/> using the specified
    /// map function.
    /// </summary>
    /// <param name="map">
    /// The function to transform each element of the <see cref="IEnumerableExpression"/>.
    /// </param>
    /// <returns>A new <see cref="IEnumerableExpression"/> with the transformed elements.</returns>
    IEnumerableExpression Map(Func<IExpression, IExpression> map);

    /// <summary>
    /// Combines the <see cref="IEnumerableExpression"/> with another <see cref="IEnumerable{IExpression}"/> using the
    /// specified combine function.
    /// </summary>
    /// <param name="expressions">The other <see cref="IEnumerable{IExpression}"/> to combine with.</param>
    /// <param name="combine">
    /// The function to combine the elements of the two <see cref="IEnumerable{IExpression}"/>.
    /// </param>
    /// <returns><see cref="IEnumerableExpression"/> with the combined elements.</returns>
    IEnumerableExpression Combine(IEnumerable<IExpression> expressions,
        Func<IExpression, IExpression, IExpression> combine);

    public static IEnumerableExpression ConvertIEnumerable(Type type, IEnumerable<IExpression> expressions)
    {
        if (!typeof(IEnumerableExpression).IsAssignableFrom(type))
            throw new ArgumentException($"{nameof(type)} must implement {nameof(IEnumerableExpression)}");

        return (IEnumerableExpression)type.GetMethod(nameof(ConvertIEnumerable))?.Invoke(null, [expressions])!;
    }
}
