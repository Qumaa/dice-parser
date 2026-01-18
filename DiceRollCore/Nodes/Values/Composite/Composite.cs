using System;
using System.Collections.Generic;

namespace DiceRoll
{
    public sealed class Composite : Sequence<INumeric>, IComposite
    {
        // todo turn composer into an object that provides distribution and evaluation (implement inode)
        public readonly INumeric UnderlyingNode;
        private readonly Composer _composer;

        public Outcome CachedEvaluation => UnderlyingNode.CachedEvaluation;

        public Composite(Composer composer, IEnumerable<INumeric> source) : base(source)
        {
            ArgumentNullException.ThrowIfNull(composer);

            _composer = composer;
            UnderlyingNode = composer.Compose(_nodes);
        }

        public Composite(Composer composer, INumeric numeric, int repetitionTimes) : base(numeric, repetitionTimes)
        {
            ArgumentNullException.ThrowIfNull(composer);

            _composer = composer;
            UnderlyingNode = composer.Compose(_nodes);
        }

        public override void NextEvaluation() =>
            UnderlyingNode.NextEvaluation();

        public RollProbabilityDistribution GetProbabilityDistribution() =>
            UnderlyingNode.GetProbabilityDistribution();

        public override void Visit<TVisitor>(TVisitor visitor) =>
            visitor.ForNumeric(this);

        protected override Sequence<INumeric> Clone(INumeric[] clonedNodes) =>
            new Composite(_composer, clonedNodes);
    }
}
