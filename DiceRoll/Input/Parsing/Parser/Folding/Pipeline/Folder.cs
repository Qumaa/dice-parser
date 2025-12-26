namespace DiceRoll.Input.Parsing
{
    public abstract class Folder
    {
        public abstract void Execute(LexemesList lexemes);
    }

    public sealed class ParenthesisFolder : Folder
    {
        public override void Execute(LexemesList lexemes) =>
            throw new System.NotImplementedException();
    }

    public sealed class OperatorFolder : Folder
    {
        public override void Execute(LexemesList lexemes) =>
            throw new System.NotImplementedException();
    }

    public sealed class NodePoolFolder : Folder
    {
        public override void Execute(LexemesList lexemes) =>
            throw new System.NotImplementedException();
    }
}
