using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mastardy.Runtime
{
    public class ArduinoEmulator : MonoBehaviour
    {
        [SerializeField]
        private MCUConfig m_MCUConfig;

        [SerializeField]
        private float m_SimulationSpeed = 1.0f;

        private Program m_Program;
        private Memory m_Memory;

        private bool m_IsRunning;
        private double m_AccumulatedCycles;
        private int m_ProgramCounter;

        private long m_DelayInCycles;

        private void Awake()
        {
            m_Memory = new Memory(m_MCUConfig);
            LoadProgram(GenerateTestProgram());
        }

        private void Update()
        {
            if (!m_IsRunning) return;

            var cyclesToExecute = (double)Time.deltaTime * m_MCUConfig.ClockSpeed * m_SimulationSpeed;
            m_AccumulatedCycles += cyclesToExecute;

            ExecuteCycles((long)m_AccumulatedCycles);

            m_AccumulatedCycles -= (long)m_AccumulatedCycles;
        }

        private void ExecuteCycles(long cycleBudget)
        {
            var cyclesExecuted = 0L;

            while (cyclesExecuted < cycleBudget && m_IsRunning)
            {
                if (m_DelayInCycles > 0)
                {
                    var cyclesInDelay = Math.Min(m_DelayInCycles, cycleBudget - cyclesExecuted);
                    m_DelayInCycles -= cyclesInDelay;
                    cyclesExecuted += cyclesInDelay;

                    if (m_DelayInCycles > 0) return;
                }

                if (m_ProgramCounter >= m_Program.Instructions.Count)
                {
                    Debug.LogError("Program counter out of bounds.");
                    m_IsRunning = false;
                    return;
                }

                var instruction = m_Program.Instructions[m_ProgramCounter];

                var instructionCycles = ExecuteInstruction(instruction);

                cyclesExecuted += instructionCycles;
            }
        }

        private int ExecuteInstruction(Instruction instruction)
        {
            var cycles = 1;
            var nextPC = m_ProgramCounter + 1;

            switch (instruction.OpCode)
            {
                case OpCode.JUMP:
                {
                    nextPC = instruction.Operands[0] is string label
                        ? m_Program.Labels[label]
                        : (int)instruction.Operands[0];
                }
                    cycles = 3;
                    break;
                case OpCode.CALL:
                {
                    m_Memory.PushStack(m_ProgramCounter + 1);
                    nextPC = instruction.Operands[0] is string label
                        ? m_Program.Labels[label]
                        : (int)instruction.Operands[0];
                }
                    cycles = 4;
                    break;
                case OpCode.DELAY:
                {
                    var delayMs = (int)instruction.Operands[0];
                    m_DelayInCycles = (long)(delayMs * m_MCUConfig.ClockSpeed * 0.001);
                }
                    cycles = 0;
                    break;
                case OpCode.RET:
                    nextPC = m_Memory.PopStack();
                    cycles = 2;
                    break;
                case OpCode.PIN_MODE:
                {
                    var pin = instruction.Operands[0] is string variable ? (int)m_Program.Variables[variable] : (int)instruction.Operands[0];
                    SetPinMode(pin, (int)instruction.Operands[1]);
                }
                    cycles = 4;
                    break;
                case OpCode.DIGITAL_WRITE:
                {
                    var pin = instruction.Operands[0] is string variable ? (int)m_Program.Variables[variable] : (int)instruction.Operands[0];
                    DigitalWrite(pin, (int)instruction.Operands[1]);
                }
                    cycles = 4;
                    break;
                case OpCode.DECLARE_FUNC:
                case OpCode.DECLARE_VAR:
                    cycles = 0;
                    break;
                default:
                    Debug.Log("Instruction not implemented: " + instruction.OpCode);
                    break;
            }

            m_ProgramCounter = nextPC;
            return cycles;
        }

        private void SetPinMode(int pin, int mode)
        {
            Debug.Log($"Setting Pin({pin}) to {(mode == 0 ? "Output" : mode == 1 ? "Input" : "Input_Pullup")}");
        }

        private void DigitalWrite(int pin, int value)
        {
            Debug.Log($"Setting Pin({pin}) to {(value == 0 ? "0V (LOW)" : "5V (HIGH)")}");
        }

        private void LoadProgram(List<Instruction> program)
        {
            m_Program = new Program(program);
            m_IsRunning = true;
        }

        private List<Instruction> GenerateTestProgram()
        {
            var program = new List<Instruction>()
            {
                new(OpCode.DECLARE_VAR, "red", 9),
                new(OpCode.DECLARE_VAR, "yellow", 8),
                new(OpCode.DECLARE_VAR, "green", 7),

                new(OpCode.CALL, "setup"),

                new(OpCode.DECLARE_FUNC, "main_loop"),
                new(OpCode.CALL, "loop"),
                new(OpCode.JUMP, "main_loop"),

                new(OpCode.DECLARE_FUNC, "setup"),
                new(OpCode.PIN_MODE, "red", 0),
                new(OpCode.PIN_MODE, "yellow", 0),
                new(OpCode.PIN_MODE, "green", 0),
                new(OpCode.RET),

                new(OpCode.DECLARE_FUNC, "loop"),
                new(OpCode.DIGITAL_WRITE, "red", 1),
                new(OpCode.DELAY, 1500),
                new(OpCode.DIGITAL_WRITE, "red", 0),

                new(OpCode.DIGITAL_WRITE, "yellow", 1),
                new(OpCode.DELAY, 1000),
                new(OpCode.DIGITAL_WRITE, "yellow", 0),
                new(OpCode.DELAY, 500),
                new(OpCode.RET)
            };

            return program;
        }
    }
}