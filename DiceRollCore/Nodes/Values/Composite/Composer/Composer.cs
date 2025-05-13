using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public abstract class Composer
    {
        public ComposerOutput Compose(IEnumerable<INumeric> source)
        {
            ArgumentNullException.ThrowIfNull(source);
            
            return Compose(source.ToArray());
        }

        public ComposerOutput Compose(INumeric[] source)
        {
            ArgumentNullException.ThrowIfNull(source);
            CompositeRepetitionArgumentException.ThrowIfBelowOne(source.Length);

            ComposerContext context = new(source.Length);
            INumeric compositeNode = Compose(source, context);

            return new ComposerOutput(compositeNode, context);
        }

        protected abstract INumeric Compose(INumeric[] source, ComposerContext context);

        protected static INumeric IteratePairs(INumeric[] source, ComposerContext context,
            PairCompositionDelegate compositionDelegate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(compositionDelegate);
            
            source[0] = context.Include(source[0]);

            for (int i = 1; i < source.Length; i++)
            {
                INumeric previous = source[i - 1];
                INumeric current = context.Include(source[i]);

                source[i] = compositionDelegate(previous, current);
            }

            return source[^1];
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

            protected override INumeric Compose(INumeric[] source, ComposerContext context) =>
                _compositionDelegate.Invoke(source);
        }

        private sealed class Composite : IComposite
        {
            private readonly ComposerContext _context;
            private CompositeEvaluation _evaluation;
            
            public INumeric AsNumeric { get; }

            public CompositeEvaluation Evaluation =>
                _evaluation ??= new CompositeEvaluation(AsNumeric.Evaluation, _context.Read());

            public Composite(INumeric @base, ComposerContext context)
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
