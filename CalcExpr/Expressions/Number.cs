﻿namespace CalcExpr.Expressions;

public class Number : IExpression
{
    public double Value { get; private set; }

    public Number(double value)
        => Value = value;

    public IExpression Evaluate()
        => Clone();

    public IExpression Simplify()
        => Clone();

    public IExpression StepEvaluate()
        => Clone();

    public IExpression StepSimplify()
        => Clone();

    public IExpression Clone()
        => new Number(Value);

    public override string ToString()
        => throw new NotImplementedException();

    public static explicit operator decimal(Number n)
        => (decimal)n.Value;

    public static explicit operator double(Number n)
        => n.Value;

    public static explicit operator float(Number n)
        => (float)n.Value;

    public static explicit operator long(Number n)
        => (long)n.Value;

    public static explicit operator int(Number n)
        => (int)n.Value;

    public static explicit operator short(Number n)
        => (short)n.Value;

    public static explicit operator sbyte(Number n)
        => (sbyte)n.Value;

    public static explicit operator ulong(Number n)
        => (ulong)n.Value;

    public static explicit operator uint(Number n)
        => (uint)n.Value;

    public static explicit operator ushort(Number n)
        => (ushort)n.Value;

    public static explicit operator byte(Number n)
        => (byte)n.Value;

    public static explicit operator Number(decimal n)
        => new Number((double)n);

    public static explicit operator Number(double n)
        => new Number(n);

    public static explicit operator Number(float n)
        => new Number(n);

    public static explicit operator Number(long n)
        => new Number(n);

    public static explicit operator Number(int n)
        => new Number(n);

    public static explicit operator Number(short n)
        => new Number(n);

    public static explicit operator Number(sbyte n)
        => new Number(n);

    public static explicit operator Number(ulong n)
        => new Number(n);

    public static explicit operator Number(uint n)
        => new Number(n);

    public static explicit operator Number(ushort n)
        => new Number(n);

    public static explicit operator Number(byte n)
        => new Number(n);
}
