using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using System.Xml.Linq;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.Expressions.Collections;

[TestClass]
public class TestSet
{
    /// <summary>
    /// Tests that the values of the Set are the same as the value passed into the Set's constructor.
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
    /// Tests that the elements within a Set are unique.
    /// </summary>
    [TestMethod]
    public void TestUniqueness()
    {
        IExpression[][] sets = [
            [(Number)1, (Number)1, (Number)2],
            [Undefined.UNDEFINED, Undefined.UNDEFINED, Undefined.UNDEFINED],
            [.. UtilFunctions.Range<Vector>(1, 10)],
        ];

        foreach (IExpression[] test_set in sets)
        {
            IEnumerable<IExpression> unique = test_set.Distinct();
            Set set = new Set(test_set);

            Assert.AreEqual(unique.Count(), set.Count);

            int[] hashes = [.. set.Select(x => x.GetHashCode())];

            for (int i = 0; i < hashes.Length; i++)
                for (int j = i + 1; j < hashes.Length; j++)
                    Assert.AreNotEqual(hashes[i], hashes[j]);
        }
    }

    /// <summary>
    /// Tests that the Set converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        for (int i = 0; i < 20; i++)
        {
            IExpression[] elements = [.. UtilFunctions.Random(i)];
            Set set = new Set(elements);

            Assert.AreEqual($"{{{string.Join<IExpression>(", ", elements)}}}", set.ToString());
        }
    }
}
