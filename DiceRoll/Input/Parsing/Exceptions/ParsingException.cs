using System;
using System.Text;

namespace DiceRoll.Input.Parsing
{
    public sealed class ParsingException : Exception
    {
        internal ParsingException(in Substring context, string message) : base(GetMessage(in context, message)) { }

        internal ParsingException(in Substring context, Exception innerException) :
            base(GetMessage(in context, innerException), innerException) { }
        
        private static string GetMessage(string message) =>
            $"Parsing failed.\n{message}";

        private static string GetMessage(in Substring context, string message) =>
            GetMessage(ContextToString(in context, message));

        private static string GetMessage(in Substring context, Exception innerException) =>
            GetMessage(in context, innerException.Message);

        private static string ContextToString(in Substring context, string errorMessage)
        {
            const char error_arrow_tip = '\u2514';  // └
            const char error_arrow_head = '\u2500'; // ─
            const char error_arrow_neck = '\u2534'; // ┴
            const char error_arrow_tail = error_arrow_head;
            
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
            stringBuilder.Append(error_arrow_tip); 

            if (context.Length > 1)
            {
                stringBuilder.Append(error_arrow_head, context.Length - 2); 
                stringBuilder.Append(error_arrow_neck); 
            }
            
            stringBuilder.Append(error_arrow_tail, context.Source.Length - context.End); 
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
