using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParserState
    {
        public readonly OperandCastingTable CastingTable;
        public readonly LexemesList Lexemes;
        public readonly InputMapper Mapper;

        public EquationParserState(OperandCastingTable castingTable)
        {
            ArgumentNullException.ThrowIfNull(castingTable);
            
            CastingTable = castingTable;
            
            Mapper = new InputMapper();
            Lexemes = new LexemesList(Mapper);
        }

        public void Reset()
        {
            Lexemes.Clear();
            Mapper.Clear();
        }
    }
}
