using System;
using System.Collections.Generic;
using System.CommandLine;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    public class Program
    {
        private static readonly ExpressionParser _expressionParser = new(TokensTable.Default);
        
        public static void Main(string[] args)
        {
            args = "analyze d20 10".Split(' ');
            
            RootCommand root = new("Enters the perpetual mode or evaluates the passed expression.");
            
            Command roll = new("roll", "Evaluate the passed expression to one result.");
            roll.AddAlias("r");

            Command analyze = new(
                "analyze",
                "Evaluate the passed expression to all possible results and specify their probabilities."
                );
            analyze.AddAlias("a");

            Argument<IEnumerable<string>> expression = new("Dice expression", "The expression to be evaluated.");

            roll.AddArgument(expression);
            roll.SetHandler(
                context =>
                {
                    IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(expression);
                    
                    if (TryParseExpression(tokens, context.Console, out INode node))
                        node.Visit(new RollVisitor(context.Console));
                });
            
            analyze.AddArgument(expression);
            analyze.SetHandler(context =>
            {
                IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(expression);
                    
                if (TryParseExpression(tokens, context.Console, out INode node))
                    node.Visit(new AnalyzeVisitor(context.Console));
            });
            
            root.AddCommand(roll);
            root.AddCommand(analyze);

            root.Invoke(args);
        }

        private static bool TryParseExpression(IEnumerable<string> expression, IConsole exceptionOutput, out INode output)
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

        private sealed class RollVisitor : INodeVisitor
        {
            private readonly IConsole _console;
            
            public RollVisitor(IConsole console)
            {
                _console = console;
            }

            public void ForNumeric(INumeric numeric) =>
                _console.WriteLine(numeric.Evaluate().ToString());

            public void ForOperation(IOperation operation) =>
                _console.WriteLine(
                    operation.Evaluate().Exists(out Outcome outcome) ?
                        outcome.ToString() :
                        "Failed to pass."
                    );

            public void ForAssertion(IAssertion assertion) =>
                _console.WriteLine(assertion.Evaluate().ToString());
        } 
        private sealed class AnalyzeVisitor : INodeVisitor
        {
            private readonly IConsole _console;
            
            public AnalyzeVisitor(IConsole console)
            {
                _console = console;
            }
            
            public void ForNumeric(INumeric numeric) =>
                Print(
                    numeric.GetProbabilityDistribution(),
                    roll => $"Probability of {roll.Outcome.Value} is {roll.Probability}"
                    );

            public void ForOperation(IOperation operation)
            {
                OptionalRollProbabilityDistribution distribution = operation.GetProbabilityDistribution();

                int rolls = 0;
                foreach (OptionalRoll roll in distribution)
                {
                    _console.Write("Probability of ");

                    if (roll.Outcome.Exists(out Outcome outcome))
                    {
                        _console.Write("rolling ");
                        _console.Write(outcome.Value.ToString());
                        rolls++;
                    }
                    else
                        _console.Write("failing");
                    
                    _console.Write(" is ");

                    _console.WriteLine(roll.Probability.ToString());
                }
                
                if (rolls <= 1)
                    return;
                
                _console.Write("Cumulative probability of succeeding is ");
                _console.WriteLine(distribution.False.Inversed().ToString());
            }

            public void ForAssertion(IAssertion assertion) =>
                Print(
                    assertion.GetProbabilityDistribution(),
                    logical => $"Probability of {logical.Outcome} is {logical.Probability}"
                    );

            private void Print<T>(ProbabilityDistribution<T> distribution, Func<T, string> selector)
            {
                foreach (T value in distribution)
                    _console.WriteLine(selector.Invoke(value));
            }
        } 
    }
}
