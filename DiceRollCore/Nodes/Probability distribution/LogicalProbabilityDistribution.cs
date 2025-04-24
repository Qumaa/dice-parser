namespace DiceRoll
{
    public sealed class LogicalProbabilityDistribution : ProbabilityDistribution<Logical>
    {
        private readonly Probability _ofTrue;
        
        public Probability True => _ofTrue;
        public Probability False => _ofTrue.Inversed();
        
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
