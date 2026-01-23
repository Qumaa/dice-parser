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
                Assert.That.ParsingResultOf<SingleNumber>().FailsToMeetExpectedPatternOf<ImplicitCompositionDice>();
            }

            [TestMethod]
            public void BoolConstantTest()
            {
                Assert.That.ParsingResultOf<SingleBoolean>().MeetsExpectedPattern();

                Assert.That.ParsingResultOf<SingleBoolean>().FailsToMeetExpectedPatternOf<SingleNumber>();
                Assert.That.ParsingResultOf<SingleBoolean>().FailsToMeetExpectedPatternOf<SingleDie>();
                Assert.That.ParsingResultOf<SingleBoolean>().FailsToMeetExpectedPatternOf<ImplicitCompositionDice>();
            }

            [TestMethod]
            public void DiceTest()
            {
                Assert.That.ParsingResultOf<SingleDie>().MeetsExpectedPattern();
                Assert.That.ParsingResultOf<ImplicitCompositionDice>().MeetsExpectedPattern();

                Assert.That.ParsingResultOf<SingleDie>().FailsToMeetExpectedPatternOf<SingleBoolean>();
                Assert.That.ParsingResultOf<ImplicitCompositionDice>().FailsToMeetExpectedPatternOf<SingleBoolean>();
                Assert.That.ParsingResultOf<SingleDie>().FailsToMeetExpectedPatternOf<SingleNumber>();
                Assert.That.ParsingResultOf<ImplicitCompositionDice>().FailsToMeetExpectedPatternOf<SingleNumber>();
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
            public void AddTests()
            {
                Assert.That.PrecedenceOf<Add>().Over<Multiply>().IsLower();
            }
            
            [TestMethod]
            public void SubtractTests()
            {
            }

            [TestMethod]
            public void MultiplyTests()
            {
                Assert.That.PrecedenceOf<Multiply>().Over<Add>().IsHigher();
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
                Assert.That.PrecedenceOf<And>().Over<And>().IsHigher();
            }
            
            [TestMethod]
            public void OrTests()
            {
            }
        }
    }
}
