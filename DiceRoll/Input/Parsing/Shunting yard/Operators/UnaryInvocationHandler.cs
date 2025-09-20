namespace DiceRoll.Input.Parsing
{
    public delegate TReturn UnaryInvocationHandler<out TReturn, in T>(T node) where TReturn : INode where T : INode;
}
