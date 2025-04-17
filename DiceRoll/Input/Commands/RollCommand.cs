using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace DiceRoll
{
    internal sealed class RollCommand : Command
    {
        public RollCommand(DiceCommandStrings strings, DiceExpressionArgument argument) : base("roll", strings.RollDescription)
        {
            AddAlias("r");
            AddArgument(argument);
                
            this.SetHandler(context => CommandHandler(context, argument, strings.RollOutputFailedToPass));
        }

        private static void CommandHandler(InvocationContext context, DiceExpressionArgument argument, string failedToPass)
        {
            IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(argument);
                    
            if (ExpressionParsingHelper.Try(tokens, context.Console, out INode node))
                node.Visit(new Visitor(context.Console, failedToPass));
        }
        
        private sealed class Visitor : INodeVisitor
        {
            private readonly IConsole _console;
            private readonly string _failedToPass;

            public Visitor(IConsole console, string failedToPass)
            {
                _console = console;
                _failedToPass = failedToPass;
            }

            public void ForNumeric(INumeric numeric) =>
                _console.WriteLine(numeric.Evaluate().ToString());

            public void ForOperation(IOperation operation) =>
                _console.WriteLine(
                    operation.Evaluate().Exists(out Outcome outcome) ?
                        outcome.ToString() :
                        _failedToPass
                    );

            public void ForAssertion(IAssertion assertion) =>
                _console.WriteLine(assertion.Evaluate().ToString());
        }
    }
}
