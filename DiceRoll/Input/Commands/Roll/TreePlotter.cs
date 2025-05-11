using System;
using System.CommandLine;
using System.Linq;
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
            
            PlotNodeRecursively(in _tree.Root, new Visitor());
        }

        private void PlotNodeRecursively(in Mapped<LinkedNode> node, Visitor visitor, int indent = 0, bool ignoreIndent = false)
        {
            string evaluationString = NodeToString(in node, visitor, ignoreIndent ? 0 : indent, out int length);
            _console.Write(evaluationString);
            _console.Write(" ");
            indent += length + 1;

            Mapped<LinkedNode>[] parents = node.Value.Parents;
            
            if (parents.Length is 0)
            {
                _console.WriteLine(string.Empty);
                return;
            }

            for (int i = parents.Length - 1; i >= 0; i--)
                PlotNodeRecursively(in parents[i], visitor, indent, i == parents.Length - 1);
        }

        private string NodeToString(in Mapped<LinkedNode> node, Visitor visitor, int indent, out int nodeStringLength)
        {
            if (node.Value.IsOperator)
                return _Indent($"({_tree.SubstringSource.Apply(in node).ToString()})", out nodeStringLength);

            INode operand = node.Value.Node;
            operand.Visit(visitor);

            string output = visitor.Output;

            if (operand is not (Dice or IComposite))
                return _Indent(output, out nodeStringLength);

            output += $" ({_tree.SubstringSource.Apply(in node.Range).ToString()}";

            if (operand is IComposite composite)
                output += $" = [{string.Join(", ", composite.Evaluation.Select(x => x.ToString()))}]";
                
            output += ")";

            return _Indent(output, out nodeStringLength);

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
