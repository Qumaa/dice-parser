using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandDefinition
    {
        public readonly IToken Token;
        public readonly OperandParsingHandler ParsingHandler;
        public readonly Type EvaluationType;

        private OperandDefinition(IToken token, OperandParsingHandler parsingHandler, Type evaluationType)
        {
            Token = token;
            ParsingHandler = parsingHandler;
            EvaluationType = evaluationType;
        }

        public static OperandDefinition New<T>(IToken token, OperandParsingHandler parsingHandler) where T : INode =>
            new(token, parsingHandler, typeof(T));
    }
}
