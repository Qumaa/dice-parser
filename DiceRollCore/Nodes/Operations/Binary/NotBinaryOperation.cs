using System;

namespace DiceRoll
{
    public sealed class NotBinaryOperation : Operation
    {
        private readonly IOperation _operation;
        
        public NotBinaryOperation(IOperation operation) 
        {
            ArgumentNullException.ThrowIfNull(operation);
            
            _operation = operation;
        }

        public override Binary Evaluate() =>
            !_operation.Evaluate();

        public override LogicalProbabilityDistribution GetProbabilityDistribution() =>
            new(_operation.GetProbabilityDistribution().False);
    }
}
