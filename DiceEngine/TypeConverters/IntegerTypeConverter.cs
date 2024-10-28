using DiceEngine.Expressions;
using DiceEngine.Expressions.Terminals;
using System.Numerics;

namespace DiceEngine.TypeConverters;

public class IntegerTypeConverter<T> : ITypeConverter<T?>
    where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
{
    public Terminal ConvertToExpression(T? value)
    {
        try
        {
            return value.HasValue
                ? (Terminal)Convert.ToDouble(value.Value)
                : Undefined.UNDEFINED;
        }
        catch
        {
            return Undefined.UNDEFINED;
        }
    }

    public T? ConvertFromExpression(IExpression? expression)
    {
        try
        {
            if (expression is not null && expression is Number num)
                return (T?)Convert.ChangeType(num.Value, typeof(T));
        }
        catch
        {
        }

        return null;
    }
}
