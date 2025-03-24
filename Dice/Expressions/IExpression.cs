namespace Dice.Expressions
{
    public interface IExpression<T>
    {
        T Evaluate();
    }
}
