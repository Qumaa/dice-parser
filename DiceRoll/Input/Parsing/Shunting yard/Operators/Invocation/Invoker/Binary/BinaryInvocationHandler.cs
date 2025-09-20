namespace DiceRoll.Input.Parsing
{
    public delegate TReturn BinaryInvocationHandler<out TReturn, in TLeft, in TRight>(TLeft left, TRight right)
        where TReturn : INode where TLeft : INode where TRight : INode;
}
