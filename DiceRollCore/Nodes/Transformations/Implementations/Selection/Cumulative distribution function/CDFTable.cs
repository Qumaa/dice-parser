using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll
{
    public sealed class CDFTable
    {
        private readonly Outcome _min;
        private readonly Outcome _max;
        private readonly SortedList<Outcome, OutcomeProbabilities> _outcomeProbabilities;

        public CDFTable(RollProbabilityDistribution distribution)
        {
            ArgumentNullException.ThrowIfNull(distribution);

            _min = distribution.Min;
            _max = distribution.Max;

            _outcomeProbabilities = BuildProbabilities(distribution);
        }

        public Probability EqualTo(Outcome outcome)
        {
            if (outcome < _min || outcome > _max)
                return Probability.Zero;

            return _outcomeProbabilities.TryGetValue(outcome, out OutcomeProbabilities probabilities) ?
                probabilities.Raw :
                Probability.Zero;
        }

        public Probability LessThanOrEqual(Outcome outcome)
        {
            if (outcome > _max)
                return Probability.Zero;

            if (outcome < _min)
                return Probability.Hundred;

            int index = BinarySearch(outcome);

            if (index < 0)
            {
                index = ~index - 1;

                if (index is -1)
                    return Probability.Zero;
            }

            return _outcomeProbabilities.GetValueAtIndex(index).Accumulated;
        }

        public Probability GreaterThanOrEqual(Outcome outcome)
        {
            if (outcome > _max)
                return Probability.Hundred;

            if (outcome < _min)
                return Probability.Zero;

            int index = BinarySearch(outcome);

            if (index < 0)
            {
                index = ~index;

                if (index is 0)
                    return Probability.Hundred;
            }

            index = _outcomeProbabilities.Count - 1 - index;

            return _outcomeProbabilities.GetValueAtIndex(index).Accumulated;
        }

        private int BinarySearch(Outcome outcome)
        {
            IComparer<Outcome> comparer = _outcomeProbabilities.Comparer;

            int lower = 0;
            int upper = _outcomeProbabilities.Count - 1;

            while (lower <= upper)
            {
                int anchor = lower + ((upper - lower) / 2);

                switch (comparer.Compare(_outcomeProbabilities.GetKeyAtIndex(anchor), outcome))
                {
                    case 0:
                        return anchor;

                    case < 0:
                        lower = anchor + 1;
                        break;

                    case > 0:
                        upper = anchor - 1;
                        break;
                }
            }

            return ~lower;
        }

        private static SortedList<Outcome, OutcomeProbabilities> BuildProbabilities(
            RollProbabilityDistribution distribution)
        {
            SortedList<Outcome, OutcomeProbabilities> outcomeProbabilities = new(Outcome.RelationalComparer);

            Probability accumulatedProbability = Probability.Zero;

            foreach (Roll roll in distribution)
            {
                Probability probability = roll.Probability;
                accumulatedProbability += probability;

                OutcomeProbabilities probabilities = new(probability, accumulatedProbability);

                outcomeProbabilities.Add(roll.Outcome, probabilities);
            }

            return outcomeProbabilities;
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct OutcomeProbabilities
        {
            public readonly Probability Raw;
            public readonly Probability Accumulated;

            public OutcomeProbabilities(Probability raw, Probability accumulated)
            {
                Raw = raw;
                Accumulated = accumulated;
            }
        }
    }

    public static class CDFTableExtensions
    {
        public static Probability NotEqualTo(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.EqualTo(outcome).Inversed();

        public static Probability GreaterThan(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.LessThanOrEqual(outcome).Inversed();

        public static Probability LessThan(this CDFTable cdfTable, Outcome outcome) =>
            cdfTable.GreaterThanOrEqual(outcome).Inversed();
    }
}
