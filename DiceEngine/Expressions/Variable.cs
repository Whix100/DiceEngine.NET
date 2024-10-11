using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the the <see cref="Variable"/> class.
/// </summary>
/// <param name="name">The name of the <see cref="Variable"/> for reference.</param>
public class Variable(string name) : IExpression, IPrefixOperable, IPostfixOperable
{
    public readonly string Name = name;

    public IExpression Evaluate()
        => Evaluate(new ExpressionContext());

    public IExpression Evaluate(ExpressionContext variables)
        => variables[Name];
    
    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext variables)
        => variables[Name];

    public IExpression PrefixOperate(string identifier, ExpressionContext context)
    {
        switch (identifier)
        {
            case PrefixOperator.PRE_DECREMENT:
                IExpression dec_val = new BinaryOperator("-", context[Name], new Number(1)).Evaluate(context);

                context[Name] = dec_val;
                return dec_val;
            case PrefixOperator.PRE_INCREMENT:
                IExpression inc_val = new BinaryOperator("+", context[Name], new Number(1)).Evaluate(context);

                context[Name] = inc_val;
                return inc_val;
            default:
                if (context[Name] is IPrefixOperable operable)
                    return operable.PrefixOperate(identifier, context);
                
            return Undefined.UNDEFINED;
        };
    }

    public IExpression PostfixOperate(string identifier, ExpressionContext context)
    {
        switch (identifier)
        {
            case PostfixOperator.POST_DECREMENT:
                IExpression dec_val = new BinaryOperator("-", context[Name], new Number(1)).Evaluate(context);

                context[Name] = dec_val;
                return this;
            case PostfixOperator.POST_INCREMENT:
                IExpression inc_val = new BinaryOperator("-", context[Name], new Number(1)).Evaluate(context);

                context[Name] = inc_val;
                return this;
            default:
                if (context[Name] is IPostfixOperable operable)
                    return operable.PostfixOperate(identifier, context);

            return Undefined.UNDEFINED;
        };
    }

    public IExpression EvaluateDice()
        => EvaluateDice(new ExpressionContext());

    public IExpression EvaluateDice(ExpressionContext context)
        => context[Name];

    public override bool Equals(object? obj)
        => obj is not null && obj is Variable v && v.Name == Name;

    public override int GetHashCode()
        => Name.GetHashCode();

    public override string ToString()
        => Name;

    public string ToString(string? format)
        => Name;
}
