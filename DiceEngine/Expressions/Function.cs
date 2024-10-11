using DiceEngine.Attributes;
using DiceEngine.Context;
using DiceEngine.Expressions.Collections;
using DiceEngine.Expressions.Components;
using DiceEngine.Extensions;
using DiceEngine.TypeConverters;
using System.Reflection;

namespace DiceEngine.Expressions;

public class Function(IEnumerable<IParameter> parameters, Delegate body, bool is_elementwise = false) : IFunction
{
    private readonly IReadOnlyList<IParameter> _parameters = parameters.ToArray() ?? [];

    public readonly Delegate Body = body;
    public readonly bool RequiresContext = parameters
        .Any(p => p is ContextParameter);

    public IParameter[] Parameters
        => _parameters.ToArray();

    public bool IsElementwise
        => is_elementwise;

    public Function(MethodInfo method)
        : this (method.ToDelegate(), method.GetCustomAttribute(typeof(ElementwiseAttribute)) is not null)
    { }

    public Function(Delegate body, bool is_elementwise = false)
        : this(body.Method.GetParameters().ToParameters(ExpressionContext.DEFAULT_TYPES)!, body, is_elementwise)
    { }

    public IExpression Invoke(IExpression[] arguments, ExpressionContext context)
    {
        object?[] args;

        if (RequiresContext)
        {
            object?[]? processed_args = ((IFunction)this).ProcessArguments(arguments, context);

            if (processed_args is null)
                return Undefined.UNDEFINED;

            int i = 0;

            args = [.. Parameters.Select(p => p is ContextParameter ? context : processed_args[i++])];
        }
        else
        {            
            object?[]? processed_args = ((IFunction)this)
                .ProcessArguments(arguments.Select(arg => arg.Evaluate(context)), context);

            if (processed_args is null)
                return Undefined.UNDEFINED;

            args = [.. processed_args];
        }

        object? result = Body.Method.Invoke(this, args);

        if (result is null)
        {
            return Undefined.UNDEFINED;
        }
        else if (result is IExpression expr)
        {
            return expr;
        }
        else
        {
            Type return_type = Body.Method.ReturnType.IsGenericType &&
                Body.Method.ReturnType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? Body.Method.ReturnType.GetGenericArguments().Single()
                    : Body.Method.ReturnType;
            ITypeConverter[] converter = context.GetTypeConverters(return_type);

            return converter.ConvertToExpression(result) ?? Undefined.UNDEFINED;
        }
    }

    public IExpression Evaluate()
        => this;

    public IExpression Evaluate(ExpressionContext context)
        => this;

    public IExpression StepEvaluate()
        => this;

    public IExpression StepEvaluate(ExpressionContext context)
        => this;

    public IExpression EvaluateDice()
        => this;

    public IExpression EvaluateDice(ExpressionContext context)
        => this;

    public override bool Equals(object? obj)
        => obj is not null && obj is Function func && func.Body.Equals(Body) &&
            func.Parameters.Select((p, i) => p.Equals(Parameters[i])).Aggregate((a, b) => a && b);

    public override int GetHashCode()
        => HashCode.Combine(Parameters, Body);

    public override string ToString()
        => ToString(null);

    public string ToString(string? format)
        => $"";
}

public interface IFunction : IExpression
{
    public IParameter[] Parameters { get; }

    public bool IsElementwise { get; }

    public static ExpressionContext ContextReconciliation(ExpressionContext outer_context,
        ExpressionContext inner_context, IEnumerable<Parameter> parameters)
    {
        foreach (string variable in inner_context.Variables.Except(parameters.Select(p => p.Name)))
            outer_context[variable] = inner_context[variable];

        return outer_context;
    }

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

    public IExpression Invoke(IExpression[] arguments, ExpressionContext context);

    public static IExpression ForEach(MethodInfo function, IEnumerable<IExpression> arguments, ExpressionContext context)
        => ForEach(new Function(function), arguments, context);

    public static IExpression ForEach(IFunction function, IEnumerable<IExpression> arguments, ExpressionContext context)
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

            MethodInfo[] methods = enum_type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            return (IEnumerableExpression?)create_method!.Invoke(null, [results])!;
        }
        else
        {
            return function.Invoke([.. arguments], context);
        }
    }
}
