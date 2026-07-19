using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationReader
    {
        private readonly EquationParserState _state;
        private readonly LexingPipeline _pipeline;

        public EquationReader(EquationParserState state, LexingPipeline pipeline)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(pipeline);
            
            _state = state;
            _pipeline = pipeline;
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
                if (!_pipeline.TryExecuteAll(in toRead, _state.Cursor, out read))
                    PushUnresolvedMember(in read);

                return read;
            }
            catch (Exception e)
            {
                Substring exceptionCause = _state.Cursor.GetSubstringOfCurrent(_state.Mapper);
                
                throw new ParsingException(exceptionCause, e);
            }
        }

        private void PushUnresolvedMember(in Substring memberSubstring) =>
            _state.Lexemes.Push(UnknownLexeme.Shared, in memberSubstring);
    }
}
