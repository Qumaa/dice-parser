using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    internal sealed class RollCommand : Command
    {
        public RollCommand(RollCommandStrings strings, DiceExpressionArgument argument) : base("roll", strings.Description)
        {
            AddAlias("r");
            AddArgument(argument);

            TreeOption treeOption = new(strings);
            AddOption(treeOption);

            TimesOption timesOption = new(strings);
            AddOption(timesOption);
                
            this.SetHandler(context => CommandHandler(context, argument, treeOption, timesOption, strings));
        }

        private static void CommandHandler(InvocationContext context, DiceExpressionArgument argument,
            TreeOption treeOption, TimesOption timesOption, RollCommandStrings strings)
        {
            IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(argument);
            
            if (!ExpressionParsingHelper.Try(tokens, context.Console, out NodeTree nodeTree))
                return;
            
            bool tree = context.ParseResult.GetValueForOption(treeOption);
            int times = context.ParseResult.GetValueForOption(timesOption);

            if (tree)
            {
                TreePlotter plotter = new(nodeTree, context.Console);
                
                for (int i = 0; i < times; i++)
                    plotter.Plot();
                
                return;
            }

            Visitor visitor = new(context.Console, strings.FailedToPass);
            
            for (int i = 0; i < times; i++)
                nodeTree.Root.Value.Node.Visit(visitor);
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
