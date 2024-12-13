using DiceEngine.Context;
using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;
using DiceEngine.Parsing;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestVariable
{
    static readonly string[] VALID_VARIABLES =
    {
        "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz",
        "ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσςΤτΥυΦφΧχΨψΩω",
        "abc_123",
        "αβγ_123",
        "abcd_αβγ_xyz",
    };

    /// <summary>
    /// Tests that the name of the Variable is unchanged when initialized.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        foreach (string name in VALID_VARIABLES)
            Assert.AreEqual(name, new Variable(name).Name);
    }

    public void TestAssignment()
    {
        ExpressionContext context = new ExpressionContext();
        Parser parser = new Parser();
        Random random = new Random();

        foreach (string name in VALID_VARIABLES)
        {
            double value = random.Next() + random.NextDouble();

            parser.Parse($"{name}={value}").Evaluate(context);
            Assert.IsTrue(context.ContainsVariable(name));
            Assert.AreEqual(new Number(value), context[name]);
        }
    }

    /// <summary>
    /// Tests that the UnaryOperator converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        foreach (string variable in VALID_VARIABLES)
            Assert.AreEqual(variable, new Variable(variable).ToString());
    }
}
