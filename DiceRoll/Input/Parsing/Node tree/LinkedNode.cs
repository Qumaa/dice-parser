using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class LinkedNode
    {
        public readonly Mapped<LinkedNode>[] Parents;
        private readonly INode _node;
        public readonly Type EvaluationType;

        public bool IsOperator => _node is null;
        
        private LinkedNode(INode node, Type evaluationType, Mapped<LinkedNode>[] parents)
        {
            _node = node;
            Parents = parents;
            EvaluationType = evaluationType;
        }

        public bool IsOperand(out INode operand)
        {
            if (IsOperator)
            {
                operand = null;
                return false;
            }

            operand = _node;
            return true;
        }

        public static LinkedNode Operand(in Operand operand, in Mapped<LinkedNode>[] parents) =>
            new(operand.Node, operand.EvaluationType, parents);

        public static LinkedNode Operand(INode node, Type evaluationType, in Mapped<LinkedNode>[] parents) =>
            Operand(new Operand(node, evaluationType), parents);

        public static LinkedNode Operand(in Operand operand, in Mapped<LinkedNode> parent) =>
            Operand(operand, new[] { parent });

        public static LinkedNode Operand(INode node, Type evaluationType, in Mapped<LinkedNode> parent) =>
            Operand(new Operand(node, evaluationType), parent);

        public static LinkedNode Operand(in Operand operand) =>
            Operand(operand, Array.Empty<Mapped<LinkedNode>>());

        public static LinkedNode Operand(INode node, Type evaluationType) =>
            Operand(new Operand(node, evaluationType));

        public static LinkedNode Operator(Mapped<LinkedNode>[] parents, Type evaluationType) =>
            new(null, evaluationType, parents);

        public static LinkedNode Operator(in Mapped<LinkedNode> parent, Type evaluationType) =>
            Operator(new[] { parent }, evaluationType);
    }
}
