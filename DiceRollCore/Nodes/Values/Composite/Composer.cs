using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public abstract class Composer
    {
        public INumeric Compose(IEnumerable<INumeric> source)
        {
            INumeric[] validatedSource = ToArray(source);

            return validatedSource.Length is 1 ? validatedSource[0] : Compose(validatedSource);
        }

        protected abstract INumeric Compose(INumeric[] source);

        protected static INumeric IteratePairs(INumeric[] source, PairCompositionDelegate compositionDelegate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(compositionDelegate);
            
            for (int i = 1; i < source.Length; i++)
            {
                INumeric previous = source[i - 1];
                INumeric current = source[i];

                source[i] = compositionDelegate(previous, current);
            }

            return source[^1];
        }

        private static INumeric[] ToArray(IEnumerable<INumeric> source) =>
            source as INumeric[] ?? source.ToArray();

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

            protected override INumeric Compose(INumeric[] source) =>
                _compositionDelegate.Invoke(source);
        }

        protected delegate INumeric PairCompositionDelegate(INumeric left, INumeric right);
    }
}
