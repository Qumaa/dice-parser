namespace DiceRoll.Expressions
{
    public interface IExpression<T>
    {
        T Evaluate();
    }
}
