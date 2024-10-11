using DiceEngine.Attributes;

namespace DiceEngine.BuiltInFunctions;

public static class FactorialFunctions
{
    [BuiltInFunction("factorial")]
    [Elementwise]
    public static double Factorial(double x)
    {
        if (x == 0 || x == 1)
        {
            return 1;
        }
        else if (x % 1 == 0 && x > 0)
        {
            double output = 1;

            for (int i = 2; i <= x; i++)
                output *= i;

            return output;
        }
        else if (Double.IsPositiveInfinity(x))
        {
            return Double.PositiveInfinity;
        }

        return Double.NaN;
    }

    [BuiltInFunction("subfactorial")]
    [Elementwise]
    public static double Subfactorial(double x)
    {
        if (x == 0)
        {
            return 1;
        }
        else if (x % 1 == 0 && x > 0)
        {
            double n_fact = Factorial(x);
            
            if (Double.IsPositiveInfinity(n_fact))
                return Double.PositiveInfinity;
            else
                return (int)(0.5 + n_fact / Math.E);
        }
        else if (Double.IsPositiveInfinity(x))
        {
            return Double.PositiveInfinity;
        }

        return Double.NaN;
    }

    [BuiltInFunction("primorial")]
    [Elementwise]
    public static double Primorial(double x)
    {
        if (x == 0)
        {
            return 1;
        }
        else if (x % 1 == 0 && x > 0)
        {
            return GetNPrimes((int)x).Aggregate((a, b) => a * b);
        }
        else if (Double.IsPositiveInfinity(x))
        {
            return Double.PositiveInfinity;
        }

        return Double.NaN;
    }

    private static int[] GetNPrimes(int length)
    {
        List<int> primes = [2, 3];

        if (length < 2)
            return [.. primes[..length]];

        for (int i = primes.Last() + 2; primes.Count < length; i += 2)
        {
            bool is_prime = true;

            for (int j = 1; j < primes.Count && is_prime; j++)
                if (i % primes[j] == 0)
                    is_prime = false;

            if (is_prime)
                primes.Add(i);
        }

        return [.. primes];
    }

    [BuiltInFunction("double_factorial")]
    [Elementwise]
    public static double DoubleFactorial(double x)
    {
        if (x == 0 || x == 1)
        {
            return 1;
        }
        else if (x % 1 == 0 && x > 0)
        {
            if (x % 2 == 0)
            {
                double n_fact = Factorial(x / 2);

                if (Double.IsPositiveInfinity(n_fact))
                {
                    return Double.PositiveInfinity;
                }
                else
                {
                    return Math.Pow(2, x / 2) * n_fact;
                }
            }
            else
            {
                double n_fact = Factorial(x);
                double n_less_fact = Factorial((x - 1) / 2);

                if (Double.IsRealNumber(n_fact) && Double.IsRealNumber(n_less_fact))
                {
                    return n_fact / (Math.Pow(2, (x - 1) / 2) * n_less_fact);
                }
                else if (Double.IsPositiveInfinity(n_fact) && Double.IsRealNumber(n_less_fact))
                {
                    return Double.PositiveInfinity;
                }
                else if (Double.IsPositiveInfinity(n_less_fact) && Double.IsRealNumber(n_fact))
                {
                    return 0;
                }
            }
        }
        else if (Double.IsPositiveInfinity(x))
        {
            return Double.PositiveInfinity;
        }

        return Double.NaN;
    }
}
