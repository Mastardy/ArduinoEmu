using System;
using Mastardy.Runtime.Utils;

public partial class ATmega328P
{
    #region ALU

    private void AddUpdateFlags(byte rdv, byte rrv, byte result, uint realResult)
    {
        SREG.Z = result == 0;
        SREG.C = realResult > 0xFF;
        SREG.N = result.GetBit(7);

        var rd7 = rdv.GetBit(7);
        var rr7 = rrv.GetBit(7);
        var r7 = result.GetBit(7);
        SREG.V = (rd7 && rr7 && !r7) || (!rd7 && !rr7 && r7);

        var rd3 = rdv.GetBit(3);
        var rr3 = rrv.GetBit(3);
        var r3 = result.GetBit(3);
        SREG.H = (rd3 && rr3) || (rr3 && !r3) || (!r3 && rd3);

        SREG.S = SREG.N ^ SREG.V;
    }

    public void ADD(byte Rd, byte Rr) // 0000 11rd dddd rrrr
    {
        UnityEngine.Debug.Assert(Rd < 0x20 && Rr < 0x20);

        var rdv = GPWR[Rd].Value; // Destination Register Value
        var rrv = GPWR[Rr].Value; // Source Register Value

        var result = (uint)(rdv + rrv);
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        AddUpdateFlags(rdv, rrv, finalResult, result);
    }

    public void ADC(byte Rd, byte Rr) // 0001 11rd dddd rrrr
    {
        UnityEngine.Debug.Assert(Rd < 0x20 && Rr < 0x20);

        var rdv = GPWR[Rd].Value; // Destination Register Value
        var rrv = GPWR[Rr].Value; // Source Register Value

        var result = (uint)(rdv + rrv + (SREG.C ? 1 : 0));
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        AddUpdateFlags(rdv, rrv, finalResult, result);
    }

    public void ADIW(byte Rdl, byte K) // 1001 0110 KKdd KKKK
    {
        UnityEngine.Debug.Assert(K < 0x40);
        UnityEngine.Debug.Assert(Rdl is > 0x17 and < 0x1F);
        UnityEngine.Debug.Assert(Rdl % 2 == 0);

        var rdv = (ushort)((GPWR[Rdl + 1] << 8) | GPWR[Rdl]); // Destination Register Value

        var result = (uint)(rdv + K);
        var finalResult = (ushort)result;

        GPWR[Rdl] = (byte)(finalResult & 0xFF);
        GPWR[Rdl + 1] = (byte)(finalResult >> 8);

        SREG.Z = finalResult == 0;
        SREG.C = !finalResult.GetBit(15) && rdv.GetBit(15);

        SREG.N = finalResult.GetBit(15);
        SREG.V = !rdv.GetBit(15) && finalResult.GetBit(15);
        SREG.S = SREG.N ^ SREG.V;
    }

    private void SubUpdateFlags(byte rdv, byte rrv, byte result, int realResult)
    {
        SREG.Z = result == 0;
        SREG.C = realResult < 0;
        SREG.N = result.GetBit(7);

        var rd7 = rdv.GetBit(7);
        var rr7 = rrv.GetBit(7);
        var r7 = result.GetBit(7);
        SREG.V = (rd7 && !rr7 && !r7) || (!rd7 && rr7 && r7);

        var rd3 = rdv.GetBit(3);
        var rr3 = rrv.GetBit(3);
        var r3 = result.GetBit(3);
        SREG.H = (!rd3 && rr3) || (rr3 && r3) || (r3 && !rd3);

        SREG.S = SREG.N ^ SREG.V;
    }

    private void SubCUpdateFlags(byte rdv, byte rrv, byte result, int realResult)
    {
        var z = SREG.Z;
        SubUpdateFlags(rdv, rrv, result, realResult);
        SREG.Z = SREG.Z && z;
    }

    public void SUB(byte Rd, byte Rr) // 0001 10rd dddd rrrr
    {
        UnityEngine.Debug.Assert(Rd < 0x20 && Rr < 0x20);

        var rdv = GPWR[Rd].Value; // Destination Register Value
        var rrv = GPWR[Rr].Value; // Source Register Value

        var result = rdv - rrv;
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        SubUpdateFlags(rdv, rrv, finalResult, result);
    }

    public void SUBI(byte Rd, byte K) // 0101 KKKK dddd KKKK
    {
        UnityEngine.Debug.Assert(Rd is > 0x0F and < 0x20);

        var rdv = GPWR[Rd].Value;

        var result = rdv - K;
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        SubUpdateFlags(rdv, K, finalResult, result);
    }

    public void SBC(byte Rd, byte Rr) // 0000 10rd dddd rrrr
    {
        UnityEngine.Debug.Assert(Rd < 0x20 && Rr < 0x20);

        var rdv = GPWR[Rd].Value;
        var rrv = GPWR[Rr].Value;

        var result = rdv - rrv - (SREG.C ? 1 : 0);
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        SubCUpdateFlags(rdv, rrv, finalResult, result);
    }

    public void SBCI(byte Rd, byte K) // 0100 KKKK dddd KKKK
    {
        UnityEngine.Debug.Assert(Rd is > 0x0F and < 0x20);

        var rdv = GPWR[Rd].Value;

        var result = rdv - K - (SREG.C ? 1 : 0);
        var finalResult = (byte)result;

        GPWR[Rd] = finalResult;

        SubCUpdateFlags(rdv, K, finalResult, result);
    }

    public void SBIW(byte Rdl, byte K) // 1001 0111 KKdd KKKK
    {
        UnityEngine.Debug.Assert(K < 40);
        UnityEngine.Debug.Assert(Rdl is > 0x17 and < 0x1F);
        UnityEngine.Debug.Assert(Rdl % 2 == 0);

        var rdv = (ushort)((GPWR[Rdl + 1] << 8) | GPWR[Rdl]);

        var result = rdv - K;
        var finalResult = (ushort)result;

        GPWR[Rdl] = (byte)(finalResult & 0xFF);
        GPWR[Rdl + 1] = (byte)(finalResult >> 8);

        SREG.Z = finalResult == 0;
        SREG.N = finalResult.GetBit(15);
        SREG.V = !finalResult.GetBit(15) && rdv.GetBit(15);
        SREG.S = SREG.N ^ SREG.V;
        SREG.C = finalResult.GetBit(15) && !rdv.GetBit(15);
    }

    private void AndUpdateFlags(byte result)
    {
        SREG.V = false;
        SREG.N = result.GetBit(7);
        SREG.S = SREG.N ^ SREG.V;
        SREG.Z = result == 0;
    }

    public void AND(byte Rd, byte Rr) // 0010 00rd dddd rrrr
    {
        UnityEngine.Debug.Assert(Rd < 0x20 && Rr < 0x20);

        var rdv = GPWR[Rd].Value; // Destination Register Value
        var rrv = GPWR[Rr].Value; // Source Register Value

        var result = (byte)(rdv & rrv);

        GPWR[Rd] = result;

        AndUpdateFlags(result);
    }

    public void ANDI(byte Rd, byte K) // 0111 KKKK dddd KKKK
    {
        UnityEngine.Debug.Assert(Rd is > 0x0F and < 0x20);

        var rdv = GPWR[Rd].Value; // Destination Register Value

        var result = (byte)(rdv & K);

        GPWR[Rd] = result;

        AndUpdateFlags(result);
    }

    #endregion

    #region Bit and Bit-Test Instructions

    public void ASR(byte Rd) // 1001 010d dddd 0101
    {
        UnityEngine.Debug.Assert(Rd < 0x20);

        var rdv = GPWR[Rd].Value; // Destination Register Value

        var result = (byte)((rdv >> 1) | (rdv & 0x80));

        GPWR[Rd] = result;

        SREG.Z = result == 0;
        SREG.N = result.GetBit(7);
        SREG.C = rdv.GetBit(0);
        SREG.V = SREG.N ^ SREG.C;
        SREG.S = SREG.N ^ SREG.V;
    }

    public void BCLR(int bit) // 1001 0100 1sss 1000
    {
        UnityEngine.Debug.Assert(bit is >= 0 and <= 7);
        SREG[bit] = false;
    }

    #endregion
}