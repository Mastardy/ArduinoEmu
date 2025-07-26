public partial class ATmega328P
{
    public short PC { get; private set; }
    public short SP { get; private set; }
    
    public ATmega328P()
    {
        SREG = new StatusRegister();

        for (var i = 0; i < 32; i++)
        {
            GPWR[i] = new Register8(0);
        }
    }

    public void Debug(Register8 register)
    {
        UnityEngine.Debug.Log($"0x{register.Value:X2}");
    }
    
    public void Debug()
    {
        UnityEngine.Debug.Log($"{SREG}");
    }
}