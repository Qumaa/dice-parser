using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct BinaryOperationDelegates
    {
        public readonly BinaryAssertionEvaluationDelegate Evaluation;
        public readonly BinaryAssertionProbabilityDelegate Probability;
        
        public BinaryOperationDelegates(BinaryAssertionEvaluationDelegate evaluation, BinaryAssertionProbabilityDelegate probability)
        {
            Evaluation = evaluation;
            Probability = probability;
        }
    }
}
