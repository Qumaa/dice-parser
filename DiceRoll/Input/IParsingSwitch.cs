namespace DiceRoll.Input
{
    public interface IParsingSwitch
    {
        void ForNumeric(INumeric numeric);
        void ForOperation(IOperation operation);
    }
}
