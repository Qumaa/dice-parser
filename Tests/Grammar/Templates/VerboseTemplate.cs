namespace Tests.Syntax.Default
{
    public abstract class VerboseTemplate
    {
        public readonly string[] SampleStrings;

        protected VerboseTemplate(params string[] sampleStrings)
        {
            SampleStrings = sampleStrings;
        }

        protected VerboseTemplate(string sampleString) : this([sampleString]) { }

        protected VerboseTemplate(string formatString, params string[] options) : this(
            options.Select(x => string.Format(formatString, x)).ToArray()
            ) { }
            
        public bool MeetsExpectedPattern(NodeTree[] trees)
        {
            if (trees.Length != SampleStrings.Length)
                return false;

            int length = trees.Length;

            for (int i = 0; i < length; i++)
                if (!MeetsExpectedPattern(trees[i], i))
                    return false;

            return true;
        }

        public abstract bool MeetsExpectedPattern(NodeTree tree, int sampleIndex);
    }
}
