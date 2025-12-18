using System.Collections.Generic;

namespace Mastardy.Runtime
{
    public class Program
    {
        public List<Instruction> Instructions { get; private set; }
        public Dictionary<string, int> Labels { get; private set; } = new();
        public Dictionary<string, object> Variables { get; private set; } = new();

        public Program(List<Instruction> instructions)
        {
            Instructions = instructions;

            for (var i = 0; i < instructions.Count; i++)
            {
                var instruction = instructions[i];

                if (instruction.OpCode == OpCode.DECLARE_FUNC)
                {
                    Labels.Add((string)instruction.Operands[0], i);
                }
                else if (instruction.OpCode == OpCode.DECLARE_VAR)
                {
                    Variables.Add((string)instruction.Operands[0], instruction.Operands[1]);
                }
            }
        }
    }
}