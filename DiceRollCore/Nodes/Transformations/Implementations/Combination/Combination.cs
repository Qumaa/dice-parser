using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS8524

namespace DiceRoll
{
    public sealed class Combination : BinaryTransformation
    {
        public readonly CombinationType CombinationType;

        public Combination(INumeric left, INumeric right, CombinationType combinationType) : base(left, right)
        {
            if (IsDivision(combinationType))
                ZeroDivisorException.ThrowIfAnyZero(Right.GetProbabilityDistribution());
            EnumValueNotDefinedException.ThrowIfValueNotDefined(combinationType);
            
            CombinationType = combinationType;
        }

        public override void NextEvaluation()
        {
            Outcome left = Left.Evaluate();
            Outcome right = Right.Evaluate();
            
            CacheEvaluation(Combine(left, right));
        }

        protected override RollProbabilityDistribution CreateProbabilityDistribution()
        {
            RollProbabilityDistribution source = Left.GetProbabilityDistribution();
            RollProbabilityDistribution other = Right.GetProbabilityDistribution();

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
            CombinationType switch
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
