using DiceEngine.Context;
using DiceEngine.Expressions.Components;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions.Functions;

public class LambdaFunction(IEnumerable<Parameter> parameters, IExpression body) : Function
{
    private readonly IReadOnlyList<Parameter> _parameters = parameters.ToArray();

    public readonly IExpression Body = body;

    public override IParameter[] Parameters => _parameters.Cast<IParameter>().ToArray();

    public override bool IsElementwise => false;

    public override Terminal Invoke(IExpression[] arguments, ExpressionContext context)
    {
        ExpressionContext innerContext = context.Clone();
        Terminal[]? args = ProcessArguments(arguments, context)?.Cast<Terminal>().ToArray();

        if (args is null)
            return Undefined.UNDEFINED;

        foreach ((IParameter parameter, Terminal? argument) in _parameters.Zip(args))
            if (parameter is Parameter param)
                innerContext[param.Name] = argument;

        Terminal result = Body.Evaluate(innerContext);
        string[] parameters = _parameters.Select(p => p.Name).ToArray();

        foreach (string variable in innerContext.Variables)
            if (!parameters.Contains(variable))
                context[variable] = innerContext[variable];

        return result;
    }

    public override bool Equals(object? obj)
    {
        if (obj is LambdaFunction lambda)
        {
            bool parametersEqual = lambda.Parameters.SequenceEqual(Parameters);

            return parametersEqual && lambda.Body.Equals(Body);
        }

        return false;
    }

    public override int GetHashCode()
        => Parameters.GetHashCode();

    public override string ToString(string? format)
        => $"({string.Join(",", _parameters)})=>{Body.ToString(format)}";
}
