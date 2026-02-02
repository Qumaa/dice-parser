using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Runtime.InteropServices;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    internal sealed class AnalyzeCommand : Command
    {
        private readonly AnalyzeCommandStrings _strings;
        private readonly DiceExpressionArgument _argument;
        private readonly AnalyzeBarsBuilder _barsBuilder;
        private readonly StyleOption _styleOption;
        private readonly MaxWidthOption _maxWidthOption;

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

            _styleOption = new StyleOption(strings);
            AddOption(_styleOption);

            _maxWidthOption = new MaxWidthOption(strings, 64);
            AddOption(_maxWidthOption);

            this.SetHandler(CommandHandler);
        }

        private void CommandHandler(InvocationContext context)
        {
            IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(_argument);

            if (!ExpressionParsingHelper.Try(tokens, context.Console, out NodeTree tree))
                return;
            
            AnalyzeOutputStyle style = context.ParseResult.GetValueForOption(_styleOption);
            int maxWidth = context.ParseResult.GetValueForOption(_maxWidthOption);

            context.Console.WriteLine();
            context.Console.WriteLine(tree.SubstringMapper.Source);
            context.Console.WriteLine();
            tree.Root.Node.Visit(new PlotVisitor(context.Console, _strings, style, _barsBuilder, maxWidth));
            context.Console.WriteLine();
        }
        
        private sealed class PlotVisitor : INodeVisitor
        {
            private readonly IConsole _console;
            private readonly AnalyzeCommandStrings _strings;
            private readonly AnalyzeOutputStyle _style;
            private readonly int _maxWidth;
            private readonly AnalyzeBarsBuilder _barsBuilder;

            public PlotVisitor(IConsole console, AnalyzeCommandStrings strings, AnalyzeOutputStyle style, AnalyzeBarsBuilder barsBuilder, int maxWidth)
            {
                _console = console;
                _strings = strings;
                _style = style;
                _barsBuilder = barsBuilder;
                _maxWidth = maxWidth;
            }

            public void ForNumeric(INumeric numeric)
            {
                Roll[] rolls = numeric.GetProbabilityDistribution().ToArray();
                int width = GetPlotWidth();
                
                WriteStats(rolls, width);
                string[] rows = FormatRows(rolls, width);

                foreach (string row in rows)
                    _console.WriteLine(row);
            }

            public void ForOperation(IOperation operation)
            {
                OptionalRollProbabilityDistribution distribution = operation.GetProbabilityDistribution();

                bool omitsRolls = _style.OmitsRolls();
                bool omitsFailure = _style.OmitsFailure();
                bool omitsSuccess = _style.OmitsSuccess();
                bool omitsCumulativeFailure = _style.OmitsCumulativeFailure();
                bool omitsCumulativeSuccess = _style.OmitsCumulativeSuccess();

                IAnalyzeCommandOutputFormatter formatter = _strings.Formatter;

                if (!omitsCumulativeFailure)
                {
                    string failing = omitsSuccess && omitsRolls ?
                        formatter.AssertingFalse(distribution.False) :
                        formatter.CumulativeFailing(distribution.False);
                    
                    _console.WriteLine(failing);
                }
                
                int successfulRolls = 0;
                if (!omitsRolls)
                {
                    Roll[] rolls = distribution.Where(x => x.Outcome.Exists)
                        .Select(x =>  new Roll(x.Outcome.Value, x.Probability))
                        .ToArray();
                    int width = GetPlotWidth();
                    
                    WriteStats(rolls, width);
                    string[] rows = FormatRows(rolls, width);

                    successfulRolls = rows.Length;

                    foreach (string row in rows)
                        _console.WriteLine(row);
                }

                if (!omitsCumulativeSuccess && successfulRolls > 1)
                {
                    Probability ofSucceeding = distribution.False.Inversed();

                    string succeeding = omitsFailure && omitsRolls ?
                        formatter.AssertingTrue(ofSucceeding) :
                        formatter.CumulativeSucceeding(ofSucceeding);

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
                IAnalyzeCommandOutputFormatter formatter = _strings.Formatter;
                
                if (!_style.OmitsFailure())
                    _console.WriteLine(formatter.AssertingFalse(distribution.False));
                
                if (!_style.OmitsSuccess())
                    _console.WriteLine(formatter.AssertingTrue(distribution.True));
            }

            private void WriteStats(Roll[] rolls, int width)
            {
                double average = rolls.Aggregate(0d, (avg, roll) => avg + roll.Outcome.Value * roll.Probability.Value);
                double deviation = Math.Sqrt(rolls.Aggregate(
                    0d,
                    (deviation, roll) =>
                    {
                        double diff = roll.Outcome.Value - average;
                        diff *= diff;
                        return deviation + (roll.Probability.Value * diff);
                    }
                    ));

                string value = $"| {_strings.Average}: {average:0.##} | {_strings.StandardDeviation}: {deviation:0.##} |";

                int left = (width - value.Length) / 2 + value.Length;

                value = value.PadLeft(left, '-');
                value = value.PadRight(width, '-');
                
                _console.WriteLine(value);
            }

            private string[] FormatRows(Roll[] rolls, int width)
            {
                PaddedColumn outcomes = CreatePaddedColumn(
                    _strings.RollResultColumnHeader,
                    rolls.Select(x => x.Outcome.ToString()),
                    padRight: false
                    );

                PaddedColumn probabilities = CreatePaddedColumn(
                    _strings.ProbabilityColumnHeader,
                    rolls.Select(x => x.Probability.ToString())
                    );

                int usedChars = 3; // due to formatting below
                usedChars += outcomes.Width;
                usedChars += probabilities.Width;

                string headers = $"{outcomes.Header}| {probabilities.Header}";
                IEnumerable<string> strings = outcomes.Items.Zip(probabilities.Items, (o, p) => (o, p)).Select(x => $"{x.o}: {x.p}");

                if (IsUniform(rolls))
                    return strings.Prepend(headers).ToArray();

                int barsWidth = width - usedChars;
                
                string barsHeader = CreateBarsHeader(barsWidth, 2, 3);
                headers = $"{headers} {barsHeader}";
                
                string[] bars = _barsBuilder.CreatePaddedBarStrings(rolls.Select(x => x.Probability), barsWidth);

                return strings.Zip(bars, (x, b) => (x, b))
                    .Select(x => $"{x.x} {x.b}")
                    .Prepend(headers)
                    .ToArray();
            }

            private int GetPlotWidth() =>
                _maxWidth <= 0 ? Console.WindowWidth : Math.Min(Console.WindowWidth, _maxWidth);

            private static string CreateBarsHeader(int length, int segments, int segmentSeparators)
            {
                char[] result = new char[length];

                int majorSeparatorsCount = segments + 1;
                int minorSeparatorsCount = segments * segmentSeparators;

                int totalSeparators = majorSeparatorsCount + minorSeparatorsCount;
                int freeSpace = length - totalSeparators;

                int baseBlockSize = freeSpace / segments;
                int remainder = freeSpace % segments;

                int index = 0;

                for (int i = 0; i < segments; i++)
                {
                    result[index++] = '|';

                    int currentBlockSize = baseBlockSize + (i < remainder ? 1 : 0);

                    int smallBlockSize = currentBlockSize / (segmentSeparators + 1);
                    int smallRemainder = currentBlockSize % (segmentSeparators + 1);

                    for (int j = 0; j < segmentSeparators + 1; j++)
                    {
                        int size = smallBlockSize + (j < smallRemainder ? 1 : 0);

                        for (int k = 0; k < size; k++)
                            result[index++] = '-';

                        if (j < segmentSeparators)
                            result[index++] = '+';
                    }
                }

                result[index] = '|';

                return new string(result);
            }

            private static bool IsUniform(Roll[] rolls)
            {
                Probability p = rolls[0].Probability;

                for (int i = 1; i < rolls.Length; i++)
                    if (rolls[i].Probability != p)
                        return false;

                return true;
            }

            private static int PadToMaxLength(string[] strings, bool padRight = true, int minLength = 0)
            {
                int maxLength = int.Max(strings[0].Length, minLength);

                for (int i = 1; i < strings.Length; i++)
                    if (strings[i].Length > maxLength)
                        maxLength = strings[i].Length;
            
                for (int i = 0; i < strings.Length; i++)
                    if (strings[i].Length < maxLength)
                        strings[i] = PadWithSpaces(strings[i], maxLength, padRight);

                return maxLength;
            }

            private static string PadWithSpaces(string s, int length, bool padRight = true) =>
                padRight ? s.PadRight(length) : s.PadLeft(length);

            private static PaddedColumn CreatePaddedColumn(string header, IEnumerable<string> items, bool padRight = true)
            {
                string[] array = items.ToArray();
                int length = PadToMaxLength(array, padRight, header.Length);

                return new PaddedColumn(array, PadWithSpaces(header, length, padRight));
            }

            [StructLayout(LayoutKind.Auto)]
            private readonly struct PaddedColumn
            {
                public readonly string[] Items;
                public readonly string Header;

                public int Width => Items is null ? 0 : Items[0].Length;
                
                public PaddedColumn(string[] items, string header)
                {
                    Items = items;
                    Header = header;
                }
            }
        }
    }
}
