namespace Tests.Grammar.Default
{
    // todo bad syntax assertions e.g. "1 +" throws not enough operands
    // todo bad type assertions e.g. "true" as numeric throws
    public partial class DefaultGrammarTests
    {
        private static readonly IEquationParser _parser = CreateDefaultGrammarParser();

        private static IEquationParser CreateDefaultGrammarParser()
        {
            EquationParserState state = new();
            LexingPipeline lexingPipeline = TokensTable.Default.ToDefaultPipeline(state.Lexemes);
            LexemesReducingPipeline reducingPipeline = LexemesReducingPipeline.CreateDefault(OperandCastingTable.Default);
            
            return new EquationParser(state, lexingPipeline, reducingPipeline);
        }

        private static NodeTree Parse(string input)
        {
            _parser.AccumulateInput(input);

            return _parser.ParseAccumulatedInput(UnknownLexemeSolver.Inert);
        }
        
        private static T Parse<T>(string input) where T : INode =>
            (T) Parse(input).Root.Value.Node;

        private static T GetTemplate<T>() where T : VerboseTemplate, new() =>
            new();

        private abstract class VerboseTemplate
        {
            public readonly string[] SampleStrings;

            protected VerboseTemplate(params string[] sampleStrings)
            {
                SampleStrings = sampleStrings;
            }

            protected VerboseTemplate(string sampleString) : this([sampleString]) { }

            protected VerboseTemplate(string formatString, params string[] options) : this(
                options.Select(x => string.Format(formatString, x)).ToArray()
                ) { }
            
            public bool MeetsExpectedPattern(NodeTree[] trees)
            {
                if (trees.Length != SampleStrings.Length)
                    return false;

                int length = trees.Length;

                for (int i = 0; i < length; i++)
                    if (!MeetsExpectedPattern(trees[i], i))
                        return false;

                return true;
            }

            public abstract bool MeetsExpectedPattern(NodeTree tree, int sampleIndex);
        }

        private abstract class Template : VerboseTemplate
        {
            protected Template(params string[] sampleStrings) : base(sampleStrings) { }
            protected Template(string sampleString) : base(sampleString) { }
            protected Template(string formatString, params string[] options) : base(formatString, options) { }

            public sealed override bool MeetsExpectedPattern(NodeTree tree, int sampleIndex) =>
                MeetsExpectedPattern(tree.Root.Value.Node);

            protected abstract bool MeetsExpectedPattern(INode node);
        }

        private sealed class OperatorPrecedenceTemplate<TL, TR> : VerboseTemplate
            where TL : OperatorContract, new() where TR : OperatorContract, new()
        {
            private readonly TL _left;
            private readonly TR _right;
            public readonly bool FavorLeft;

            public OperatorPrecedenceTemplate(bool favorLeft) : this(new TL(), new TR(), favorLeft) { }

            private OperatorPrecedenceTemplate(TL left, TR right, bool favorLeft) : base(
                GenerateSamples(left, right, favorLeft)
                )
            {
                _left = left;
                _right = right;
                FavorLeft = favorLeft;
            }

            public override bool MeetsExpectedPattern(NodeTree tree, int sampleIndex)
            {
                (NodeTree leftOp, NodeTree rightOp) = ExtractNodes(tree);

                if (!VerifyOperators(leftOp, rightOp, sampleIndex))
                    return false;

                return _left.ArgumentsSampler.VerifyLeft(leftOp.Root) && _right.ArgumentsSampler.VerifyRight(rightOp.Root);
            }

            private bool VerifyOperators(NodeTree leftOp, NodeTree rightOp, int sampleIndex)
            {
                string leftSymbol = leftOp.RootSubstring().ToString();
                string rightSymbol = rightOp.RootSubstring().ToString();

                int leftSamplesCount = _left.SampleStrings.Length;
                
                int leftSampleIndex = sampleIndex / leftSamplesCount;
                int rightSampleIndex = sampleIndex % leftSamplesCount;

                if (_left.SampleStrings[leftSampleIndex] != leftSymbol)
                    return false;

                if (_right.SampleStrings[rightSampleIndex] != rightSymbol)
                    return false;

                if (!_left.MeetsExpectedPattern(leftOp, leftSampleIndex))
                    return false;

                if (!_right.MeetsExpectedPattern(rightOp, rightSampleIndex))
                    return false;

                return true;
            }

            private (NodeTree leftOp, NodeTree rightOp) ExtractNodes(NodeTree tree) =>
                FavorLeft ?
                    (tree.Navigate().Descend(0).ExtractSubtree(), tree) :
                    (tree, tree.Navigate().Descend(^1).ExtractSubtree());

            private static string[] GenerateSamples(OperatorContract left, OperatorContract right, bool favorLeft)
            {
                List<string> parts = [];
                string leftArg = left.ArgumentsSampler.GenerateLeft();
                string rightArg = right.ArgumentsSampler.GenerateRight();

                for (int i = 0; i < left.Arity.Left; i++)
                    parts.Add(leftArg);

                parts.Add("{0}");

                int inBetweenArgs = (left.Arity.Right + right.Arity.Left) - 1;
                int breakPoint = left.Arity.Right;

                if (!favorLeft)
                    breakPoint--;
                
                for (int i = 0; i < inBetweenArgs; i++)
                    parts.Add(i >= breakPoint ? rightArg : leftArg);

                parts.Add("{1}");

                for (int i = 0; i < right.Arity.Right; i++)
                    parts.Add(rightArg);

                string format = string.Join(" ", parts);

                return left.SampleStrings.SelectMany(_ => right.SampleStrings, (l, r) => string.Format(format, l, r)).ToArray();
            }
        }

        private abstract class OperatorContract : Template
        {
            public readonly ArgumentsSampler ArgumentsSampler;
            public readonly Arity Arity;
            
            protected OperatorContract(ArgumentsSampler sampler, Arity arity, params string[] operatorSymbols) : base(operatorSymbols)
            {
                ArgumentsSampler = sampler;
                Arity = arity;
            }
        }

        private abstract class ArgumentsSampler
        {
            public static readonly ArgumentsSampler Numeric = new NumericImpl();
            public static readonly ArgumentsSampler Boolean = new BooleanImpl();
            
            public abstract string Generate(int operatorIndex);

            public string GenerateLeft() =>
                Generate(0);

            public string GenerateRight() =>
                Generate(1);

            public bool Verify(Mapped<LinkedNode> operatorNode, int operatorIndex) =>
                Verify(
                    operatorNode.Value.Parents.Where(x => x.Value.IsOperand).Select(x => x.Value.Node),
                    Generate(operatorIndex)
                    );

            public bool VerifyLeft(Mapped<LinkedNode> operatorNode) =>
                Verify(operatorNode, 0);

            public bool VerifyRight(Mapped<LinkedNode> operatorNode) =>
                Verify(operatorNode, 1);
            
            protected abstract bool Verify(IEnumerable<INode> arguments, string value);
            
            private sealed class NumericImpl : ArgumentsSampler
            {
                public override string Generate(int operatorIndex) =>
                    (operatorIndex + 1).ToString();

                protected override bool Verify(IEnumerable<INode> arguments, string value) =>
                    arguments.All(x => x is NumericConstant constant && constant.Value.ToString().Equals(value));
            }

            private sealed class BooleanImpl : ArgumentsSampler
            {
                public override string Generate(int operatorIndex) =>
                    (operatorIndex % 2 is 0).ToString();

                protected override bool Verify(IEnumerable<INode> arguments, string value) =>
                    arguments.All(x => x is BinaryConstant constant && constant.Value.ToString().Equals(value));
            }
        }

        private static class AssertThat<TL> where TL : OperatorContract, new()
        {
            public static class PrecedenceOver<TR> where TR : OperatorContract, new()
            {
                public static void IsHigher() =>
                    Run(true);

                public static void IsLower() =>
                    Run(false);

                private static void Run(bool favorLeft)
                {
                    if (TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.IsEmpty ||
                        TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.Instance!.FavorLeft != favorLeft)
                        TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.CacheInstance(new OperatorPrecedenceTemplate<TL, TR>(favorLeft));
                    
                    TemplateCache<OperatorPrecedenceTemplate<TL, TR>>.AssertMeetsExpectedPattern();
                }
            }
        }

        private static class AssertParsingResultOf<TTemplate> where TTemplate : VerboseTemplate, new()
        {
            static AssertParsingResultOf()
            {
                Cache<TTemplate>();
            }

            public static void FailsToMeetExpectedPatternOf<TOther>() where TOther : VerboseTemplate, new()
            {
                if (TemplateCache<TOther>.IsEmpty)
                    Cache<TOther>();
                
                TOther otherTemplate = TemplateCache<TOther>.Instance!;
                
                TemplateCache<TTemplate>.AssertFailsToMeetExpectedPatternOf(otherTemplate);
            }

            public static void MeetsExpectedPattern() =>
                TemplateCache<TTemplate>.AssertMeetsExpectedPattern();

            private static void Cache<T>() where T : VerboseTemplate, new() =>
                TemplateCache<T>.CacheInstance(GetTemplate<T>());
        }

        private static class TemplateCache<T> where T : VerboseTemplate
        {
            public static T? Instance { get; private set; }
            public static NodeTree[]? SampleParseResult { get; private set; }
            public static bool IsEmpty => Instance is null;

            public static void CacheInstance(T instance)
            {
                Instance = instance;
                SampleParseResult = Instance.SampleStrings.Select(Parse).ToArray();
            }
            
            public static void AssertFailsToMeetExpectedPatternOf<TOther>(TOther other) where TOther : VerboseTemplate
            {
                if (IsEmpty)
                    return;
                
                if (other.MeetsExpectedPattern(SampleParseResult!))
                    Assert.Fail($"Parsing a sample of {typeof(T).Name} produced a result that also meets the expected pattern of {typeof(TOther).Name}, which it must not.");
            }

            public static void AssertMeetsExpectedPattern()
            {
                if (IsEmpty)
                    return;
                
                if (!Instance!.MeetsExpectedPattern(SampleParseResult!))
                    Assert.Fail($"Parsing a sample of {typeof(T).Name} produced a result that doesn't meet the expected pattern.");
            }
        }
    }
}
