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