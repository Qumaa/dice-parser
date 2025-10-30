using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandDefinition
    {
        public readonly IToken Token;
        public readonly OperandParser Parser;
        public readonly Type EvaluationType;

        public OperandDefinition(IToken token, OperandParser parser, Type evaluationType)
        {
            ArgumentNullException.ThrowIfNull(token);
            ArgumentNullException.ThrowIfNull(parser);
            CommonException.ThrowIfTypeIsNotNode(evaluationType);
            
            Token = token;
            Parser = parser;
            EvaluationType = evaluationType;
        }

        public static OperandDefinition OfType<T>(IToken token, OperandParser parsingHandler) where T : INode =>
            new(token, parsingHandler, typeof(T));

        public static OperandDefinition OfType<T>(IToken token, FlatOperandParsingHandler parsingHandler)
            where T : INode =>
            OfType<T>(token, OperandParser.FromDelegate(parsingHandler));

        public static OperandDefinition OfType<T>(IToken token, RecursiveOperandParsingHandler parsingHandler)
            where T : INode =>
            OfType<T>(token, OperandParser.FromDelegate(parsingHandler));
    }
}
