using System;
using System.Text;

namespace DiceRoll.Input
{
    public sealed class DiceExpressionParsingException : Exception
    {
        public DiceExpressionParsingException(in MatchInfo context, string message) : base(GetMessage(in context, message)) { }

        public DiceExpressionParsingException(Exception innerException) :
            base(GetMessage(innerException), innerException) { }

        public DiceExpressionParsingException(in MatchInfo context, Exception innerException) :
            base(GetMessage(in context, innerException), innerException) { }

        private static string GetMessage(string message) =>
            $"Parsing failed.\n{message}";

        private static string GetMessage(Exception innerException) =>
            GetMessage(GetInnerMessage(innerException));

        private static string GetMessage(in MatchInfo context, string message) =>
            GetMessage(ContextToString(in context, message));

        private static string GetMessage(in MatchInfo context, Exception innerException) =>
            GetMessage(in context, GetInnerMessage(innerException));
        
        private static string GetInnerMessage(Exception innerException) =>
            $"{innerException.GetType().Name}: {innerException.Message}";
        
        private static string ContextToString(in MatchInfo context, string errorMessage)
        {
            StringBuilder stringBuilder = new();

            const string error_position = "> Error at position ";
            stringBuilder.Append(error_position);
            stringBuilder.Append(context.Start);
            stringBuilder.AppendLine();

            stringBuilder.Append('>');
            stringBuilder.Append(' ');
            stringBuilder.Append(context.Source);
            stringBuilder.AppendLine();
            
            stringBuilder.Append('>');
            stringBuilder.Append(' ', 1 + context.Start);
            stringBuilder.Append('\u2514'); // └
            stringBuilder.Append('\u2500', context.UntilSourceEnd); // ─
            stringBuilder.Append(' ');

            stringBuilder.Append('[');
            stringBuilder.Append(' ');
            stringBuilder.Append(errorMessage);
            stringBuilder.Append(' ');
            stringBuilder.Append(']');
            stringBuilder.AppendLine();
            
            return stringBuilder.ToString();
        }
    }
}
