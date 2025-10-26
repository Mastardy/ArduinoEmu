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

    public bool S // Sign Bit, S = N ^(XOR) V
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

    public StatusRegister() : base(0) { }

    public override string ToString()
    {
        return $"SREG (0x{Value:X2}): C={C}, Z={Z}, N={N}, V={V}, S={S}, H={H}, T={T}, I={I}";
    }
}