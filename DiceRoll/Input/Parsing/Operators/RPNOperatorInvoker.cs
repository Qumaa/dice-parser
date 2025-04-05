using System.Collections.Generic;

namespace DiceRoll.Input
{
    public abstract class RPNOperatorInvoker
    {
        public readonly int RequiredOperands;

        protected RPNOperatorInvoker(int requiredOperands)
        {
            RequiredOperands = requiredOperands;
        }

        public abstract void Invoke(Stack<INode> operands);
    }
}
