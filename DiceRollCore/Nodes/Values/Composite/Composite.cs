using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class Composite : Numeric, IComposite
    {
        private readonly INumeric _composite;
        
        private readonly ComposerContext _context;
        private CompositeEvaluation _compositeEvaluation;

        CompositeEvaluation INode<CompositeEvaluation>.Evaluation => _compositeEvaluation;

        INumeric IComposite.AsNumeric => _composite;

        public Composite(IEnumerable<INumeric> sequence, Composer composer)
        {
            ArgumentNullException.ThrowIfNull(sequence);
            ArgumentNullException.ThrowIfNull(composer);

            ComposerOutput output = composer.Compose(sequence.ToArray());
            
            _composite = output.CompositeNode;
            _context = output.Context;
        }

        public Composite(INumeric node, int repetitionCount, Composer composer) :
            this(Enumerable.Repeat(node, repetitionCount), composer) { }

        public override void Next()
        {
            _composite.Next();
            
            CacheEvaluation(_composite.Evaluation);

            _compositeEvaluation = new CompositeEvaluation(Evaluation, _context.Read());
            _context.Reset();
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            _composite.GetProbabilityDistribution();
    }
}
