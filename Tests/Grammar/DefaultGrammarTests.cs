namespace Tests.Grammar.Default
{
    [TestClass]
    public partial class DefaultGrammarTests
    {
        // todo composition once done
        // todo range
        // todo advanced dice (once changed to be an operator)

        [TestMethod]
        public void WeirdDie()
        {
            INumeric die = Parse<INumeric>("d4dd4");
        }
        
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
            }

            [TestMethod]
            public void BoolConstantTest()
            {
                AssertParsingResultOf<SingleBoolean>.MeetsExpectedPattern();

                AssertParsingResultOf<SingleBoolean>.FailsToMeetExpectedPatternOf<SingleNumber>();
                AssertParsingResultOf<SingleBoolean>.FailsToMeetExpectedPatternOf<SingleDie>();
                AssertParsingResultOf<SingleBoolean>.FailsToMeetExpectedPatternOf<ImplicitCompositionDice>();
            }

            [TestMethod]
            public void DiceTest()
            {
                AssertParsingResultOf<SingleDie>.MeetsExpectedPattern();
                AssertParsingResultOf<ImplicitCompositionDice>.MeetsExpectedPattern();

                AssertParsingResultOf<SingleDie>.FailsToMeetExpectedPatternOf<SingleBoolean>();
                AssertParsingResultOf<ImplicitCompositionDice>.FailsToMeetExpectedPatternOf<SingleBoolean>();
                AssertParsingResultOf<SingleDie>.FailsToMeetExpectedPatternOf<SingleNumber>();
                AssertParsingResultOf<ImplicitCompositionDice>.FailsToMeetExpectedPatternOf<SingleNumber>();
            }

            [TestMethod]
            public void SequenceTest()
            {
                AssertParsingResultOf<SimpleSequence>.MeetsExpectedPattern();
                AssertParsingResultOf<NestedSequence>.MeetsExpectedPattern();
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
            public void ParenthesisTest()
            {
                AssertParsingResultOf<PrecedenceOverridenByParenthesis>.MeetsExpectedPattern();
            }

            [TestMethod]
            public void AddTests()
            {
                AssertThat<Add>.PrecedenceOver<Multiply>.IsLower();
            }
            
            [TestMethod]
            public void SubtractTests()
            {
            }

            [TestMethod]
            public void MultiplyTests()
            {
                AssertThat<Multiply>.PrecedenceOver<Add>.IsHigher();
            }

            [TestMethod]
            public void DivideRoundDownTests()
            {
            }
            
            [TestMethod]
            public void DivideRoundUpTests()
            {
            }

            [TestMethod]
            public void NegateTests()
            {
            }
            
            [TestMethod]
            public void GreaterThanTests()
            {
            }
            
            [TestMethod]
            public void GreaterThanOrEqualTests()
            {
            }
            
            [TestMethod]
            public void LessThanTests()
            {
            }
            
            [TestMethod]
            public void LessThanOrEqualTests()
            {
            }

            [TestMethod]
            public void EqualTests()
            {
            }
            
            [TestMethod]
            public void AndTests()
            {
                AssertThat<And>.PrecedenceOver<And>.IsHigher();
            }
            
            [TestMethod]
            public void OrTests()
            {
            }
        }
    }
}
