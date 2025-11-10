using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct OperationDelegates
    {
        public readonly OperationEvaluationDelegate Evaluation;
        public readonly OperationDistributionDelegate Distribution;
        public readonly AssertionDistributionDelegate AssertionDistribution;
        
        public OperationDelegates(OperationEvaluationDelegate evaluation, OperationDistributionDelegate distribution,
            AssertionDistributionDelegate assertionDistribution)
        {
            Evaluation = evaluation;
            AssertionDistribution = assertionDistribution;
            Distribution = distribution;
        }
    }
}
