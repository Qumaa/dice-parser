using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class SequenceReducingHandler : LexemesReducingHandler
    {
        public override Range Reduce(LexemesList lexemes, in Range range, Cursor cursor)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.Count);

            if (length <= 1)
                return range;
            
            Mapped<Operand>[] operands = new Mapped<Operand>[length];

            for (int i = 0; i < length; i++)
            {
                Mapped<Lexeme> lexeme = lexemes.Get(start + i);
                
                cursor.MoveTo(in lexeme.Range);

                Mapped<Operand> operand = CastToOperandOrThrow(in lexeme, cursor);
                
                operands[i] = operand;
                
                cursor.MoveToPrevious();
            }

            Mapped<Operand> pool = SequenceUtils.GroupNodes(operands);

            lexemes.Replace(in range, in pool);

            return start..(start + 1);
        }

        private static Mapped<Operand> CastToOperandOrThrow(in Mapped<Lexeme> lexeme, Cursor cursor)
        {
            if (lexeme.TryCastValue(out Mapped<Operand> operand))
                return operand;

            throw new InvalidCastException($"Invalid element among operands: \"{cursor.GetSubstringOfCurrent()}\". Expected an operand.");
        }
    }
}
