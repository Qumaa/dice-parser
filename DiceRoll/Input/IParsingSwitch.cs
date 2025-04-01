namespace DiceRoll.Input
{
    public interface IParsingSwitch
    {
        void ForNumeric(IAnalyzable numeric);
        void ForOperation(IOperation operation);
    }
}
