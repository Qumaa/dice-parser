namespace DiceRoll.Input
{
    public interface IParser
    {
        bool TryParse(in string expression, IParsingSwitch parsingSwitch, out ParsingExceptionInfo exceptionInfo);
    }
}
