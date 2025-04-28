namespace DiceRoll
{
    public interface INode<out T> : INode
    {
        T Evaluation { get; }
    }

    public interface INode
    {
        void Visit<T>(T visitor) where T : INodeVisitor;
        void Next();
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
