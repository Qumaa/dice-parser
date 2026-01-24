namespace Tests.Grammar.Default
{
    public abstract class ArgumentsSampler
    {
        public static readonly ArgumentsSampler Numeric = new NumericImpl();
        public static readonly ArgumentsSampler Boolean = new BooleanImpl();
        public static readonly ArgumentsSampler Composite = new CompositeImpl();
            
        public abstract string Generate(int operatorIndex);

        public string GenerateLeft() =>
            Generate(0);

        public string GenerateRight() =>
            Generate(1);

        public bool Verify(LinkedNode operatorNode, int operatorIndex) =>
            Verify(
                operatorNode.Parents.Where(x => x.IsOperand).Select(x => x.Node),
                operatorIndex
                );

        public bool VerifyLeft(LinkedNode operatorNode) =>
            Verify(operatorNode, 0);

        public bool VerifyRight(LinkedNode operatorNode) =>
            Verify(operatorNode, 1);
            
        protected abstract bool Verify(IEnumerable<INode> arguments, int operatorIndex);
            
        private sealed class NumericImpl : ArgumentsSampler
        {
            public override string Generate(int operatorIndex) =>
                (operatorIndex + 1).ToString();

            protected override bool Verify(IEnumerable<INode> arguments, int operatorIndex) =>
                arguments.All(x => x is NumericConstant constant && constant.Value == operatorIndex + 1);
        }

        private sealed class BooleanImpl : ArgumentsSampler
        {
            public override string Generate(int operatorIndex) =>
                (operatorIndex % 2 is 0).ToString();

            protected override bool Verify(IEnumerable<INode> arguments, int operatorIndex) =>
                arguments.All(x => x is BinaryConstant constant && constant.Value == operatorIndex % 2 is 0);
        }
        
        private sealed class CompositeImpl : ArgumentsSampler
        {
            public override string Generate(int operatorIndex)
            {
                int value = operatorIndex + 1;

                return $"{value}d{value}";
            }

            protected override bool Verify(IEnumerable<INode> arguments, int operatorIndex) =>
                arguments.All(x =>
                        x is IComposite composite && composite.Count == (operatorIndex + 1) &&
                        composite.All(y => y is Die die && die.Faces == (operatorIndex + 1))
                    );
        }
    }
}
