using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
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
        
        public static Mapped<Operand> GroupNodes(IEnumerable<Mapped<Operand>> nodes) =>
            GroupNodes(nodes.ToArray());
        
        public static Mapped<Operand> GroupNodes(Mapped<Operand>[] nodes)
        {
            Range poolRange = nodes[0].Range;
            
            for (int i = 1; i < nodes.Length; i++)
                poolRange = poolRange.And(nodes[i].Range);

            NodePool pool = new(nodes.Select(x => x.Value.Node));
            Operand linkedPool = new(pool, typeof(INode), nodes);
            
            return new Mapped<Operand>(linkedPool, poolRange);
        }
    }
}
