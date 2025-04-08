using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct OperationDelegates
    {
        public readonly OperationEvaluationDelegate Evaluation;
        public readonly OperationDistributionDelegate Distribution;
        public readonly AssertionEvaluationDelegate AssertionEvaluation;
        
        public OperationDelegates(OperationEvaluationDelegate evaluation, OperationDistributionDelegate distribution,
            AssertionEvaluationDelegate assertionEvaluation)
        {
            Evaluation = evaluation;
            AssertionEvaluation = assertionEvaluation;
            Distribution = distribution;
        }
    }
}
