using DiceEngine.Context;
using DiceEngine.Expressions;
using TestDiceEngine.TestData;

namespace TestDiceEngine;

[TestClass]
public class TestExpressionContext
{
    [TestMethod]
    public void TestInit()
    {
        ExpressionContext context = new ExpressionContext(TestCases.ContextVariables, false);

        // TODO: Replace with streamlined function when created.
        foreach (KeyValuePair<string, IFunction> func in TestCases.ContextFunctions)
            context.SetFunction(func.Key, func.Value);

        foreach (string variable in TestCases.ContextVariables.Keys)
        {
            Assert.IsTrue(context.ContainsVariable(variable));

            if (TestCases.ContextVariables[variable] is Function)
                Assert.IsTrue(context.ContainsFunction(variable));
        }

        foreach (string function in TestCases.ContextFunctions.Keys)
        {
            Assert.IsTrue(context.ContainsVariable(function));
            Assert.IsTrue(context.ContainsFunction(function));
        }
    }

    [TestMethod]
    public void TestVariables()
    {
        ExpressionContext context = new ExpressionContext();

        foreach (string variable in TestCases.ContextVariables.Keys)
        {
            context.SetVariable(variable, TestCases.ContextVariables[variable]);
            Assert.IsTrue(context.ContainsVariable(variable));

            if (TestCases.ContextVariables[variable].GetType() == typeof(Function))
                Assert.IsTrue(context.ContainsFunction(variable));

            Assert.AreEqual(TestCases.ContextVariables[variable], context[variable]);
            context.RemoveVariable(variable);
            Assert.IsFalse(context.ContainsVariable(variable));
            Assert.AreEqual(Undefined.UNDEFINED, context[variable]);
        }
    }

    [TestMethod]
    public void TestFunctions()
    {
        ExpressionContext context = new ExpressionContext();

        foreach (string function in TestCases.ContextFunctions.Keys)
        {
            Assert.IsTrue(context.SetVariable(function, TestCases.ContextFunctions[function]));
            Assert.IsTrue(context.ContainsVariable(function));
            Assert.IsTrue(context.ContainsFunction(function));
            Assert.AreEqual(TestCases.ContextFunctions[function], context[function]);
            Assert.IsTrue(context.RemoveVariable(function));
            Assert.IsFalse(context.ContainsVariable(function));
            Assert.IsFalse(context.ContainsFunction(function));
            Assert.AreEqual(Undefined.UNDEFINED, context[function]);

            Assert.IsTrue(context.SetFunction(function, TestCases.ContextFunctions[function]));
            Assert.IsTrue(context.ContainsVariable(function));
            Assert.IsTrue(context.ContainsFunction(function));
            Assert.AreEqual(TestCases.ContextFunctions[function], context[function]);
            Assert.IsTrue(context.RemoveFunction(function));
            Assert.IsFalse(context.ContainsVariable(function));
            Assert.IsFalse(context.ContainsFunction(function));
            Assert.AreEqual(Undefined.UNDEFINED, context[function]);
        }
    }

    private class ExtendedContext : ExpressionContext
    {
        public override IExpression this[string name]
        {
            get => Undefined.UNDEFINED;
        }

        public ExtendedContext() : base([], false)
        {
            SetVariable("abcd", Infinity.POSITIVE);
        }
    }

    [TestMethod]
    public void TestExention()
    {
        ExtendedContext context = new ExtendedContext();
        Variable variable = new Variable("abcd");

        Assert.IsTrue(context.ContainsVariable("abcd"));
        Assert.AreEqual(Undefined.UNDEFINED, variable.Evaluate(context));
    }
}
