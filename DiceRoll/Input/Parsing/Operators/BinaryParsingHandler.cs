namespace DiceRoll.Input
{
    public delegate INode BinaryParsingHandler<in TLeft, in TRight>(TLeft left, TRight right)
        where TLeft : INode where TRight : INode;
}
