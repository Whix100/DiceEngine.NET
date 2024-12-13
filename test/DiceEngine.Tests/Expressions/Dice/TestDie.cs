using DiceEngine.Expressions.Dice;

namespace TestDiceEngine.Expressions.Dice;

[TestClass]
public class TestDie
{
    /// <summary>
    /// Tests that the value of the Die is the same as the value passed into the Die's constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            int size = random.Next(1, 100);
            Die die = new Die(size);

            Assert.AreEqual(size, die.Size);
        }
    }

    /// <summary>
    /// Tests that the Die converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            uint size = (uint)random.Next(1, 100);
            Die die = new Die(size);

            Assert.AreEqual($"d{size}", die.ToString());
        }
    }
}
