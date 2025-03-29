using System.Runtime.InteropServices;

namespace DiceRoll.Nodes
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct OperationDelegates
    {
        public readonly OperationDelegate Evaluation;
        public readonly OperationProbabilityDelegate Probability;
        
        public OperationDelegates(OperationDelegate evaluation, OperationProbabilityDelegate probability)
        {
            Evaluation = evaluation;
            Probability = probability;
        }
    }
}
