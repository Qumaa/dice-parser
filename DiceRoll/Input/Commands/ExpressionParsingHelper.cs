using System;
using System.Collections.Generic;
using System.CommandLine;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    internal static class ExpressionParsingHelper
    {
        private static readonly ExpressionParser _expressionParser = new(TokensTable.Default, OperandCastersTable.Default);

        public static bool Try(IEnumerable<string> expression, IConsole exceptionOutput, out NodeTree output)
        {
            try
            {
                output = _expressionParser.Parse(expression);
                return true;
            }
            catch (Exception e)
            {
                exceptionOutput.WriteLine(e.Message);
                output = null;
                return false;
            }
        }
    }

    internal static class ConsoleExtensions
    {
        public static void Space(this IConsole console) =>
            console.Write(" ");
        
        public static void Space(this IConsole console, int count)
        {
            for (int i = 0; i < count; i++)
                console.Space();
        }
        
        public static void WriteLine(this IConsole console) =>
            console.WriteLine(string.Empty);
    }
}
