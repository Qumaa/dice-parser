namespace Tests.Grammar.Default
{
    public abstract class OperatorContract : Template
    {
        public readonly ArgumentsSampler ArgumentsSampler;
        public readonly Arity Arity;
            
        protected OperatorContract(ArgumentsSampler sampler, Arity arity, params string[] operatorSymbols) : base(operatorSymbols)
        {
            ArgumentsSampler = sampler;
            Arity = arity;
        }
    }
}
