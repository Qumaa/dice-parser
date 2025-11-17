using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
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

    internal static class NodePoolUtils
    {
        public static Mapped<LinkedNode> GroupNodes(IEnumerable<Mapped<LinkedNode>> nodes) =>
            GroupNodes(nodes.ToArray());

        public static Mapped<LinkedNode> GroupNodes(Mapped<LinkedNode>[] nodes)
        {
            Range poolRange = nodes[0].Range;
            
            for (int i = 1; i < nodes.Length; i++)
                poolRange = poolRange.And(nodes[i].Range);

            NodePool pool = new(nodes.Select(x => x.Value.Node));
            LinkedNode linkedPool = new(pool, typeof(INode), nodes);
            
            return new Mapped<LinkedNode>(linkedPool, poolRange);
        }
    }
}
