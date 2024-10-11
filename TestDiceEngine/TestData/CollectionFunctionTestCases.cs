using DiceEngine.Expressions;
using DiceEngine.Expressions.Collections;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.TestData;

public static partial class TestCases
{
    public readonly static FunctionTestCase[] CollectionFunctions =
    [
        new FunctionTestCase("map", new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), new Function(TestValues.F)], new Vector() },
            { [UtilFunctions.Range<Vector>(1, 10), new Function(TestValues.F)], UtilFunctions.Range<Vector>(1, 10) },
            { [UtilFunctions.Range<Vector>(1, 10), new Function(TestValues.SubFromTen)],
                UtilFunctions.Range<Vector>(9, 10, -1) },
            { [new Set(), new Function(TestValues.F)], new Set() },
            { [UtilFunctions.Range<Set>(1, 10), new Function(TestValues.F)], UtilFunctions.Range<Set>(1, 10) },
            { [UtilFunctions.Range<Set>(1, 10), new Function(TestValues.SubFromTen)],
                UtilFunctions.Range<Set>(9, 10, -1) },
        }),
        new FunctionTestCase(["filter", "where"], new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), new Function(TestValues.IsEven)], new Vector() },
            { [UtilFunctions.Range<Vector>(1, 10), new Function(TestValues.IsEven)],
                UtilFunctions.Range<Vector>(2, 5, 2) },
            { [UtilFunctions.Range<Vector>(2, 10, 2), new Function(TestValues.IsEven)],
                UtilFunctions.Range<Vector>(2, 10, 2) },
            { [UtilFunctions.Range<Vector>(1, 10, 2), new Function(TestValues.IsEven)], new Vector() },
            { [new Set(), new Function(TestValues.IsEven)], new Set() },
            { [UtilFunctions.Range<Set>(1, 10), new Function(TestValues.IsEven)], UtilFunctions.Range<Set>(2, 5, 2) },
            { [UtilFunctions.Range<Set>(2, 10, 2), new Function(TestValues.IsEven)],
                UtilFunctions.Range<Set>(2, 10, 2) },
            { [UtilFunctions.Range<Set>(1, 10, 2), new Function(TestValues.IsEven)], new Set() },
        }),
        new FunctionTestCase("aggregate", new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), new Function(TestValues.Aggregate)], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), new Function(TestValues.Aggregate)], (Number)55 },
            { [UtilFunctions.Range<Vector>(2, 10, 2), new Function(TestValues.Aggregate)], (Number)110 },
            { [new Set(), new Function(TestValues.Aggregate)], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10), new Function(TestValues.Aggregate)], (Number)55 },
            { [UtilFunctions.Range<Set>(1, 10, 2), new Function(TestValues.Aggregate)], (Number)100 },
        }),
        new FunctionTestCase("range", new Dictionary<IExpression[], IExpression>
        {
            { [(Number)1, (Number)10, (Number)1], UtilFunctions.Range<Vector>(1, 10) },
            { [(Number)1, (Number)10, (Number)2], UtilFunctions.Range<Vector>(1, 10, 2) },
            { [(Number)10, (Number)6, (Number)7], UtilFunctions.Range<Vector>(10, 6, 7) },
            { [(Number)0, (Number)7, (Number)(-2)], UtilFunctions.Range<Vector>(0, 7, -2) },
            { [(Number)4, (Number)(-3), (Number)1], Undefined.UNDEFINED },
        }),
        // The 'random' function is not deterministic, so we can't test it here.
        new FunctionTestCase(["sort", "order"], new Dictionary<IExpression, IExpression>
        {
            { new Vector(), new Vector() },
            { UtilFunctions.Range<Vector>(1, 10), UtilFunctions.Range<Vector>(1, 10) },
            { new Vector([(Number)5, (Number)4, (Number)2, (Number)3]),
                new Vector([(Number)2, (Number)3, (Number)4, (Number)5]) },
            { new Set(), new Set() },
            { UtilFunctions.Range<Set>(1, 10), UtilFunctions.Range<Set>(1, 10) },
            { new Set([(Number)5, (Number)4, (Number)2, (Number)3]),
                new Set([(Number)5, (Number)4, (Number)2, (Number)3]) },
        }),
        new FunctionTestCase(["concat", "concatenate"], new Dictionary<IExpression[], IExpression>
        {
            { [new Set(), new Vector()], new Set() },
            { [UtilFunctions.Range<Vector>(1, 10), UtilFunctions.Range<Set>(2, 10)],
                new Vector(UtilFunctions.Range<Vector>(1, 10).Concat(UtilFunctions.Range<Vector>(2, 10))) },
            { [new Set([(Number)3]), new Set([(Number)2])], new Set([(Number)2, (Number)3]) },
        }),
        new FunctionTestCase("append", new Dictionary<IExpression[], IExpression>
        {
            { [new Set(), (Number)1], new Set([(Number)1]) },
            { [UtilFunctions.Range<Vector>(1, 10), UtilFunctions.Range<Set>(2, 10)],
                new Vector(UtilFunctions.Range<Vector>(1, 10).Append(UtilFunctions.Range<Set>(2, 10))) },
            { [new Set([(Number)3]), new Set([(Number)2])], new Set([(Number)3, new Set([(Number)2])]) },
        }),
        new FunctionTestCase("prepend", new Dictionary<IExpression[], IExpression>
        {
            { [new Set(), (Number)1], new Set([(Number)1]) },
            { [UtilFunctions.Range<Vector>(1, 10), UtilFunctions.Range<Set>(2, 10)],
                new Vector(UtilFunctions.Range<Vector>(1, 10).Prepend(UtilFunctions.Range<Set>(2, 10))) },
            { [new Set([(Number)3]), new Set([(Number)2])], new Set([(Number)3, new Set([(Number)2])]) },
        }),
        new FunctionTestCase("insert", new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), TestValues.INFINITY, (Number)0], new Vector([TestValues.INFINITY]) },
            { [UtilFunctions.Range<Vector>(1, 10), TestValues.INFINITY, (Number)0],
                new Vector(UtilFunctions.Range<Vector>(1, 10).Prepend(TestValues.INFINITY)) },
            { [UtilFunctions.Range<Vector>(1, 10), TestValues.INFINITY, (Number)4],
                new Vector(UtilFunctions.Range<Vector>(1, 4)
                    .Append(TestValues.INFINITY)
                    .Concat(UtilFunctions.Range<Vector>(5, 6))) },
            { [UtilFunctions.Range<Vector>(1, 10), TestValues.INFINITY, (Number)10],
                new Vector(UtilFunctions.Range<Vector>(1, 10).Append(TestValues.INFINITY)) },
            { [UtilFunctions.Range<Vector>(1, 10), TestValues.INFINITY, (Number)11], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), TestValues.INFINITY, (Number)(-3)],
                new Vector(UtilFunctions.Range<Vector>(1, 7)
                    .Append(TestValues.INFINITY)
                    .Concat(UtilFunctions.Range<Vector>(8, 3))) },
            { [new Set(), TestValues.INFINITY, (Number)0], new Set([TestValues.INFINITY]) },
            { [UtilFunctions.Range<Set>(1, 10), TestValues.INFINITY, (Number)0],
                new Set(UtilFunctions.Range<Set>(1, 10).Prepend(TestValues.INFINITY)) },
            { [UtilFunctions.Range<Set>(1, 10), TestValues.INFINITY, (Number)4],
                new Set(UtilFunctions.Range<Set>(1, 4)
                    .Append(TestValues.INFINITY)
                    .Concat(UtilFunctions.Range<Set>(5, 6))) },
            { [UtilFunctions.Range<Set>(1, 10), TestValues.INFINITY, (Number)10],
                new Set(UtilFunctions.Range<Set>(1, 10).Append(TestValues.INFINITY)) },
            { [UtilFunctions.Range<Set>(1, 10), TestValues.INFINITY, (Number)11], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10), TestValues.INFINITY, (Number)(-3)],
                new Set(UtilFunctions.Range<Set>(1, 7)
                    .Append(TestValues.INFINITY)
                    .Concat(UtilFunctions.Range<Set>(8, 3))) },
        }),
        new FunctionTestCase("remove", new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), (Number)0], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)0], UtilFunctions.Range<Vector>(2, 9) },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)4],
                new Vector(UtilFunctions.Range<Vector>(1, 4).Concat(UtilFunctions.Range<Vector>(6, 5))) },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)10], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)(-3)],
                new Vector(UtilFunctions.Range<Vector>(1, 7).Concat([(Number)9, (Number)10])) },
            { [new Set(), (Number)0], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10), (Number)0], UtilFunctions.Range<Set>(2, 9) },
            { [UtilFunctions.Range<Set>(1, 10), (Number)4],
                new Set(UtilFunctions.Range<Set>(1, 4).Concat(UtilFunctions.Range<Set>(6, 5))) },
            { [UtilFunctions.Range<Set>(1, 10), (Number)10], TestValues.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10), (Number)(-3)],
                new Set(UtilFunctions.Range<Set>(1, 7).Concat([(Number)9, (Number)10])) },
        }),
        new FunctionTestCase(["any", "some"], new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), new Function(TestValues.IsEven)], Logical.FALSE },
            { [UtilFunctions.Range<Vector>(1, 10), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Vector>(2, 10, 2), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Vector>(1, 10, 2), new Function(TestValues.IsEven)], Logical.FALSE },
            { [new Set(), new Function(TestValues.IsEven)], Logical.FALSE },
            { [UtilFunctions.Range<Set>(1, 10), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Set>(2, 10, 2), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Set>(1, 10, 2), new Function(TestValues.IsEven)], Logical.FALSE },
        }),
        new FunctionTestCase("all", new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Vector>(1, 10), new Function(TestValues.IsEven)], Logical.FALSE },
            { [UtilFunctions.Range<Vector>(2, 10, 2), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Vector>(1, 10, 2), new Function(TestValues.IsEven)], Logical.FALSE },
            { [new Set(), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Set>(1, 10), new Function(TestValues.IsEven)], Logical.FALSE },
            { [UtilFunctions.Range<Set>(2, 10, 2), new Function(TestValues.IsEven)], Logical.TRUE },
            { [UtilFunctions.Range<Set>(1, 10, 2), new Function(TestValues.IsEven)], Logical.FALSE },
        }),
        new FunctionTestCase("find", new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)5], (Number)4 },
            { [UtilFunctions.Range<Vector>(2, 10, 2), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10, 2), (Number)5], (Number)2 },
            { [new Vector([(Number)5, (Number)5]), (Number)5], (Number)0 },
            { [new Set(), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10), (Number)5], (Number)4 },
            { [UtilFunctions.Range<Set>(2, 10, 2), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10, 2), (Number)5], (Number)2 },
        }),
        new FunctionTestCase("find_last", new Dictionary<IExpression[], IExpression>
        {
            { [new Vector(), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10), (Number)5], (Number)4 },
            { [UtilFunctions.Range<Vector>(2, 10, 2), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Vector>(1, 10, 2), (Number)5], (Number)2 },
            { [new Vector([(Number)5, (Number)5]), (Number)5], (Number)1 },
            { [new Set(), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10), (Number)5], (Number)4 },
            { [UtilFunctions.Range<Set>(2, 10, 2), (Number)5], Undefined.UNDEFINED },
            { [UtilFunctions.Range<Set>(1, 10, 2), (Number)5], (Number)2 },
        }),
        new FunctionTestCase("reverse", new Dictionary<IExpression, IExpression>
        {
            { new Vector(), new Vector() },
            { UtilFunctions.Range<Vector>(1, 10), UtilFunctions.Range<Vector>(10, 10, -1) },
            { UtilFunctions.Range<Vector>(2, 10, 2), UtilFunctions.Range<Vector>(20, 10, -2) },
            { UtilFunctions.Range<Vector>(1, 10, 2), UtilFunctions.Range<Vector>(19, 10, -2) },
            { UtilFunctions.Range<Set>(1, 10), UtilFunctions.Range<Set>(10, 10, -1) },
        }),
        new FunctionTestCase("zip", new Dictionary<IExpression[], IExpression>
        {
            { [new Set(), new Vector(), new Function(TestValues.Aggregate)], new Set() },
            { [UtilFunctions.Range<Vector>(1, 10), UtilFunctions.Range<Set>(1, 10), new Function(TestValues.Aggregate)],
                new Vector(UtilFunctions.Range<Vector>(2, 10, 2)) },
            { [new Set([(Number)3]), new Set([(Number)2]), new Function(TestValues.Aggregate)], new Set([(Number)5]) },
        }),
    ];
}
