namespace Tests
{
    [TestClass]
    public partial class DefaultGrammarTests
    {
        // todo composition once done
        // todo range
        // todo advanced dice (once changed to be an operator)
        
        [TestClass]
        public class OperandTests
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

        [TestMethod]
        public void SimpleTests()
        {
            AssertParsingResultOf<SimpleAddition>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleSubtraction>.MeetsExpectedPattern();
            AssertParsingResultOf<SimpleNegation>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleMultiplication>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleDivideRoundDown>.MeetsExpectedPattern();
            AssertParsingResultOf<SimpleDivideRoundUp>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleEqualNumeric>.MeetsExpectedPattern();
            AssertParsingResultOf<SimpleNotEqualNumeric>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleEqualBinary>.MeetsExpectedPattern();
            AssertParsingResultOf<SimpleNotEqualBinary>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleGreaterThan>.MeetsExpectedPattern();
            AssertParsingResultOf<SimpleGreaterThanOrEqual>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleLessThan>.MeetsExpectedPattern();
            AssertParsingResultOf<SimpleLessThanOrEqual>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleAnd>.MeetsExpectedPattern();
            AssertParsingResultOf<SimpleOr>.MeetsExpectedPattern();
            
            AssertParsingResultOf<SimpleNot>.MeetsExpectedPattern();
        }

        [TestClass]
        public class PrecedenceTests
        {
            [TestMethod]
            public void ParenthesisTest() =>
                AssertParsingResultOf<PrecedenceOverridenByParenthesis>.MeetsExpectedPattern();
            
            [TestMethod]
            public void AddTests()
            {
                AssertParsingResultOf<PrecedenceAddOverAdd>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceAddOverSubtract>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceAddOverNegation>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceAddOverMultiply>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceAddOverDivideRoundDown>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceAddOverDivideRoundUp>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceAddOverGreaterThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceAddOverGreaterThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceAddOverLessThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceAddOverLessThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceAddOverEqual>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceAddOverNotEqual>.MeetsExpectedPattern();
            }
            
            [TestMethod]
            public void SubtractTests()
            {
                AssertParsingResultOf<PrecedenceSubtractOverAdd>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceSubtractOverSubtract>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceSubtractOverNegation>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceSubtractOverMultiply>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceSubtractOverDivideRoundDown>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceSubtractOverDivideRoundUp>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceSubtractOverGreaterThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceSubtractOverGreaterThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceSubtractOverLessThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceSubtractOverLessThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceSubtractOverEqual>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceSubtractOverNotEqual>.MeetsExpectedPattern();
            }

            [TestMethod]
            public void MultiplyTests()
            {
                AssertParsingResultOf<PrecedenceMultiplyOverAdd>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceMultiplyOverSubtract>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceMultiplyOverNegation>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceMultiplyOverMultiply>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceMultiplyOverDivideRoundDown>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceMultiplyOverDivideRoundUp>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceMultiplyOverGreaterThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceMultiplyOverGreaterThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceMultiplyOverLessThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceMultiplyOverLessThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceMultiplyOverEqual>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceMultiplyOverNotEqual>.MeetsExpectedPattern();
            }

            [TestMethod]
            public void DivideRoundDownTests()
            {
                AssertParsingResultOf<PrecedenceDivideRoundDownOverAdd>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundDownOverSubtract>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundDownOverNegation>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundDownOverMultiply>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundDownOverDivideRoundDown>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundDownOverDivideRoundUp>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundDownOverGreaterThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundDownOverGreaterThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundDownOverLessThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundDownOverLessThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundDownOverEqual>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundDownOverNotEqual>.MeetsExpectedPattern();
            }
            
            [TestMethod]
            public void DivideRoundUpTests()
            {
                AssertParsingResultOf<PrecedenceDivideRoundUpOverAdd>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundUpOverSubtract>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundUpOverNegation>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundUpOverMultiply>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundUpOverDivideRoundDown>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundUpOverDivideRoundUp>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundUpOverGreaterThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundUpOverGreaterThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundUpOverLessThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundUpOverLessThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceDivideRoundUpOverEqual>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceDivideRoundUpOverNotEqual>.MeetsExpectedPattern();
            }

            [TestMethod]
            public void NegateTests()
            {
                AssertParsingResultOf<PrecedenceNegateOverAdd>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceNegateOverSubtract>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceNegateOverMultiply>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceNegateOverDivideRoundDown>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceNegateOverDivideRoundUp>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceNegateOverGreaterThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceNegateOverGreaterThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceNegateOverLessThan>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceNegateOverLessThanOrEqual>.MeetsExpectedPattern();
            
                AssertParsingResultOf<PrecedenceNegateOverEqual>.MeetsExpectedPattern();
                AssertParsingResultOf<PrecedenceNegateOverNotEqual>.MeetsExpectedPattern();
            }
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
            public readonly string[] SampleStrings;
            
            protected Template(string sampleString)
            {
                SampleStrings = [sampleString];
            }

            protected Template(string formatString, params string[] options)
            {
                SampleStrings = options.Select(x => string.Format(formatString, x)).ToArray();
            }

            public bool MeetsExpectedPattern(INode[] nodes) =>
                nodes.All(MeetsExpectedPattern);

            protected abstract bool MeetsExpectedPattern(INode node);
        }

        private static class AssertParsingResultOf<TTemplate> where TTemplate : Template, new()
        {
            private static readonly TTemplate _template;
            private static readonly INode[] _sampleParseResult;

            static AssertParsingResultOf()
            {
                _template = GetTemplate<TTemplate>();
                _sampleParseResult = _template.SampleStrings.Select(Parse<INode>).ToArray();
            }
            
            public static void FailsToMeetExpectedPatternOf<TOther>() where TOther : Template, new()
            {
                TOther otherTemplate = AssertParsingResultOf<TOther>._template;
                
                if (otherTemplate.MeetsExpectedPattern(_sampleParseResult))
                    Assert.Fail($"Parsing a sample of {typeof(TTemplate).Name} produced a result that also meets the expected pattern of {typeof(TOther).Name}, which it must not.");
            }

            public static void MeetsExpectedPattern()
            {
                if (!_template.MeetsExpectedPattern(_sampleParseResult))
                    Assert.Fail($"Parsing a sample of {typeof(TTemplate).Name} produced a result that doesn't meet the expected pattern.");
            }
        }
    }

    // templates
    public partial class DefaultGrammarTests
    {
    #region Operands

        private sealed class SingleNumber : Template
        {
            public SingleNumber() : base("15") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is NumericConstant { Value: 15 };
        }

        private sealed class SingleBoolean : Template
        {
            public SingleBoolean() : base("true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is BinaryConstant { Value: true };
        }

        private sealed class SingleDie : Template
        {
            public SingleDie() : base("d8") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Dice { Faces: 8 };
        }

        private sealed class ImplicitCompositionDice : Template
        {
            public ImplicitCompositionDice() : base("2d6") { }

            protected override bool MeetsExpectedPattern(INode node) =>
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

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Composite
                {
                    UnderlyingNode: DefaultSelection
                    {
                        SelectionType: SelectionType.Highest,
                        Left: Dice { Faces: 20 },
                        Right: Dice { Faces: 20 }
                    }
                };
        }

        private sealed class SimpleNodePool : Template
        {
            public SimpleNodePool() : base("true 15") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is NodePool pool &&
                pool.ToArray() is [BinaryConstant { Value: true }, NumericConstant { Value: 15 }];
        }

        private sealed class NestedNodePool : Template
        {
            public NestedNodePool() : base("4 ( true 15 )") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is NodePool pool &&
                pool.ToArray() is [NumericConstant { Value: 4 }, NodePool pool2] &&
                pool2.ToArray() is [BinaryConstant { Value: true }, NumericConstant { Value: 15 }];
        }

    #endregion

    #region Operators

        private sealed class SimpleAddition : Template
        {
            public SimpleAddition() : base("d4 + 1") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 1 }
                };
        }

        private sealed class SimpleSubtraction : Template
        {
            public SimpleSubtraction() : base("d4 - 1") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 1 }
                };
        }

        private sealed class SimpleNegation : Template
        {
            public SimpleNegation() : base("- 1") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Negation { Source: NumericConstant { Value: 1 } };
        }

        private sealed class SimpleMultiplication : Template
        {
            public SimpleMultiplication() : base("d4 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleDivideRoundDown : Template
        {
            public SimpleDivideRoundDown() : base("d4 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleDivideRoundUp : Template
        {
            public SimpleDivideRoundUp() : base("d4 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleEqualNumeric : Template
        {
            public SimpleEqualNumeric() : base("d4 {0} 3", "=", "==") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal, Left: Dice { Faces: 4 }, Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleNotEqualNumeric : Template
        {
            public SimpleNotEqualNumeric() : base("d4 {0} 3", "!=", "=/=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleEqualBinary : Template
        {
            public SimpleEqualBinary() : base("false {0} true", "=", "==") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleNotEqualBinary : Template
        {
            public SimpleNotEqualBinary() : base("false {0} true", "!=", "=/=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleGreaterThan : Template
        {
            public SimpleGreaterThan() : base("d4 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleGreaterThanOrEqual : Template
        {
            public SimpleGreaterThanOrEqual() : base("d4 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleLessThan : Template
        {
            public SimpleLessThan() : base("d4 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleLessThanOrEqual : Template
        {
            public SimpleLessThanOrEqual() : base("d4 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class SimpleAnd : Template
        {
            public SimpleAnd() : base("false {0} true", "&", "&&", "and") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleOr : Template
        {
            public SimpleOr() : base("false {0} true", "|", "||", "or") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }
        
        private sealed class SimpleNot : Template
        {
            public SimpleNot() : base("{0} false", "!", "not") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is NotAssertion
                {
                    Source: BinaryConstant { Value: false }
                };
        }

    #endregion

    #region Precedence

        private sealed class PrecedenceOverridenByParenthesis : Template
        {
            public PrecedenceOverridenByParenthesis() : base("(1 + 2) * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverAdd : Template
        {
            public PrecedenceAddOverAdd() : base("1 + 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverSubtract : Template
        {
            public PrecedenceAddOverSubtract() : base("1 + 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverNegation : Template
        {
            public PrecedenceAddOverNegation() : base("1 + - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceAddOverMultiply : Template
        {
            public PrecedenceAddOverMultiply() : base("1 + 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceAddOverDivideRoundDown : Template
        {
            public PrecedenceAddOverDivideRoundDown() : base("1 + 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceAddOverDivideRoundUp : Template
        {
            public PrecedenceAddOverDivideRoundUp() : base("1 + 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceAddOverGreaterThan : Template
        {
            public PrecedenceAddOverGreaterThan() : base("1 + 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverLessThan : Template
        {
            public PrecedenceAddOverLessThan() : base("1 + 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverGreaterThanOrEqual : Template
        {
            public PrecedenceAddOverGreaterThanOrEqual() : base("1 + 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverLessThanOrEqual : Template
        {
            public PrecedenceAddOverLessThanOrEqual() : base("1 + 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverEqual : Template
        {
            public PrecedenceAddOverEqual() : base("1 + 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverNotEqual : Template
        {
            public PrecedenceAddOverNotEqual() : base("1 + 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class PrecedenceSubtractOverAdd : Template
        {
            public PrecedenceSubtractOverAdd() : base("1 - 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverSubtract : Template
        {
            public PrecedenceSubtractOverSubtract() : base("1 - 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverNegation : Template
        {
            public PrecedenceSubtractOverNegation() : base("1 - - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverMultiply : Template
        {
            public PrecedenceSubtractOverMultiply() : base("1 - 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverDivideRoundDown : Template
        {
            public PrecedenceSubtractOverDivideRoundDown() : base("1 - 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverDivideRoundUp : Template
        {
            public PrecedenceSubtractOverDivideRoundUp() : base("1 - 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverGreaterThan : Template
        {
            public PrecedenceSubtractOverGreaterThan() : base("1 - 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverLessThan : Template
        {
            public PrecedenceSubtractOverLessThan() : base("1 - 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverGreaterThanOrEqual : Template
        {
            public PrecedenceSubtractOverGreaterThanOrEqual() : base("1 - 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverLessThanOrEqual : Template
        {
            public PrecedenceSubtractOverLessThanOrEqual() : base("1 - 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverEqual : Template
        {
            public PrecedenceSubtractOverEqual() : base("1 - 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverNotEqual : Template
        {
            public PrecedenceSubtractOverNotEqual() : base("1 - 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class PrecedenceMultiplyOverAdd : Template
        {
            public PrecedenceMultiplyOverAdd() : base("1 * 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverSubtract : Template
        {
            public PrecedenceMultiplyOverSubtract() : base("1 * 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverNegation : Template
        {
            public PrecedenceMultiplyOverNegation() : base("1 * - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceMultiplyOverMultiply : Template
        {
            public PrecedenceMultiplyOverMultiply() : base("1 * 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverDivideRoundDown : Template
        {
            public PrecedenceMultiplyOverDivideRoundDown() : base("1 * 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverDivideRoundUp : Template
        {
            public PrecedenceMultiplyOverDivideRoundUp() : base("1 * 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverGreaterThan : Template
        {
            public PrecedenceMultiplyOverGreaterThan() : base("1 * 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverLessThan : Template
        {
            public PrecedenceMultiplyOverLessThan() : base("1 * 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverGreaterThanOrEqual : Template
        {
            public PrecedenceMultiplyOverGreaterThanOrEqual() : base("1 * 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverLessThanOrEqual : Template
        {
            public PrecedenceMultiplyOverLessThanOrEqual() : base("1 * 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverEqual : Template
        {
            public PrecedenceMultiplyOverEqual() : base("1 * 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverNotEqual : Template
        {
            public PrecedenceMultiplyOverNotEqual() : base("1 * 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class PrecedenceDivideRoundDownOverAdd : Template
        {
            public PrecedenceDivideRoundDownOverAdd() : base("1 / 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverSubtract : Template
        {
            public PrecedenceDivideRoundDownOverSubtract() : base("1 / 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverNegation : Template
        {
            public PrecedenceDivideRoundDownOverNegation() : base("1 / - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverMultiply : Template
        {
            public PrecedenceDivideRoundDownOverMultiply() : base("1 / 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverDivideRoundDown : Template
        {
            public PrecedenceDivideRoundDownOverDivideRoundDown() : base("1 / 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverDivideRoundUp : Template
        {
            public PrecedenceDivideRoundDownOverDivideRoundUp() : base("1 / 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverGreaterThan : Template
        {
            public PrecedenceDivideRoundDownOverGreaterThan() : base("1 / 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverLessThan : Template
        {
            public PrecedenceDivideRoundDownOverLessThan() : base("1 / 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverGreaterThanOrEqual : Template
        {
            public PrecedenceDivideRoundDownOverGreaterThanOrEqual() : base("1 / 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverLessThanOrEqual : Template
        {
            public PrecedenceDivideRoundDownOverLessThanOrEqual() : base("1 / 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverEqual : Template
        {
            public PrecedenceDivideRoundDownOverEqual() : base("1 / 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverNotEqual : Template
        {
            public PrecedenceDivideRoundDownOverNotEqual() : base("1 / 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class PrecedenceDivideRoundUpOverAdd : Template
        {
            public PrecedenceDivideRoundUpOverAdd() : base("1 // 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverSubtract : Template
        {
            public PrecedenceDivideRoundUpOverSubtract() : base("1 // 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverNegation : Template
        {
            public PrecedenceDivideRoundUpOverNegation() : base("1 // - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverMultiply : Template
        {
            public PrecedenceDivideRoundUpOverMultiply() : base("1 // 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverDivideRoundDown : Template
        {
            public PrecedenceDivideRoundUpOverDivideRoundDown() : base("1 // 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverDivideRoundUp : Template
        {
            public PrecedenceDivideRoundUpOverDivideRoundUp() : base("1 // 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverGreaterThan : Template
        {
            public PrecedenceDivideRoundUpOverGreaterThan() : base("1 // 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverLessThan : Template
        {
            public PrecedenceDivideRoundUpOverLessThan() : base("1 // 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverGreaterThanOrEqual : Template
        {
            public PrecedenceDivideRoundUpOverGreaterThanOrEqual() : base("1 // 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverLessThanOrEqual : Template
        {
            public PrecedenceDivideRoundUpOverLessThanOrEqual() : base("1 // 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverEqual : Template
        {
            public PrecedenceDivideRoundUpOverEqual() : base("1 // 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverNotEqual : Template
        {
            public PrecedenceDivideRoundUpOverNotEqual() : base("1 // 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class PrecedenceNegateOverAdd : Template
        {
            public PrecedenceNegateOverAdd() : base("- 1 + 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverSubtract : Template
        {
            public PrecedenceNegateOverSubtract() : base("- 1 - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverMultiply : Template
        {
            public PrecedenceNegateOverMultiply() : base("- 1 * 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverDivideRoundDown : Template
        {
            public PrecedenceNegateOverDivideRoundDown() : base("- 1 / 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverDivideRoundUp : Template
        {
            public PrecedenceNegateOverDivideRoundUp() : base("- 1 // 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverGreaterThan : Template
        {
            public PrecedenceNegateOverGreaterThan() : base("- 1 > 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverLessThan : Template
        {
            public PrecedenceNegateOverLessThan() : base("- 1 < 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverGreaterThanOrEqual : Template
        {
            public PrecedenceNegateOverGreaterThanOrEqual() : base("- 1 >= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverLessThanOrEqual : Template
        {
            public PrecedenceNegateOverLessThanOrEqual() : base("- 1 <= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverEqual : Template
        {
            public PrecedenceNegateOverEqual() : base("- 1 = 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverNotEqual : Template
        {
            public PrecedenceNegateOverNotEqual() : base("- 1 != 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

    #endregion
    }
}
