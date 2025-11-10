namespace Tests
{
    [TestClass]
    public class TokenTests
    {
        private readonly Substring _sample = "start end all all 123 allall startend";
        
        [TestMethod]
        public void Matches()
        {
            IToken token = StringComparisonToken.CaseInsensitive("123");
            
            Assert.IsTrue(token.Matches(in _sample));
        }
        
        [TestMethod]
        public void MatchesAll()
        {
            IToken token = StringComparisonToken.CaseInsensitive("all");
            
            Assert.AreEqual(token.EnumerateMatches(_sample).Count(), 4);
        }

        [TestMethod]
        public void MatchesStart()
        {
            IToken token = StringComparisonToken.CaseInsensitive("start");
            
            Assert.IsTrue(token.MatchesStart(in _sample, out _));
        }
        
        [TestMethod]
        public void MatchesStartFaulty()
        {
            IToken token = StringComparisonToken.CaseInsensitive("end");
            
            Assert.IsFalse(token.MatchesStart(in _sample, out _));
        }

        [TestMethod]
        public void MatchesEnd()
        {
            IToken token = StringComparisonToken.CaseInsensitive("end");
            
            Assert.IsTrue(token.MatchesEnd(in _sample, out _));
        }
        
        [TestMethod]
        public void MatchesEndFaulty()
        {
            IToken token = StringComparisonToken.CaseInsensitive("start");
            
            Assert.IsFalse(token.MatchesEnd(in _sample, out _));
        }
    }
}
