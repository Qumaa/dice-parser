namespace DiceRoll.Nodes
{
    public interface INode<T>
    {
        T Evaluate();
    }
}
