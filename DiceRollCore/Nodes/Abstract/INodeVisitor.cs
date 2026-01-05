namespace DiceRoll
{
    public interface INodeVisitor
    {
        void ForNumeric(INumeric numeric);
        void ForAssertion(IAssertion assertion);
        void ForOperation(IOperation operation);
        void ForSequence<T>(ISequence<T> sequence) where T : INode;
        // void ForOther(INode other);
    }
}
