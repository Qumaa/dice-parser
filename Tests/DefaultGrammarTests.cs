namespace Tests
{
    [TestClass]
    public partial class DefaultGrammarTests
    {
        [TestMethod]
        public void IntConstantTest()
        {
            AssertParsingResultOf<SingleNumber>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SingleNumber>.FailsToMeetExpectedPatternOf<SingleBoolean>();
            AssertParsingResultOf<SingleNumber>.FailsToMeetExpectedPatternOf<SingleDie>();
            AssertParsingResultOf<SingleNumber>.FailsToMeetExpectedPatternOf<ImplicitCompositionDice>();
            AssertParsingResultOf<SingleNumber>.FailsToMeetExpectedPatternOf<ExplicitCompositionDice>();
        }
        
        [TestMethod]
        public void BoolConstantTest()
        {
            AssertParsingResultOf<SingleBoolean>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SingleBoolean>.FailsToMeetExpectedPatternOf<SingleNumber>();
            AssertParsingResultOf<SingleBoolean>.FailsToMeetExpectedPatternOf<SingleDie>();
            AssertParsingResultOf<SingleBoolean>.FailsToMeetExpectedPatternOf<ImplicitCompositionDice>();
            AssertParsingResultOf<SingleBoolean>.FailsToMeetExpectedPatternOf<ExplicitCompositionDice>();
        }

        [TestMethod]
        public void DiceTest()
        {
            AssertParsingResultOf<SingleDie>.MeetsExpectedPattern();
            AssertParsingResultOf<ExplicitCompositionDice>.MeetsExpectedPattern();
            AssertParsingResultOf<ImplicitCompositionDice>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SingleDie>.FailsToMeetExpectedPatternOf<SingleBoolean>();
            AssertParsingResultOf<ExplicitCompositionDice>.FailsToMeetExpectedPatternOf<SingleBoolean>();
            AssertParsingResultOf<ImplicitCompositionDice>.FailsToMeetExpectedPatternOf<SingleBoolean>();
            AssertParsingResultOf<SingleDie>.FailsToMeetExpectedPatternOf<SingleNumber>();
            AssertParsingResultOf<ExplicitCompositionDice>.FailsToMeetExpectedPatternOf<SingleNumber>();
            AssertParsingResultOf<ImplicitCompositionDice>.FailsToMeetExpectedPatternOf<SingleNumber>();
        }


        [TestMethod]
        public void NodePoolTest()
        {
            AssertParsingResultOf<SimpleNodePool>.MeetsExpectedPattern();
            AssertParsingResultOf<NestedNodePool>.MeetsExpectedPattern();
        }
    }

    // misc
    // todo bad syntax assertions e.g. "1 +" throws not enough operands
    // todo bad type assertions e.g. "true" as numeric throws
    public partial class DefaultGrammarTests
    {
        private static readonly EquationParser _parser = CreateDefaultGrammarParser();

        private static EquationParser CreateDefaultGrammarParser()
        {
            EquationParserState state = new();
            LexingPipeline lexingPipeline = TokensTable.Default.ToDefaultPipeline(state.Lexemes);
            LexemesReducingPipeline reducingPipeline = LexemesReducingPipeline.CreateDefault(OperandCastingTable.Default);
            
            return new EquationParser(state, lexingPipeline, reducingPipeline);
        }
        
        private static T Parse<T>(string input) where T : INode
        {
            _parser.Read(input);

            return (T) _parser.Collapse(UnknownLexemeSolver.Inert).Root.Value.Node;
        }

        private static T GetTemplate<T>() where T : Template, new() =>
            new();

        private abstract class Template
        {
            public readonly string SampleString;
            
            protected Template(string sampleString)
            {
                SampleString = sampleString;
            }

            public abstract bool MeetsExpectedPattern(INode node);
        }

        private static class AssertParsingResultOf<TTemplate> where TTemplate : Template, new()
        {
            public static void FailsToMeetExpectedPatternOf<TOther>() where TOther : Template, new()
            {
                TTemplate template = GetTemplate<TTemplate>();
                TOther other = GetTemplate<TOther>();

                INode node = Parse<INode>(other.SampleString);
            
                if (template.MeetsExpectedPattern(node))
                    Assert.Fail();
            }

            public static void MeetsExpectedPattern()
            {
                TTemplate template = GetTemplate<TTemplate>();

                INode node = Parse<INode>(template.SampleString);
            
                if (!template.MeetsExpectedPattern(node))
                    Assert.Fail();
            }
        }
    }

    // templates
    public partial class DefaultGrammarTests
    {
        private sealed class SingleNumber : Template
        {
            public SingleNumber() : base("15") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is NumericConstant { Value: 15 };
        }

        private sealed class SingleBoolean : Template
        {
            public SingleBoolean() : base("true") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is BinaryConstant { Value: true };
        }
        
        private sealed class SingleDie : Template
        {
            public SingleDie() : base("d8") { }
            
            public override bool MeetsExpectedPattern(INode node) =>
                node is Dice { Faces: 8 };
        }

        private sealed class ImplicitCompositionDice : Template
        {
            public ImplicitCompositionDice() : base("2d6") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is Composite
                {
                    UnderlyingNode: Combination
                    {
                        CombinationType: CombinationType.Add, Left: Dice { Faces: 6 }, Right: Dice { Faces: 6 }
                    }
                };
        }

        private sealed class ExplicitCompositionDice : Template
        {
            public ExplicitCompositionDice() : base("2d20highest") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is Composite
                {
                    UnderlyingNode: DefaultSelection
                    {
                        SelectionType: SelectionType.Highest, Left: Dice { Faces: 20 }, Right: Dice { Faces: 20 }
                    }
                };
        }

        private sealed class SimpleNodePool : Template
        {
            public SimpleNodePool() : base("true 15") { }
            public override bool MeetsExpectedPattern(INode node) =>
                node is NodePool pool &&
                pool.ToArray() is [BinaryConstant { Value: true }, NumericConstant { Value: 15 }];
        }

        private sealed class NestedNodePool : Template
        {
            public NestedNodePool() : base("4 ( true 15 )") { }
            public override bool MeetsExpectedPattern(INode node) =>
                node is NodePool pool &&
                pool.ToArray() is  [NumericConstant { Value: 4 }, NodePool pool2] &&
                pool2.ToArray() is [BinaryConstant { Value: true }, NumericConstant { Value: 15 }];
        }
    }
}
