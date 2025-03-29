using System;

namespace DiceRoll.Nodes
{
    public abstract class Operation : IOperation
    {
        protected readonly IAnalyzable _left;
        protected readonly IAnalyzable _right;

        protected Operation(IAnalyzable left, IAnalyzable right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
        }

        public abstract Binary Evaluate();

        public abstract LogicalProbabilityDistribution GetProbabilityDistribution();
        
        public abstract OperationVerboseEvaluation EvaluateVerbose();
    }
}
