using System;

namespace DiceRoll
{
    public interface INode<out T> : INode
    {
        T CachedEvaluation { get; }
    }

    public interface INode : ICloneable
    {
        void Visit<T>(T visitor) where T : INodeVisitor;
        void NextEvaluation();
    }

    public static class NodeExtensions
    {
        public static T Evaluate<T>(this INode<T> node)
        {
            node.NextEvaluation();
            return node.CachedEvaluation;
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public static T CloneTyped<T>(this T node) where T : INode
        {
            object clone = node.Clone();
            return clone is T casted ? casted : default;
        }
    }
}
