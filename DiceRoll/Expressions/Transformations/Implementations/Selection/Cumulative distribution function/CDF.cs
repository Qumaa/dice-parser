namespace DiceRoll.Expressions
{
    public readonly struct CDF
    {
        public readonly Probability Equal;
        public readonly Probability EqualOr;
                
        public CDF(Probability equal, Probability equalOr)
        {
            Equal = equal;
            EqualOr = equalOr;
        }
    }
}
