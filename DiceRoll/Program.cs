using System;

namespace DiceRoll
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var left = Node.Value.Dice(6);
            left = Node.Value.Summation(left, left);
            var right = Node.Value.Dice(6);

            IOperation operation = Node.Operator.Equal(left, right);

            IAssertion explicitAssertion = operation.AsAssertion();

            IAssertion implicitAssertion = (IAssertion) operation;

            TestVisitor visitor = new();
            
            explicitAssertion.Visit(visitor); // Assertion
            implicitAssertion.Visit(visitor); // Operation
        }
        
        private class TestVisitor : INodeVisitor
        {
            public void ForNumeric(INumeric numeric) =>
                throw new InvalidOperationException();

            public void ForAssertion(IAssertion assertion) =>
                Console.WriteLine("Assertion");

            public void ForOperation(IOperation operation) =>
                Console.WriteLine("Operation");
        }
    }
}
