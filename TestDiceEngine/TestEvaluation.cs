using DiceEngine.Context;
using DiceEngine.Exceptions;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Dice;
using System.Reflection;
using TestDiceEngine.TestData;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine;

[TestClass]
public class TestEvaluation
{
    private const int DIGITS = 6;

    [TestMethod]
    public void TestEvaluate()
    {
        EvaluateTestCases(TestCases.Expressions);
    }

    [TestMethod]
    public void TestEvaluateCollections()
    {
        EvaluateTestCases(TestCases.Collections);
    }

    private static void EvaluateTestCases(IEnumerable<TestCase> test_cases)
    {
        foreach (TestCase test_case in test_cases)
        {
            try
            {
                ExpressionContext context = new ExpressionContext(TestCases.ContextVariables);

                foreach (string func in TestCases.ContextFunctions.Keys)
                    context[func] = TestCases.ContextFunctions[func];

                IExpression result = test_case.Parsed.Evaluate(context);

                UtilFunctions.AreEqual(test_case.Evaluated, result, DIGITS,
                    $"Test case: '{test_case.ExpressionString}'.");
            }
            catch (TooManyDiceRollsException)
            {
                Assert.Fail($"Too many dice rolls. Test case: '{test_case.Parsed}'.");
            }
        }
    }

    [TestMethod]
    public void TestStepEvaluate()
    {
        StepEvaluateTestCases(TestCases.Expressions);
    }

    [TestMethod]
    public void TestStepEvaluateCollections()
    {
        StepEvaluateTestCases(TestCases.Collections);
    }

    private static void StepEvaluateTestCases(IEnumerable<TestCase> test_cases)
    {
        foreach (TestCase test_case in test_cases)
        {
            ExpressionContext context = new ExpressionContext(TestCases.ContextVariables);

            foreach (string func in TestCases.ContextFunctions.Keys)
                context[func] = TestCases.ContextFunctions[func];

            IExpression start = test_case.Parsed;

            for (int i = 0; i < test_case.StepEvaluated.Length; i++)
            {
                IExpression evaluated = start.StepEvaluate(context);

                UtilFunctions.AreEqual(test_case.StepEvaluated[i], evaluated, DIGITS,
                    $"Test case: '{test_case.ExpressionString}'.");
                start = evaluated;
            }
        }
    }

    [TestMethod]
    public void TestEvaluateFunctions()
    {
        IEnumerable<FunctionTestCase[]?> test_case_groups = typeof(TestCases)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.FieldType == typeof(FunctionTestCase[]))
            .Select(field => (FunctionTestCase[]?)field.GetValue(null));

        foreach (FunctionTestCase[]? test_cases in test_case_groups)
        {
            if (test_cases is null)
                continue;

            foreach (FunctionTestCase test_case in test_cases)
            {
                int i = 0;

                foreach ((IExpression[] args, IExpression evaluated) in test_case.Evaluated)
                {
                    int j = i++ % test_case.FunctionAliases.Length;

                    IExpression result = new FunctionCall(test_case.FunctionAliases[j], args)
                        .Evaluate(test_case.Context ?? new ExpressionContext());
                    string error_message = "Test case: " +
                        $"'{test_case.FunctionAliases[j]}({String.Join(", ", args.Select(x => x.ToString()))})'.";

                    UtilFunctions.AreEqual(evaluated, result, DIGITS, error_message);
                }
            }
        }
    }

    [TestMethod]
    public void TestRandomFunction()
    {
        // The random function is tested separately because it is non-deterministic.
        Random random = new Random();
        bool all_integers = true;

        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Number min = (Number)(random.NextDouble() * random.Next(100) * random.Next(-1, 2));
                Number max = (Number)(random.NextDouble() * random.Next(200) + Math.Max(0, min.Value));

                // Test the random function with the integer argument set to true.
                IEnumerableExpression results = (IEnumerableExpression)new FunctionCall("random",
                        [(Number)i, min, max, Logical.TRUE])
                    .Evaluate(new ExpressionContext());

                Assert.AreEqual(i, results.Count(), "The number of elements in the enumeration is incorrect.");

                double t_min = Math.Truncate(min.Value);
                double t_max = Math.Truncate(max.Value);

                foreach (IExpression element in results)
                {
                    if ((max.Value == min.Value && t_max != max.Value) ||
                        (t_max == t_min && max.Value - min.Value < 1 &&
                            min.Value != t_min && Math.Sign(min.Value) == Math.Sign(max.Value)))
                    {
                        Assert.AreEqual(Undefined.UNDEFINED, element,
                            $"Min: {min.Value}. Max: {max.Value}. Element is not undefined.");
                        continue;
                    }
                    else
                    {
                        Assert.IsTrue(element is Number,
                            $"Min: {min.Value}. Max: {max.Value}. Element {element.ToString()} is not a number.");
                    }

                    Number n = (Number)element;

                    UtilFunctions.IsLessThanOrEqual(max.Value, n.Value,
                        $"Element {element.ToString()} is not less than or equal to {max.Value}.");
                    UtilFunctions.IsGreaterThanOrEqual(min.Value, n.Value,
                        $"Element {element.ToString()} is not greater than or equal to {min.Value}.");
                    Assert.IsTrue(n.Value % 1 == 0, "Not all elements are integers.");
                }

                // Test the random function with the integer argument set to false.
                results = (IEnumerableExpression)new FunctionCall("random", [(Number)i, min, max, Logical.FALSE])
                    .Evaluate(new ExpressionContext());

                Assert.AreEqual(i, results.Count(), "The number of elements in the enumeration is incorrect.");

                foreach (IExpression element in results)
                {
                    Assert.IsTrue(element is Number, $"Element {element.ToString()} is not a number.");

                    Number n = (Number)element;

                    UtilFunctions.IsLessThanOrEqual(max.Value, n.Value,
                        $"Element {element.ToString()} is not less than or equal to {max.Value}.");
                    UtilFunctions.IsGreaterThanOrEqual(min.Value, n.Value,
                        $"Element {element.ToString()} is not greater than or equal to {min.Value}.");

                    all_integers &= n.Value % 1 == 0;
                }
            }
        }

        Assert.IsFalse(all_integers, "All elements were integers.");
    }

    [TestMethod]
    public void TestEvaluateDice()
    {
        for (int i = 0; i < 100; i++)
            EvaluateTestCases(TestCases.DiceNotation);

        foreach (TestCase testCase in TestCases.DiceNotation)
        {
            IExpression noDice = testCase.Parsed.EvaluateDice();

            Assert.IsFalse(CheckForDie(noDice), $"Test case: '{testCase.ExpressionString}'.");
        }
    }

    private static bool CheckForDie(IExpression? expression)
    {
        if (expression is IDie)
            return true;
        else if (expression is null)
            return false;

        Type expressionType = expression.GetType();

        foreach (PropertyInfo prop in expressionType.GetProperties())
        {
            if (prop.PropertyType == typeof(IExpression))
                if (CheckForDie((IExpression?)prop.GetValue(expression)))
                    return true;
        }

        foreach (FieldInfo field in expressionType.GetFields())
        {
            if (field.FieldType == typeof(IExpression))
                if (CheckForDie((IExpression?)field.GetValue(expression)))
                    return true;
        }

        return false;
    }

    [TestMethod]
    public void TestStepEvaluateDice()
    {
        for (int i = 0; i < 100; i++)
            StepEvaluateTestCases(TestCases.DiceNotation);
    }
}
