namespace DiceRoll.Nodes
{
    /// <summary>
    /// <para>
    /// Base interface for any numerical node that takes part in expressions.
    /// A shorthand for implementing <see cref="IRollable"/> and <see cref="IDistributable{T,TType}">IDistributable</see>
    /// of the <see cref="Roll"/> type.
    /// </para>
    /// <para>
    /// Implementations are expected to <see cref="INode{T}.Evaluate">evaluate</see> themselves to <see cref="Outcome"/>
    /// and provide a <see cref="RollProbabilityDistribution">distribution</see> of every possible evaluation value.
    /// </para>
    /// </summary>
    public interface IAnalyzable : IRollable, IDistributable<RollProbabilityDistribution, Roll> { }
}
