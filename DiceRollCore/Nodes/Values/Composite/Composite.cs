using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class Composite : Numeric, IComposite
    {
        private readonly IComposite _composite;

        public override Outcome Evaluation => _composite.AsNumeric.Evaluation;
        CompositeEvaluation INode<CompositeEvaluation>.Evaluation => _composite.Evaluation;

        INumeric IComposite.AsNumeric => _composite.AsNumeric;

        public Composite(IEnumerable<INumeric> sequence, Composer composer)
        {
            ArgumentNullException.ThrowIfNull(sequence);
            ArgumentNullException.ThrowIfNull(composer);

            INumeric[] sourceNodes = sequence.ToArray();

            CompositeRepetitionArgumentException.ThrowIfBelowOne(sourceNodes.Length);
            
            _composite = composer.Compose(sourceNodes);
        }

        public Composite(INumeric node, int repetitionCount, Composer composer) :
            this(Enumerable.Repeat(node, repetitionCount), composer) { }

        public override void Next() =>
            _composite.Next();

        protected override RollProbabilityDistribution CreateProbabilityDistribution() =>
            _composite.AsNumeric.GetProbabilityDistribution();
    }

    public interface IComposite : INode<CompositeEvaluation>
    {
        INumeric AsNumeric { get; }
    }

    public sealed class CompositeEvaluation : IEnumerable<Outcome>
    {
        public readonly Outcome Outcome;
        private readonly Outcome[] _source;
        
        public CompositeEvaluation(Outcome outcome, Outcome[] source)
        {
            _source = source;
            Outcome = outcome;
        }
        
        public IEnumerator<Outcome> GetEnumerator() =>
            (_source as IEnumerable<Outcome>).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
