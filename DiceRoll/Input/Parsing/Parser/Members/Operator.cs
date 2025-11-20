namespace DiceRoll.Input.Parsing
{
    public sealed class Operator : EquationMember
    {
        public readonly OperatorDefinition[] Definitions;
        
        public Operator(OperatorDefinition[] definitions)
        {
            Definitions = definitions;
        }
    }
}
