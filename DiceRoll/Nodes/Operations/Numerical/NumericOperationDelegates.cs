using System.Runtime.InteropServices;

namespace DiceRoll.Nodes
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct NumericOperationDelegates
    {
        public readonly NumericOperationEvaluationDelegate Evaluation;
        public readonly NumericOperationProbabilityDelegate Probability;
        
        public NumericOperationDelegates(NumericOperationEvaluationDelegate evaluation, NumericOperationProbabilityDelegate probability)
        {
            Evaluation = evaluation;
            Probability = probability;
        }
    }
}
