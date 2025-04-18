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

            Option<AnalyzeOutputStyle> style = new(
                "--style",
                () => AnalyzeOutputStyle.Full,
                strings.AnalyzeStyleOptionDescription
                );
            
            style.AddAlias("-s");
            
            AddOption(style);

            this.SetHandler(context => CommandHandler(context, argument, style, strings.AnalyzeCommandOutput));
        }

        private static void CommandHandler(InvocationContext context, DiceExpressionArgument argument, 
            Option<AnalyzeOutputStyle> styleOption, IAnalyzeCommandOutputFormatter formatter)
        {
            IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(argument);
            AnalyzeOutputStyle style = context.ParseResult.GetValueForOption(styleOption);
                    
            if (ExpressionParsingHelper.Try(tokens, context.Console, out INode node))
                node.Visit(new Visitor(context.Console, formatter, style));
        }
        
        private sealed class Visitor : INodeVisitor
        {
            private readonly IConsole _console;
            private readonly IAnalyzeCommandOutputFormatter _formatter;
            private readonly AnalyzeOutputStyle _style;

            public Visitor(IConsole console, IAnalyzeCommandOutputFormatter formatter, AnalyzeOutputStyle style)
            {
                _console = console;
                _formatter = formatter;
                _style = style;
            }

            public void ForNumeric(INumeric numeric)
            {
                foreach (Roll roll in numeric.GetProbabilityDistribution())
                    _console.WriteLine(_formatter.Rolling(roll.Outcome, roll.Probability));
            }

            public void ForOperation(IOperation operation)
            {
                OptionalRollProbabilityDistribution distribution = operation.GetProbabilityDistribution();

                bool omitsRolls = _style.OmitsRolls();
                bool omitsFailure = _style.OmitsFailure();
                bool omitsSuccess = _style.OmitsSuccess();
                bool omitsCumulativeFailure = _style.OmitsCumulativeFailure();
                bool omitsCumulativeSuccess = _style.OmitsCumulativeSuccess();
                
                int successfulRolls = 0;
                foreach (OptionalRoll roll in distribution)
                {
                    Probability probability = roll.Probability;
                    
                    if (roll.Outcome.Exists(out Outcome outcome))
                    {
                        if (!omitsRolls)
                            _console.WriteLine(_formatter.Rolling(outcome, probability));
                        
                        successfulRolls++;
                        continue;
                    }

                    if (omitsCumulativeFailure)
                        continue;

                    string failing = omitsSuccess && omitsRolls ?
                        _formatter.AssertingFalse(probability) :
                        _formatter.CumulativeFailing(probability);
                    
                    _console.WriteLine(failing);
                }

                if (omitsCumulativeSuccess || successfulRolls <= 1)
                    return;

                Probability ofSucceeding = distribution.False.Inversed();
                
                string succeeding = omitsFailure && omitsRolls ?
                        _formatter.AssertingTrue(ofSucceeding):
                        _formatter.CumulativeSucceeding(ofSucceeding);
                
                _console.WriteLine(succeeding);
            }

            public void ForAssertion(IAssertion assertion)
            {
                LogicalProbabilityDistribution distribution = assertion.GetProbabilityDistribution();
                
                if (!_style.OmitsFailure())
                    _console.WriteLine(_formatter.AssertingFalse(distribution.False));
                
                if (!_style.OmitsSuccess())
                    _console.WriteLine(_formatter.AssertingTrue(distribution.True));
            }
        }
    }
}
