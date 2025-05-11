using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public abstract class Composer
    {
        public IComposite Compose(IEnumerable<INumeric> source)
        {
            INumeric[] sourceArray = source.ToArray();

            return Compose(sourceArray);
        }

        protected abstract IComposite Compose(INumeric[] source);

        protected static IComposite IteratePairs(INumeric[] source, PairCompositionDelegate compositionDelegate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(compositionDelegate);

            WrapperContext context = new(source.Length);

            source[0] = new Wrapper(source[0], context);

            for (int i = 1; i < source.Length; i++)
            {
                INumeric previous = source[i - 1];
                INumeric current = new Wrapper(source[i], context);

                source[i] = compositionDelegate(previous, current);
            }

            return new Composite(source[^1], context);
        }

        public static Composer FromDelegate(CompositionDelegate compositionDelegate) =>
            new FuncComposer(compositionDelegate);

        private sealed class FuncComposer : Composer
        {
            private readonly CompositionDelegate _compositionDelegate;
            
            public FuncComposer(CompositionDelegate func)
            {
                ArgumentNullException.ThrowIfNull(func);
                
                _compositionDelegate = func;
            }

            protected override IComposite Compose(INumeric[] source) =>
                _compositionDelegate.Invoke(source);
        }

        private sealed class Wrapper : INumeric
        {
            private readonly INumeric _base;
            private readonly WrapperContext _context;

            public Outcome Evaluation => _base.Evaluation;

            public Wrapper(INumeric @base, WrapperContext context)
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

        private sealed class WrapperContext
        {
            private readonly Outcome[] _array;
            private int _count;

            public WrapperContext(int capacity)
            {
                _array = new Outcome[capacity];
                _count = 0;
            }
            
            public void Write(in Outcome outcome) =>
                _array[_count++] = outcome;

            public void Reset() =>
                _count = 0;

            public Outcome[] Read() =>
                _array;
        }

        private sealed class Composite : IComposite
        {
            private readonly WrapperContext _context;
            private CompositeEvaluation _evaluation;
            
            public INumeric AsNumeric { get; }

            public CompositeEvaluation Evaluation =>
                _evaluation ??= new CompositeEvaluation(AsNumeric.Evaluation, _context.Read());

            public Composite(INumeric @base, WrapperContext context)
            {
                AsNumeric = @base;
                _context = context;
            }

            public void Visit<T>(T visitor) where T : INodeVisitor =>
                AsNumeric.Visit(visitor);

            public void Next()
            {
                _context.Reset();
                _evaluation = null;
                
                AsNumeric.Next();
            }
        }

        protected delegate INumeric PairCompositionDelegate(INumeric left, INumeric right);
    }
}
