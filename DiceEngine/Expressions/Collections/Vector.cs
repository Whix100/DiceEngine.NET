using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;
using System.Collections;

namespace DiceEngine.Expressions.Collections;

public class Vector(IEnumerable<IExpression> elements) : IEnumerableExpression, IBinaryOperable
{
    private readonly IExpression[] _elements = elements.ToArray();

    public IExpression this[int index]
        => _elements[index];

    public IExpression[] this[Range range]
        => _elements[range];

    public int Length
        => _elements.Length;

    public Vector() : this([])
    { }

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
        => new Vector(this.Select(x => x.Evaluate(context)));

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        for (int i = 0; i < Length; i++)
        {
            IExpression evaluated = this[i].StepEvaluate(context);

            if (!this[i].Equals(evaluated))
                return new Vector(this[..i].Append(evaluated).Concat(this[(i + 1)..]));
        }

        return this;
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => new Vector(this.Select(x => x.EvaluateDice(context)));

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
        => ConvertIEnumerable(_elements.Select(x => map(x)));

    public IEnumerableExpression Combine(IEnumerable<IExpression> expressions,
        Func<IExpression, IExpression, IExpression> combine)
    {
        List<IExpression> result = [];
        int count = expressions.Count();

        for (int i = 0; i < Length && i < count; i++)
            result.Add(combine(_elements[i], expressions.ElementAt(i)));

        return new Vector(result);
    }

    public static IEnumerableExpression ConvertIEnumerable(IEnumerable<IExpression> elements)
        => new Vector(elements);

    public override bool Equals(object? obj)
    {
        if (obj is not null && obj is Vector vect)
        {
            bool elements_equal = vect.Length == Length &&
                (Length == 0 || !vect.Select((arg, i) => arg.Equals(this[i])).Any(x => !x));

            return elements_equal;
        }

        return false;
    }

    public override int GetHashCode()
        => _elements.GetHashCode();

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"[{String.Join(", ", elements.Select(e => e.ToString(format)))}]";

    public IEnumerator<IExpression> GetEnumerator()
        => ((IEnumerable<IExpression>)_elements).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _elements.GetEnumerator();
}
