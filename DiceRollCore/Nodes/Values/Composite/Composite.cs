using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class Composite : Numeric, IComposite
    {
        private readonly Composer _composer;
        private readonly INumeric _compositeNumeric;
        private readonly IEnumerable<INumeric> _sourceNodes;

        // WARNING: source MUST NOT create new nodes during enumeration, but always point to the same nodes
        public Composite(Composer composer, IEnumerable<INumeric> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(composer);

            _composer = composer;
            _sourceNodes = source;
            _compositeNumeric = composer.Compose(_sourceNodes);
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
            _sourceNodes = sourceNodes;
            _compositeNumeric = composer.Compose(sourceNodes);
        }

        private Composite(Composite cloneFrom)
        {
            _sourceNodes = cloneFrom._sourceNodes.Select(x => x.CloneTyped()).ToArray();
            _compositeNumeric = cloneFrom._composer.Compose(_sourceNodes);
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

        public IEnumerator<INumeric> GetEnumerator() =>
            _sourceNodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable) _sourceNodes).GetEnumerator();
    }
}
