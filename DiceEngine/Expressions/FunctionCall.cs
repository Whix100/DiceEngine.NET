using DiceEngine.Context;

namespace DiceEngine.Expressions;

public class FunctionCall(string name, IEnumerable<IExpression> arguments) : IExpression
{
    private readonly IReadOnlyList<IExpression> _arguments = arguments.ToArray();

    public readonly string Name = name;

    public IExpression[] Arguments
        => _arguments.ToArray();

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext context)
        => context.InvokeFunction(Name, _arguments);

    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext context)
    {
        IExpression variable = context[Name];

        if (!(variable is Function function && function.RequiresContext))
        {
            for (int i = 0; i < _arguments.Count; i++)
            {
                IExpression arg = _arguments[i];
                IExpression eval_arg = arg.StepEvaluate(context);

                if (!eval_arg.Equals(arg))
                    return new FunctionCall(Name, Arguments.Select((a, j) => (j == i) ? eval_arg : a));
            }
        }

        return context.InvokeFunction(Name, _arguments);
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => new FunctionCall(Name, Arguments.Select(a => a.EvaluateDice(context)));

    public override bool Equals(object? obj)
        => obj is not null && obj is FunctionCall call && call.Name == Name &&
            call._arguments.Select((a, i) => a.Equals(_arguments[i])).Aggregate((a, b) => a && b);

    public override int GetHashCode()
        => HashCode.Combine(Name, _arguments);

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"{Name}({String.Join(", ", Arguments.Select(arg => arg.ToString(format)))})";
}