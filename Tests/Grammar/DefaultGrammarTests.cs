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
                Assert.That.ParsingResultOf<SingleNumber>().MeetsExpectedPattern();

                Assert.That.ParsingResultOf<SingleNumber>().FailsToMeetExpectedPatternOf<SingleBoolean>();
                Assert.That.ParsingResultOf<SingleNumber>().FailsToMeetExpectedPatternOf<SingleDie>();
                Assert.That.ParsingResultOf<SingleNumber>().FailsToMeetExpectedPatternOf<SingleDice>();
            }

            [TestMethod]
            public void BoolConstantTest()
            {
                Assert.That.ParsingResultOf<SingleBoolean>().MeetsExpectedPattern();

                Assert.That.ParsingResultOf<SingleBoolean>().FailsToMeetExpectedPatternOf<SingleNumber>();
                Assert.That.ParsingResultOf<SingleBoolean>().FailsToMeetExpectedPatternOf<SingleDie>();
                Assert.That.ParsingResultOf<SingleBoolean>().FailsToMeetExpectedPatternOf<SingleDice>();
            }

            [TestMethod]
            public void DiceTest()
            {
                Assert.That.ParsingResultOf<SingleDie>().MeetsExpectedPattern();
                Assert.That.ParsingResultOf<SingleDice>().MeetsExpectedPattern();

                Assert.That.ParsingResultOf<SingleDie>().FailsToMeetExpectedPatternOf<SingleBoolean>();
                Assert.That.ParsingResultOf<SingleDice>().FailsToMeetExpectedPatternOf<SingleBoolean>();
                Assert.That.ParsingResultOf<SingleDie>().FailsToMeetExpectedPatternOf<SingleNumber>();
                Assert.That.ParsingResultOf<SingleDice>().FailsToMeetExpectedPatternOf<SingleNumber>();
            }

            [TestMethod]
            public void SequenceTest()
            {
                Assert.That.ParsingResultOf<SimpleSequence>().MeetsExpectedPattern();
                Assert.That.ParsingResultOf<NestedSequence>().MeetsExpectedPattern();
            }
        }

        [TestMethod]
        public void SimpleTests()
        {
            Assert.That.ParsingResultOf<SimpleAddition>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleSubtraction>().MeetsExpectedPattern();
            Assert.That.ParsingResultOf<SimpleNegation>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleMultiplication>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleDivideRoundDown>().MeetsExpectedPattern();
            Assert.That.ParsingResultOf<SimpleDivideRoundUp>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleEqualNumeric>().MeetsExpectedPattern();
            Assert.That.ParsingResultOf<SimpleNotEqualNumeric>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleEqualBinary>().MeetsExpectedPattern();
            Assert.That.ParsingResultOf<SimpleNotEqualBinary>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleGreaterThan>().MeetsExpectedPattern();
            Assert.That.ParsingResultOf<SimpleGreaterThanOrEqual>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleLessThan>().MeetsExpectedPattern();
            Assert.That.ParsingResultOf<SimpleLessThanOrEqual>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleAnd>().MeetsExpectedPattern();
            Assert.That.ParsingResultOf<SimpleOr>().MeetsExpectedPattern();
            
            Assert.That.ParsingResultOf<SimpleNot>().MeetsExpectedPattern();
        }

        [TestClass]
        public class PrecedenceTests
        {
            [TestMethod]
            public void ParenthesisTest()
            {
                Assert.That.ParsingResultOf<PrecedenceOverridenByParenthesis>().MeetsExpectedPattern();
            }
            
            [TestMethod]
            public void UnaryDieTests()
            {
                Assert.That.PrecedenceOf<UnaryDie>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<UnaryDie>().Over<BinaryDice>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<Times>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<Range>().IsHigher();
                // Assert.That.PrecedenceOf<UnaryDie>().Over<Negate>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryDie>().Over<NumericNotEqual>().IsHigher();
            }
            
            [TestMethod]
            public void BinaryDiceTests()
            {
                Assert.That.PrecedenceOf<BinaryDice>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<BinaryDice>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<BinaryDice>().Over<Times>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<Range>().IsHigher();
                // Assert.That.PrecedenceOf<BinaryDice>().Over<Negate>().IsHigher(); negative faces is invalid
                Assert.That.PrecedenceOf<BinaryDice>().Over<UnaryTotal>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<UnaryHighest>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<UnaryLowest>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<BinaryDice>().Over<NumericNotEqual>().IsHigher();
            }

            [TestMethod]
            public void TimesTests()
            {
                Assert.That.PrecedenceOf<Times>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<Times>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<Times>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<Times>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<Times>().Over<UnaryLowest>().IsLower();
            }

            [TestMethod]
            public void RangeTests()
            {
                Assert.That.PrecedenceOf<Range>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<Range>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<Range>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<Range>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<Range>().Over<UnaryLowest>().IsLower();
            }

            [TestMethod]
            public void UnaryTotalTests()
            {
                // Assert.That.PrecedenceOf<UnaryTotal>().Over<BinaryDice>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<Times>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<Range>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryTotal>().Over<NumericNotEqual>().IsHigher();
            }

            [TestMethod]
            public void UnaryHighestTests()
            {
                // Assert.That.PrecedenceOf<UnaryHighest>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<Times>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<Range>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryHighest>().Over<NumericNotEqual>().IsHigher();
            }

            [TestMethod]
            public void UnaryLowestTests()
            {
                // Assert.That.PrecedenceOf<UnaryLowest>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<Times>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<Range>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<UnaryLowest>().Over<NumericNotEqual>().IsHigher();
            }

            [TestMethod]
            public void AddTests()
            {
                Assert.That.PrecedenceOf<Add>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<Add>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<Add>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<Add>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Add>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Add>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<Add>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<Add>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<Add>().Over<NumericNotEqual>().IsHigher();
            }
            
            [TestMethod]
            public void SubtractTests()
            {
                Assert.That.PrecedenceOf<Subtract>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<Subtract>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<Subtract>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<Subtract>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Subtract>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Subtract>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<Subtract>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<Subtract>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<Subtract>().Over<NumericNotEqual>().IsHigher();
            }

            [TestMethod]
            public void MultiplyTests()
            {
                Assert.That.PrecedenceOf<Multiply>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<Multiply>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<Multiply>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<Multiply>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<Multiply>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<Multiply>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<Multiply>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<Multiply>().Over<NumericNotEqual>().IsHigher();
            }

            [TestMethod]
            public void DivideRoundDownTests()
            {
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundDown>().Over<NumericNotEqual>().IsHigher();
            }
            
            [TestMethod]
            public void DivideRoundUpTests()
            {
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<DivideRoundUp>().Over<NumericNotEqual>().IsHigher();
            }

            [TestMethod]
            public void NegateTests()
            {
                Assert.That.PrecedenceOf<Negate>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<Negate>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<Negate>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<Negate>().Over<Multiply>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<DivideRoundUp>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<DivideRoundDown>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<Add>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<Subtract>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<GreaterThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<LessThanOrEqual>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<GreaterThan>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<LessThan>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<NumericEqual>().IsHigher();
                Assert.That.PrecedenceOf<Negate>().Over<NumericNotEqual>().IsHigher();
            }
            
            [TestMethod]
            public void GreaterThanTests()
            {
                Assert.That.PrecedenceOf<GreaterThan>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<Add>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<Subtract>().IsLower();
                Assert.That.PrecedenceOf<GreaterThan>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<GreaterThan>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<GreaterThan>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<GreaterThan>().Over<Or>().IsHigher();
            }
            
            [TestMethod]
            public void GreaterThanOrEqualTests()
            {
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<Add>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<Subtract>().IsLower();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<GreaterThanOrEqual>().Over<Or>().IsHigher();
            }
            
            [TestMethod]
            public void LessThanTests()
            {
                Assert.That.PrecedenceOf<LessThan>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<Add>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<Subtract>().IsLower();
                Assert.That.PrecedenceOf<LessThan>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<LessThan>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<LessThan>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<LessThan>().Over<Or>().IsHigher();
            }
            
            [TestMethod]
            public void LessThanOrEqualTests()
            {
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<Add>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<Subtract>().IsLower();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<LessThanOrEqual>().Over<Or>().IsHigher();
            }

            [TestMethod]
            public void EqualTests()
            {
                Assert.That.PrecedenceOf<NumericEqual>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<Add>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<Subtract>().IsLower();
                Assert.That.PrecedenceOf<NumericEqual>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<NumericEqual>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<NumericEqual>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<NumericEqual>().Over<Or>().IsHigher();

                Assert.That.PrecedenceOf<BooleanEqual>().Over<Not>().IsLower();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<GreaterThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<LessThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<GreaterThan>().IsLower();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<LessThan>().IsLower();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<BooleanEqual>().Over<Or>().IsHigher();
            }

            [TestMethod]
            public void NotEqualTests()
            {
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<UnaryDie>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<BinaryDice>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<Negate>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<UnaryTotal>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<UnaryHighest>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<UnaryLowest>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<Multiply>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<DivideRoundUp>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<DivideRoundDown>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<Add>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<Subtract>().IsLower();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<NumericNotEqual>().Over<Or>().IsHigher();

                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<Not>().IsLower();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<GreaterThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<LessThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<GreaterThan>().IsLower();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<LessThan>().IsLower();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<BooleanEqual>().IsHigher();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<BooleanNotEqual>().IsHigher();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<BooleanNotEqual>().Over<Or>().IsHigher();
            }
            
            [TestMethod]
            public void AndTests()
            {
                Assert.That.PrecedenceOf<And>().Over<Not>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<GreaterThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<LessThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<GreaterThan>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<LessThan>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<NumericEqual>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<NumericNotEqual>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<BooleanEqual>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<BooleanNotEqual>().IsLower();
                Assert.That.PrecedenceOf<And>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<And>().Over<Or>().IsHigher();
            }
            
            [TestMethod]
            public void OrTests()
            {
                Assert.That.PrecedenceOf<Or>().Over<Not>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<GreaterThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<LessThanOrEqual>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<GreaterThan>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<LessThan>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<NumericEqual>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<NumericNotEqual>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<BooleanEqual>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<BooleanNotEqual>().IsLower();
                Assert.That.PrecedenceOf<Or>().Over<And>().IsHigher();
                Assert.That.PrecedenceOf<Or>().Over<Or>().IsHigher();
            }
        }
    }
}
