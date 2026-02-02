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
        private readonly DiceExpressionArgument _argument;
        private readonly MaxWidthOption _maxWidthOption;
        private readonly NormalizePlotBarsOption _normalizeOption;
        private readonly Plotter _plotter;

        public AnalyzeCommand(AnalyzeCommandStrings strings, DiceExpressionArgument argument,
            AnalyzeBarsBuilder barsBuilder) : base(
            "analyze",
            strings.Description
            )
        {
            _argument = argument;
            AddAlias("a");
            AddArgument(argument);

            _maxWidthOption = new MaxWidthOption(strings, 64);
            AddOption(_maxWidthOption);

            _normalizeOption = new NormalizePlotBarsOption(strings, false);
            AddOption(_normalizeOption);

            _plotter = new Plotter(strings, barsBuilder);

            this.SetHandler(CommandHandler);
        }

        private void CommandHandler(InvocationContext context)
        {
            IEnumerable<string> tokens = context.ParseResult.GetValueForArgument(_argument);

            if (!ExpressionParsingHelper.Try(tokens, context.Console, out NodeTree tree))
                return;
            
            int maxWidth = context.ParseResult.GetValueForOption(_maxWidthOption);
            bool normalize = context.ParseResult.GetValueForOption(_normalizeOption);

            context.Console.WriteLine();
            context.Console.WriteLine(tree.SubstringMapper.Source);
            context.Console.WriteLine();
            _plotter.PlotRoot(context.Console, tree, maxWidth, normalize);
            context.Console.WriteLine();
        }
        
        private sealed class Plotter : INodeVisitor
        {
            private readonly AnalyzeCommandStrings _strings;
            private readonly AnalyzeBarsBuilder _barsBuilder;
            
            private IConsole _console;
            private int _maxWidth;
            private NodeTree _tree;
            private bool _normalize;

            private LinkedNode _currentNode;

            public Plotter(AnalyzeCommandStrings strings, AnalyzeBarsBuilder barsBuilder)
            {
                _strings = strings;
                _barsBuilder = barsBuilder;
            }

            void INodeVisitor.ForNumeric(INumeric numeric)
            {
                Roll[] rolls = numeric.GetProbabilityDistribution().ToArray();
                PlotRolls(rolls);
                _console.WriteLine();
            }

            void INodeVisitor.ForOperation(IOperation operation)
            {
                OptionalRollProbabilityDistribution distribution = operation.GetProbabilityDistribution();

                Roll[] rolls = distribution.Where(x => x.Outcome.Exists)
                    .Select(x =>  new Roll(x.Outcome.Value, x.Probability))
                    .ToArray();
                    
                PlotRolls(rolls);

                _console.WriteLine();

                Probability ofTrue = distribution.False.Inversed();
                    
                PlotTrueFalse(ofTrue);

                _console.WriteLine();
            }

            void INodeVisitor.ForSequence<T>(ISequence<T> sequence)
            {
                string separator = new('=', GetPlotWidth());

                bool isRoot = ReferenceEquals(_currentNode, _tree.Root);
                
                _console.WriteLine(separator);

                for (int i = 0; i < sequence.Count; i++)
                {
                    T node = sequence[i];
                    string source = GetNodeString(_currentNode.Parents[i]);

                    _console.WriteLine();
                    _console.WriteLine(source);
                    _console.WriteLine();

                    LinkedNode previous = _currentNode;
                    _currentNode = _currentNode.Parents[i];
                    node.Visit(this);
                    _currentNode = previous;
                    
                    if (i < sequence.Count - 1 || isRoot)
                        _console.WriteLine(separator);
                }
            }

            void INodeVisitor.ForAssertion(IAssertion assertion)
            {
                LogicalProbabilityDistribution distribution = assertion.GetProbabilityDistribution();

                Probability ofTrue = distribution.False.Inversed();
                
                PlotTrueFalse(ofTrue);
            }

            private string GetNodeString(LinkedNode node)
            {
                Range range = _AccumulateRangeRecursive(node);

                return _tree.SubstringMapper.Source[range];
                
                static Range _AccumulateRangeRecursive(LinkedNode linkedNode) =>
                    linkedNode.Parents.Aggregate(
                        linkedNode.MappingRange,
                        (current, node) => current.And(_AccumulateRangeRecursive(node))
                        );
            }

            public void PlotRoot(IConsole contextConsole, NodeTree tree, int maxWidth, bool normalize)
            {
                _console = contextConsole;
                _tree = tree;
                _maxWidth = maxWidth;
                _normalize = normalize;

                PlotNode(tree.Root);
            }

            private void PlotNode(LinkedNode node)
            {
                _currentNode = node;
                node.Node.Visit(this);
                _currentNode = null;
            }

            private void PlotTrueFalse(Probability ofTrue)
            {
                string binaryBlock = CreateTrueFalseBlock(ofTrue, GetPlotWidth());

                _console.WriteLine(binaryBlock);
            }

            private string CreateTrueFalseBlock(Probability ofTrue, int width)
            {
                string f = false.ToString();
                string pf = ofTrue.Inversed().ToString();
                
                string t = true.ToString();
                string pt = ofTrue.ToString();

                int left = Math.Max(t.Length, pt.Length);
                int right = Math.Max(f.Length, pf.Length);

                t = t.PadLeft(left);
                pt = pt.PadLeft(left);

                f = f.PadRight(right);
                pf = pf.PadRight(right);

                int usedChars = 6 + left + right;
                int barWidth = width - usedChars;

                string bar = _barsBuilder.CreatePaddedBarString(ofTrue, Probability.Hundred, barWidth);
                string scale = CreateScale(barWidth, 2, 3);

                return $"{t} [ {bar} ] {f}\n{pt} [ {scale} ] {pf}";
            }

            private void PlotRolls(Roll[] rolls)
            {
                if (rolls is not { Length: > 0 })
                    return;
                
                int width = GetPlotWidth();
                
                PlotStats(rolls, width);
                
                foreach (string row in FormatRows(rolls, width))
                    _console.WriteLine(row);
            }

            private int GetPlotWidth() =>
                _maxWidth <= 0 ? Console.WindowWidth : Math.Min(Console.WindowWidth, _maxWidth);

            private void PlotStats(Roll[] rolls, int width)
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

            private IEnumerable<string> FormatRows(Roll[] rolls, int width)
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

                int usedChars = 3;
                usedChars += outcomes.Width;
                usedChars += probabilities.Width;

                string headers = $"{outcomes.Header}  {probabilities.Header}";
                IEnumerable<string> rows = outcomes.Items.Zip(probabilities.Items, (o, p) => (o, p)).Select(x => $"{x.o}: {x.p}");

                int barsWidth = width - usedChars;
                
                string barsHeader = CreateScale(barsWidth, 2, 3);
                headers = $"{headers} {barsHeader}";

                IEnumerable<Probability> enumerable = rolls.Select(x => x.Probability);
                string[] bars = !_normalize || IsUniform(rolls) ? 
                    _barsBuilder.CreatePaddedBarStrings(enumerable, Probability.Hundred, barsWidth) :
                    _barsBuilder.CreateNormalizedPaddedBarStrings(enumerable, barsWidth);

                return rows.Zip(bars, (x, b) => (x, b))
                    .Select(x => $"{x.x} {x.b}")
                    .Prepend(headers);
            }

            private static string CreateScale(int length, int segments, int segmentSeparators)
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
