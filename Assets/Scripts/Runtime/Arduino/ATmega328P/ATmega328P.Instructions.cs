using System;

public partial class ATmega328P
{
    #region ALU

    public void ADD(byte Rd, byte Rr) // 0000 11rd dddd rrrr
    {
        if (Rd > 0x1F || Rr > 0x1F) throw new ArgumentException("Invalid Register Index");

        var rdv = GPWR[Rd].Value; // Destiny Register Value
        var rrv = GPWR[Rr].Value; // Source Register Value

        var result = (uint)(rdv + rrv);
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        SREG.Z = finalResult == 0;
        SREG.C = result > 0xFF;
        SREG.N = (finalResult & 0x80) != 0;

        var rd7 = (rdv & 0x80) != 0;
        var rr7 = (rrv & 0x80) != 0;
        var r7 = (finalResult & 0x80) != 0;
        SREG.V = (rd7 && rr7 && !r7) || (!rd7 && !rr7 && r7);

        var rd3 = (rdv & 0x08) != 0;
        var rr3 = (rrv & 0x08) != 0;
        var r3 = (finalResult & 0x08) != 0;
        SREG.H = (rd3 && rr3) || (rr3 && !r3) || (!r3 && rd3);

        SREG.S = SREG.N ^ SREG.V;
    }

    public void ADC(byte Rd, byte Rr) // 0001 11rd dddd rrrr
    {
        if (Rd > 0x1F || Rr > 0x1F) throw new ArgumentException("Invalid Register Index");

        var rdv = GPWR[Rd].Value; // Destiny Register Value
        var rrv = GPWR[Rr].Value; // Source Register Value

        var result = (uint)(rdv + rrv + (SREG.C ? 1 : 0));
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        SREG.Z = finalResult == 0;
        SREG.C = result > 0xFF;
        SREG.N = (finalResult & 0x80) != 0;

        var rd7 = (rdv & 0x80) != 0;
        var rr7 = (rrv & 0x80) != 0;
        var r7 = (finalResult & 0x80) != 0;
        SREG.V = (rd7 && rr7 && !r7) || (!rd7 && !rr7 && r7);

        var rd3 = (rdv & 0x08) != 0;
        var rr3 = (rrv & 0x08) != 0;
        var r3 = (finalResult & 0x08) != 0;
        SREG.H = (rd3 && rr3) || (rr3 && !r3) || (!r3 && rd3);

        SREG.S = SREG.N ^ SREG.V;
    }

    public void ADIW()
    {
        
    }

    #endregion
}