using System.Runtime.CompilerServices;

namespace Mastardy.Runtime.Utils
{
    public static class BitHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(this byte b, int offset) => ((b >> offset) & 1) == 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetBit(this ushort w, int offset) => ((w >> offset) & 1) == 1;
    }
}