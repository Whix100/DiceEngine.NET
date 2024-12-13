using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestBinaryOperator
{
    private string[] _operators =
    {
        "+",
        "-",
        "*",
        "×",
        "/",
        "÷",
        "^",
        "%",
        "%%",
        "//",
        "&&",
        "∧",
        "||",
        "∨",
        "⊕",
        "==",
        "!=",
        "≠",
        "<>",
        "<",
        ">",
        ">=",
        "≥",
        "<=",
        "≤",
    };

    /// <summary>
    /// Tests that the properties of the BinaryOperator are the same as when initialized.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        foreach (string op in _operators)
        {
            Number a = new Number(random.NextDouble() + random.Next());
            Number b = new Number(random.NextDouble() + random.Next());
            BinaryOperator unop = new BinaryOperator(op, a, b);

            Assert.AreEqual(op, unop.Identifier);
            Assert.AreEqual(a, unop.Left);
            Assert.AreEqual(b, unop.Right);
        }
    }

    /// <summary>
    /// Tests that the BinaryOperator converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Dictionary<BinaryOperator, string> expressions = new Dictionary<BinaryOperator, string>()
        {
            { new BinaryOperator("+", new Number(1), new Number(2)), "1+2" },
            { new BinaryOperator("-", new Number(1), new Number(2)), "1-2" },
            { new BinaryOperator("*", new Number(2), new Number(3)), "2*3" },
            { new BinaryOperator("×", new Number(6), new Number(7)), "6×7" },
            { new BinaryOperator("/", new Number(1), new Number(2)), "1/2" },
            { new BinaryOperator("÷", new Number(2), new Number(2)), "2÷2" },
            { new BinaryOperator("^", new Number(2), new Number(3)), "2^3" },
            { new BinaryOperator("%", new Number(13), new Number(12)), "13%12" },
            { new BinaryOperator("%", new Number(-13), new Number(-12)), "-13%-12" },
            { new BinaryOperator("%", new Number(13.2), new Number(12.5)), "13.2%12.5" },
            { new BinaryOperator("%", new Number(-13.2), new Number(-12.5)), "-13.2%-12.5" },
            { new BinaryOperator("%%", new Number(13), new Number(12)), "13%%12" },
            { new BinaryOperator("%%", new Number(-13), new Number(-12)), "-13%%-12" },
            { new BinaryOperator("%%", new Number(13.2), new Number(12.5)), "13.2%%12.5" },
            { new BinaryOperator("%%", new Number(-13.2), new Number(-12.5)), "-13.2%%-12.5" },
            { new BinaryOperator("//", new Number(13), new Number(12)), "13//12" },
            { new BinaryOperator("//", new Number(-13), new Number(-12)), "-13//-12" },
            { new BinaryOperator("//", new Number(13.2), new Number(12.5)), "13.2//12.5" },
            { new BinaryOperator("//", new Number(-13.2), new Number(-12.5)), "-13.2//-12.5" },
            { new BinaryOperator("&&", new Number(1), new Number(2)), "1&&2" },
            { new BinaryOperator("&", new Number(0), new Number(2)), "0&2" },
            { new BinaryOperator("∧", new Number(0), new Number(0)), "0∧0" },
            { new BinaryOperator("||", new Number(1), new Number(2)), "1||2" },
            { new BinaryOperator("|", new Number(1), new Number(0)), "1|0" },
            { new BinaryOperator("∨", new Number(0), new Number(0)), "0∨0" },
            { new BinaryOperator("⊕", new Number(1), new Number(1)), "1⊕1" },
            { new BinaryOperator("⊕", new Number(0), new Number(1)), "0⊕1" },
            { new BinaryOperator("⊕", new Number(0), new Number(0)), "0⊕0" },
            { new BinaryOperator("==", new Number(1), new Number(2)), "1==2" },
            { new BinaryOperator("!=", new Number(1), new Number(2)), "1!=2" },
            { new BinaryOperator("≠", new Number(1), new Number(1)), "1≠1" },
            { new BinaryOperator("<>", new Number(1), new Number(2)), "1<>2" },
            { new BinaryOperator("<", new Number(1), new Number(2)), "1<2" },
            { new BinaryOperator(">", new Number(1), new Number(2)), "1>2" },
            { new BinaryOperator(">=", new Number(1), new Number(2)), "1>=2" },
            { new BinaryOperator("≥", new Number(2), new Number(2)), "2≥2" },
            { new BinaryOperator("<=", new Number(1), new Number(2)), "1<=2" },
            { new BinaryOperator("≤", new Number(1), new Number(1)), "1≤1" },
        };

        foreach (BinaryOperator expression in expressions.Keys)
            Assert.AreEqual(expressions[expression], expression.ToString());
    }
}
