﻿using CalcExpr.Expressions;

namespace TestCalcExpr;

[TestClass]
public class TestParentheses
{
    /// <summary>
    /// Tests that the properties of the BinaryOperator are the same as when initialized.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        for (int i = 0; i < 10; i++)
        {
            Number n = new Number(random.NextDouble() + random.Next());

            Assert.AreEqual(n, new Parentheses(n).Inside);
        }
    }

    /// <summary>
    /// Tests that the operand gets evaluated into a simpler expression with the minimum levels to the expression tree.
    /// </summary>
    [TestMethod]
    public void TestEvaluate()
    {
        Random random = new Random();

        for (int i = 0; i < 10; i++)
        {
            double a = random.NextDouble() + random.Next();
            double b = random.NextDouble() + random.Next();

            Assert.AreEqual(new Number(a + b),
                new Parentheses(new BinaryOperator("+", new Number(a), new Number(b))).Evaluate());
        }
    }

    /// <summary>
    /// Tests that the BinaryOperator converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Random random = new Random();

        for (int i = 0; i < 10; i++)
        {
            double a = random.NextDouble() + random.Next();
            double b = random.NextDouble() + random.Next();

            Assert.AreEqual($"({a}+{b})", new Parentheses(new BinaryOperator("+", new Number(a), new Number(b))).ToString());
        }
    }
}
