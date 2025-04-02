namespace DiceRoll
{
    /// <summary>
    /// Basic interface for any object that expressions may be composed of.
    /// Represents an arbitrary action or element that may be <see cref="Evaluate">evaluated</see> to the specified type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    public interface INode<out T> : INode
    {
        /// <summary>
        /// <para>
        /// Evaluate the result.
        /// </para>
        /// <para>
        /// Implementations are expected to delay evaluation of their result until this method is called.
        /// </para>
        /// </summary>
        /// <returns>The evaluation result of type <typeparamref name="T"/>.</returns>
        T Evaluate();
    }

    public interface INode
    {
        void Visit(INodeVisitor visitor);
    }
}
