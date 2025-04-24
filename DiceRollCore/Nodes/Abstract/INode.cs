namespace DiceRoll
{
    public interface INode<out T> : INode
    {
        T Evaluate();
    }

    public interface INode
    {
        void Visit(INodeVisitor visitor);
    }
}
