namespace Mastardy.Runtime
{
    public enum OpCode
    {
        Halt = 0,

        DECLARE_FUNC,
        DECLARE_VAR,

        CALL,
        JUMP,
        RET,

        PIN_MODE,
        DIGITAL_WRITE,
        DELAY
    }
}