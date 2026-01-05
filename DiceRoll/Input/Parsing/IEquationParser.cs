namespace DiceRoll.Input.Parsing
{
    public interface IEquationParser
    {
        void AccumulateInput(string input);
        NodeTree ParseAccumulatedInput(UnknownLexemeSolver solver);
    }
}
