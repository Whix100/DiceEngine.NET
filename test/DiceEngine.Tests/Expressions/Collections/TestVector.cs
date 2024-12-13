using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.Expressions.Collections;

[TestClass]
public class TestVector
{
    /// <summary>
    /// Tests that the values of the Vector are the same as the value passed into the Vector's constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        for (int i = 0; i < 20; i++)
        {
            IExpression[] elements = [.. UtilFunctions.Random(i)];
            Vector vect = new Vector(elements);

            Assert.AreEqual(elements.Length, vect.Length);

            for (int j = 0; j < vect.Length; j++)
                Assert.AreEqual(elements[j], vect[j]);
        }
    }

    /// <summary>
    /// Tests that the Vector converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        for (int i = 0; i < 20; i++)
        {
            IExpression[] elements = [.. UtilFunctions.Random(i)];
            Vector vect = new Vector(elements);

            Assert.AreEqual($"[{string.Join<IExpression>(", ", elements)}]", vect.ToString());
        }
    }
}
