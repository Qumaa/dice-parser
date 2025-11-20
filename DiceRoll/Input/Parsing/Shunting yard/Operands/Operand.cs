using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing.Deprecated
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Operand
    {
        public readonly INode Node;
        public readonly Type EvaluationType;

        internal Operand(INode node, Type evaluationType)
        {
            Node = node;
            EvaluationType = evaluationType;
        }
    }
}
