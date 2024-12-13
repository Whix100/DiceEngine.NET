using DiceEngine.Expressions;
using DiceEngine.Expressions.Components;
using DiceEngine.Expressions.Functions;
using DiceEngine.Expressions.Terminals;
using System.Linq.Expressions;
using System.Reflection;
using TestDiceEngine.Extensions;
using TestDiceEngine.TestData;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestFunction
{
    /// <summary>
    /// Tests that the values of the Function are the same as the values passed into the Function's constructor.
    /// </summary>
    [TestMethod]
    public void TestFunctionInit()
    {
        foreach (MethodInfo method in typeof(TestCases).GetMethods(BindingFlags.Static))
        {
            Delegate del = method.ToDelegate();
            IParameter[] parameters = [.. method.GetParameters().ToParameters([])];
            NativeFunction function = new NativeFunction(del);

            Assert.AreEqual(method, function.Body.Method);

            for (int i = 0; i < parameters.Length; i++)
                Assert.AreEqual(parameters[i], function.Parameters[i]);

            function = new NativeFunction(parameters, del);
            Assert.AreEqual(method, function.Body.Method);

            for (int i = 0; i < parameters.Length; i++)
                Assert.AreEqual(parameters[i], function.Parameters[i]);
        }
    }

    /// <summary>
    /// Tests that the values of the LambdaFunction are the same as the values passed into its constructor.
    /// </summary>
    [TestMethod]
    public void TestLambdaFunctionInit()
    {
        (Parameter[] args, IExpression)[] test_cases = new (Parameter[], IExpression)[]
        {
            ([ "x" ], new BinaryOperator("+", new Variable("x"), new Number(1))),
            ([ "x", "y" ], new BinaryOperator("+", new Variable("x"), new Variable("y"))),
            ([ "x", "y", "z" ], new BinaryOperator("+", new BinaryOperator("+", new Variable("x"),
                new Variable("y")), new Variable("z"))),
        };

        foreach ((Parameter[] args, IExpression body) in test_cases)
        {
            LambdaFunction lambda_function = new LambdaFunction(args, body);

            for (int i = 0; i < args.Length; i++)
                Assert.AreEqual(args[i], lambda_function.Parameters[i]);

            Assert.AreEqual(body, lambda_function.Body);
        }
    }

    /// <summary>
    /// Tests that the values of the FunctionCall are the same as the values passed into the FunctionCall's constructor.
    /// </summary>
    [TestMethod]
    public void TestFunctionCallInit()
    {
        Random random = new Random();

        foreach (string function_name in TestCases.ContextFunctions.Keys)
        {
            IExpression[] args = new IExpression[TestCases.ContextFunctions[function_name].Parameters.Length]
                .Select((x) => new Number(random.NextDouble() + random.Next())).ToArray();
            FunctionCall function_call = new FunctionCall(function_name, args);

            Assert.AreEqual(function_name, function_call.Name);

            for (int i = 0; i < args.Length; i++)
                Assert.AreEqual(args[i], function_call.Arguments[i]);
        }
    }

    /// <summary>
    /// Tests that the LambdaFunction converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestLambdaFunctionToString()
    {
        Dictionary<LambdaFunction, string> test_cases = new Dictionary<LambdaFunction, string>
        {
            { new LambdaFunction([ "x" ], new BinaryOperator("+", new Variable("x"), new Number(1))),"(x)=>x+1" },
            { new LambdaFunction([ "x", "y" ], new BinaryOperator("+", new Variable("x"), new Variable("y"))),
                "(x,y)=>x+y" },
            { new LambdaFunction([ "x", "y", "z" ], new BinaryOperator("+", new BinaryOperator("+", new Variable("x"),
                new Variable("y")), new Variable("z"))), "(x,y,z)=>x+y+z" },
        };

        foreach (KeyValuePair<LambdaFunction, string> func in test_cases)
            Assert.AreEqual(func.Value, func.Key.ToString());
    }

    /// <summary>
    /// Tests that the FunctionCall converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestFunctionCallToString()
    {
        Dictionary<FunctionCall, string> expressions = new Dictionary<FunctionCall, string>()
        {
            { new FunctionCall("f", [ new Number(1) ]), "f(1)" },
            { new FunctionCall("g", [ new Number(1), new Number(2), new Number(3) ]), "g(1, 2, 3)" },
            { new FunctionCall("p", [ new Number(1) ]), "p(1)" },
        };

        foreach (FunctionCall expression in expressions.Keys)
            Assert.AreEqual(expressions[expression], expression.ToString());
    }
}
