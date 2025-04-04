namespace DiceRoll.Input
{
    public delegate INode UnaryOperatorParseHandler<in T>(T node) where T : INode;
}
