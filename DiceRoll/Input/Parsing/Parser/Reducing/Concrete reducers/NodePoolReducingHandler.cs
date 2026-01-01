using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class NodePoolReducingHandler : LexemesReducingHandler
    {
        public override Range Reduce(LexemesList lexemes, in Range range, ReducerCursor cursor)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.Count);

            if (length <= 1)
                return range;
            
            Mapped<Operand>[] operands = new Mapped<Operand>[length];

            for (int i = 0; i < length; i++)
            {
                Mapped<Lexeme> lexeme = lexemes.Get(start + i);
                
                cursor.MoveTo(in lexeme.Range);

                if (!lexeme.TryCastValue(out Mapped<Operand> operand))
                    throw new InvalidCastException("Unable to group operands since there is an operator among them.");
                
                operands[i] = operand;
                
                cursor.MoveToPrevious();
            }

            Mapped<Operand> pool = NodePoolUtils.GroupNodes(operands);

            lexemes.Replace(in range, in pool);

            return start..(start + 1);
        }
    }
}
