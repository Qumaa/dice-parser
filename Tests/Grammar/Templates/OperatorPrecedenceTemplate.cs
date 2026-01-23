namespace Tests.Grammar.Default
{
    public sealed class OperatorPrecedenceTemplate<TL, TR> : VerboseTemplate
        where TL : OperatorContract, new() where TR : OperatorContract, new()
    {
        private readonly TL _left;
        private readonly TR _right;
        public readonly bool FavorLeft;

        public OperatorPrecedenceTemplate(bool favorLeft) : this(new TL(), new TR(), favorLeft) { }

        private OperatorPrecedenceTemplate(TL left, TR right, bool favorLeft) : base(
            GenerateSamples(left, right, favorLeft)
            )
        {
            _left = left;
            _right = right;
            FavorLeft = favorLeft;
        }

        public override bool MeetsExpectedPattern(NodeTree tree, int sampleIndex)
        {
            (NodeTree leftOp, NodeTree rightOp) = ExtractNodes(tree);

            if (!VerifyOperators(leftOp, rightOp, sampleIndex))
                return false;

            return _left.ArgumentsSampler.VerifyLeft(leftOp.Root) && _right.ArgumentsSampler.VerifyRight(rightOp.Root);
        }

        private bool VerifyOperators(NodeTree leftOp, NodeTree rightOp, int sampleIndex)
        {
            string leftSymbol = leftOp.RootSubstring().ToString();
            string rightSymbol = rightOp.RootSubstring().ToString();

            int leftSamplesCount = _left.SampleStrings.Length;
                
            int leftSampleIndex = sampleIndex / leftSamplesCount;
            int rightSampleIndex = sampleIndex % leftSamplesCount;

            if (_left.SampleStrings[leftSampleIndex] != leftSymbol)
                return false;

            if (_right.SampleStrings[rightSampleIndex] != rightSymbol)
                return false;

            if (!_left.MeetsExpectedPattern(leftOp, leftSampleIndex))
                return false;

            if (!_right.MeetsExpectedPattern(rightOp, rightSampleIndex))
                return false;

            return true;
        }

        private (NodeTree leftOp, NodeTree rightOp) ExtractNodes(NodeTree tree) =>
            FavorLeft ?
                (tree.Navigate().Descend(0).ExtractSubtree(), tree) :
                (tree, tree.Navigate().Descend(^1).ExtractSubtree());

        private static string[] GenerateSamples(OperatorContract left, OperatorContract right, bool favorLeft)
        {
            List<string> parts = [];
            string leftArg = left.ArgumentsSampler.GenerateLeft();
            string rightArg = right.ArgumentsSampler.GenerateRight();

            for (int i = 0; i < left.Arity.Left; i++)
                parts.Add(leftArg);

            parts.Add("{0}");

            int inBetweenArgs = (left.Arity.Right + right.Arity.Left) - 1;
            int breakPoint = left.Arity.Right;

            if (!favorLeft)
                breakPoint--;
                
            for (int i = 0; i < inBetweenArgs; i++)
                parts.Add(i >= breakPoint ? rightArg : leftArg);

            parts.Add("{1}");

            for (int i = 0; i < right.Arity.Right; i++)
                parts.Add(rightArg);

            string format = string.Join(" ", parts);

            return left.SampleStrings.SelectMany(_ => right.SampleStrings, (l, r) => string.Format(format, l, r)).ToArray();
        }
    }
}
