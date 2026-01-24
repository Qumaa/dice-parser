using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
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
                for (int i = 0; i < times; i++)
                {
                    nodeTree.Next();
                    TreePlotter.Plot(context.Console, nodeTree);
                }

                return;
            }

            INode node = nodeTree.Root.Node;
            
            Visitor visitor = new(strings.FailedToPass);
            
            for (int i = 0; i < times; i++)
            {
                node.NextEvaluation();
                context.Console.WriteLine(visitor.VisitForString(node));
            }
        }
        
        private sealed class Visitor : INodeVisitor
        {
            private readonly string _failedToPass;
            private string _visitResult;

            public Visitor(string failedToPass)
            {
                _failedToPass = failedToPass;
            }

            public void ForNumeric(INumeric numeric) =>
                _visitResult = numeric.CachedEvaluation.ToString();

            public void ForOperation(IOperation operation) =>
                _visitResult = operation.CachedEvaluation.Exists(out Outcome outcome) ?
                    outcome.ToString() :
                    _failedToPass;

            public void ForSequence<T>(ISequence<T> sequence) where T : INode =>
                _visitResult = $"[{string.Join(", ", sequence.Select(x => VisitForString(x)))}]";

            public void ForAssertion(IAssertion assertion) =>
                _visitResult = assertion.CachedEvaluation.ToString();

            public string VisitForString(INode node)
            {
                node.Visit(this);
                return _visitResult;
            }
        }
    }
}
