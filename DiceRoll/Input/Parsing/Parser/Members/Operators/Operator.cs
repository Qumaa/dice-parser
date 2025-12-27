namespace DiceRoll.Input.Parsing
{
    public sealed class Operator : Lexeme
    {
        public readonly OperatorDefinition[] Definitions;
        
        public Operator(OperatorDefinition[] definitions)
        {
            Definitions = definitions;
        }
    }
}
