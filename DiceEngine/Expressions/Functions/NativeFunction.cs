using DiceEngine.Attributes;
using DiceEngine.Context;
using DiceEngine.Expressions.Components;
using DiceEngine.Expressions.Terminals;
using DiceEngine.Extensions;
using DiceEngine.TypeConverters;
using System.Reflection;

namespace DiceEngine.Expressions.Functions;

public class NativeFunction(IEnumerable<IParameter> parameters, Delegate body, bool isElementwise = false) : Function
{
    private readonly IReadOnlyList<IParameter> _parameters = parameters.ToArray() ?? [];

    public readonly Delegate Body = body;
    public readonly bool RequiresContext = parameters.Any(p => p is ContextParameter);

    public override IParameter[] Parameters => [.. _parameters];

    public override bool IsElementwise => isElementwise;

    public NativeFunction(MethodInfo method)
        : this(method.ToDelegate(), method.GetCustomAttribute(typeof(ElementwiseAttribute)) is not null)
    { }

    public NativeFunction(Delegate body, bool isElementwise = false)
        : this(body.Method.GetParameters().ToParameters(ExpressionContext.DEFAULT_TYPES)!, body, isElementwise)
    { }

    public override Terminal Invoke(IExpression[] arguments, ExpressionContext context)
    {
        object?[] args;

        if (RequiresContext)
        {
            object?[]? processedArgs = ProcessArguments(arguments, context);

            if (processedArgs is null)
                return Undefined.UNDEFINED;

            int i = 0;

            args = [.. Parameters.Select(p => p is ContextParameter ? context : processedArgs[i++])];
        }
        else
        {
            object?[]? processedArgs = ProcessArguments(arguments.Select(arg => arg.Evaluate(context)), context);

            if (processedArgs is null)
                return Undefined.UNDEFINED;

            args = [.. processedArgs];
        }

        object? result = Body.Method.Invoke(this, args);

        if (result is null)
        {
            return Undefined.UNDEFINED;
        }
        else if (result is Terminal expr)
        {
            return expr;
        }
        else
        {
            Type returnType = Body.Method.ReturnType.IsGenericType &&
                Body.Method.ReturnType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Body.Method.ReturnType.GetGenericArguments().Single()
                    : Body.Method.ReturnType;
            ITypeConverter[] converter = context.GetTypeConverters(returnType);

            return converter.ConvertToExpression(result) ?? Undefined.UNDEFINED;
        }
    }

    public override bool Equals(object? obj)
        => obj is NativeFunction func && func.Body.Equals(Body) && func.Parameters.SequenceEqual(Parameters);

    public override int GetHashCode()
        => HashCode.Combine(Parameters, Body);

    public override string ToString(string? _)
        => $"";
}
