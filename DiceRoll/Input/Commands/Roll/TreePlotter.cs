using System;
using System.CommandLine;
using System.Runtime.InteropServices;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct TreePlotter
    {
        private readonly NodeTree _tree;
        private readonly IConsole _console;

        public TreePlotter(NodeTree tree, IConsole console)
        {
            _tree = tree;
            _console = console;
        }
        
        public void Plot()
        {
            _tree.Root.Value.Node.Next();
            
            PlotNodeRecursively(in _tree.Root);
        }

        private void PlotNodeRecursively(in Mapped<LinkedNode> node, int indent = 0, bool ignoreIndent = false)
        {
            string evaluationString = NodeToString(in node, ignoreIndent ? 0 : indent, out int length);
            _console.Write(evaluationString);
            _console.Write(" ");
            indent += length + 1;

            Mapped<LinkedNode>[] parents = node.Value.Parents;
            
            if (parents.Length is 0 || node.Value.Node is Composite)
            {
                _console.WriteLine(string.Empty);
                return;
            }

            for (int i = parents.Length - 1; i >= 0; i--)
                PlotNodeRecursively(in parents[i], indent, i == parents.Length - 1);
        }

        private string NodeToString(in Mapped<LinkedNode> node, int indent, out int i)
        {
            if (node.Value.IsOperator)
                return _Indent($"({_tree.SubstringSource.Apply(in node).ToString()})", out i);
            
            Visitor visitor = new();
            
            node.Value.Node.Visit(visitor);

            string output = node.Value.Node is Dice or Composite ?
                $"{visitor.Output} ({_tree.SubstringSource.Apply(in node.Range).ToString()})" :
                visitor.Output;
            
            return _Indent(output, out i);

            string _Indent(string input, out int inputLength)
            {
                inputLength = input.Length;
                
                return indent is 0 ? input : new string(' ', indent) + input;
            }
        }

        private class Visitor : INodeVisitor
        {
            public string Output;
            
            public void ForNumeric(INumeric numeric) =>
                Output = numeric.Evaluation.ToString();

            public void ForAssertion(IAssertion assertion) =>
                Output = assertion.Evaluation.ToString();

            public void ForOperation(IOperation operation) =>
                Output = operation.Evaluation.ToString();
        }
    }
}
