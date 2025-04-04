using System;
using System.Text.RegularExpressions;
using DiceRoll.Input;

namespace DiceRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RPNBuilder builder = new(BuildTable());

            foreach (string s in args)
                builder.Push(s);

            var output = builder.Build();

            foreach (string token in output)
            {
                Console.Write(token);
                Console.Write(" ");
            }
        }

        private static TokensTable BuildTable()
        {
            TokensTableBuilder builder = new("(", ")");
            
            builder.AddOperandToken(static x => Node.Value.Constant(int.Parse(x)), new Regex(@"^\d+$"));
            builder.AddOperandToken(DiceOperand.Default);
            
            // todo: unary
            // builder.AddOperatorToken(200, "!", "not");
            // builder.AddOperatorToken(200, "-");
            
            builder.AddOperatorToken(100, "*", "x");
            builder.AddOperatorToken(100, "/");
            
            builder.AddOperatorToken(90, "+");
            builder.AddOperatorToken(90, "-");
            
            builder.AddOperatorToken(80, ">");
            builder.AddOperatorToken(80, ">=");
            builder.AddOperatorToken(80, "<");
            builder.AddOperatorToken(80, "<=");
            
            builder.AddOperatorToken(70, "=", "==");
            builder.AddOperatorToken(70, "!=", "=/=");
            
            builder.AddOperatorToken(60, "&", "&&", "and");
            builder.AddOperatorToken(60, "|", "||", "or");

            return builder.Build();
        }
    }
}
