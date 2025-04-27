namespace DiceRoll
{
    public interface INode<out T> : INode
    {
        T Evaluation { get; }

        void Next();
    }

    public interface INode
    {
        void Visit(INodeVisitor visitor);
    }

    public static class NodeExtensions
    {
        public static T Evaluate<T>(this INode<T> node)
        {
            node.Next();
            return node.Evaluation;
        }
    }
}
