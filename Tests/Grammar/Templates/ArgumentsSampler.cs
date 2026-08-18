namespace Tests.Syntax.Default
{
    public abstract class ArgumentsSampler
    {
        public static readonly ArgumentsSampler Numeric = new NumericImpl();
        public static readonly ArgumentsSampler Boolean = new BooleanImpl();
        public static readonly ArgumentsSampler Sequence = new SequenceImpl();
            
        public abstract string[] Generate(int operatorIndex, int argumentsCount);

        public string[] GenerateLeft(OperatorContract left) =>
            Generate(0, left.Arity);

        public string[] GenerateRight(OperatorContract right) =>
            Generate(1, right.Arity);

        public bool Verify(LinkedNode operatorNode, int operatorIndex) =>
            operatorNode.Parents
                .Select((x, i) => (node: x, i))
                .Where(x => x.node.IsOperand)
                .All(x => Verify(x.node.Node, operatorIndex, x.i));

        public bool VerifyLeft(LinkedNode operatorNode) =>
            Verify(operatorNode, 0);

        public bool VerifyRight(LinkedNode operatorNode) =>
            Verify(operatorNode, 1);

        protected string[] GenerateRepeating(string argumentString, int argumentsCount)
        {
            string[] args = new string[argumentsCount];

            for (int i = 0; i < args.Length; i++)
                args[i] = argumentString;

            return args;
        }
            
        protected abstract bool Verify(INode argument, int operatorIndex, int argumentIndex);
            
        private sealed class NumericImpl : ArgumentsSampler
        {
            public override string[] Generate(int operatorIndex, int argumentsCount) =>
                GenerateRepeating((operatorIndex + 1).ToString(), argumentsCount);

            protected override bool Verify(INode argument, int operatorIndex, int argumentIndex) =>
                argument is NumericConstant constant && constant.Value == operatorIndex + 1;
        }

        private sealed class BooleanImpl : ArgumentsSampler
        {
            public override string[] Generate(int operatorIndex, int argumentsCount) =>
                GenerateRepeating((operatorIndex % 2 is 0).ToString(), argumentsCount);

            protected override bool Verify(INode argument, int operatorIndex, int argumentIndex) =>
                argument is BinaryConstant constant && constant.Value == operatorIndex % 2 is 0;
        }
        
        private sealed class SequenceImpl : ArgumentsSampler
        {
            public override string[] Generate(int operatorIndex, int argumentsCount) =>
                Enumerable.Range(0, argumentsCount).Select(x => $"({operatorIndex + 1} {operatorIndex + 1 + x})").ToArray();

            protected override bool Verify(INode argument, int operatorIndex, int argumentIndex) =>
                argument is ISequence<INumeric> and [NumericConstant op, NumericConstant arg] &&
                op.Value == (operatorIndex + 1) &&
                arg.Value == (operatorIndex + 1 + argumentIndex);
        }
    }
}
