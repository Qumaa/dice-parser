namespace DiceRoll.Input
{
    public delegate INode UnaryParsingHandler<in T>(T node) where T : INode;
}
