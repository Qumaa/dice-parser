using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS8524

namespace DiceRoll
{
    public sealed class Combination : BinaryTransformation
    {
        private readonly CombinationType _combinationType;

        public Combination(INumeric left, INumeric right, CombinationType combinationType) : base(left, right)
        {
            if (IsDivision(combinationType))
                ZeroDivisorException.ThrowIfAnyZero(_right.GetProbabilityDistribution());
            EnumValueNotDefinedException.ThrowIfValueNotDefined(combinationType);
            
            _combinationType = combinationType;
        }

        public override void NextEvaluation()
        {
            Outcome left = _left.Evaluate();
            Outcome right = _right.Evaluate();
            
            CacheEvaluation(Combine(left, right));
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution()
        {
            RollProbabilityDistribution source = _left.GetProbabilityDistribution();
            RollProbabilityDistribution other = _right.GetProbabilityDistribution();

            SortedList<Outcome, Probability> probabilities = new(Outcome.RelationalComparer);
            
            foreach (Roll sourceRoll in source)
            foreach (Roll otherRoll in other)
            {
                Outcome outcome = Combine(sourceRoll.Outcome, otherRoll.Outcome);
                Probability probability = sourceRoll.Probability * otherRoll.Probability;

                if (!probabilities.TryAdd(outcome, probability))
                    probabilities[outcome] += probability;
            }

            return probabilities
                .Select(x => new Roll(x.Key, x.Value))
                .ToRollProbabilityDistribution();
        }

        private Outcome Combine(Outcome left, Outcome right) =>
            _combinationType switch
            {
                CombinationType.Add => left + right,
                CombinationType.Subtract => left - right,
                CombinationType.Multiply => left * right,
                CombinationType.DivideRoundDownwards => left / right,
                CombinationType.DivideRoundUpwards => DivideRoundUpwards(left, right)
            };

        private static bool IsDivision(CombinationType combinationType) =>
            combinationType is CombinationType.DivideRoundDownwards or CombinationType.DivideRoundUpwards;

        private static Outcome DivideRoundUpwards(Outcome left, Outcome right)
        {
            Outcome outcome = left / right;
            if (((left ^ right) >= 0) && (left % right != 0))
                outcome++;

            return outcome;
        }
    }
}
