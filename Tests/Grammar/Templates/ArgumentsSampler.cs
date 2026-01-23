namespace Tests.Grammar.Default
{
    public abstract class ArgumentsSampler
    {
        public static readonly ArgumentsSampler Numeric = new NumericImpl();
        public static readonly ArgumentsSampler Boolean = new BooleanImpl();
            
        public abstract string Generate(int operatorIndex);

        public string GenerateLeft() =>
            Generate(0);

        public string GenerateRight() =>
            Generate(1);

        public bool Verify(Mapped<LinkedNode> operatorNode, int operatorIndex) =>
            Verify(
                operatorNode.Value.Parents.Where(x => x.Value.IsOperand).Select(x => x.Value.Node),
                Generate(operatorIndex)
                );

        public bool VerifyLeft(Mapped<LinkedNode> operatorNode) =>
            Verify(operatorNode, 0);

        public bool VerifyRight(Mapped<LinkedNode> operatorNode) =>
            Verify(operatorNode, 1);
            
        protected abstract bool Verify(IEnumerable<INode> arguments, string value);
            
        private sealed class NumericImpl : ArgumentsSampler
        {
            public override string Generate(int operatorIndex) =>
                (operatorIndex + 1).ToString();

            protected override bool Verify(IEnumerable<INode> arguments, string value) =>
                arguments.All(x => x is NumericConstant constant && constant.Value.ToString().Equals(value));
        }

        private sealed class BooleanImpl : ArgumentsSampler
        {
            public override string Generate(int operatorIndex) =>
                (operatorIndex % 2 is 0).ToString();

            protected override bool Verify(IEnumerable<INode> arguments, string value) =>
                arguments.All(x => x is BinaryConstant constant && constant.Value.ToString().Equals(value));
        }
    }
}
