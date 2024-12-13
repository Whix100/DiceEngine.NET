using DiceEngine.Expressions.Dice;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.Expressions.Dice;

[TestClass]
public class TestFateDie
{
    /// <summary>
    /// Tests that the FateDie rolls properly.
    /// </summary>
    [TestMethod]
    public void TestRoll()
    {
        FateDie die = new FateDie();
        HashSet<int> results = [];

        while (results.Count < 3)
        {
            int result = die.Roll();

            UtilFunctions.IsGreaterThanOrEqual(-1, result, $"{result} is not greater than or equal to -1.");
            UtilFunctions.IsLessThanOrEqual(1, result, $"{result} is not less than or equal to 1.");
            results.Add(result);
        }
    }

    /// <summary>
    /// Tests that the FateDie converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        FateDie die = new FateDie();

        Assert.AreEqual("dF", die.ToString());
    }
}
