using System;
using System.Linq;

namespace DiceRoll.Expressions
{
    public sealed class Keep : ProbabilityDistributionTransformation
    {
        private readonly ProbabilityDistribution _other;
        private readonly KeepMode _keepMode;

        public Keep(ProbabilityDistribution source, ProbabilityDistribution other, KeepMode keepMode) : base(source)
        {
            _other = other;
            _keepMode = keepMode;
        }

        public override ProbabilityDistribution Evaluate()
        {
            double[] probabilities = AllocateProbabilitiesArray(out int indexToValueOffset);
            FillProbabilities(probabilities, indexToValueOffset);

            return new ProbabilityDistribution(probabilities.Select((x, i) => new Roll(i + indexToValueOffset, x)));
        }

        private double[] AllocateProbabilitiesArray(out int offset)
        {
            int min = Math.Min(_source.Min.Value, _other.Min.Value);
            int max = _keepMode is KeepMode.Lowest ? 
                Math.Min(_source.Max.Value, _other.Max.Value) : 
                Math.Max(_source.Max.Value, _other.Max.Value);

            offset = min;
            return new double[max - min + 1];
        }

        private void FillProbabilities(double[] probabilities, int offset)
        {
            bool reversed = _keepMode is KeepMode.Lowest;
            CDFTable sourceCdf = new(_source, reversed);
            CDFTable otherCdf = new(_other, reversed);

            for (int i = 0; i < probabilities.Length; i++)
            {
                Outcome outcome = new(i + offset);

                CDFData sourceCdfData = sourceCdf.ForOutcome(outcome);
                CDFData otherCdfData = otherCdf.ForOutcome(outcome);

                probabilities[i] = sourceCdfData.Equal.Value * otherCdfData.EqualOr.Value +
                                   otherCdfData.Equal.Value * sourceCdfData.EqualOr.Value -
                                   sourceCdfData.Equal.Value * otherCdfData.Equal.Value;
            }
        }

        private sealed class CDFTable
        {
            private readonly Outcome _min;
            private readonly Outcome _max;
            private readonly Roll[] _rolls;
            private readonly Probability[] _cdfProbabilities;
            private readonly bool _reversed;
            
            public CDFTable(ProbabilityDistribution distribution, bool reversed)
            {
                _reversed = reversed;
                _min = distribution.Min;
                _max = distribution.Max;
                
                _rolls = distribution.ToArray();
                _cdfProbabilities = BuildCDF(_rolls, _reversed);
            }

            public CDFData ForOutcome(Outcome outcome)
            {
                if (OutOfBounds(outcome, out CDFData data))
                    return data;
                
                int i = outcome.Value - _min.Value;

                return new CDFData(_rolls[i].Probability, _cdfProbabilities[i]);
            }

            private bool OutOfBounds(Outcome outcome, out CDFData cdfData)
            {
                Probability equalOr;
                
                if (outcome.Value < _min.Value)
                {
                    equalOr = _reversed ? Probability.Hundred : Probability.Zero;
                    goto outOfBounds;
                }

                if (outcome.Value > _max.Value)
                {
                    equalOr = _reversed ? Probability.Zero : Probability.Hundred;
                    goto outOfBounds;
                }

                cdfData = default;
                return false;
                
                outOfBounds:
                cdfData = new CDFData(Probability.Zero, equalOr);
                return true;
            }

            private static Probability[] BuildCDF(Roll[] rolls, bool reversed)
            {
                Probability[] cdf = new Probability[rolls.Length];
                cdf[0] = rolls[0].Probability;
                
                for (int i = 1; i < cdf.Length; i++)
                    cdf[i] = new Probability(rolls[i].Probability.Value + cdf[i - 1].Value);
                
                if (reversed)
                    Array.Reverse(cdf);

                return cdf;
            }
        }

        private readonly struct CDFData
        {
            public readonly Probability Equal;
            public readonly Probability EqualOr;
                
            public CDFData(Probability equal, Probability equalOr)
            {
                Equal = equal;
                EqualOr = equalOr;
            }
        }
    }
}
