using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Terminals;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.Expressions.Collections;

[TestClass]
public class TestIndexer
{
    /// <summary>
    /// Tests that the values of the Indexer are the same as the value passed into the constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        for (int j = 0; j < 20; j++)
        {
            Vector elements = new Vector(UtilFunctions.Random(j));

            for (int i = 0; i < elements.Length; i++)
            {
                Number index = (Number)i;
                Indexer indexer = new Indexer(elements, index);

                Assert.AreEqual(elements, indexer.Collection);
                Assert.AreEqual(index, indexer.Index);
            }
        }
    }

    /// <summary>
    /// Tests that the Indexer converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        for (int i = 0; i < 20; i++)
        {
            IExpression[] elements = [.. UtilFunctions.Random(i)];
            Vector vect = new Vector(elements);
            Indexer indexer = new Indexer(vect, new Number(i));

            Assert.AreEqual($"{vect}[{i}]", indexer.ToString());
        }
    }
}
