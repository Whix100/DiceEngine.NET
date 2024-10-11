using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DiceEngine.Expressions.Collections;

public class Set(IEnumerable<IExpression> elements) : IEnumerableExpression, IBinaryOperable
{
    private readonly struct HashedExpression(IExpression value) : IComparable
    {
        public IExpression Value { get; } = value;

        public override bool Equals([NotNullWhen(true)] object? obj)
            => obj is not null && (obj is HashedExpression he && Value.Equals(he.Value) ||
                obj is IExpression expr && Value.Equals(expr));

        public override int GetHashCode()
            => Value.GetHashCode();

        public int CompareTo(object? obj)
            => Value.GetHashCode().CompareTo(obj?.GetHashCode() ?? 0);
    }

    private readonly SortedSet<HashedExpression> _elements = [.. elements.Select(e => new HashedExpression(e))];

    public int Count
        => _elements.Count;

    public Set() : this([])
    { }

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
        => new Set(elements.Select(x => x.Evaluate(context)));

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        for (int i = 0; i < Count; i++)
        {
            IExpression element = _elements.ElementAt(i).Value;
            IExpression evaluated = element.StepEvaluate(context);

            if (!evaluated.Equals(element))
                return new Set(_elements.Select((x, j) => j == i ? evaluated : x.Value));
        }

        return this;
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => new Set(this.Select(x => x.EvaluateDice(context)));

    public IExpression? BinaryLeftOperate(string identifier, IExpression right, ExpressionContext _)
    {
        if (right is IEnumerableExpression)
            return identifier switch
            {
                BinaryOperator.IS_EQUAL => (Logical)Equals(right),
                BinaryOperator.NOT_EQUAL or BinaryOperator.NOT_EQUAL_ALT or BinaryOperator.GREATER_OR_LESS_THAN
                    => (Logical)!Equals(right),
                _ => null
            };

        return null;
    }

    public IExpression? BinaryRightOperate(string identifier, IExpression left, ExpressionContext _)
    {
        if (left is IEnumerableExpression)
            return identifier switch
            {
                BinaryOperator.IS_EQUAL => (Logical)Equals(left),
                BinaryOperator.NOT_EQUAL or BinaryOperator.NOT_EQUAL_ALT or BinaryOperator.GREATER_OR_LESS_THAN
                    => (Logical)!Equals(left),
                _ => null
            };

        return null;
    }

    public IEnumerableExpression Map(Func<IExpression, IExpression> map)
        => new Set(_elements.Select(x => map(x.Value)));

    public IEnumerableExpression Combine(IEnumerable<IExpression> expressions,
        Func<IExpression, IExpression, IExpression> combine)
    {
        List<IExpression> result = [];
        int count = expressions.Count();

        for (int i = 0; i < Count && i < count; i++)
            result.Add(combine(_elements.ElementAt(i).Value, expressions.ElementAt(i)));

        return new Set(result);
    }

    public static IEnumerableExpression ConvertIEnumerable(IEnumerable<IExpression> elements)
        => new Set(elements);

    public override bool Equals(object? obj)
    {
        if (obj is not null && obj is Set set)
        {
            bool elements_equal = set.Count == Count &&
                (Count == 0 || !set.Select((arg, i) => arg.Equals(this.ElementAt(i))).Any(x => !x));

            return elements_equal;
        }

        return false;
    }

    public override int GetHashCode()
        => _elements.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{{{String.Join(", ", elements.Select(e => e.ToString(format)))}}}";

    public IEnumerator<IExpression> GetEnumerator()
        => _elements.Select(h => h.Value).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _elements.Select(h => h.Value).GetEnumerator();
}
