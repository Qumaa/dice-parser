using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class SequenceReducingHandler : LexemesReducingHandler
    {
        public override Range Reduce(EquationParserState state, in Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(state.Lexemes.Count);

            if (length <= 1)
                return range;
            
            Mapped<Operand>[] operands = new Mapped<Operand>[length];
            Range groupRange = default;

            for (int i = 0; i < length; i++)
            {
                Mapped<Lexeme> lexeme = state.Lexemes.Get(start + i);
                
                state.Cursor.MoveTo(in lexeme.Range);

                Mapped<Operand> operand = CastToOperandOrThrow(in lexeme, state.Cursor, state.Mapper);
                
                operands[i] = operand;

                groupRange = i is 0 ? operand.Range : groupRange.And(operand.Range);
                
                state.Cursor.MoveToPrevious();
            }

            if (!SequenceUtils.TryGroupNodes(operands, out Mapped<Operand> pool))
            {
                state.Cursor.MoveTo(groupRange);
                throw new InvalidOperationException("All operands must be of the same type to be grouped.");
            }

            state.Lexemes.Replace(in range, in pool);

            return start..(start + 1);
        }

        private static Mapped<Operand> CastToOperandOrThrow(in Mapped<Lexeme> lexeme, Cursor cursor, InputMapper mapper)
        {
            if (lexeme.TryCastValue(out Mapped<Operand> operand))
                return operand;

            throw new InvalidCastException($"Invalid element among operands: \"{cursor.GetSubstringOfCurrent(mapper)}\". Expected an operand.");
        }
    }
}
