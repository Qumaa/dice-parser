using System;

namespace DiceRoll.Nodes
{
    public sealed class NotBinaryOperation : IOperation
    {
        private readonly IOperation _operation;
        
        public NotBinaryOperation(IOperation operation) 
        {
            ArgumentNullException.ThrowIfNull(operation);
            
            _operation = operation;
        }

        public Binary Evaluate() =>
            !_operation.Evaluate();

        public LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_operation.GetProbabilityDistribution().False);
    }
}
