using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct BinaryOperationDelegates
    {
        public readonly BinaryOperationEvaluationDelegate Evaluation;
        public readonly BinaryOperationProbabilityDelegate Probability;
        
        public BinaryOperationDelegates(BinaryOperationEvaluationDelegate evaluation, BinaryOperationProbabilityDelegate probability)
        {
            Evaluation = evaluation;
            Probability = probability;
        }
    }
}
