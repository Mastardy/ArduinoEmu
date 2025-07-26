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

public class Register8
{
    public static implicit operator byte(Register8 r) => r.Value;
    public static implicit operator Register8(byte b) => new(b);

    private byte _value;

    public byte Value
    {
        get => _value;
        set => _value = value;
    }

    public Register8(byte value)
    {
        _value = value;
    }

    private bool GetBit(int offset) => ((_value >> offset) & 1) == 1;

    private void SetBit(int offset, bool value) =>
        _value = (byte)((value ? 1 : 0) << offset | (_value & ~(1 << offset)));

    public bool this[int offset]
    {
        get => GetBit(offset);
        set => SetBit(offset, value);
    }
}

public class StatusRegister : Register8
{
    public bool C // Carry Flag
    {
        get => this[0];
        set => this[0] = value;
    }

    public bool Z // Zero Flag
    {
        get => this[1];
        set => this[1] = value;
    }

    public bool N // Negative Flag
    {
        get => this[2];
        set => this[2] = value;
    }

    public bool V // Two's Complement Overflow Flag
    {
        get => this[3];
        set => this[3] = value;
    }

    public bool S // Sign Bit, S = N + V
    {
        get => this[4];
        set => this[4] = value;
    }

    public bool H // Half-Carry Flag
    {
        get => this[5];
        set => this[5] = value;
    }

    public bool T // Bit Copy Storage
    {
        get => this[6];
        set => this[6] = value;
    }

    public bool I // Global Interrupt Enable
    {
        get => this[7];
        set => this[7] = value;
    }

    public StatusRegister() : base(0)
    {
    }

    public override string ToString()
    {
        return $"SREG (0x{Value:X2}): C={C}, Z={Z}, N={N}, V={V}, S={S}, H={H}, T={T}, I={I}";
    }
}