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
            WriteWithChildren(output, tree.SubstringMapper, tree.Root, isLast: tree.Root.IsOperand);
        }
        
        private static void WriteHeader(IConsole output, string expression) =>
            output.WriteLine($"Input: \'{expression}\'");

        private static void WriteWithChildren(IConsole output, SubstringMapper mapper,
            LinkedNode node, string indent = null, bool isLast = false)
        {
            string nodeString = NodeToString(mapper, in node);

            WriteNode(output, nodeString, indent, isLast);
            indent = UpdateIndent(indent, isLast);
            
            if (node.Node is IComposite composite)
            {
                WriteCompositeNodeEvaluations(output, mapper, composite, node.Parents, indent);
                return;
            }
            
            if (node.IsOperand)
                return;

            LinkedNode[] parents = node.Parents;
            for (int i = 0; i < parents.Length; i++)
            {
                bool isLastChild = i == (parents.Length - 1);
                
                WriteWithChildren(output, mapper, parents[i], indent, isLastChild);
            }
        }
        
        private static string NodeToString(SubstringMapper mapper, in LinkedNode mappedNode)
        {
            if (mappedNode.IsOperator && mappedNode.Node is not ISequence)
                return StringFormatter.ToOperatorString(mapper, in mappedNode);

            if (mappedNode.Node is Die or IComposite)
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
            LinkedNode[] parents, string indent)
        {
            // todo: only simple composite (repeated node) are handled now
            using IEnumerator<INumeric> enumerator = composite.GetEnumerator();
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
                        parents[1].Parents,
                        UpdateIndent(indent, isLast)
                        );
            }
        }

        private static class StringFormatter
        {
            private static readonly EvaluationStringVisitor _visitor = new();
            
            public static string ToExpressionString(SubstringMapper mapper, in LinkedNode node) =>
                mapper.GetSubstringOf(in node.MappingRange).ToString();

            public static string ToEvaluationString(in LinkedNode mappedNode) =>
                ToEvaluationString(mappedNode.Node);
            public static string ToEvaluationString(INode node) =>
                _visitor.GetEvaluationString(node);

            public static string ToOperatorString(SubstringMapper mapper, in LinkedNode operatorNode)
            {
                string evaluationString = ToEvaluationString(in operatorNode);
                string operatorString = OperatorStringFormatter.Format(mapper, in operatorNode);

                if (string.Equals(evaluationString, operatorString, StringComparison.OrdinalIgnoreCase))
                    operatorString = ToExpressionString(mapper, in operatorNode);

                return $"{evaluationString} ({operatorString})";
            }

            public static string ToRolledResultString(string expressionString, INode node) =>
                $"{expressionString} = {ToEvaluationString(node)}";

            public static string ToRolledResultString(SubstringMapper mapper, in LinkedNode rolledNode) =>
                ToRolledResultString(ToExpressionString(mapper, in rolledNode), rolledNode.Node);

            private sealed class EvaluationStringVisitor : INodeVisitor
            {
                private string _output;

                public void ForNumeric(INumeric numeric) =>
                    _output = numeric.CachedEvaluation.ToString();

                public void ForAssertion(IAssertion assertion) =>
                    _output = assertion.CachedEvaluation.ToString();

                public void ForOperation(IOperation operation) =>
                    ForAssertion(operation.AsAssertion);

                public void ForSequence<T>(ISequence<T> sequence) where T : INode =>
                    _output = $"[{string.Join(", ", sequence.Select(x => GetEvaluationString(x)))}]";

                public string GetEvaluationString(INode node)
                {
                    node.Visit(this);
                    return _output;
                }
            }

            private static class OperatorStringFormatter
            {
                public static string Format(SubstringMapper mapper, in LinkedNode operatorNode)
                {
                    if (operatorNode.Parents.Length is 1)
                        return UnaryLikeString(mapper, in operatorNode);
                    
                    if (operatorNode.Node is IComposite composite)
                        return CompositionString(composite, ToExpressionString(mapper, in operatorNode));

                    return GenericSpacedString(mapper, in operatorNode);
                }
                
                private static string UnaryLikeString(SubstringMapper mapper, in LinkedNode operatorNode)
                {
                    string first = ToExpressionString(mapper, in operatorNode);
                    string second = ToEvaluationString(GetOperand(operatorNode, 0));

                    if (GetOperandPosition(in operatorNode, 0, mapper.Source.Length) is OperandPosition.Left)
                        _Swas(ref first, ref second);

                    return $"{first}{second}";

                    static void _Swas(ref string first, ref string second) =>
                        (first, second) = (second, first);
                }
                
                private static string CompositionString(IComposite composite, string operatorString) =>
                    $"[{string.Join(',', composite.Select(x => x.CachedEvaluation.ToString()))}] {operatorString}";
                
                private static string GenericSpacedString(SubstringMapper mapper, in LinkedNode operatorNode)
                {
                    OperandPosition previousPosition = OperandPosition.Left;
                    string operatorString = ToExpressionString(mapper, in operatorNode);
                    string accumulated = null;

                    for (int i = 0; i < operatorNode.Parents.Length; i++)
                    {
                        LinkedNode operand = GetOperand(operatorNode, i);

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
                
                private static LinkedNode GetOperand(LinkedNode node, int operandIndex) =>
                    node.Parents[operandIndex];

                private static OperandPosition GetOperandPosition(in LinkedNode operatorNode, int operandIndex,
                    int expressionLength)
                {
                    Range operandRange = GetOperand(operatorNode, operandIndex).MappingRange;
                    Range operatorRange = operatorNode.MappingRange;

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
