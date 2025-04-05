namespace DiceRoll.Input
{
    public delegate INode UnaryInvocationHandler<in T>(T node) where T : INode;
}
