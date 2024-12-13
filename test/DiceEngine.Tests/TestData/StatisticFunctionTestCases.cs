using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Terminals;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.TestData;

public static partial class TestCases
{
    public readonly static FunctionTestCase[] StatisticalTestCases =
    [
        new FunctionTestCase(["count", "length", "len"], new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, (Number)1 },
            { UtilFunctions.Range<Vector>(1, 10), (Number)10 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)10 },
        }),
        new FunctionTestCase(["max", "maximum"], new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), (Number)10 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)20 },
        }),
        new FunctionTestCase(["average", "mean"], new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), (Number)5.5 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)11 },
        }),
        new FunctionTestCase("median", new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), (Number)5.5 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)11 },
        }),
        new FunctionTestCase(["min", "minimum"], new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), (Number)1 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)2 },
        }),
        new FunctionTestCase("mode", new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), UtilFunctions.Range<Set>(1, 10) },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)),
                UtilFunctions.Range<Set>(1, 10).Map(x => (Number)(((Number)x).Value * 2)) },
            { new Vector([(Number)1, (Number)1, (Number)2]), (Number)1 },
        }),
        new FunctionTestCase("percentile", new Dictionary<IExpression[], IExpression>
        {
            { [TestValues.UNDEFINED, TestValues.UNDEFINED], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0], (Number)1 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.11111111111111111111111111111], (Number)2 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.22222222222222222222222222222], (Number)3 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.33333333333333333333333333333], (Number)4 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.44444444444444444444444444444], (Number)5 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.55555555555555555555555555556], (Number)6 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.66666666666666666666666666667], (Number)7 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.77777777777777777777777777778], (Number)8 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0.88888888888888888888888888889], (Number)9 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)1], (Number)10 },
        }),
        new FunctionTestCase("quartile", new Dictionary<IExpression[], IExpression>
        {
            { [TestValues.UNDEFINED, TestValues.UNDEFINED], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0], (Number)1 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)1], (Number)3.25 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)2], (Number)5.5 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)3], (Number)7.75 },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)4], (Number)10 },
        }),
        new FunctionTestCase(["stdev", "stdevs"], new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), (Number)3.02765035409749 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)6.05530070819498 },
        }),
        new FunctionTestCase("stdevp", new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), (Number)2.87228132326901 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)5.74456264653803 },
        }),
        new FunctionTestCase(["sum", "total"], new Dictionary<IExpression, IExpression>
        {
            { TestValues.UNDEFINED, TestValues.UNDEFINED },
            { UtilFunctions.Range<Vector>(1, 10), (Number)55 },
            { UtilFunctions.Range<Vector>(1, 10).Map(x => (Number)(((Number)x).Value * 2)), (Number)110 },
        }),
    ];
}
