using DiceEngine.Expressions;
using DiceEngine.Expressions.Dice;

namespace TestDiceEngine.Expressions.Dice;

[TestClass]
public class TestRollResult
{
    /// <summary>
    /// Tests that the value of the RollResult is the same as the value passed into the RollResult's constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            int size = random.Next(1, 100);
            Die die = new Die(size);
            RollResult rollResult = new RollResult(random.Next(1, size + 1), die);

            Assert.AreEqual(die, rollResult.Die);
        }
    }

    /// <summary>
    /// Tests that the RollResult converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            int size = random.Next(1, 100);
            Die die = new Die(size);
            int number = random.Next(1, size + 1);
            RollResult rollResult = new RollResult(number, die);

            Assert.AreEqual($"{die} ({number})", rollResult.ToString());
        }
    }
}
