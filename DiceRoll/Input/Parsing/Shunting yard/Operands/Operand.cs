using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Operand
    {
        public readonly INode Node;
        public readonly Type EvaluationType;
        
        public Operand(INode node, Type evaluationType)
        {
            Node = node;
            EvaluationType = evaluationType;
        }

        public Operand(OperandDefinition definition, in Substring substring) : this(
            definition.ParsingHandler(substring),
            definition.EvaluationType
            ) { }
    }
}
