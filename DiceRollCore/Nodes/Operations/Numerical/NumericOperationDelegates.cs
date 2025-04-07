using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct NumericOperationDelegates
    {
        public readonly NumericOperationEvaluationDelegate Evaluation;
        public readonly NumericOperationProbabilityDelegate Probability;
        public readonly NumericOperationOptionalRollsDelegate OptionalRolls;
        
        public NumericOperationDelegates(NumericOperationEvaluationDelegate evaluation, NumericOperationProbabilityDelegate probability, NumericOperationOptionalRollsDelegate optionalRolls)
        {
            Evaluation = evaluation;
            Probability = probability;
            OptionalRolls = optionalRolls;
        }
    }
}
