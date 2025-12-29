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

        [TestMethod]
        public void PrecedenceTests()
        {
            // ( )
            AssertParsingResultOf<PrecedenceOverridenByParenthesis>.MeetsExpectedPattern();
            
            // +
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
            
            // -
            
            // *
            
            // /
            
            // //
            
            // >
            
            // <
            
            // >=
            
            // <=
            
            // =
            
            // !=
            
            // !
            
            // &
            
            // |
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
            private static readonly TTemplate _template;
            private static readonly INode _sampleParseResult;

            static AssertParsingResultOf()
            {
                _template = GetTemplate<TTemplate>();
                _sampleParseResult = Parse<INode>(_template.SampleString);
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
                        SelectionType: SelectionType.Highest,
                        Left: Dice { Faces: 20 },
                        Right: Dice { Faces: 20 }
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
                pool.ToArray() is [NumericConstant { Value: 4 }, NodePool pool2] &&
                pool2.ToArray() is [BinaryConstant { Value: true }, NumericConstant { Value: 15 }];
        }

    #endregion

    #region Operators

        private sealed class SimpleAddition : Template
        {
            public SimpleAddition() : base("d4 + 1") { }

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
                node is Negation { Source: NumericConstant { Value: 1 } };
        }

        private sealed class SimpleMultiplication : Template
        {
            public SimpleMultiplication() : base("d4 * 3") { }

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleEqualNumeric : Template
        {
            public SimpleEqualNumeric() : base("d4 = 3") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal, Left: Dice { Faces: 4 }, Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleNotEqualNumeric : Template
        {
            public SimpleNotEqualNumeric() : base("d4 != 3") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleEqualBinary : Template
        {
            public SimpleEqualBinary() : base("false = true") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleNotEqualBinary : Template
        {
            public SimpleNotEqualBinary() : base("false != true") { }

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Dice { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class SimpleAnd : Template
        {
            public SimpleAnd() : base("false & true") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleOr : Template
        {
            public SimpleOr() : base("false | true") { }

            public override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }
        
        private sealed class SimpleNot : Template
        {
            public SimpleNot() : base("!false") { }

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation { Source: NumericConstant { Value: 2 } }
                };
        }

        private sealed class PrecedenceAddOverMultiply : Template
        {
            public PrecedenceAddOverMultiply() : base("1 + 2 * 3") { }

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

            public override bool MeetsExpectedPattern(INode node) =>
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

    #endregion
    }
}
