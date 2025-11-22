namespace DiceRoll.Input.Parsing
{
    public abstract class Folder
    {
        public abstract void Execute(EquationParserState state);
    }

    public sealed class ParenthesisFolder : Folder
    {
        public override void Execute(EquationParserState state) =>
            throw new System.NotImplementedException();
    }

    public sealed class OperatorFolder : Folder
    {
        public override void Execute(EquationParserState state) =>
            throw new System.NotImplementedException();
    }

    public sealed class NodePoolFolder : Folder
    {
        public override void Execute(EquationParserState state) =>
            throw new System.NotImplementedException();
    }
}
