using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class LinkedNode
    {
        public readonly Mapped<LinkedNode>[] Parents;
        public readonly INode Node;
        public readonly Type EvaluationType;

        public bool IsOperator => !IsOperand;

        public bool IsOperand => Parents.Length is 0;

        internal LinkedNode(INode node, Type evaluationType, Mapped<LinkedNode>[] parents)
        {
            CommonException.ThrowIfTypeIsNotNode(evaluationType);
            
            Node = node;
            Parents = parents;
            EvaluationType = evaluationType;
        }

        internal LinkedNode(INode node, Type evaluationType) : this(
            node,
            evaluationType,
            Array.Empty<Mapped<LinkedNode>>()
            ) { }
    }
}
