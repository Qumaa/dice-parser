namespace DiceRoll
{
    public interface INodeVisitor
    {
        void ForNumeric(INumeric numeric);
        void ForAssertion(IAssertion assertion);
        void ForOperation(IOperation operation);
        // todo:
        // void ForNodePool(INodePool pool);
        // void ForOther(INode other);
    }
}
