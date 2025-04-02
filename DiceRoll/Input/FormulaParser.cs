using System;

namespace DiceRoll.Input
{
    public class FormulaParser
    {
        public bool TryParse<T>(string[] args, int start, out int end, out IVisitableNode visitableNode)
        {
            
        }
    }

    public interface IVisitableNode
    {
        void Visit(INodeVisitor nodeVisitor);
    }

    public static class NodeExtensions
    {
        public static void Visit<T>(this INode<T> node, INodeVisitor nodeVisitor)
        {
            switch (node)
            {
                case IAnalyzable analyzable:
                    nodeVisitor.ForNumeric(analyzable);
                    break;

                case IOperation operation:
                    nodeVisitor.ForOperation(operation);
                    break;

                case IConditional conditional:
                    nodeVisitor.ForConditional(conditional);
                    break;
            }
        }
    }

    public interface INodeVisitor
    {
        void ForNumeric(IAnalyzable analyzable);
        void ForOperation(IOperation operation);
        void ForConditional(IConditional conditional);
    }
}
