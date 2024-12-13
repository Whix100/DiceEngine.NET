using DiceEngine.Expressions.Terminals;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestNumber
{
    /// <summary>
    /// Tests that the value of the Number is the same as the value passed into the Number's constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        for (int i = 0; i < 100; i++)
        {
            double val = random.NextDouble() * double.MaxValue * (random.Next(2) == 0 ? -1 : 1);

            Assert.AreEqual(val, new Number(val).Value);
        }
    }

    /// <summary>
    /// Tests that the Number can be cast to and from decimal, double, float, long, int, short, sbyte, ulong, uint,
    /// ushort, and byte.
    /// </summary>
    [TestMethod]
    public void TestConvert()
    {
        Assert.AreEqual(0.156489m, (decimal)new Number((double)0.156489m));
        Assert.AreEqual(0.24567841318, (double)new Number(0.24567841318));
        Assert.AreEqual(-5648.0549f, (float)new Number(-5648.0549f));
        Assert.AreEqual(-123157L, (long)new Number(-123157L));
        Assert.AreEqual(-1234853, (int)new Number(-1234853));
        Assert.AreEqual((short)-4364, (short)new Number(-4364));
        Assert.AreEqual((sbyte)-127, (sbyte)new Number(-127));
        Assert.AreEqual(123475UL, (ulong)new Number(123475UL));
        Assert.AreEqual(1237488U, (uint)new Number(1237488U));
        Assert.AreEqual((ushort)4364, (ushort)new Number(4364));
        Assert.AreEqual((byte)255, (byte)new Number(255));

        Assert.AreEqual(new Number((double)decimal.MaxValue).Value, ((Number)decimal.MaxValue).Value);
        Assert.AreEqual(new Number(double.MaxValue).Value, ((Number)double.MaxValue).Value);
        Assert.AreEqual(new Number(float.MaxValue).Value, ((Number)float.MaxValue).Value);
        Assert.AreEqual(new Number(long.MaxValue).Value, ((Number)long.MaxValue).Value);
        Assert.AreEqual(new Number(int.MaxValue).Value, ((Number)int.MaxValue).Value);
        Assert.AreEqual(new Number(short.MaxValue).Value, ((Number)short.MaxValue).Value);
        Assert.AreEqual(new Number(sbyte.MaxValue).Value, ((Number)sbyte.MaxValue).Value);
        Assert.AreEqual(new Number(ulong.MaxValue).Value, ((Number)ulong.MaxValue).Value);
        Assert.AreEqual(new Number(uint.MaxValue).Value, ((Number)uint.MaxValue).Value);
        Assert.AreEqual(new Number(ushort.MaxValue).Value, ((Number)ushort.MaxValue).Value);
        Assert.AreEqual(new Number(byte.MaxValue).Value, ((Number)byte.MaxValue).Value);
    }

    /// <summary>
    /// Tests that the Number converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Dictionary<Number, string> expressions = new Dictionary<Number, string>()
        {
            { new Number(0.1234), "0.1234" },
            { new Number(1234), "1234" },
            { new Number(436.246), "436.246" },
            { new Number(1230), "1230" },
            { new Number(489.38400), "489.384" }
        };

        foreach (Number expression in expressions.Keys)
            Assert.AreEqual(expressions[expression], expression.ToString());
    }
}
