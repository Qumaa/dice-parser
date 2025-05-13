namespace DiceRoll
{
    public sealed class ComposerWrapper : INumeric
    {
        private readonly INumeric _base;
        private readonly ComposerContext _context;

        public Outcome Evaluation => _base.Evaluation;

        public ComposerWrapper(INumeric @base, ComposerContext context)
        {
            _base = @base;
            _context = context;
        }

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            _base.Visit(visitor);

        public void Next()
        {
            _base.Next();
            _context.Write(Evaluation);
        }

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            _base.GetProbabilityDistribution();
    }
}
