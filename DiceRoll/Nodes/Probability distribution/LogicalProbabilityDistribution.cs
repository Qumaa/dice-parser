namespace DiceRoll.Nodes
{
    /// <summary>
    /// A boolean implementation of <see cref="ProbabilityDistribution{T}">ProbabilityDistribution</see>
    /// of type <see cref="Logical"/> with a reasonable naming.
    /// Always contains 2 boolean values.
    /// </summary>
    public sealed class LogicalProbabilityDistribution : ProbabilityDistribution<Logical>
    {
        private readonly Probability _ofTrue;
        
        public Probability True => _ofTrue;
        public Probability False => _ofTrue.Inversed();
        
        /// <param name="ofTrue">
        /// <see cref="Probability"/> of true within the distribution.
        /// Must be within the 0% to 100% range to properly calculate the probability of false.
        /// </param>
        /// <exception cref="DiceRoll.Exceptions.NegativeProbabilityException">
        /// When <paramref name="ofTrue"/> is above 100%, resulting in the probability of false being below 0%.
        /// </exception>
        public LogicalProbabilityDistribution(Probability ofTrue) : base(ToEnumerable(ofTrue.Normalized()))
        {
            _ofTrue = ofTrue.Normalized();
        }

        private static Logical[] ToEnumerable(Probability ofTrue) =>
            new []
            {
                new Logical(true, ofTrue),
                new Logical(false, ofTrue.Inversed())
            };
    }
}
