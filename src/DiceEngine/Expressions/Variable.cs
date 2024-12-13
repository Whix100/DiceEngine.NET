using DiceEngine.Context;
using DiceEngine.Expressions.Interfaces;
using DiceEngine.Expressions.Terminals;

namespace DiceEngine.Expressions;

/// <summary>
/// Initializes a new instance of the <see cref="Variable"/> class.
/// </summary>
/// <param name="name">The name of the <see cref="Variable"/> for reference.</param>
public class Variable(string name) : IExpression, IPrefixOperable, IPostfixOperable
{
    public readonly string Name = name;

    public Terminal Evaluate()
        => Evaluate(new ExpressionContext());

    public Terminal Evaluate(ExpressionContext context)
        => context[Name];
    
    public IExpression StepEvaluate()
        => StepEvaluate(new ExpressionContext());

    public IExpression StepEvaluate(ExpressionContext variables)
        => variables[Name];

    public Terminal PrefixOperate(string identifier, ExpressionContext context)
    {
        switch (identifier)
        {
            case PrefixOperator.PRE_DECREMENT:
                Terminal dec_val = new BinaryOperator("-", context[Name], new Number(1)).Evaluate(context);

                context[Name] = dec_val;
                return dec_val;
            case PrefixOperator.PRE_INCREMENT:
                Terminal inc_val = new BinaryOperator("+", context[Name], new Number(1)).Evaluate(context);

                context[Name] = inc_val;
                return inc_val;
            default:
                if (context[Name] is IPrefixOperable operable)
                    return operable.PrefixOperate(identifier, context);
                
            return Undefined.UNDEFINED;
        };
    }

    public Terminal PostfixOperate(string identifier, ExpressionContext context)
    {
        switch (identifier)
        {
            case PostfixOperator.POST_DECREMENT:
                Terminal preDecValue = context[Name];
                Terminal decVal = new BinaryOperator("-", preDecValue, new Number(1)).Evaluate(context);

                context[Name] = decVal;
                return preDecValue;
            case PostfixOperator.POST_INCREMENT:
                Terminal preIncValue = context[Name];
                Terminal incVal = new BinaryOperator("+", preIncValue, new Number(1)).Evaluate(context);

                context[Name] = incVal;
                return preIncValue;
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
