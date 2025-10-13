using System;
using System.CommandLine;
using System.Linq;
using System.Runtime.InteropServices;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    // todo: make tree plotter replaceable
    internal sealed class TreePlotter
    {
        private readonly NodeTree _tree;
        private readonly IConsole _console;
        private readonly Visitor _visitor;

        public TreePlotter(NodeTree tree, IConsole console)
        {
            _tree = tree;
            _console = console;

            _visitor = new Visitor();
        }

        public void Plot()
        {
            _tree.Next();

            Header();
            PlotNodeRecursively(in _tree.Root);
        }

        private void Header() =>
            _console.WriteLine($"Input: \'{_tree.SubstringMapper.Source}\'");

        private void PlotNodeRecursively(in Mapped<LinkedNode> node, string indent = null, bool isLast = false)
        {
            string nodeString = NodeToString(in node);

            indent = WriteNodeAndUpdateIndent(nodeString, indent, isLast);
            
            Mapped<LinkedNode>[] parents = node.Value.Parents;

            if (node.Value is { Parents: { Length: 0 } } or { IsOperator: true, Node: IComposite })
                return; // todo: list every composite result as if they were distinct nodes

            for (int i = 0; i < parents.Length; i++)
            {
                bool isLastChild = i == (parents.Length - 1);
                
                PlotNodeRecursively(in parents[i], indent, isLastChild);
            }
        }

        private string NodeToString(in Mapped<LinkedNode> treeNode)
        {
            INode node = treeNode.Value.Node;

            string value = _visitor.VisitAndGetEvaluationString(node);

            if (treeNode.Value.IsOperator)
            {
                string operatorString = ToOperatorString(in treeNode);

                if (string.Equals(value, operatorString, StringComparison.OrdinalIgnoreCase))
                    operatorString = ToExpressionString(in treeNode);

                return $"{value} ({operatorString})";
            }

            if (node is not (Dice or IComposite))
                return value;

            value = $"{ToExpressionString(in treeNode)} = {value}";

            if (node is IComposite composite)
                value += $" (rolled [{string.Join(", ", composite.Evaluation.Select(x => x.ToString()))}])";

            return value;
        }

        private string WriteNodeAndUpdateIndent(string nodeString, string indent, bool isLast)
        {
            const string child_tip = "├─";
            const string child_indent = "│ ";
            
            const string last_child_tip = "└─";
            const string last_child_indent = "  ";
            
            if (indent is not null)
            {
                if (indent is { Length: > 0 })
                    _console.Write(indent);

                if (isLast)
                {
                    _console.Write(last_child_tip);
                    indent += last_child_indent;
                }
                else
                {
                    _console.Write(child_tip);
                    indent += child_indent;
                }
            }
            else
                indent = string.Empty;

            _console.Write("*");
            _console.Space();

            _console.Write(nodeString);
            
            _console.WriteLine();

            return indent;
        }

        private string ToExpressionString(in Range range) =>
            _tree.SubstringMapper.GetSubstringOf(in range).ToString();

        private string ToExpressionString(in Mapped<LinkedNode> treeNode) =>
            ToExpressionString(in treeNode.Range);

        private string ToOperatorString(in Mapped<LinkedNode> operatorNode) =>
            new OperatorToStringConversion(this, in operatorNode).Execute();

        private class Visitor : INodeVisitor
        {
            private string _output;

            public void ForNumeric(INumeric numeric) =>
                _output = numeric.Evaluation.ToString();

            public void ForAssertion(IAssertion assertion) =>
                _output = assertion.Evaluation.ToString();

            public void ForOperation(IOperation operation) =>
                ForAssertion(operation.AsAssertion);

            public string VisitAndGetEvaluationString(INode node)
            {
                node.Visit(this);
                return _output;
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct OperatorToStringConversion
        {
            private readonly TreePlotter _context;
            private readonly Mapped<LinkedNode> _operatorNode;

            private int _operandsCount => _operatorNode.Value.Parents.Length;

            private SubstringMapper _substringMapper => _context._tree.SubstringMapper;

            public OperatorToStringConversion(TreePlotter context, in Mapped<LinkedNode> operatorNode)
            {
                _operatorNode = operatorNode;
                _context = context;
            }

            public string Execute() =>
                _operandsCount is 1 ?
                    UnaryLikeString() :
                    _operatorNode.Value.Node is IComposite composite ?
                        CompositionString(composite) :
                        GenericSpacedString();

            private string UnaryLikeString()
            {
                string first = GetStringOf(in _operatorNode);
                string second = GetStringOfEvaluation(GetOperand(0));

                if (GetOperandPosition(0) is OperandPosition.Left)
                    _SwapValues(ref first, ref second);

                return $"{first}{second}";

                static void _SwapValues(ref string first, ref string second) =>
                    (first, second) = (second, first);
            }

            private string CompositionString(IComposite confirmed) =>
                $"[{string.Join(',', confirmed.Evaluation.Select(x => x.ToString()))}] {GetStringOf(in _operatorNode)}";

            private string GenericSpacedString()
            {
                OperandPosition previousPosition = OperandPosition.Left;
                string operatorString = GetStringOf(in _operatorNode);
                string accumulated = null;

                for (int i = 0; i < _operandsCount; i++)
                {
                    Mapped<LinkedNode> operand = GetOperand(i);

                    string s = GetStringOfEvaluation(operand);

                    OperandPosition position = GetOperandPosition(i);
                    if (position != previousPosition)
                        s = $"{operatorString} {s}";
                    previousPosition = position;

                    accumulated = accumulated is null ? s : $"{accumulated} {s}";
                }

                if (previousPosition is OperandPosition.Left)
                    accumulated = $"{accumulated} {operatorString}";

                return accumulated;
            }

            private string GetStringOfEvaluation(Mapped<LinkedNode> operand) =>
                _context._visitor.VisitAndGetEvaluationString(operand.Value.Node);

            private string GetStringOf(in Mapped<LinkedNode> node) =>
                _substringMapper.GetSubstringOf(in node).ToString();

            private Mapped<LinkedNode> GetOperand(int operandIndex) =>
                _operatorNode.Value.Parents[operandIndex];

            private OperandPosition GetOperandPosition(int operandIndex)
            {
                Range operandRange = GetOperand(operandIndex).Range;
                Range operatorRange = _operatorNode.Range;

                return AverageFromRange(in operandRange) < AverageFromRange(in operatorRange) ?
                    OperandPosition.Left :
                    OperandPosition.Right;
            }

            private int AverageFromRange(in Range range)
            {
                (int offset, int length) = range.GetOffsetAndLength(_substringMapper.Source.Length);
                return offset + (length / 2);
            }

            private enum OperandPosition
            {
                Left,
                Right
            }
        }
    }
}
