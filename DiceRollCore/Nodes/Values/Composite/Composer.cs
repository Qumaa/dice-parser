using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    /// <summary>
    /// <para>
    /// Base class to determine the behaviour of <see cref="Composite"/> nodes.
    /// All inheritors must not only define a way to compose multiple <see cref="INumeric">numerical nodes</see>
    /// into one, but also a way to correctly combine their
    /// <see cref="RollProbabilityDistribution">probability distributions</see>.
    /// </para>
    /// <para>
    /// All default implementations simply extend the appliance of a <see cref="Transformation"/> implementations
    /// to a sequence of nodes rather than to just two of them.
    /// </para>
    /// </summary>
    public abstract class Composer
    {
        /// <summary>
        /// Combines all the passed nodes into one node.
        /// </summary>
        /// <param name="source">A sequence of nodes.</param>
        /// <returns>A combined node.</returns>
        public INumeric Compose(IEnumerable<INumeric> source)
        {
            INumeric[] validatedSource = ToArray(source);

            return validatedSource.Length is 1 ? validatedSource[0] : Compose(validatedSource);
        }

        /// <summary>
        /// This method is called by the base class when the input is validated.
        /// If the input is valid but only contains 1 element, the base class returns it and never
        /// proceeds to call this method. 
        /// </summary>
        /// <param name="source">An array of objects to compose with at least 2 elements in it. Safe to modify.</param>
        /// <returns>
        /// A single object that implements <see cref="INumeric"/> and combines <paramref name="source"/>.
        /// </returns>
        protected abstract INumeric Compose(INumeric[] source);

        /// <summary>
        /// This method iterates <paramref name="source"/> in pairs, applying the specified
        /// <see cref="PairCompositionDelegate">delegate</see> to every pair of adjacent array elements.
        /// Use this method inside <see cref="Compose">Compose</see> to automate the process when it boils down to combining pairs
        /// of input nodes.
        /// </summary>
        /// <param name="source">An array of objects to compose with at least 2 elements in it. Safe to modify.</param>
        /// <param name="compositionDelegate">
        /// A <see cref="PairCompositionDelegate">delegate</see> to apply to <paramref name="source"/>.
        /// </param>
        /// <returns>
        /// A single object that implements <see cref="INumeric"/> and combines <paramref name="source"/>.
        /// </returns>
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

        /// <summary>
        /// Create a <see cref="Composer"/> instance out of <see cref="CompositionDelegate">CompositionDelegate</see>
        /// instead of deriving from the base class.
        /// </summary>
        /// <param name="compositionDelegate">The actual composition logic.</param>
        /// <returns>
        /// A <see cref="Composer"/> instance out of <see cref="CompositionDelegate">CompositionDelegate</see>.
        /// </returns>
        /// <exception cref="ArgumentNullException">When <paramref name="compositionDelegate"/> is null.</exception>
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

        /// <summary>
        /// <para>Delegate used to combine two nodes into one.</para>
        /// <para>Usually delegates the call to a fitting <see cref="Transformation"/> implementation evaluation.</para>
        /// </summary>
        protected delegate INumeric PairCompositionDelegate(INumeric left, INumeric right);
    }
}
