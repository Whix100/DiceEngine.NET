using DiceEngine.Context;
using DiceEngine.Expressions;

namespace TestDiceEngine.TestUtils;

public readonly struct FunctionTestCase(string[] function_aliases, Dictionary<IExpression[], IExpression> evaluated,
    ExpressionContext? context = null)
{
    public readonly string[] FunctionAliases = function_aliases;
    public readonly Dictionary<IExpression[], IExpression> Evaluated = evaluated;
    public readonly ExpressionContext? Context = context;

    public FunctionTestCase(string function_name, Dictionary<IExpression[], IExpression> evaluated,
        ExpressionContext? context = null)
        : this([function_name], evaluated, context)
    { }

    public FunctionTestCase(string[] function_aliases, Dictionary<IExpression, IExpression> evaluated,
        ExpressionContext? context = null)
        : this(function_aliases, evaluated.ToDictionary(kvp => new IExpression[] { kvp.Key }, kvp => kvp.Value),
              context)
    { }

    public FunctionTestCase(string function_name, Dictionary<IExpression, IExpression> evaluated,
        ExpressionContext? context = null)
        : this([function_name], evaluated.ToDictionary(kvp => new IExpression[] { kvp.Key }, kvp => kvp.Value),
              context)
    { }

    public FunctionTestCase(string[] function_aliases, IExpression evaluated, ExpressionContext? context = null)
        : this(function_aliases, new Dictionary<IExpression[], IExpression> { { [], evaluated } }, context)
    { }

    public FunctionTestCase(string function_name, IExpression evaluated, ExpressionContext? context = null)
        : this([function_name], new Dictionary<IExpression[], IExpression> { { [], evaluated } }, context)
    { }
}
