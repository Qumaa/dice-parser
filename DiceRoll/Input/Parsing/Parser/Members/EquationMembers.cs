using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationMembers
    {
        private readonly InputMapper _mapper;
        private readonly List<Mapped<EquationMember>> _members;
        
        public EquationMembers(InputMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            
            _mapper = mapper;
            _members = new List<Mapped<EquationMember>>();
        }

        public void Push(EquationMember member, in Substring substring)
        {
            if (member is null)
                return;
            
            _members.Add(_mapper.Map(member, in substring));
        }

        public void Clear() =>
            _members.Clear();
    }
}
