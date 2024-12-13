using DiceEngine.Context;
using DiceEngine.Expressions.Terminals;
using DiceEngine.FunctionAttributes;
using DiceEngine.TypeConverters;

namespace DiceEngine.Expressions.Components;

public readonly struct TypeParameter<T>(IEnumerable<FunctionAttribute> attributes, bool allow_null) : IParameter
{
    public readonly Type ParameterType = typeof(T);

    public FunctionAttribute[] Attributes { get; } = attributes.ToArray();

    public bool AllowNull { get; } = allow_null;

    public readonly object? ProcessArgument(IExpression argument, ExpressionContext context)
    {
        IExpression? expression = IParameter.ApplyAttributes(argument, Attributes);
        ITypeConverter[] converters = context.GetTypeConverters<T>();

        return expression is null
            ? null
            : TypeParameter.ConvertFromExpression(converters, expression);
    }

    public readonly override int GetHashCode()
        => HashCode.Combine(ParameterType, AllowNull);

    public readonly override bool Equals(object? obj)
        => obj is TypeParameter<T> parameter && ParameterType == parameter.ParameterType &&
            AllowNull == parameter.AllowNull;
}

public static class TypeParameter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeParameter{T}"/> struct.
    /// </summary>
    /// <param name="type">The type of the parameter.</param>
    /// <param name="attributes">The attributes of the parameter.</param>
    /// <param name="allow_null">Whether the parameter allows null values.</param>
    /// <returns>A new instance of the <see cref="TypeParameter{T}"/> struct.</returns>
    public static object? InitializeTypeParameter(Type type, IEnumerable<FunctionAttribute> attributes, bool allow_null)
    {
        Type parameter_type = typeof(TypeParameter<>).MakeGenericType(type);

        return Activator.CreateInstance(parameter_type, attributes, allow_null);
    }

    /// <summary>
    /// Processes the argument with the specified context.
    /// </summary>
    /// <param name="converters">The <see cref="ITypeConverter"/>s to use.</param>
    /// <param name="expression">The expression to convert.</param>
    /// <returns>The converted object.</returns>
    public static object? ConvertFromExpression(this IEnumerable<ITypeConverter> converters, IExpression expression)
    {
        foreach (ITypeConverter converter in converters)
        {
            object? result = converter.GetType().GetMethod("ConvertFromExpression")?.Invoke(converter, [expression]);

            if (result is not null)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Converts the specified value to an <see cref="IExpression"/>;
    /// </summary>
    /// <param name="converters">The <see cref="ITypeConverter"/>s to use.</param>"/>
    /// <param name="value">The value to convert.</param>
    /// <returns>An <see cref="IExpression"/> representing the value.</returns>
    public static Terminal? ConvertToExpression(this IEnumerable<ITypeConverter> converters, object? value)
    {
        foreach (ITypeConverter converter in converters)
        {
            object? result = converter.GetType().GetMethod("ConvertToExpression")?.Invoke(converter, [value]);

            if (result is not null)
                return (Terminal?)result;
        }

        return null;
    }
}
