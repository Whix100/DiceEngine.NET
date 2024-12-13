using DiceEngine.Expressions.Dice;
using System.Drawing;

namespace TestDiceEngine.Expressions.Dice;

[TestClass]
public class TestDiceSet
{
    /// <summary>
    /// Tests that the value of the DiceSet is the same as the value passed into the DiceSet's constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            int size = random.Next(1, 100);
            int setSize = random.Next(1, 100);
            Die die = new Die(size);
            DiceSet diceSet = new DiceSet(setSize, die);

            Assert.AreEqual(setSize, diceSet.Size);
            Assert.AreEqual(die, diceSet.Die);
        }

        int percentileSetSize = random.Next(1, 100);
        PercentileDie percentileDie = new PercentileDie();
        DiceSet percentileDiceSet = new DiceSet(percentileSetSize, percentileDie);

        Assert.AreEqual(percentileSetSize, percentileDiceSet.Size);
        Assert.AreEqual(percentileDie, percentileDiceSet.Die);

        int fateSetSize = random.Next(1, 100);
        FateDie fateDie = new FateDie();
        DiceSet fateDiceSet = new DiceSet(fateSetSize, fateDie);

        Assert.AreEqual(fateSetSize, fateDiceSet.Size);
        Assert.AreEqual(fateDie, fateDiceSet.Die);
    }

    /// <summary>
    /// Tests that the DiceSet converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            int size = random.Next(1, 100);
            int setSize = random.Next(1, 100);
            Die die = new Die(size);
            DiceSet diceSet = new DiceSet(setSize, die);

            Assert.AreEqual($"{setSize}d{size}", diceSet.ToString());
        }

        int percentileSetSize = random.Next(1, 100);
        PercentileDie percentileDie = new PercentileDie();
        DiceSet percentileDiceSet = new DiceSet(percentileSetSize, percentileDie);

        Assert.AreEqual($"{percentileSetSize}d%", percentileDiceSet.ToString());

        int fateSetSize = random.Next(1, 100);
        FateDie fateDie = new FateDie();
        DiceSet fateDiceSet = new DiceSet(fateSetSize, fateDie);

        Assert.AreEqual($"{fateSetSize}dF", fateDiceSet.ToString());
    }
}
