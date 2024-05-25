﻿using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
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

    public static void AreEqual(IExpression expected, IExpression actual, int decimal_places = 0, string message = "")
    {
        if (expected is IEnumerable<IExpression> exp_enum && actual is IEnumerable<IExpression> act_enum)
        {
            if (expected is RandomCollection randCollection)
            {
                if (randCollection.MinCount.HasValue)
                    IsGreaterThanOrEqual(randCollection.MinCount.Value, act_enum.Count(), message);

                if (randCollection.MaxCount.HasValue)
                    IsLessThanOrEqual(randCollection.MaxCount.Value, act_enum.Count(), message);

                if (randCollection.Min.HasValue)
                    foreach (IExpression expr in act_enum)
                        if (expr is Number num)
                            IsGreaterThanOrEqual(randCollection.Min.Value, num.Value, message);

                if (randCollection.Max.HasValue)
                    foreach (IExpression expr in act_enum)
                        if (expr is Number num)
                            IsLessThanOrEqual(randCollection.Max.Value, num.Value, message);

                if (randCollection.Validate is not null)
                    foreach (IExpression expr in act_enum)
                        Assert.IsTrue(randCollection.Validate(expr), message);

                return;
            }
            else if (exp_enum.GetType() == act_enum.GetType() && exp_enum.Count() == act_enum.Count())
            {
                IEnumerator<IExpression> exp_enumerator = exp_enum.GetEnumerator();
                IEnumerator<IExpression> act_enumerator = act_enum.GetEnumerator();

                while (exp_enumerator.MoveNext() && act_enumerator.MoveNext())
                    AreEqual(exp_enumerator.Current, act_enumerator.Current, decimal_places, message);

                return;
            }
        }
        else if (actual is Number actualNum)
        {
            if (expected is Number expectedNum)
            {
                Assert.AreEqual(Math.Round(expectedNum.Value, decimal_places),
                    Math.Round(actualNum.Value, decimal_places), message);
                return;
            }
            else if (expected is RandomValue randVal)
            {
                if (randVal.Min.HasValue)
                    IsGreaterThanOrEqual(randVal.Min.Value, actualNum.Value, message);

                if (randVal.Max.HasValue)
                    IsLessThanOrEqual(randVal.Max.Value, actualNum.Value, message);

                if (randVal.Validate is not null)
                    Assert.IsTrue(randVal.Validate(actualNum), message);
            }
        }
        else
        {
            Assert.AreEqual(expected, actual, message);
        }
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
}
