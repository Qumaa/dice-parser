using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationReader
    {
        private readonly InputMapper _inputMapper;
        private readonly LexemesList _lexemes;
        private readonly LexingPipeline _pipeline;
        public EquationReader(InputMapper inputMapper, LexemesList lexemes, LexingPipeline pipeline)
        {
            ArgumentNullException.ThrowIfNull(inputMapper);
            ArgumentNullException.ThrowIfNull(lexemes);
            ArgumentNullException.ThrowIfNull(pipeline);
            
            _inputMapper = inputMapper;
            _lexemes = lexemes;
            _pipeline = pipeline;
        }

        public void Read(string equation)
        {
            // todo with cursor
            if (string.IsNullOrWhiteSpace(equation))
                return;
            
            AppendToMapper(equation);
            ReadIteratively(equation);
        }

        private void AppendToMapper(string equation) =>
            _inputMapper.Append(equation);

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
                if (!_pipeline.TryExecuteAll(in toRead, out read))
                    PushUnresolvedMember(in read);

                return read;
            }
            catch (Exception e)
            {
                Substring exceptionCause = _inputMapper.MapAndGetSubstringOf(in read);
                
                throw new ParsingException(exceptionCause, e);
            }
        }

        private void PushUnresolvedMember(in Substring memberSubstring) =>
            _lexemes.Push(UnknownLexeme.Shared, in memberSubstring);
    }
}
