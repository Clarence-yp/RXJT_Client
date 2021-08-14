using System;

namespace ScriptRuntime.Graphics.PackedVector
{
    // Token: 0x0200000C RID: 12
    internal static class PackUtils
    {
        // Token: 0x06000243 RID: 579 RVA: 0x0000D9FB File Offset: 0x0000BBFB
        private static double ClampAndRound(float value, float min, float max)
        {
            if (float.IsNaN(value))
            {
                return 0.0;
            }
            if (float.IsInfinity(value))
            {
                if (!float.IsNegativeInfinity(value))
                {
                    return (double)max;
                }
                return (double)min;
            }
            else
            {
                if (value < min)
                {
                    return (double)min;
                }
                if (value > max)
                {
                    return (double)max;
                }
                return Math.Round((double)value);
            }
        }

        // Token: 0x06000244 RID: 580 RVA: 0x0000DA3C File Offset: 0x0000BC3C
        public static uint PackSigned(uint bitmask, float value)
        {
            float num = bitmask >> 1;
            float min = -num - 1f;
            return (uint)((int)PackUtils.ClampAndRound(value, min, num) & (int)bitmask);
        }

        // Token: 0x06000245 RID: 581 RVA: 0x0000DA64 File Offset: 0x0000BC64
        public static uint PackSNorm(uint bitmask, float value)
        {
            float num = bitmask >> 1;
            value *= num;
            return (uint)((int)PackUtils.ClampAndRound(value, -num, num) & (int)bitmask);
        }

        // Token: 0x06000246 RID: 582 RVA: 0x0000DA88 File Offset: 0x0000BC88
        public static uint PackUNorm(float bitmask, float value)
        {
            value *= bitmask;
            return (uint)PackUtils.ClampAndRound(value, 0f, bitmask);
        }

        // Token: 0x06000247 RID: 583 RVA: 0x0000DA9C File Offset: 0x0000BC9C
        public static uint PackUnsigned(float bitmask, float value)
        {
            return (uint)PackUtils.ClampAndRound(value, 0f, bitmask);
        }

        // Token: 0x06000248 RID: 584 RVA: 0x0000DAAC File Offset: 0x0000BCAC
        public static float UnpackSNorm(uint bitmask, uint value)
        {
            uint num = bitmask + 1U >> 1;
            if ((value & num) != 0U)
            {
                if ((value & bitmask) == num)
                {
                    return -1f;
                }
                value |= ~bitmask;
            }
            else
            {
                value &= bitmask;
            }
            float num2 = bitmask >> 1;
            return value / num2;
        }

        // Token: 0x06000249 RID: 585 RVA: 0x0000DAE8 File Offset: 0x0000BCE8
        public static float UnpackUNorm(uint bitmask, uint value)
        {
            value &= bitmask;
            return value / bitmask;
        }
    }
}
