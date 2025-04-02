namespace DiceRoll
{
    public interface INodeVisitor
    {
        void ForNumeric(INumeric numeric);
        void ForOperation(IOperation operation);
        void ForConditional(IConditional conditional);
    }
}
