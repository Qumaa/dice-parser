using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    internal sealed class AnalyzeCommand : Command
    {
        private readonly AnalyzeCommandStrings _strings;
        private readonly DiceExpressionArgument _argument;
        private readonly AnalyzeBarsBuilder _barsBuilder;
        private readonly StyleOption _style;

        public AnalyzeCommand(AnalyzeCommandStrings strings, DiceExpressionArgument argument,
            AnalyzeBarsBuilder barsBuilder) : base(
            "analyze",
            strings.Description
            )
        {
            _strings = strings;
            _argument = argument;
            _barsBuilder = barsBuilder;
            AddAlias("a");
            AddArgument(argument);

            _style = new StyleOption(strings);

            AddOption(_style);

            this.SetHandler(CommandHandler);
        }

        private void CommandHandler(InvocationContext context)
        {
            IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(_argument);
            AnalyzeOutputStyle style = context.ParseResult.GetValueForOption(_style);
                    
            if (ExpressionParsingHelper.Try(tokens, context.Console, out NodeTree tree))
                tree.Root.Node.Visit(new Visitor(context.Console, _strings.AnalyzeCommandOutput, style, _barsBuilder));
        }
        
        private sealed class Visitor : INodeVisitor
        {
            private readonly IConsole _console;
            private readonly IAnalyzeCommandOutputFormatter _formatter;
            private readonly AnalyzeOutputStyle _style;
            private readonly AnalyzeBarsBuilder _barsBuilder;

            public Visitor(IConsole console, IAnalyzeCommandOutputFormatter formatter, AnalyzeOutputStyle style, AnalyzeBarsBuilder barsBuilder)
            {
                _console = console;
                _formatter = formatter;
                _style = style;
                _barsBuilder = barsBuilder;
            }

            public void ForNumeric(INumeric numeric)
            {
                string[] rolledStrings = GetRolledStrings(numeric.GetProbabilityDistribution());

                foreach (string rolledString in rolledStrings)
                    _console.WriteLine(rolledString);
            }

            public void ForOperation(IOperation operation)
            {
                OptionalRollProbabilityDistribution distribution = operation.GetProbabilityDistribution();

                bool omitsRolls = _style.OmitsRolls();
                bool omitsFailure = _style.OmitsFailure();
                bool omitsSuccess = _style.OmitsSuccess();
                bool omitsCumulativeFailure = _style.OmitsCumulativeFailure();
                bool omitsCumulativeSuccess = _style.OmitsCumulativeSuccess();

                if (!omitsCumulativeFailure)
                {
                    string failing = omitsSuccess && omitsRolls ?
                        _formatter.AssertingFalse(distribution.False) :
                        _formatter.CumulativeFailing(distribution.False);
                    
                    _console.WriteLine(failing);
                }
                
                int successfulRolls = 0;
                if (!omitsRolls)
                {
                    string[] rolledStrings = GetRolledStrings(
                        distribution.Where(x => x.Outcome.Exists)
                            .Select(x =>  new Roll(x.Outcome.Value, x.Probability))
                        );

                    successfulRolls = rolledStrings.Length;

                    foreach (string rolledString in rolledStrings)
                        _console.WriteLine(rolledString);
                }

                if (!omitsCumulativeSuccess && successfulRolls > 1)
                {
                    Probability ofSucceeding = distribution.False.Inversed();

                    string succeeding = omitsFailure && omitsRolls ?
                        _formatter.AssertingTrue(ofSucceeding) :
                        _formatter.CumulativeSucceeding(ofSucceeding);

                    _console.WriteLine(succeeding);
                }
            }

            // todo better support
            public void ForSequence<T>(ISequence<T> sequence) where T : INode
            {
                foreach (T node in sequence)
                {
                    node.Visit(this);
                    _console.WriteLine();
                }
            }

            public void ForAssertion(IAssertion assertion)
            {
                LogicalProbabilityDistribution distribution = assertion.GetProbabilityDistribution();
                
                if (!_style.OmitsFailure())
                    _console.WriteLine(_formatter.AssertingFalse(distribution.False));
                
                if (!_style.OmitsSuccess())
                    _console.WriteLine(_formatter.AssertingTrue(distribution.True));
            }

            private string[] GetRolledStrings(IEnumerable<Roll> rolls)
            {
                rolls = rolls as Roll[] ?? rolls.ToArray();

                string[] outcomes = rolls.Select(x => x.Outcome.ToString()).ToArray();
                string[] probabilities = rolls.Select(x => x.Probability.ToString()).ToArray();
                string[] bars = _barsBuilder.CreatePaddedBarStrings(rolls.Select(x => x.Probability), 32); // todo change with variable length
                
                PadToMaxLength(outcomes, false);
                PadToMaxLength(probabilities);

                return outcomes.Zip(probabilities, (o, p) => (o, p))
                    .Zip(bars, (x, b) => (x.o, x.p, b))
                    .Select(x => $"{x.o}: {x.p} {x.b}")
                    .ToArray();
            }

            private static void PadToMaxLength(string[] strings, bool right = true)
            {
                int maxLength = strings[0].Length;

                for (int i = 1; i < strings.Length; i++)
                    if (strings[i].Length > maxLength)
                        maxLength = strings[i].Length;
            
                for (int i = 0; i < strings.Length; i++)
                    if (strings[i].Length < maxLength)
                        strings[i] = right ? strings[i].PadRight(maxLength) : strings[i].PadLeft(maxLength);
            }
        }
    }
}
