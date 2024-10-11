﻿using DiceEngine.Expressions;
using DiceEngine.Expressions.Components;
using DiceEngine.Expressions.Dice;
using TestDiceEngine.TestModels;
using TestDiceEngine.TestUtils;

namespace TestDiceEngine.TestData;

public static partial class TestCases
{
    public readonly static TestCase[] DiceNotation =
    [
        new TestCase("d1", (Die)1, new RandomRollResult(1, 1, 1, 1,
            enumerableValidator: x => CompareDieToResult((Die)1, x))),
        new TestCase("d2", (Die)2, new RandomRollResult(1, 1, 1, 2,
            enumerableValidator: x => CompareDieToResult((Die)2, x))),
        new TestCase("d4", (Die)4, new RandomRollResult(1, 1, 1, 4,
            enumerableValidator: x => CompareDieToResult((Die)4, x))),
        new TestCase("d6", (Die)6, new RandomRollResult(1, 1, 1, 6,
            enumerableValidator: x => CompareDieToResult((Die)6, x))),
        new TestCase("d8", (Die)8, new RandomRollResult(1, 1, 1, 8,
            enumerableValidator: x => CompareDieToResult((Die)8, x))),
        new TestCase("d10", (Die)10, new RandomRollResult(1, 1, 1, 10,
            enumerableValidator: x => CompareDieToResult((Die)10, x))),
        new TestCase("d12", (Die)12, new RandomRollResult(1, 1, 1, 12,
            enumerableValidator: x => CompareDieToResult((Die)12, x))),
        new TestCase("d20", (Die)20, new RandomRollResult(1, 1, 1, 20,
            enumerableValidator: x => CompareDieToResult((Die)20, x))),
        new TestCase("d100", (Die)100, new RandomRollResult(1, 1, 1, 100,
            enumerableValidator: x => CompareDieToResult((Die)100, x))),
        new TestCase("1+d1", new BinaryOperator("+", (Number)1, (Die)1),
            new RandomValue(2, 2), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 1, enumerableValidator: x => CompareDieToResult((Die)1, x)))),
        new TestCase("1+d2", new BinaryOperator("+", (Number)1, (Die)2),
            new RandomValue(2, 3), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 2, enumerableValidator: x => CompareDieToResult((Die)2, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 2))),
        new TestCase("1+d4", new BinaryOperator("+", (Number)1, (Die)4),
            new RandomValue(2, 5), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 4, enumerableValidator: x => CompareDieToResult((Die)4, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 4))),
        new TestCase("1+d6", new BinaryOperator("+", (Number)1, (Die)6),
            new RandomValue(2, 7), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 6, enumerableValidator: x => CompareDieToResult((Die)6, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 6))),
        new TestCase("1+d8", new BinaryOperator("+", (Number)1, (Die)8),
            new RandomValue(2, 9), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 8, enumerableValidator: x => CompareDieToResult((Die)8, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 8))),
        new TestCase("1+d10", new BinaryOperator("+", (Number)1, (Die)10),
            new RandomValue(2, 11), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 10, enumerableValidator: x => CompareDieToResult((Die)10, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 10))),
        new TestCase("1+d12", new BinaryOperator("+", (Number)1, (Die)12),
            new RandomValue(2, 13), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 12, enumerableValidator: x => CompareDieToResult((Die)12, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 12))),
        new TestCase("1+d20", new BinaryOperator("+", (Number)1, (Die)20),
            new RandomValue(2, 21), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 20, enumerableValidator: x => CompareDieToResult((Die)20, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 20))),
        new TestCase("1+d100", new BinaryOperator("+", (Number)1, (Die)100),
            new RandomValue(2, 101), new BinaryOperator("+", (Number)1,
                new RandomRollResult(1, 1, 1, 100, enumerableValidator: x => CompareDieToResult((Die)100, x))),
            new BinaryOperator("+", (Number)1, new RandomValue(1, 100))),
        new TestCase("d%", new PercentileDie(), new RandomRollResult(1, 1, 0, 90,
            x => x % 10 == 0, x => CompareDieToResult(new PercentileDie(), x))),
        new TestCase("dF", new FateDie(), new RandomRollResult(1, 1, -1, 1,
            enumerableValidator: x => CompareDieToResult(new FateDie(), x))),
        new TestCase("1d1", new DiceSet(1, 1), new RandomRollResult(1, 1, 1, 1,
            enumerableValidator: x => CompareDieToResult(new DiceSet(1, 1), x))),
        new TestCase("2d2", new DiceSet(2, 2), new RandomRollResult(2, 2, 1, 2,
            enumerableValidator: x => CompareDieToResult(new DiceSet(2, 2), x))),
        new TestCase("3d4", new DiceSet(3, 4), new RandomRollResult(3, 3, 1, 4,
            enumerableValidator: x => CompareDieToResult(new DiceSet(3, 4), x))),
        new TestCase("4d6", new DiceSet(4, 6), new RandomRollResult(4, 4, 1, 6,
            enumerableValidator: x => CompareDieToResult(new DiceSet(4, 6), x))),
        new TestCase("5d8", new DiceSet(5, 8), new RandomRollResult(5, 5, 1, 8,
            enumerableValidator: x => CompareDieToResult(new DiceSet(5, 8), x))),
        new TestCase("6d10", new DiceSet(6, 10), new RandomRollResult(6, 6, 1, 10,
            enumerableValidator: x => CompareDieToResult(new DiceSet(6, 10), x))),
        new TestCase("7d12", new DiceSet(7, 12), new RandomRollResult(7, 7, 1, 12,
            enumerableValidator: x => CompareDieToResult(new DiceSet(7, 12), x))),
        new TestCase("8d20", new DiceSet(8, 20), new RandomRollResult(8, 8, 1, 20,
            enumerableValidator: x => CompareDieToResult(new DiceSet(8, 20), x))),
        new TestCase("9d100", new DiceSet(9, 100), new RandomRollResult(9, 9, 1, 100,
            enumerableValidator: x => CompareDieToResult(new DiceSet(9, 100), x))),
        new TestCase("45d%", new DiceSet(45, new PercentileDie()), new RandomRollResult(45, 45, 0, 90,
            x => x % 10 == 0,
            x => CompareDieToResult(new DiceSet(45, new PercentileDie()), x))),
        new TestCase("33dF", new DiceSet(33, new FateDie()), new RandomRollResult(33, 33, -1, 1,
            enumerableValidator: x => CompareDieToResult(new DiceSet(33, new FateDie()), x))),
        // Test selectors with keep operator.
        new TestCase("10d10k1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector(1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector(1))),
        new TestCase("10d10kh1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector("h", 1)),
            new RandomRollResult(1, 1, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("h", 1))),
        new TestCase("10d10kl1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector("l", 1)),
            new RandomRollResult(1, 1, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("l", 1))),
        new TestCase("10d10k<1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector("<", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("<", 1))),
        new TestCase("10d10k<=1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector("<=", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("<=", 1))),
        new TestCase("10d10k≤1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector("≤", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("≤", 1))),
        new TestCase("10d10k>1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector(">", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector(">", 1))),
        new TestCase("10d10k>=1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector(">=", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector(">=", 1))),
        new TestCase("10d10k≥1", new DiceOperator("k", new DiceSet(10, 10), new ResultSelector("≥", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("≥", 1))),
        // Test selectors with drop operator.
        new TestCase("10d10d1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector(1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector(1))),
        new TestCase("10d10dh1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector("h", 1)),
            new RandomRollResult(9, 9, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("h", 1))),
        new TestCase("10d10dl1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector("l", 1)),
            new RandomRollResult(9, 9, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("l", 1))),
        new TestCase("10d10d<3", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector("<", 3)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("<", 3))),
        new TestCase("10d10d<=1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector("<=", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("<=", 1))),
        new TestCase("10d10d≤1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector("≤", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("≤", 1))),
        new TestCase("10d10d>1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector(">", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector(">", 1))),
        new TestCase("10d10d>=1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector(">=", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector(">=", 1))),
        new TestCase("10d10d≥1", new DiceOperator("d", new DiceSet(10, 10), new ResultSelector("≥", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("≥", 1))),
        // Test selectors withe other operators.
        new TestCase("3d10rr1", new DiceOperator("rr", new DiceSet(3, 10), new ResultSelector(1)),
            new RandomRollResult(3, 3, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(3, 10), x)),
            new DiceOperator("rr", new RandomRollResult(3, 3, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(3, 10), x)), new ResultSelector(1))),
        new TestCase("10d10roh1", new DiceOperator("ro", new DiceSet(10, 10), new ResultSelector("h", 1)),
            new RandomRollResult(10, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("ro", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("h", 1))),
        new TestCase("10d10ral1", new DiceOperator("ra", new DiceSet(10, 10), new ResultSelector("l", 1)),
            new RandomRollResult(10, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("ra", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("l", 1))),
        new TestCase("3d10e3", new DiceOperator("e", new DiceSet(3, 10), new ResultSelector(3)),
            new RandomRollResult(3, null, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(3, 10), x)),
            new DiceOperator("e", new RandomRollResult(3, 3, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(3, 10), x)), new ResultSelector(3))),
        new TestCase("10d10ro≤1", new DiceOperator("ro", new DiceSet(10, 10), new ResultSelector("≤", 1)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("ro", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("≤", 1))),
        new TestCase("10d10ra>1", new DiceOperator("ra", new DiceSet(10, 10), new ResultSelector(">", 1)),
            new RandomRollResult(10, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("ra", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector(">", 1))),
        new TestCase("10d10e<=1", new DiceOperator("e", new DiceSet(10, 10), new ResultSelector("<=", 1)),
            new RandomRollResult(10, null, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("e", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("<=", 1))),
        new TestCase("10d10mi5", new DiceOperator("mi", new DiceSet(10, 10), new ResultSelector("", 5)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("mi", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("", 5))),
        new TestCase("10d10ma5", new DiceOperator("ma", new DiceSet(10, 10), new ResultSelector("", 5)),
            new RandomRollResult(0, 10, 1, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("ma", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), new ResultSelector("", 5))),
        new TestCase("10d10<=1", new BinaryOperator("<=", new DiceSet(10, 10), (Number)1),
            Logical.FALSE,
            new BinaryOperator("<=", new RandomRollResult(10, 10, 1, 10,
                enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)), (Number)1),
            new BinaryOperator("<=", new RandomValue(10, 100), (Number)1)),
        new TestCase("10d10k>1dl1", new DiceOperator("d",
                new DiceOperator("k", new DiceSet(10, 10), new ResultSelector(">", 1)),
                new ResultSelector("l", 1)),
            new RandomRollResult(0, 9, 2, 10, enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
            new DiceOperator("d",
                new DiceOperator("k", new RandomRollResult(10, 10, 1, 10,
                        enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
                    new ResultSelector(">", 1)),
                new ResultSelector("l", 1)),
            new DiceOperator("d", new RandomRollResult(0, 10, 2, 10,
                    enumerableValidator: x => CompareDieToResult(new DiceSet(10, 10), x)),
                new ResultSelector("l", 1))),
    ];

    private static bool CompareDieToResult(IDie expected, IEnumerable<RollValue> actual)
        => actual is RollResult result && expected.Equals(result.Die);
}
