using DiceEngine.Expressions;
using System.Numerics;

namespace DiceEngine.TypeConverters;

public class FloatTypeConverter<T> : ITypeConverter<T?>
    where T : struct, IFloatingPointIeee754<T>, IMinMaxValue<T>
{
    public IExpression ConvertToExpression(T? value)
    {
        try
        {
            if (value.HasValue)
            {
                if (T.IsPositiveInfinity(value.Value))
                    return Infinity.POSITIVE;
                else if (T.IsNegativeInfinity(value.Value))
                    return Infinity.NEGATIVE;
                else if (T.IsNaN(value.Value))
                    return Undefined.UNDEFINED;

                return (Number)Convert.ToDouble(value.Value);
            }
        }
        catch
        {
        }

        return Undefined.UNDEFINED;
    }

    public T? ConvertFromExpression(IExpression? expression)
    {
        try
        {
            if (expression is not null && expression is Number num)
            {
                if (num.Value > Convert.ToDouble(T.MaxValue))
                    return T.PositiveInfinity;
                else if (num.Value < Convert.ToDouble(T.MinValue))
                    return T.NegativeInfinity;
                
                return (T?)Convert.ChangeType(num.Value, typeof(T));
            }
            else if (expression is Infinity infinity)
            {
                return infinity.IsPositive ? T.PositiveInfinity : T.NegativeInfinity;
            }
            else if (expression is Logical logical)
            {
                return logical.Value ? T.One : T.Zero;
            }
            else if (expression is Undefined)
            {
                return T.NaN;
            }
        }
        catch
        {
        }

        return null;
    }
}
