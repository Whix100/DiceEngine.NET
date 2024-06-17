﻿using DiceEngine.Expressions;
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
        new TestCase("1d1", new DiceSet(1, (Die)1), new RandomRollResult(1, 1, 1, 1,
            enumerableValidator: x => CompareDieToResult(new DiceSet(1, (Die)1), x))),
        new TestCase("2d2", new DiceSet(2, (Die)2), new RandomRollResult(2, 2, 1, 2,
            enumerableValidator: x => CompareDieToResult(new DiceSet(2, (Die)2), x))),
        new TestCase("3d4", new DiceSet(3, (Die)4), new RandomRollResult(3, 3, 1, 4,
            enumerableValidator: x => CompareDieToResult(new DiceSet(3, (Die)4), x))),
        new TestCase("4d6", new DiceSet(4, (Die)6), new RandomRollResult(4, 4, 1, 6,
            enumerableValidator: x => CompareDieToResult(new DiceSet(4, (Die)6), x))),
        new TestCase("5d8", new DiceSet(5, (Die)8), new RandomRollResult(5, 5, 1, 8,
            enumerableValidator: x => CompareDieToResult(new DiceSet(5, (Die)8), x))),
        new TestCase("6d10", new DiceSet(6, (Die)10), new RandomRollResult(6, 6, 1, 10,
            enumerableValidator: x => CompareDieToResult(new DiceSet(6, (Die)10), x))),
        new TestCase("7d12", new DiceSet(7, (Die)12), new RandomRollResult(7, 7, 1, 12,
            enumerableValidator: x => CompareDieToResult(new DiceSet(7, (Die)12), x))),
        new TestCase("8d20", new DiceSet(8, (Die)20), new RandomRollResult(8, 8, 1, 20,
            enumerableValidator: x => CompareDieToResult(new DiceSet(8, (Die)20), x))),
        new TestCase("9d100", new DiceSet(9, (Die)100), new RandomRollResult(9, 9, 1, 100,
            enumerableValidator: x => CompareDieToResult(new DiceSet(9, (Die)100), x))),
        new TestCase("45d%", new DiceSet(45, new PercentileDie()), new RandomRollResult(45, 45, 0, 90,
            x => x % 10 == 0,
            x => CompareDieToResult(new DiceSet(45, new PercentileDie()), x))),
        new TestCase("33dF", new DiceSet(33, new FateDie()), new RandomRollResult(33, 33, -1, 1,
            enumerableValidator: x => CompareDieToResult(new DiceSet(33, new FateDie()), x))),
    ];

    private static bool CompareDieToResult(IDie expected, IEnumerable<RollValue> actual)
        => actual is RollResult result && expected.Equals(result.Die);
}
