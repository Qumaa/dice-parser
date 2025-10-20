using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class Composite : Numeric, IComposite
    {
        private readonly Composer _composer;
        private readonly INumeric _compositeNumeric;

        public IEnumerable<INumeric> SourceNodes { get; }

        // WARNING: source MUST NOT create new nodes during enumeration, but always point to the same nodes
        public Composite(Composer composer, IEnumerable<INumeric> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(composer);

            _composer = composer;
            SourceNodes = source;
            _compositeNumeric = composer.Compose(SourceNodes);
        }

        public Composite(Composer composer, INumeric numeric, int repetitionTimes)
        {
            ArgumentNullException.ThrowIfNull(composer);
            ArgumentNullException.ThrowIfNull(numeric);
            ArgumentOutOfRangeException.ThrowIfLessThan(repetitionTimes, 1);
            
            INumeric[] sourceNodes = new INumeric[repetitionTimes];
            sourceNodes[0] = numeric;

            for (int i = 1; i < sourceNodes.Length; i++)
                sourceNodes[i] = numeric.CloneTyped();

            _composer = composer;
            SourceNodes = sourceNodes;
            _compositeNumeric = composer.Compose(sourceNodes);
        }

        private Composite(Composite cloneFrom)
        {
            SourceNodes = cloneFrom.SourceNodes.Select(x => x.CloneTyped()).ToArray();
            _compositeNumeric = cloneFrom._composer.Compose(SourceNodes);
        }

        public override void NextEvaluation()
        {
            _compositeNumeric.NextEvaluation();
            
            CacheEvaluation(_compositeNumeric.CachedEvaluation);
        }

        public override object Clone() =>
            new Composite(this);

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            _compositeNumeric.GetProbabilityDistribution();
    }
}
