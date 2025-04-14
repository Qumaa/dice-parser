using System;
using System.Text;

namespace DiceRoll.Input
{
    public sealed class ParsingException : Exception
    {
        private const char _ERROR_ARROW_TIP = '\u2514';
        private const char _ERROR_ARROW_HEAD = '\u2500';
        private const char _ERROR_ARROW_NECK = '\u2534';
        private const char _ERROR_ARROW_TAIL = '\u2500';
        
        public ParsingException(in Substring context, string message) : base(GetMessage(in context, message)) { }

        public ParsingException(in Substring context, Exception innerException) :
            base(GetMessage(in context, innerException), innerException) { }
        
        private static string GetMessage(string message) =>
            $"Parsing failed.\n{message}";

        private static string GetMessage(in Substring context, string message) =>
            GetMessage(ContextToString(in context, message));

        private static string GetMessage(in Substring context, Exception innerException) =>
            GetMessage(in context, innerException.Message);

        private static string ContextToString(in Substring context, string errorMessage)
        {
            StringBuilder stringBuilder = new();

            // header
            const string error_position = "> Error at position ";
            stringBuilder.Append(error_position);
            stringBuilder.Append(context.Start);
            if (context.Length > 1)
            {
                stringBuilder.Append("..");
                stringBuilder.Append(context.End - 1);
            }
            stringBuilder.AppendLine();

            // the formula
            stringBuilder.Append('>');
            stringBuilder.Append(' ');
            stringBuilder.Append(context.Source);
            stringBuilder.AppendLine();
            
            // the arrow pointer
            stringBuilder.Append('>');
            stringBuilder.Append(' ', 1 + context.Start);
            stringBuilder.Append(_ERROR_ARROW_TIP); // └

            if (context.Length > 1)
            {
                stringBuilder.Append(_ERROR_ARROW_HEAD, context.Length - 2); // ─
                stringBuilder.Append(_ERROR_ARROW_NECK); // ┴
            }
            
            stringBuilder.Append(_ERROR_ARROW_TAIL, context.UntilSourceEnd); // ─
            stringBuilder.Append(' ');

            // the error message
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
