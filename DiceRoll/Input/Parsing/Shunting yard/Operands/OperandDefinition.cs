using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandDefinition
    {
        public readonly IToken Token;
        public readonly OperandParsingHandler ParsingHandler;
        public readonly Type OperandType;

        private OperandDefinition(IToken token, OperandParsingHandler parsingHandler, Type operandType)
        {
            Token = token;
            ParsingHandler = parsingHandler;
            OperandType = operandType;
        }

        public static OperandDefinition New<T>(IToken token, OperandParsingHandler parsingHandler) where T : INode =>
            new(token, parsingHandler, typeof(T));
    }
}
