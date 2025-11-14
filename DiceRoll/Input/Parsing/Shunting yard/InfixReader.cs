using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class InfixReader
    {
        private readonly ShuntingYardState _state;
        private readonly TokenGroupChain _chain;
        
        public InfixReader(ShuntingYardState state, TokenGroupChain chain)
        {
            _state = state;
            _chain = chain;
        }
        
        public void Read(string expression)
        {
            _state.Mapper.Append(expression);
            ParseTokensIteratively(expression);
        }

        private void ParseTokensIteratively(in Substring expression)
        {
            Substring notParsed = expression.Trim();
            
            do notParsed = ParseSubstringStartOrThrow(in notParsed); 
            while (!notParsed.IsEmpty);
        }
        
        private Substring ParseSubstringStartOrThrow(in Substring notParsed)
        {
            Substring parsed = notParsed;

            try
            {
                ParseSubstringStart(in notParsed, out parsed);
                return notParsed.MoveStart(parsed.Length).TrimStart();
            }
            catch (Exception e)
            {
                throw new ParsingException(_state.Mapper.MapAndGetSubstringOf(in parsed), e);
            }
        }

        private void ParseSubstringStart(in Substring notParsed, out Substring parsed)
        {
            if (_chain.TryExecuteAll(in notParsed, out parsed))
                return;

            throw new UnknownTokenException(in parsed);
        }

        // todo this lazy shit patch barely works
        
        // private sealed class InlineParser : FlatOperandParser
        // {
        //     private readonly InfixReader _infixReader;
        //     private readonly int _capturedOperands;
        //     
        //     public InlineParser(InfixReader infixReader)
        //     {
        //         _infixReader = infixReader;
        //         _capturedOperands = infixReader._state.Operands.Count;
        //     }
        //
        //     public override INode Parse(in Substring expression)
        //     {
        //         _infixReader.OpenParenthesis(default); // todo
        //         _infixReader.ParseTokensIteratively(in expression);
        //         _infixReader.CloseParenthesis();
        //         
        //         // todo if not 1 operands produced then throw
        //
        //         return _infixReader._operands.Pop().Value.Node;
        //     }
        // }
    }
}
