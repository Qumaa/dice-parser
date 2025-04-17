namespace DiceRoll.Input.Parsing
{
    public delegate INode UnaryInvocationHandler<in T>(T node) where T : INode;
}
