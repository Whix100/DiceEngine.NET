using DiceEngine.Expressions;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestConstant
{
    static readonly string[] CONSTANTS =
    {
        "∞",
        "inf",
        "infinity",
        "π",
        "pi",
        "τ",
        "tau",
        "e",
        "true",
        "false",
    };

    /// <summary>
    /// Tests that the value of the Constant is the same as the value passed into the Constant's constructor.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        foreach (string constant in CONSTANTS)
            Assert.AreEqual(constant, new Constant(constant).Identifier);
    }

    /// <summary>
    /// Tests that the Constant converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        foreach (string constant in CONSTANTS)
            Assert.AreEqual(constant, new Constant(constant).ToString());
    }
}
