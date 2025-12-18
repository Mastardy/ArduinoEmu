using UnityEngine;

namespace Mastardy.Runtime
{
    public class Memory
    {
        private byte[] m_Memory;

        private int m_SRAMStart;
        private int m_EEPROMStart;
        private int m_FlashStart;

        private int m_StackPointer;

        public Memory(MCUConfig config)
        {
            m_Memory = new byte[config.SRAM + config.EEPROM + (config.Flash - config.BootloaderSize)];

            m_SRAMStart = 0;
            m_EEPROMStart = config.SRAM;
            m_FlashStart = config.SRAM + config.EEPROM;

            m_StackPointer = m_EEPROMStart - 1;
        }

        public void PushStack(int data)
        {
            if (m_StackPointer < m_EEPROMStart - 32)
            {
                throw new System.Exception("Stack overflow");
            }

            m_Memory[m_StackPointer--] = (byte)(data & 0xFF);
            m_Memory[m_StackPointer--] = (byte)((data >> 8) & 0xFF);
        }

        public int PopStack()
        {
            if (m_StackPointer >= m_EEPROMStart - 2)
            {
                throw new System.Exception("Stack underflow");
            }

            return (m_Memory[++m_StackPointer] << 8) | m_Memory[++m_StackPointer];
        }
    }
}