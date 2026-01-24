namespace Tests.Grammar.Default
{
    public abstract class OperatorContract : Template
    {
        public readonly ArgumentsSampler ArgumentsSampler;
        public readonly Arity Arity;
        public readonly bool SpaceAroundSymbol;
            
        protected OperatorContract(ArgumentsSampler sampler, Arity arity, bool spaceAroundSymbol, params string[] operatorSymbols) : base(operatorSymbols)
        {
            ArgumentsSampler = sampler;
            Arity = arity;
            SpaceAroundSymbol = spaceAroundSymbol;
        }

        protected OperatorContract(ArgumentsSampler sampler, Arity arity, params string[] operatorSymbols) : this(
            sampler,
            arity,
            true,
            operatorSymbols
            ) { }
    }
}
