using System;
using System.Collections.Generic;
using System.CommandLine;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    internal static class ExpressionParsingHelper
    {
        private static ExpressionParser _expressionParser = new(TokensTable.Default);

        public static void SetTokens(TokensTable tokens) =>
            _expressionParser = new ExpressionParser(tokens);

        public static bool Try(IEnumerable<string> expression, IConsole exceptionOutput, out INode output)
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
    }
}
