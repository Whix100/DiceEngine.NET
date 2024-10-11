using DiceEngine.Expressions;

namespace TestDiceEngine.Expressions;

[TestClass]
public class TestPostfixOperator
{
    private string[] _operators =
    [
        "!",
        "%",
        "!!",
        "#",
        "--",
        "++",
    ];

    /// <summary>
    /// Tests that the properties of the PostfixOperator are the same as when initialized.
    /// </summary>
    [TestMethod]
    public void TestInit()
    {
        Random random = new Random();

        foreach (string op in _operators)
        {
            Number num = new Number(random.NextDouble() + random.Next());
            PostfixOperator unop = new PostfixOperator(op, num);

            Assert.AreEqual(op, unop.Identifier);
            Assert.AreEqual(num, unop.Inside);
        }
    }

    /// <summary>
    /// Tests that the PostfixOperator converts to a string properly.
    /// </summary>
    [TestMethod]
    public void TestToString()
    {
        Dictionary<PostfixOperator, string> expressions = new Dictionary<PostfixOperator, string>()
        {
            { new PostfixOperator("!", new Number(5)), "5!" },
            { new PostfixOperator("%", new Number(1)), "1%" },
            { new PostfixOperator("!!", new Number(5)), "5!!" },
            { new PostfixOperator("#", new Number(5)), "5#" },
            { new PostfixOperator("--", new Variable("abc")), "abc--" },
            { new PostfixOperator("++", new Variable("abc")), "abc++" },
        };

        foreach (PostfixOperator expression in expressions.Keys)
            Assert.AreEqual(expressions[expression], expression.ToString());
    }
}
