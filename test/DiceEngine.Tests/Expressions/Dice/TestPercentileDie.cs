using DiceEngine.Expressions.Dice;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.Expressions.Dice;

[TestClass]
public class TestPercentileDie
{
    /// <summary>
    /// Tests that the PercentileDie rolls properly.
    /// </summary>
    [TestMethod]
    public void TestRoll()
    {
        PercentileDie die = new PercentileDie();
        HashSet<int> results = [];

        while (results.Count < 10)
        {
            int result = die.Roll();

            UtilFunctions.IsGreaterThanOrEqual(0, result, $"{result} is not greater than or equal to 0.");
            UtilFunctions.IsLessThanOrEqual(90, result, $"{result} is not less than or equal to 90.");
            Assert.IsTrue(result % 10 == 0, $"{result} is not a multiple of 10.");
            results.Add(result);
        }
    }

    /// <summary>
    /// Tests that the PercentileDie converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        PercentileDie die = new PercentileDie();

        Assert.AreEqual("d%", die.ToString());
    }
}
