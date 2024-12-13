using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Terminals;
using TestDiceEngine.TestModels;

namespace TestDiceEngine.TestUtils;

internal static class UtilFunctions
{
    public static T Range<T>(int start, int count)
        where T : IEnumerableExpression
        => (T)T.ConvertIEnumerable(Enumerable.Range(start, count).Select(i => (Number)i));

    public static T Range<T>(int start, int count, int step)
        where T : IEnumerableExpression
        => (T)T.ConvertIEnumerable(Enumerable.Range(0, count).Select(i => (Number)(start + i * step)));

    public static T Random<T>(int count, int min = Int32.MinValue, int max = Int32.MaxValue)
        where T : IEnumerableExpression
    {
        Random random = new Random();

        return (T)T.ConvertIEnumerable(
            new int[count].Select(i => (Number)(random.Next(min, max) + random.NextDouble())));
    }

    public static IEnumerable<IExpression> Random(int count, int min = Int32.MinValue, int max = Int32.MaxValue)
    {
        Random random = new Random();

        return new int[count].Select(i => (Number)(random.Next(min, max) + random.NextDouble()));
    }

    public static void AreEqual(IExpression expected, IExpression actual, int decimalPlaces = 0, string message = "")
    {
        if (expected is RandomCollection || expected is RandomRollResult)
        {
            Assert.AreEqual(expected, actual, message);
            return;
        }
        else if (expected is IEnumerable<IExpression> expEnum && actual is IEnumerable<IExpression> actEnum)
        {
            if (MatchingEnumerableTypes(expEnum.GetType(), actEnum.GetType()) && expEnum.Count() == actEnum.Count())
            {
                IEnumerator<IExpression> exp_enumerator = expEnum.GetEnumerator();
                IEnumerator<IExpression> act_enumerator = actEnum.GetEnumerator();

                while (exp_enumerator.MoveNext() && act_enumerator.MoveNext())
                    AreEqual(exp_enumerator.Current, act_enumerator.Current, decimalPlaces, message);

                return;
            }
        }
        else if (expected is Number exp_num && actual is Number act_num)
        {
            Assert.AreEqual(Math.Round(exp_num.Value, decimalPlaces), Math.Round(act_num.Value, decimalPlaces),
                message);
            return;
        }
        
        Assert.AreEqual(expected, actual, message);
    }

    public static void IsLessThan(double expected, double actual, string message = "")
    {
        if (actual >= expected)
        {
            string text = $"Expected: {expected}. Actual: {actual}.";

            if (!String.IsNullOrEmpty(message))
                text += $" {message}";

            HandleFail("UtilFunctions.IsLessThan", text);
        }
    }
    
    public static void IsLessThanOrEqual(double expected, double actual, string message = "")
    {
        if (actual > expected)
        {
            string text = $"Expected: {expected}. Actual: {actual}.";

            if (!String.IsNullOrEmpty(message))
                text += $" {message}";

            HandleFail("UtilFunctions.IsLessThanOrEqual", text);
        }
    }
    
    public static void IsGreaterThan(double expected, double actual, string message = "")
    {
        if (actual <= expected)
        {
            string text = $"Expected: {expected}. Actual: {actual}.";

            if (!String.IsNullOrEmpty(message))
                text += $" {message}";

            HandleFail("UtilFunctions.IsGreaterThan", text);
        }
    }
    
    public static void IsGreaterThanOrEqual(double expected, double actual, string message = "")
    {
        if (actual < expected)
        {
            string text = $"Expected: {expected}. Actual: {actual}.";

            if (!String.IsNullOrEmpty(message))
                text += $" {message}";

            HandleFail("UtilFunctions.IsGreaterThanOrEqual", text);
        }
    }

    private static void HandleFail(string assertion_name, string message)
    {
        string text = $"{assertion_name} failed.";

        if (!String.IsNullOrEmpty(message))
            text += $" {message}";

        throw new AssertFailedException(text);
    }

    private static bool MatchingEnumerableTypes(Type a, Type b)
    {
        return a == b || (a.IsGenericType && a.GetGenericArguments().SingleOrDefault() == b) ||
            (b.IsGenericType && a == b.GetGenericArguments().SingleOrDefault());
    }

    public static void AreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = "")
    {
        if (expected.Count() == actual.Count())
        {
            IEnumerator<T> expEnum = expected.Order().GetEnumerator();
            IEnumerator<T> actEnum = actual.Order().GetEnumerator();

            while (expEnum.MoveNext() && actEnum.MoveNext())
            {
                if ((expEnum.Current is null && actEnum.Current is not null) ||
                    (expEnum.Current is not null && !expEnum.Current.Equals(actEnum.Current)))
                    HandleFail("UtilFunctions.AreEqual", $"Expected:{expEnum.Current}. Actual:{actEnum.Current}." +
                        (string.IsNullOrEmpty(message) ? "" : $" {message}"));
            }

            return;
        }

        HandleFail("UtilFunctions.AreEqual", $"Expected count:{expected.Count()}. Actual:{actual.Count()}." +
            (string.IsNullOrEmpty(message) ? "" : $" {message}"));
    }
}
