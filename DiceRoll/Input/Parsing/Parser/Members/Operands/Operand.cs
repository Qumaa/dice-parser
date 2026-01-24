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
        public static LinkedNode ToLinkedNode(this Mapped<Operand> lexeme)
        {
            Operand operand = lexeme.Value;
            Mapped<Operand>[] parents = operand.Parents;
            
            if (parents.Length is 0)
                return new LinkedNode(operand.Node, lexeme.Range);

            LinkedNode[] linkedParents = new LinkedNode[parents.Length];

            for (int i = 0; i < linkedParents.Length; i++)
                linkedParents[i] = ToLinkedNode(parents[i]);

            return new LinkedNode(operand.Node, in lexeme.Range, linkedParents);
        }

        public static LinkedNode ToLinkedNode(this Operand lexeme, in Range mappingRange) =>
            new Mapped<Operand>(lexeme, mappingRange).ToLinkedNode();
    }
}
