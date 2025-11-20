using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationReader
    {
        private readonly EquationParserState _state;
        private readonly TokenGroupChain _chain;
        
        public EquationReader(EquationParserState state, TokenGroupChain chain)
        {
            _state = state;
            _chain = chain;
        }

        public void Read(string equation)
        {
            if (string.IsNullOrWhiteSpace(equation))
                return;
            
            AppendToMapper(equation);
            ReadIteratively(equation);
        }

        private void AppendToMapper(string equation) =>
            _state.Mapper.Append(equation);

        private void ReadIteratively(in Substring equation)
        {
            Substring toRead = equation.Trim();
            
            do toRead = ReadEquationStartAndAdvance(in toRead); 
            while (!toRead.IsEmpty);
        }
        
        private Substring ReadEquationStartAndAdvance(in Substring toRead)
        {
            Substring read = ReadEquationStartOrThrow(in toRead);
            return toRead.SetStart(read.End).TrimStart();
        }
        
        private Substring ReadEquationStartOrThrow(in Substring toRead)
        {
            Substring read = toRead;
            
            try
            {
                if (!_chain.TryExecuteAll(in toRead, out read))
                    PushUnresolvedMember(in read);

                return read;
            }
            catch (Exception e)
            {
                Substring exceptionCause = _state.Mapper.MapAndGetSubstringOf(in read);
                
                _state.Reset();
                
                throw new ParsingException(exceptionCause, e);
            }
        }

        private void PushUnresolvedMember(in Substring memberSubstring) =>
            _state.Members.Push(UnresolvedMember.Shared, in memberSubstring);
    }
}
