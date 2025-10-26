using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Mastardy.Testing
{
    [TestFixture]
    public class ATmega328PTest
    {
        private ATmega328P chip;

        [SetUp]
        public void SetUp()
        {
            chip = new ATmega328P();
        }

        private static IEnumerable<TestCaseData> AddTestCases()
        {
            yield return new TestCaseData((byte)0, (byte)1, (byte)1, (byte)2, (byte)3, false, false, false, false, false)
                .SetName("0x01 + 0x02 = 0x03 (No Flags)");

            yield return new TestCaseData((byte)0, (byte)1, (byte)0xFF, (byte)0x01, (byte)0, true, true, false, false, true)
                .SetName("0xFF + 0x01 = 0x00 (C Z H)");

            yield return new TestCaseData((byte)0, (byte)1, (byte)0, (byte)0, (byte)0, false, true, false, false, false)
                .SetName("0x00 + 0x00 = 0x00 (Z)");

            yield return new TestCaseData((byte)0, (byte)1, (byte)0x7F, (byte)0x01, (byte)0x80, false, false, true, true, true)
                .SetName("0x7F + 0x01 = 0x80 (N V H)");

            yield return new TestCaseData((byte)0, (byte)1, (byte)0x0F, (byte)0x01, (byte)0x10, false, false, false, false, true)
                .SetName("0x0F + 0x01 = 0x10 (H)");
        }

        [Test, TestCaseSource(nameof(AddTestCases))]
        [Category("ALU")]
        public void AddInstruction(
            byte rd, byte rr, byte rdv, byte rrv,
            byte expectedRdv,
            bool expectedC, bool expectedZ, bool expectedN, bool expectedV, bool expectedH)
        {
            chip.GPWR[rd].Value = rdv;
            chip.GPWR[rr].Value = rrv;

            chip.ADD(rd, rr);

            Assert.AreEqual(expectedRdv, chip.GPWR[rd].Value, "RD Value");
            Assert.AreEqual(rrv, chip.GPWR[rr].Value, "RR Value");

            Assert.AreEqual(expectedC, chip.SREG.C, "C Flag");
            Assert.AreEqual(expectedZ, chip.SREG.Z, "Z Flag");
            Assert.AreEqual(expectedN, chip.SREG.N, "N Flag");
            Assert.AreEqual(expectedV, chip.SREG.V, "V Flag");
            Assert.AreEqual(expectedH, chip.SREG.H, "H Flag");
        }

        private static IEnumerable<TestCaseData> AdcTestCases()
        {
            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0x01, (byte)0x02, false,
                    (byte)0x03, false, false, false, false, false)
                .SetName("0x01 + 0x02 + C(0) = 0x03 (No Flags)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0x01, (byte)0x02, true,
                    (byte)0x04, false, false, false, false, false)
                .SetName("0x01 + 0x02 + C(1) = 0x04 (No Flags)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0xFF, (byte)0x01, false,
                    (byte)0x00, true, true, false, false, true)
                .SetName("0xFF + 0x01 + C(0) = 0x00 (C Z H)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0xFE, (byte)0x01, true,
                    (byte)0x00, true, true, false, false, true)
                .SetName("0xFE + 0x01 + C(1) = 0x00 (C Z H)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0x00, (byte)0x00, false,
                    (byte)0x00, false, true, false, false, false)
                .SetName("0x00 + 0x00 + C(0) = 0x00 (Z)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0x7F, (byte)0x01, false,
                    (byte)0x80, false, false, true, true, true)
                .SetName("0x7F + 0x01 + C(0) = 0x80 (N V H)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0x7E, (byte)0x01, true,
                    (byte)0x80, false, false, true, true, true)
                .SetName("0x7E + 0x01 + C(1) = 0x80 (N V H)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0x0F, (byte)0x01, false,
                    (byte)0x10, false, false, false, false, true)
                .SetName("0x0F + 0x01 + C(0) = 0x10 (H)");

            yield return new TestCaseData(
                    (byte)0x00, (byte)0x01, (byte)0x0E, (byte)0x01, true,
                    (byte)0x10, false, false, false, false, true)
                .SetName("0x0E + 0x01 + C(1) = 0x10 (H)");
        }

        [Test, TestCaseSource(nameof(AdcTestCases))]
        [Category("ALU")]
        public void AdcInstruction(
            byte rd, byte rr, byte rdv, byte rrv, bool C,
            byte expectedRdv, bool expectedC, bool expectedZ, bool expectedN, bool expectedV, bool expectedH)
        {
            chip.GPWR[rd].Value = rdv;
            chip.GPWR[rr].Value = rrv;
            chip.SREG.C = C;

            chip.ADC(rd, rr);

            Assert.AreEqual(expectedRdv, chip.GPWR[rd].Value, "RD Value");

            Assert.AreEqual(expectedC, chip.SREG.C, "C Flag");
            Assert.AreEqual(expectedZ, chip.SREG.Z, "Z Flag");
            Assert.AreEqual(expectedN, chip.SREG.N, "N Flag");
            Assert.AreEqual(expectedV, chip.SREG.V, "V Flag");
            Assert.AreEqual(expectedH, chip.SREG.H, "H Flag");
        }

        private static IEnumerable<TestCaseData> AdiwTestCases()
        {
            yield return new TestCaseData(
                    (byte)0x18, (byte)0x01, (byte)0x00, (byte)0x01,
                    (byte)0x02, (byte)0x00, false, false, false, false, false)
                .SetName("0x0001 + 0x01 = 0x0002 (No Flags)");
        }

        [Test, TestCaseSource(nameof(AdiwTestCases))]
        [Category("ALU")]
        public void AdiwInstruction(
            byte rd, byte low, byte high, byte k,
            byte expectedLow, byte expectedHigh, bool expectedC, bool expectedZ, bool expectedN, bool expectedV, bool expectedH)
        {
            chip.GPWR[rd].Value = low;
            chip.GPWR[rd + 1].Value = high;

            chip.ADIW(rd, k);

            Assert.AreEqual(expectedLow, chip.GPWR[rd].Value, "Low Value");
            Assert.AreEqual(expectedHigh, chip.GPWR[rd + 1].Value, "High Value");

            Assert.AreEqual(expectedC, chip.SREG.C, "C Flag");
            Assert.AreEqual(expectedZ, chip.SREG.Z, "Z Flag");
            Assert.AreEqual(expectedN, chip.SREG.N, "N Flag");
            Assert.AreEqual(expectedV, chip.SREG.V, "V Flag");
            Assert.AreEqual(expectedH, chip.SREG.H, "H Flag");
        }
    }
}