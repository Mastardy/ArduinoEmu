namespace Mastardy.Runtime
{
    public class Instruction
    {
        public OpCode OpCode { get; private set; }
        public object[] Operands { get; private set; }

        public Instruction(OpCode opCode, params object[] operands)
        {
            OpCode = opCode;
            Operands = operands;
        }
    }
}