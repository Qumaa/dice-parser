namespace DiceRoll.Input.Parsing
{
    public delegate INode BinaryInvocationHandler<in TLeft, in TRight>(TLeft left, TRight right)
        where TLeft : INode where TRight : INode;
}
