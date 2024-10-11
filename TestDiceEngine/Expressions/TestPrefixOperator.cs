using DiceEngine.Expressions;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestPrefixOperator
{
    private string[] _operators =
    [
        "+",
        "-",
        "~",
        "¬",
        "!",
        "--",
        "++",
    ];

    /// <summary>
    /// Tests that the properties of the PrefixOperator are the same as when initialized.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        foreach (string op in _operators)
        {
            Number num = new Number(random.NextDouble() + random.Next());
            PrefixOperator unop = new PrefixOperator(op, num);

            Assert.AreEqual(op, unop.Identifier);
            Assert.AreEqual(num, unop.Inside);
        }
    }

    /// <summary>
    /// Tests that the PrefixOperator converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Dictionary<PrefixOperator, string> expressions = new Dictionary<PrefixOperator, string>()
        {
            { new PrefixOperator("+", new Number(1)), "+1" },
            { new PrefixOperator("-", new Number(1)), "-1" },
            { new PrefixOperator("~", new Number(1)), "~1" },
            { new PrefixOperator("¬", new Number(1)), "¬1" },
            { new PrefixOperator("!", new Number(5)), "!5" },
            { new PrefixOperator("--", new Variable("abc")), "--abc" },
            { new PrefixOperator("++", new Variable("abc")), "++abc" },
            // TODO ± prefix.
        };

        foreach (PrefixOperator expression in expressions.Keys)
            Assert.AreEqual(expressions[expression], expression.ToString());
    }
}
