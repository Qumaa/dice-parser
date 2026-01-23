using System;
using System.Runtime.InteropServices;

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

        public static Substring RootSubstring(this NodeTree tree) =>
            tree.SubstringMapper.GetSubstring(in tree.Root.Range);

        public static Navigator Navigate(this NodeTree tree) =>
            new(tree);
        
        [StructLayout(LayoutKind.Auto)]
        public readonly struct Navigator
        {
            private readonly SubstringMapper _mapper;
            private readonly Mapped<LinkedNode> _root;

            public Mapped<LinkedNode> Current => _root;

            public Navigator(NodeTree tree) : this(tree.Root, tree.SubstringMapper) { }

            private Navigator(Mapped<LinkedNode> root, SubstringMapper mapper)
            {
                _root = root;
                _mapper = mapper;
            }

            public Navigator Descend(Index index) =>
                new(_root.Value.Parents[index], _mapper);

            public NodeTree ExtractSubtree() =>
                new(_mapper, _root);
        }
    }
}
