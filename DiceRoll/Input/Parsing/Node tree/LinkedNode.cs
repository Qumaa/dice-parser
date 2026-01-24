using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class LinkedNode
    {
        public readonly LinkedNode[] Parents;
        public readonly INode Node;
        public readonly Range MappingRange;

        public bool IsOperator => !IsOperand;

        public bool IsOperand => Parents.Length is 0;

        internal LinkedNode(INode node, in Range mappingRange, LinkedNode[] parents)
        {
            Node = node;
            Parents = parents;
            MappingRange = mappingRange;
        }

        internal LinkedNode(INode node, in Range mappingRange) : this(node, mappingRange, Array.Empty<LinkedNode>()) { }
    }
}
