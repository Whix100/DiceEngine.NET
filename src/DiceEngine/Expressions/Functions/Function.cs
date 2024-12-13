using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Components;
using DiceEngine.Expressions.Terminals;
using System.Reflection;

namespace DiceEngine.Expressions.Functions;

public abstract class Function : Terminal
{
    public abstract IParameter[] Parameters { get; }

    public abstract bool IsElementwise { get; }

    public abstract Terminal Invoke(IExpression[] arguments, ExpressionContext context);

    public object?[]? ProcessArguments(IEnumerable<IExpression> arguments, ExpressionContext context)
    {
        IParameter[] parameters = Parameters.Where(p => p is not ContextParameter).ToArray();
        IExpression[] args = arguments.ToArray();
        List<object?> results = [];

        for (int i = 0; i < parameters.Length; i++)
        {
            object? argument = parameters[i].ProcessArgument(args[i], context);

            if (argument is null && !parameters[i].AllowNull)
                return null;

            results.Add(argument);
        }

        return [.. results];
    }

    public static ExpressionContext ContextReconciliation(ExpressionContext outer_context,
        ExpressionContext inner_context, IEnumerable<Parameter> parameters)
    {
        foreach (string variable in inner_context.Variables.Except(parameters.Select(p => p.Name)))
            outer_context[variable] = inner_context[variable];

        return outer_context;
    }

    public static Terminal ForEach(MethodInfo function, IEnumerable<IExpression> arguments, ExpressionContext context)
        => ForEach(new NativeFunction(function), arguments, context);

    public static Terminal ForEach(Function function, IEnumerable<IExpression> arguments, ExpressionContext context)
    {
        if (arguments.Count() != function.Parameters.Where(p => p is not ContextParameter).Count())
            return Undefined.UNDEFINED;

        if (function.IsElementwise && arguments.Any(arg => arg is IEnumerableExpression))
        {
            int length = arguments.Where(arg => arg is IEnumerableExpression)
                .Select(arg => ((IEnumerableExpression)arg).Count())
                .Min();
            List<IExpression> results = [];

            for (int i = 0; i < length; i++)
                results.Add(function.Invoke(arguments.Select(arg => arg is IEnumerableExpression enum_expr
                    ? enum_expr.ElementAt(i)
                    : arg).ToArray(), context));

            Type enum_type = arguments.First(arg => arg is IEnumerableExpression).GetType();
            MethodInfo? create_method = enum_type.GetMethod("ConvertIEnumerable", [typeof(IEnumerable<IExpression>)]);

            return TerminalCollection.TerminateCollection((IEnumerableExpression?)create_method!.Invoke(null, [results])!);
        }
        else
        {
            return function.Invoke([.. arguments], context);
        }
    }
}
