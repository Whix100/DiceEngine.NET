using DiceEngine.Expressions;
using DiceEngine.Expressions.Functions;
using DiceEngine.Expressions.Terminals;
using DiceEngine.Extensions;
using System.Reflection;

namespace DiceEngine.Context;

public partial class ExpressionContext
{
    protected readonly Dictionary<string, Function> _functions;

    public string[] Functions
        => [.. _functions.Keys];

    public virtual Terminal InvokeFunction(string function, IEnumerable<IExpression> arguments)
    {
        if (ContainsFunction(function))
        {
            Function func = _functions[function];

            return Function.ForEach(func, arguments, this);
        }

        return Undefined.UNDEFINED;
    }

    public virtual void SetFunctions(Assembly assembly)
    {
        foreach (Type t in assembly.GetTypes())
        {
            Dictionary<string[], Function> built_in_functions = t.GetFunctions(_type_converters.Keys);

            SetFunctions(built_in_functions);
        }
    }

    public virtual void SetFunctions(IEnumerable<KeyValuePair<string[], Function>> functions)
    {
        foreach (KeyValuePair<string[], Function> func in functions)
            SetFunctions(func.Key.ToDictionary(k => k, k => func.Value));
    }

    public virtual void SetFunctions(IEnumerable<KeyValuePair<string, Function>> functions)
    {
        foreach (KeyValuePair<string, Function> func in functions)
            SetFunction(func.Key, func.Value);
    }

    public virtual bool SetFunction(string name, Function function)
    {
        if (function is null)
            return _functions.Remove(name) && _aliases.Remove(name);

        _variables.Remove(name);
        _functions[name] = function;
        _aliases[name] = true;
        return true;
    }

    public virtual bool RemoveFunction(string name)
        => _functions.Remove(name) && _aliases.Remove(name);

    public virtual bool ContainsFunction(string name)
        => _functions.ContainsKey(name);
}
