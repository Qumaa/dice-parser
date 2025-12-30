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
}
