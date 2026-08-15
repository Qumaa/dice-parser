namespace DiceRoll.Input.Parsing
{
    public interface IGrammar
    {
        GrammarProbe ProbeContext(ParseContext context);
    }
}