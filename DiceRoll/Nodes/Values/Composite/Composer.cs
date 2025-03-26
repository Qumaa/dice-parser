using System;
using System.Collections.Generic;
using System.Linq;
using DiceRoll.Exceptions;

namespace DiceRoll.Nodes
{
    public abstract class Composer
    {
        public IAnalyzable Compose(IEnumerable<IAnalyzable> source)
        {
            IAnalyzable[] validatedSource = Validate(source);

            return validatedSource.Length is 1 ? validatedSource[0] : Compose(validatedSource);
        }

        /// <summary>
        /// This method is called by the base class when the input is validated.
        /// If the input is valid but only contains 1 element, the base class returns it and never
        /// proceeds to call this method. 
        /// </summary>
        /// <param name="source">An array of objects to compose with at least 2 elements in it. Safe to modify.</param>
        /// <returns>A single object that implements <see cref="IAnalyzable"/> and combines the sequence in a
        /// user-determined way.</returns>
        protected abstract IAnalyzable Compose(IAnalyzable[] source);

        /// <summary>
        /// This method iterates the input in pairs, applying the specified delegate to every pair.
        /// Use this method inside <see cref="Compose"/> to automate the process.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compositionDelegate"></param>
        /// <returns></returns>
        protected static IAnalyzable IteratePairs(IAnalyzable[] source, ByPairsCompositionDelegate compositionDelegate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(compositionDelegate);
            
            for (int i = 1; i < source.Length; i++)
            {
                IAnalyzable previous = source[i - 1];
                IAnalyzable current = source[i];

                source[i] = compositionDelegate(previous, current);
            }

            return source[^1];
        }

        private static IAnalyzable[] Validate(IEnumerable<IAnalyzable> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            IAnalyzable[] sourceArray = source as IAnalyzable[] ?? source.ToArray();

            if (sourceArray.Length is 0)
                throw new EmptySequenceException("Expression composer may not compose empty sequence of expressions.",
                    nameof(source));

            return sourceArray;
        }

        public static Composer FromDelegate(CompositionDelegate compositionDelegate) =>
            new FuncComposer(compositionDelegate);

        protected abstract class Composed : IAnalyzable
        {
            protected readonly IAnalyzable _left;
            protected readonly IAnalyzable _right;
            
            protected Composed(IAnalyzable left, IAnalyzable right)
            {
                _left = left;
                _right = right;
            }

            public abstract Outcome Evaluate();
            public abstract RollProbabilityDistribution GetProbabilityDistribution();
        }

        private sealed class FuncComposer : Composer
        {
            private readonly CompositionDelegate _compositionDelegate;
            
            public FuncComposer(CompositionDelegate func)
            {
                ArgumentNullException.ThrowIfNull(func);
                
                _compositionDelegate = func;
            }

            protected override IAnalyzable Compose(IAnalyzable[] source) =>
                _compositionDelegate.Invoke(source);
        }

        protected delegate IAnalyzable ByPairsCompositionDelegate(IAnalyzable left, IAnalyzable right);
    }
}
