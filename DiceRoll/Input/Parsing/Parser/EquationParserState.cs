using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParserState
    {
        public readonly OperandCastingTable CastingTable;
        public readonly EquationMembers Members;
        public readonly InputMapper Mapper;

        public EquationParserState(OperandCastingTable castingTable)
        {
            ArgumentNullException.ThrowIfNull(castingTable);
            
            CastingTable = castingTable;
            
            Mapper = new InputMapper();
            Members = new EquationMembers(Mapper);
        }

        public void Reset()
        {
            Members.Clear();
            Mapper.Clear();
        }
    }
}
