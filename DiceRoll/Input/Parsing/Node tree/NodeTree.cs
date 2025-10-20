namespace DiceRoll.Input.Parsing
{
    public sealed class NodeTree
    {
        public readonly SubstringMapper SubstringMapper;
        public readonly Mapped<LinkedNode> Root;

        internal NodeTree(SubstringMapper substringMapper, Mapped<LinkedNode> root)
        {
            SubstringMapper = substringMapper;
            Root = root;
        }
    }

    public static class NodeTreeExtensions
    {
        public static void Next(this NodeTree nodeTree) =>
            nodeTree.Root.Value.Node.NextEvaluation();
    }
}
