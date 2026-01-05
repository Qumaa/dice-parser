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
            const char error_indicator = '^';
            
            StringBuilder stringBuilder = new();

            // header
            if (context.Length > 0)
            {
                const string error_position = "Error at position ";
                stringBuilder.Append(error_position);
                stringBuilder.Append(context.Start + 1);
                if (context.Length > 1)
                {
                    stringBuilder.Append("..");
                    stringBuilder.Append(context.End);
                }

                stringBuilder.AppendLine();
            }

            // the formula
            stringBuilder.Append(context.Source);
            stringBuilder.AppendLine();
            
            // the arrow pointer
            stringBuilder.Append(' ', context.Start);
            for (int i = 0; i < context.Length; i++)
                stringBuilder.Append(error_indicator);

            // the error message
            stringBuilder.AppendLine();
            stringBuilder.Append(errorMessage);
            
            return stringBuilder.ToString();
        }
    }
}
