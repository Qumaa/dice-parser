namespace Tests.Grammar.Default
{
    public abstract class Template : VerboseTemplate
    {
        protected Template(params string[] sampleStrings) : base(sampleStrings) { }
        protected Template(string sampleString) : base(sampleString) { }
        protected Template(string formatString, params string[] options) : base(formatString, options) { }

        public sealed override bool MeetsExpectedPattern(NodeTree tree, int sampleIndex) =>
            MeetsExpectedPattern(tree.Root.Node);

        protected abstract bool MeetsExpectedPattern(INode node);
    }
}
