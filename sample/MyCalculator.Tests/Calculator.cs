using System.Collections.Generic;

namespace MyCalculator.Tests
{
    public class Calculator
    {
        private readonly Stack<int> operands = new Stack<int>();

        public int Result
        {
            get { return operands.Peek(); }
        }

        public void Enter(int operand)
        {
            operands.Push(operand);
        }

        public void Add()
        {
            var op2 = operands.Pop();
            var op1 = operands.Pop();
            operands.Push(Addition.Add(op1, op2));
        }

        public void Multiply()
        {
            var op2 = operands.Pop();
            var op1 = operands.Pop();
            operands.Push(op1 * op2);
        }
    }
}
