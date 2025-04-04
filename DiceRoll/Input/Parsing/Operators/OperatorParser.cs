using System.Collections.Generic;

namespace DiceRoll.Input
{
    public abstract class OperatorParser
    {
        public readonly int RequiredOperands;

        protected OperatorParser(int requiredOperands)
        {
            RequiredOperands = requiredOperands;
        }

        public abstract void TransformOperands(Stack<INode> operands);
    }
}
