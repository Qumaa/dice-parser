namespace DiceRoll.Input.Parsing
{
    public sealed class NodeTree
    {
        public readonly SubstringSource SubstringSource;
        public readonly Mapped<LinkedNode> Root;
        
        public NodeTree(SubstringSource substringSource, Mapped<LinkedNode> root)
        {
            SubstringSource = substringSource;
            Root = root;
        }
    }
}
