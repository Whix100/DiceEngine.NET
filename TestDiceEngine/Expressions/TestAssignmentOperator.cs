using DiceEngine.Exceptions;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestAssignmentOperator
{
    /// <summary>
    /// Tests that the properties of the AssignmentOperator are the same as when initialized.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        for (char c = 'a'; c <= 'z'; c++)
        {
            Variable variable = new Variable($"c");
            Number num = new Number(random.NextDouble() * 1000);
            AssignmentOperator assn_op = new AssignmentOperator(variable, num);

            Assert.AreEqual(variable, assn_op.AssignedVariable);
            Assert.AreEqual(num, assn_op.Value);
        }

        try
        {
            _ = new AssignmentOperator(new Number(0), new Number(0));
            Assert.Fail();
        }
        catch (InvalidAssignmentException)
        {
            return;
        }
    }

    /// <summary>
    /// Tests that the AssignmentOperator converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Random random = new Random();

        for (char c = 'a'; c <= 'z'; c++)
        {
            Variable variable = new Variable($"c");
            Number num = new Number(random.NextDouble() * 1000);
            AssignmentOperator assn_op = new AssignmentOperator(variable, num);

            Assert.AreEqual($"{variable.Name}={num}", assn_op.ToString());
        }
    }
}
