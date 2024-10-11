using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Extensions;

namespace DiceEngine.Expressions;

public class Indexer(IExpression collection, IExpression index) : IExpression
{
    public IExpression Collection { get; } = collection;

    public IExpression Index { get; } = index;

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
    {
        IExpression collection_eval = Collection.Evaluate(context);
        IExpression index_eval = Index.Evaluate(context);

        if (collection_eval is IEnumerableExpression collection_enum && index_eval is Number index_num &&
            index_num.Value > -collection_enum.Count() - 1 && index_num.Value < collection_enum.Count())
        {
            int index = EnumerableExtensions.NormalizeIndex(index_num.Value, collection_enum.Count());

            return collection_enum.ElementAt(index);
        }

        return Undefined.UNDEFINED;
    }

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        IExpression collection_eval = Collection.StepEvaluate(context);

        if (!Collection.Equals(collection_eval))
            return new Indexer(collection_eval, Index);

        IExpression index_eval = Index.StepEvaluate(context);

        if (!Index.Equals(index_eval))
            return new Indexer(Collection, index_eval);

        if (Collection is not IEnumerableExpression || Index is not Number)
            return Undefined.UNDEFINED;

        return Evaluate(context);
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => new Indexer(Collection.EvaluateDice(context), Index.EvaluateDice(context));

    public override bool Equals(object? obj)
        => obj is not null && obj is Indexer idx && Collection.Equals(idx.Collection) && Index.Equals(idx.Index);

    public override int GetHashCode()
        => HashCode.Combine(Collection, Index);

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{Collection.ToString(format)}[{Index.ToString(format)}]";
}
