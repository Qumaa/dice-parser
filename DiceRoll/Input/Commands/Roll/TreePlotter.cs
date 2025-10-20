using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using DiceRoll.Input.Parsing;

namespace DiceRoll
{
    internal static class TreePlotter
    {
        public static void Plot(IConsole output, NodeTree tree)
        {
            WriteHeader(output, tree.SubstringMapper.Source);
            WriteWithChildren(output, tree.SubstringMapper, tree.Root, isLast: tree.Root.Value.IsOperand);
        }
        
        private static void WriteHeader(IConsole output, string expression) =>
            output.WriteLine($"Input: \'{expression}\'");

        private static void WriteWithChildren(IConsole output, SubstringMapper mapper,
            in Mapped<LinkedNode> node, string indent = null, bool isLast = false)
        {
            string nodeString = NodeToString(mapper, in node);

            WriteNode(output, nodeString, indent, isLast);
            indent = UpdateIndent(indent, isLast);
            
            if (node.Value.Node is IComposite composite)
            {
                WriteCompositeNodeEvaluations(output, mapper, composite, node.Value.Parents, indent);
                return;
            }
            
            if (node.Value.IsOperand)
                return;

            Mapped<LinkedNode>[] parents = node.Value.Parents;
            for (int i = 0; i < parents.Length; i++)
            {
                bool isLastChild = i == (parents.Length - 1);
                
                WriteWithChildren(output, mapper, in parents[i], indent, isLastChild);
            }
        }
        
        private static string NodeToString(SubstringMapper mapper, in Mapped<LinkedNode> mappedNode)
        {
            if (mappedNode.Value.IsOperator)
                return StringFormatter.ToOperatorString(mapper, in mappedNode);

            if (mappedNode.Value.Node is Dice or IComposite)
                return StringFormatter.ToRolledResultString(mapper, in mappedNode);

            return StringFormatter.ToEvaluationString(in mappedNode);
        }
        
        private static void WriteNode(IConsole output, string nodeString, string indent, bool isLast)
        {
            const string child_tip = "├─";
            
            const string last_child_tip = "└─";
            
            if (indent is not null)
            {
                if (indent.Length > 0)
                    output.Write(indent);

                output.Write(isLast ? last_child_tip : child_tip);
            }

            output.Write("*");
            output.Space();

            output.Write(nodeString);
            
            output.WriteLine();
        }
        
        private static string UpdateIndent(string indent, bool isLast)
        {
            const string child_indent = "│ ";
            const string last_child_indent = "  ";

            if (indent is null)
                return string.Empty;

            string extraIndent = isLast ? last_child_indent : child_indent;
            
            return indent + extraIndent;
        }
        
        private static void WriteCompositeNodeEvaluations(IConsole output, SubstringMapper mapper, IComposite composite, 
            Mapped<LinkedNode>[] parents, string indent)
        {
            // todo: only simple composite (repeated node) are handled now
            using IEnumerator<INumeric> enumerator = composite.SourceNodes.GetEnumerator();
            bool isLast = !enumerator.MoveNext();
            
            if (isLast)
                return;
            
            while (!isLast)
            {
                INumeric sourceNode = enumerator.Current!;

                string outcomeString = parents.Length is 0 ?
                    sourceNode.CachedEvaluation.ToString() :
                    StringFormatter.ToRolledResultString(
                        StringFormatter.ToExpressionString(mapper, in parents[1]),
                        sourceNode
                        );
                
                isLast = !enumerator.MoveNext();

                WriteNode(output, outcomeString, indent, isLast);
                
                if (sourceNode is IComposite nestedComposite)
                    WriteCompositeNodeEvaluations(
                        output,
                        mapper,
                        nestedComposite,
                        parents[1].Value.Parents,
                        UpdateIndent(indent, isLast)
                        );
            }
        }

        private static class StringFormatter
        {
            private static readonly EvaluationStringVisitor _visitor = new();
            
            public static string ToExpressionString(SubstringMapper mapper, in Mapped<LinkedNode> node) =>
                mapper.GetSubstringOf(in node).ToString();

            public static string ToEvaluationString(in Mapped<LinkedNode> mappedNode) =>
                ToEvaluationString(mappedNode.Value.Node);
            public static string ToEvaluationString(INode node) =>
                _visitor.GetEvaluationString(node);

            public static string ToOperatorString(SubstringMapper mapper, in Mapped<LinkedNode> operatorNode)
            {
                string evaluationString = ToEvaluationString(in operatorNode);
                string operatorString = OperatorStringFormatter.Format(mapper, in operatorNode);

                if (string.Equals(evaluationString, operatorString, StringComparison.OrdinalIgnoreCase))
                    operatorString = ToExpressionString(mapper, in operatorNode);

                return $"{evaluationString} ({operatorString})";
            }

            public static string ToRolledResultString(string expressionString, INode node) =>
                $"{expressionString} = {ToEvaluationString(node)}";

            public static string ToRolledResultString(SubstringMapper mapper, in Mapped<LinkedNode> rolledNode) =>
                ToRolledResultString(ToExpressionString(mapper, in rolledNode), rolledNode.Value.Node);

            private sealed class EvaluationStringVisitor : INodeVisitor
            {
                private string _output;

                public void ForNumeric(INumeric numeric) =>
                    _output = numeric.CachedEvaluation.ToString();

                public void ForAssertion(IAssertion assertion) =>
                    _output = assertion.CachedEvaluation.ToString();

                public void ForOperation(IOperation operation) =>
                    ForAssertion(operation.AsAssertion);

                public string GetEvaluationString(INode node)
                {
                    node.Visit(this);
                    return _output;
                }
            }

            private static class OperatorStringFormatter
            {
                public static string Format(SubstringMapper mapper, in Mapped<LinkedNode> operatorNode)
                {
                    if (operatorNode.Value.Parents.Length is 1)
                        return UnaryLikeString(mapper, in operatorNode);
                    
                    if (operatorNode.Value.Node is IComposite composite)
                        return CompositionString(composite, ToExpressionString(mapper, in operatorNode));

                    return GenericSpacedString(mapper, in operatorNode);
                }
                
                private static string UnaryLikeString(SubstringMapper mapper, in Mapped<LinkedNode> operatorNode)
                {
                    string first = ToExpressionString(mapper, in operatorNode);
                    string second = ToEvaluationString(GetOperand(operatorNode.Value, 0));

                    if (GetOperandPosition(in operatorNode, 0, mapper.Source.Length) is OperandPosition.Left)
                        _SwapValues(ref first, ref second);

                    return $"{first}{second}";

                    static void _SwapValues(ref string first, ref string second) =>
                        (first, second) = (second, first);
                }
                
                private static string CompositionString(IComposite composite, string operatorString) =>
                    $"[{string.Join(',', composite.SourceNodes.Select(x => x.CachedEvaluation.ToString()))}] {operatorString}";
                
                private static string GenericSpacedString(SubstringMapper mapper, in Mapped<LinkedNode> operatorNode)
                {
                    OperandPosition previousPosition = OperandPosition.Left;
                    string operatorString = ToExpressionString(mapper, in operatorNode);
                    string accumulated = null;

                    for (int i = 0; i < operatorNode.Value.Parents.Length; i++)
                    {
                        Mapped<LinkedNode> operand = GetOperand(operatorNode.Value, i);

                        string s = ToEvaluationString(in operand);

                        OperandPosition position = GetOperandPosition(in operatorNode, i, mapper.Source.Length);
                        if (position != previousPosition)
                            s = $"{operatorString} {s}";
                        previousPosition = position;

                        accumulated = accumulated is null ? s : $"{accumulated} {s}";
                    }

                    if (previousPosition is OperandPosition.Left)
                        accumulated = $"{accumulated} {operatorString}";

                    return accumulated;
                }
                
                private static Mapped<LinkedNode> GetOperand(LinkedNode node, int operandIndex) =>
                    node.Parents[operandIndex];

                private static OperandPosition GetOperandPosition(in Mapped<LinkedNode> operatorNode, int operandIndex,
                    int expressionLength)
                {
                    Range operandRange = GetOperand(operatorNode.Value, operandIndex).Range;
                    Range operatorRange = operatorNode.Range;

                    return AverageFromRange(in operandRange, expressionLength) <
                           AverageFromRange(in operatorRange, expressionLength) ?
                        OperandPosition.Left :
                        OperandPosition.Right;
                }

                private static int AverageFromRange(in Range range, int expressionLength)
                {
                    (int offset, int length) = range.GetOffsetAndLength(expressionLength);
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
}
