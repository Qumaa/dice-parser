namespace DiceRoll
{
    /// <summary>
    /// <para>
    /// Base interface for any numerical node that takes part in expressions.
    /// A shorthand for implementing <see cref="IRollable"/> and <see cref="IDistributable{T,TType}">IDistributable</see>
    /// of type <see cref="Roll"/>.
    /// </para>
    /// <para>
    /// Implementations are expected to <see cref="INode{T}.Evaluate">evaluate</see> themselves to <see cref="Outcome"/>
    /// and provide a <see cref="RollProbabilityDistribution">probability distribution</see> of every possible
    /// evaluation value.
    /// </para>
    /// </summary>
    public interface IAnalyzable : IRollable, IDistributable<RollProbabilityDistribution, Roll> { }
}
