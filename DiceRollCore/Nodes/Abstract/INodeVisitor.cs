namespace DiceRoll
{
    public interface INodeVisitor
    {
        void ForNumeric(INumeric numeric);
        void ForAssertion(IAssertion assertion);
        void ForOperation(IOperation operation);
        // todo:
        void ForNodePool<T>(INodePool<T> pool) where T : INode;
        // void ForOther(INode other);
    }
}
