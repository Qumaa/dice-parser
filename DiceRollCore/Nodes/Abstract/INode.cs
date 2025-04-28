namespace DiceRoll
{
    public interface INode<out T> : INode
    {
        T Evaluation { get; }

        void Next();
    }

    public interface INode
    {
        void Visit<T>(T visitor) where T : INodeVisitor;
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
