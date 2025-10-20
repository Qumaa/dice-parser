using System;
using System.Collections.Generic;

namespace DiceRoll
{
    public abstract class Composer
    {
        public abstract INumeric Compose(IEnumerable<INumeric> source);

        protected static INumeric Aggregate(IEnumerable<INumeric> source, AggregationDelegate aggregationDelegate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(aggregationDelegate);

            using IEnumerator<INumeric> enumerator = source.GetEnumerator();

            // check if it has at least 1 element
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Source nodes sequence contains no elements.");
            
            INumeric aggregated = enumerator.Current; // 1st element
            
            while (enumerator.MoveNext()) // iterating from 2nd element
                aggregated = aggregationDelegate(aggregated, enumerator.Current);

            ArgumentNullException.ThrowIfNull(aggregated);
            
            return aggregated;
        }

        public static Composer FromDelegate(ComposerDelegate composerDelegate) =>
            new FuncComposer(composerDelegate);

        private sealed class FuncComposer : Composer
        {
            private readonly ComposerDelegate _composerDelegate;
            
            public FuncComposer(ComposerDelegate func)
            {
                ArgumentNullException.ThrowIfNull(func);
                
                _composerDelegate = func;
            }

            public override INumeric Compose(IEnumerable<INumeric> source) =>
                _composerDelegate.Invoke(source);
        }

        protected delegate INumeric AggregationDelegate(INumeric aggregated, INumeric current);
    }
}
