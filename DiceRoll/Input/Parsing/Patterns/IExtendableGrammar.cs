namespace DiceRoll.Input.Parsing
{
    /// <summary>
    /// An <see cref="IGrammar"/> that contains multiple other grammars (a, b, c ...)
    /// </summary>
    public interface IExtendableGrammar : IGrammar
    {
        void Add(IGrammar grammar);
    }
}