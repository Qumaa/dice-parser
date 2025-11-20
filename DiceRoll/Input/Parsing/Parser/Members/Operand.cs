using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class Operand : EquationMember
    {
        public readonly INode Node;
        public readonly Type EvaluationType;
        
        public Operand(INode node, Type evaluationType)
        {
            ArgumentNullException.ThrowIfNull(node);
            CommonException.ThrowIfTypeIsNotNodeOrNull(evaluationType);
            
            EvaluationType = evaluationType;
            Node = node;
        }
    }
}
