﻿using CalcExpr.Expressions;
using System.Numerics;

namespace CalcExpr.TypeConverters;

public class IntegerTypeConverter<T> : ITypeConverter<T?>
    where T : struct, IBinaryInteger<T>, IMinMaxValue<T>
{
    public IExpression ConvertToExpression(T? value)
    {
        try
        {
            return value.HasValue
                ? (Number)Convert.ToDouble(value.Value)
                : Constant.UNDEFINED;
        }
        catch
        {
            return Constant.UNDEFINED;
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
