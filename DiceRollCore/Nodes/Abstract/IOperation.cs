namespace DiceRoll
{
    public interface IOperation : INode<Optional<Outcome>>,
        IDistributable<OptionalRollProbabilityDistribution, OptionalRoll>
    {
        IAssertion AsAssertion { get; }
    }

    public static class OperationExtensions
    {
        public static IAssertion And(this IAssertion node, IAssertion other) =>
            Node.Operator.And(node, other);
            
        public static IAssertion Or(this IAssertion node, IAssertion other) =>
            Node.Operator.Or(node, other);
            
        public static IAssertion Equal(this IAssertion node, IAssertion other) =>
            Node.Operator.Equal(node, other);
        
        public static IAssertion NotEqual(this IAssertion node, IAssertion other) =>
            Node.Operator.NotEqual(node, other);
            
        public static IAssertion Not(this IAssertion node) =>
            Node.Operator.Not(node);
    }
}
