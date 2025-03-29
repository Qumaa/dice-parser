namespace DiceRoll.Nodes
{
    /// <summary>
    /// <para>
    /// Base interface for any node of binary operation between two <see cref="IAnalyzable">numerical nodes</see>
    /// that takes part in expressions.
    /// Implements <see cref="INode{T}">INode</see> of type <see cref="Binary"/>
    /// and <see cref="IDistributable{T,TType}">IDistributable</see> of type <see cref="Logical"/>.
    /// </para>
    /// <para>
    /// Implementations are expected to <see cref="INode{T}.Evaluate">evaluate</see> themselves to <see cref="Binary"/>,
    /// provide a <see cref="RollProbabilityDistribution">probability distribution</see> of true and false,
    /// and provide an <see cref="OperationVerboseEvaluation"/> instance.
    /// </para>
    /// </summary>
    public interface IOperation : INode<Binary>, IDistributable<LogicalProbabilityDistribution, Logical>
    {
        /// <summary>
        /// Provides a verbose evaluation result that complements the operation result with numeric values of both
        /// nodes.
        /// </summary>
        /// <returns>An <see cref="OperationVerboseEvaluation"/> instance.</returns>
        OperationVerboseEvaluation EvaluateVerbose();
    }
}
