namespace DiceEngine.Extensions;

internal static class EnumerableExtensions
{
    public static int NormalizeIndex(int index, int count, bool inclusive = false)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(count, -count, nameof(count));

        if (inclusive || count == 0)
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, count, nameof(index));
        else
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, count, nameof(index));

        return index < 0 ? count + index : index;
    }

    public static int NormalizeIndex(double index, int count, bool inclusive = false)
        => NormalizeIndex((int)index, count, inclusive);

    public static IEnumerable<T> Insert<T>(this IEnumerable<T> enumerable, int index, T item)
    {
        index = NormalizeIndex(index, enumerable.Count(), true);

        foreach (T element in enumerable)
        {
            if (index-- == 0)
                yield return item;

            yield return element;
        }

        if (index == 0)
            yield return item;
    }

    public static IEnumerable<T> Remove<T>(this IEnumerable<T> enumerable, int index)
    {
        index = NormalizeIndex(index, enumerable.Count());

        foreach (T element in enumerable)
        {
            if (index-- == 0)
                continue;

            yield return element;
        }
    }
}
