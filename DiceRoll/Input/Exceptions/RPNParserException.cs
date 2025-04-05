using System;
using System.Text;

namespace DiceRoll.Input
{
    public sealed class RPNParserException : Exception
    {
        private const char _HIGHLIGHT_BACKGROUND_SUCCESSFUL = '.';
        private const char _HIGHLIGHT_BACKGROUND_FAILED = '/';
        private const char _HIGHLIGHT_EXCEPTION_CAUSE = '?';

        public RPNParserException(in MatchInfo context, string message) : base(GetMessage(in context, message)) { }

        public RPNParserException(Exception innerException) :
            base(GetMessage(innerException), innerException) { }

        public RPNParserException(in MatchInfo context, Exception innerException) :
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
            stringBuilder.Append('\u2514');
            stringBuilder.Append('\u2500', context.UntilSourceEnd);
            stringBuilder.Append(' ');

            stringBuilder.Append('[');
            stringBuilder.Append(' ');
            stringBuilder.Append(errorMessage);
            stringBuilder.Append(' ');
            stringBuilder.Append(']');
            stringBuilder.AppendLine();

            // const string in_expression = "In expression ";
            // stringBuilder.Append(in_expression);
            // stringBuilder.AppendLine(context.Source.ToString());
            //
            // const string position = "Position:";
            // stringBuilder.Append(position);
            // stringBuilder.Append(' ', in_expression.Length - position.Length);
            // stringBuilder.Append(_HIGHLIGHT_BACKGROUND_SUCCESSFUL, context.Start);
            // stringBuilder.Append(_HIGHLIGHT_EXCEPTION_CAUSE, context.Length);
            // stringBuilder.Append(_HIGHLIGHT_BACKGROUND_FAILED, context.Source.Length - (context.Start + context.Length));
            
            return stringBuilder.ToString();
        }
    }
}
