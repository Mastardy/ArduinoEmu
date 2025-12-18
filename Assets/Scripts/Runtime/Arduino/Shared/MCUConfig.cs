using UnityEngine;

namespace Mastardy.Runtime
{
    [CreateAssetMenu(fileName = "MCU Config", menuName = "Arduino", order = 0)]
    public class MCUConfig : ScriptableObject
    {
        public string Name;

        [Header("Memory (bytes)")]
        public int EEPROM;
        public int SRAM;
        public int Flash;
        public int BootloaderSize;

        [Header("Timers (MHz)")]
        public long ClockSpeed;
    }
}