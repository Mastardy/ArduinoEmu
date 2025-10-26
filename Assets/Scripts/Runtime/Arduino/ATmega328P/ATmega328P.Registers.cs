public partial class ATmega328P
{
    public StatusRegister SREG { get; private set; } = new();
    public Register8[] GPWR { get; private set; } = new Register8[32];

    public ushort X
    {
        get => (ushort)(GPWR[27].Value << 8 | GPWR[26].Value);
        set
        {
            GPWR[26] = (byte)value;
            GPWR[27] = (byte)(value >> 8);
        }
    }

    public ushort Y
    {
        get => (ushort)(GPWR[29].Value << 8 | GPWR[28].Value);
        set
        {
            GPWR[28] = (byte)value;
            GPWR[29] = (byte)(value >> 8);
        }
    }

    public ushort Z
    {
        get => (ushort)(GPWR[31].Value << 8 | GPWR[30].Value);
        set
        {
            GPWR[30] = (byte)value;
            GPWR[31] = (byte)(value >> 8);
        }
    }
}