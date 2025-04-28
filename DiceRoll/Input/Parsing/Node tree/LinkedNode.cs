using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class LinkedNode
    {
        public readonly INode Node;
        public readonly Mapped<LinkedNode>[] Parents;

        public bool IsOperator => Node is null;

        public bool IsOperand => !IsOperator;
        
        public LinkedNode(INode node, Mapped<LinkedNode>[] parents)
        {
            Node = node;
            Parents = parents;
        }

        public LinkedNode(INode node) : this(node, Array.Empty<Mapped<LinkedNode>>()) { }

        public LinkedNode(INode node, Mapped<LinkedNode> parent) : this(node, new[] { parent }) { }
    }
}
