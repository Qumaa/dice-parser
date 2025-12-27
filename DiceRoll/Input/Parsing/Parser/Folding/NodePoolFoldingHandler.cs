using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class NodePoolFoldingHandler : LexemesFoldingHandler
    {
        public override Range Fold(LexemesList lexemes, in Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.Count);

            if (length is 1)
                return range;

            Mapped<Operand>[] operands = lexemes.GetTypedOrThrow<Operand>(in range);

            Mapped<Operand> pool = NodePoolUtils.GroupNodes(operands);

            lexemes.Replace(in range, in pool);

            return start..(start + 1);
        }
    }
}
