using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class Operand : Lexeme
    {
        public readonly INode Node;
        public readonly Type EvaluationType;
        public readonly Mapped<Operand>[] Parents;
        
        public Operand(INode node, Type evaluationType, Mapped<Operand>[] parents)
        {
            ArgumentNullException.ThrowIfNull(node);
            CommonException.ThrowIfTypeIsNotNodeOrNull(evaluationType);
            
            EvaluationType = evaluationType;
            Parents = parents ?? Array.Empty<Mapped<Operand>>();
            Node = node;
        }
        
        public Operand(INode node, Type evaluationType) : this(node, evaluationType, Array.Empty<Mapped<Operand>>()) { }
    }
    
    public static class OperandExtensions
    {
        public static Mapped<LinkedNode> ToLinkedNode(this Mapped<Operand> lexeme)
        {
            Operand operand = lexeme.Value;
            Mapped<Operand>[] source = operand.Parents;
            
            if (source.Length is 0)
                return new Mapped<LinkedNode>(new LinkedNode(operand.Node, operand.EvaluationType), lexeme.Range);

            Mapped<LinkedNode>[] linkedNodes = new Mapped<LinkedNode>[source.Length];

            for (int i = 0; i < linkedNodes.Length; i++)
                linkedNodes[i] = ToLinkedNode(source[i]);

            LinkedNode linkedNode = new(operand.Node, operand.EvaluationType, linkedNodes);
            return new Mapped<LinkedNode>(linkedNode, lexeme.Range);
        }

        public static Mapped<LinkedNode> ToLinkedNode(this Operand lexeme, in Range mappingRange) =>
            new Mapped<Operand>(lexeme, mappingRange).ToLinkedNode();
    }
}
