using DiceEngine.Expressions;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.TestData;

public static partial class TestCases
{
    public readonly static FunctionTestCase[] FactorialFunctions =
    [
        new FunctionTestCase("factorial", new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { TestValues.INFINITY, TestValues.INFINITY },
            { TestValues.NEGATIVE_INFINITY, TestValues.UNDEFINED },
            { TestValues.ZERO, TestValues.ONE },
            { TestValues.ONE, TestValues.ONE },
            { (Number)2, (Number)2 },
            { (Number)3, (Number)6 },
            { (Number)4, (Number)24 },
            { (Number)5, (Number)120 },
            { TestValues.NEGATIVE_ONE, TestValues.UNDEFINED },
            { TestValues.PI, TestValues.UNDEFINED },
        }),
        new FunctionTestCase("subfactorial", new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { TestValues.INFINITY, TestValues.INFINITY },
            { TestValues.NEGATIVE_INFINITY, TestValues.UNDEFINED },
            { TestValues.ZERO, TestValues.ONE },
            { TestValues.ONE, TestValues.ZERO },
            { (Number)2, (Number)1 },
            { (Number)3, (Number)2 },
            { (Number)4, (Number)9 },
            { (Number)5, (Number)44 },
            { TestValues.NEGATIVE_ONE, TestValues.UNDEFINED },
            { TestValues.PI, TestValues.UNDEFINED },
        }),
        new FunctionTestCase("primorial", new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { TestValues.INFINITY, TestValues.INFINITY },
            { TestValues.NEGATIVE_INFINITY, TestValues.UNDEFINED },
            { TestValues.ZERO, TestValues.ONE },
            { TestValues.ONE, (Number)2 },
            { (Number)2, (Number)6 },
            { (Number)3, (Number)30 },
            { (Number)4, (Number)210 },
            { (Number)5, (Number)2310 },
            { TestValues.NEGATIVE_ONE, TestValues.UNDEFINED },
            { TestValues.PI, TestValues.UNDEFINED },
        }),
        new FunctionTestCase("double_factorial", new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { TestValues.INFINITY, TestValues.INFINITY },
            { TestValues.NEGATIVE_INFINITY, TestValues.UNDEFINED },
            { TestValues.ZERO, TestValues.ONE },
            { TestValues.ONE, TestValues.ONE },
            { (Number)2, (Number)2 },
            { (Number)3, (Number)3 },
            { (Number)4, (Number)8 },
            { (Number)5, (Number)15 },
            { TestValues.NEGATIVE_ONE, TestValues.UNDEFINED },
            { TestValues.PI, TestValues.UNDEFINED },
        }),
    ];
}
