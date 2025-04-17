using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace DiceRoll
{
    internal sealed class AnalyzeCommand : Command
    {
        public AnalyzeCommand(DiceCommandStrings strings, DiceExpressionArgument argument) : base(
            "analyze",
            strings.AnalyzeDescription
            )
        {
            AddAlias("a");
            AddArgument(argument);

            this.SetHandler(context => CommandHandler(context, argument, strings.AnalyzeCommandOutput));
        }

        private static void CommandHandler(InvocationContext context, DiceExpressionArgument argument,
            IAnalyzeCommandOutputFormatter formatter)
        {
            IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(argument);
                    
            if (ExpressionParsingHelper.Try(tokens, context.Console, out INode node))
                node.Visit(new Visitor(context.Console, formatter));
        }
        
        private sealed class Visitor : INodeVisitor
        {
            private readonly IConsole _console;
            private readonly IAnalyzeCommandOutputFormatter _formatter;

            public Visitor(IConsole console, IAnalyzeCommandOutputFormatter formatter)
            {
                _console = console;
                _formatter = formatter;
            }

            public void ForNumeric(INumeric numeric)
            {
                foreach (Roll roll in numeric.GetProbabilityDistribution())
                    _console.WriteLine(_formatter.Rolling(roll.Outcome, roll.Probability));
            }

            public void ForOperation(IOperation operation)
            {
                OptionalRollProbabilityDistribution distribution = operation.GetProbabilityDistribution();

                int successfulRolls = 0;
                foreach (OptionalRoll roll in distribution)
                    if (roll.Outcome.Exists(out Outcome outcome))
                    {
                        _console.WriteLine(_formatter.Rolling(outcome, roll.Probability));
                        successfulRolls++;
                    }
                    else
                        _console.WriteLine(_formatter.Failing(roll.Probability));
                
                if (successfulRolls <= 1)
                    return;
                
                _console.WriteLine(_formatter.Succeeding(distribution.False.Inversed()));
            }

            public void ForAssertion(IAssertion assertion)
            {
                foreach (Logical logical in assertion.GetProbabilityDistribution())
                    _console.WriteLine(_formatter.Asserting(in logical));
            }
        }
    }
}
