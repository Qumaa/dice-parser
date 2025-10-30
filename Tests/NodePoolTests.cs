namespace Tests
{
    [TestClass]
    public class NodePoolTests
    {
        private static readonly NodePoolToken _token;
        private static readonly Parser _parser;

        static NodePoolTests()
        {
            _token = (NodePoolToken) NodePoolOperand.Default.Token;
            _parser = new Parser();
        }
        
        [TestMethod]
        public void SimpleDetection()
        {
            const string to_match = "(1,0)";
            const string input = $"aaa {to_match} garbage h";
            
            Assert.IsTrue(_token.Matches(input, out Substring match) && match.ToString() is to_match);
            Assert.IsTrue(
                _parser.Parse(in match) is INodePool pool && pool.ToArray() is
                [
                    NumericConstant { CachedEvaluation.Value: 1 },
                    NumericConstant { CachedEvaluation.Value: 0 }
                ]
                );
        }
        
        [TestMethod]
        public void NestedSimpleDetection()
        {
            const string to_match = "(2d4,d6,(d10,(1,0)), 3 + -1)";
            const string input = $"aaa {to_match} garbage h";
            
            Assert.IsTrue(_token.Matches(input, out Substring match) && match.ToString() is to_match);
            Assert.IsTrue(_token.EnumerateMatches(input).Select(x => x.ToString()).ToArray() is [to_match]);
            
            if (_parser.Parse(in match) is not INodePool pool)
            {
                Assert.Fail("Parsing failed");
                return;
            }

            INode[] nodes = pool.ToArray();

            if (nodes is not
                [
                    IComposite composite, Dice dice, INodePool pool2, INumeric numeric
                ])
            {
                Assert.Fail();
                return;
            }
            
            if (composite.SourceNodes.ToArray() is not [ Dice { Faces: 4 }, Dice {Faces: 4 } ])
                Assert.Fail();
            
            if (dice is not { Faces: 6 })
                Assert.Fail();
            
            if (numeric.Evaluate().Value is not 2)
                Assert.Fail();

            INode[] nodes2 = pool2.ToArray();
            
            if (nodes2 is not [ Dice { Faces: 10 }, INodePool pool3 ])
            {
                Assert.Fail();
                return;
            }

            if (pool3.ToArray() is not
                [
                    NumericConstant { CachedEvaluation.Value: 1 }, NumericConstant { CachedEvaluation.Value: 0 }
                ])
                Assert.Fail();
        }

        [TestMethod]
        public void SequentialDetection()
        {
            const string to_match = "(2d4, 2)";
            const string to_match2 = "(1,0)";
            const string input = $"aaa {to_match} garbage {to_match2} h";

            string[] strings = _token.EnumerateMatches(input).Select(x => x.ToString()).ToArray();
            Assert.IsTrue(strings is [ to_match, to_match2 ]);
        }
        
        [TestMethod]
        public void NoSeparator()
        {
            const string to_match = "(2d4 + 2)";
            const string input = $"aaa {to_match} garbage h";
            
            Assert.IsFalse(_token.Matches(input));
        }

        [TestMethod]
        public void ExtraSeparator()
        {
            const string to_match = "(2d4, 2,)";
            const string input = $"aaa {to_match} garbage h";
            
            const string to_match2 = "(,2d4, 2)";
            const string input2 = $"aaa {to_match2} garbage h";

            const string to_match3 = "(2d4,, 2)";
            const string input3 = $"aaa {to_match3} garbage h";
            
            Assert.IsFalse(_token.Matches(input));
            Assert.IsFalse(_token.Matches(input2));
            Assert.IsFalse(_token.Matches(input3));
        }

        [TestMethod]
        public void OnlySeparator()
        {
            const string to_match = "(  , )";
            const string input = $"aaa {to_match} garbage h";
            
            Assert.IsFalse(_token.Matches(input));
        }

        private sealed class Parser
        {
            private readonly ShuntingYard _shuntingYard = new(
                TokensTable.Default,
                OperandCastingTable.Default
                );
            
            public INode Parse(in Substring operandSubstring)
            {
                _shuntingYard.Append(operandSubstring.ToString());
                return _shuntingYard.Parse().Root.Value.Node;
            }
        }
    }
}
