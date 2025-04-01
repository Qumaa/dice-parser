namespace DiceRoll
{
    public abstract class BinaryOperation : IOperation
    {
        protected readonly IOperation _left;
        protected readonly IOperation _right;

        protected BinaryOperation(IOperation left, IOperation right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            
            _left = left;
            _right = right;
        }

        public abstract Binary Evaluate();
        
        public abstract LogicalProbabilityDistribution GetProbabilityDistribution();
    }
}
