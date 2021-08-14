using System;

namespace ScriptRuntime
{
    // Token: 0x0200000A RID: 10
    public static class MathHelper
    {
        // Token: 0x060001D7 RID: 471 RVA: 0x00008BD9 File Offset: 0x00006DD9
        public static float Clamp(float value, float min, float max)
        {
            value = ((value > max) ? max : value);
            value = ((value < min) ? min : value);
            return value;
        }

        // Token: 0x060001D8 RID: 472 RVA: 0x00008BF0 File Offset: 0x00006DF0
        public static int Clamp(int value, int min, int max)
        {
            value = ((value > max) ? max : value);
            value = ((value < min) ? min : value);
            return value;
        }

        // Token: 0x060001D9 RID: 473 RVA: 0x00008C08 File Offset: 0x00006E08
        public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            return value1 * num3 + value2 * num4 + tangent1 * num5 + tangent2 * num6;
        }

        // Token: 0x060001DA RID: 474 RVA: 0x00008C6B File Offset: 0x00006E6B
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        // Token: 0x060001DB RID: 475 RVA: 0x00008C74 File Offset: 0x00006E74
        public static float Max(float a, float b)
        {
            if (a > b)
            {
                return a;
            }
            return b;
        }

        // Token: 0x060001DC RID: 476 RVA: 0x00008C7D File Offset: 0x00006E7D
        public static float Min(float a, float b)
        {
            if (a < b)
            {
                return a;
            }
            return b;
        }

        // Token: 0x060001DD RID: 477 RVA: 0x00008C86 File Offset: 0x00006E86
        public static int Max(int a, int b)
        {
            if (a > b)
            {
                return a;
            }
            return b;
        }

        // Token: 0x060001DE RID: 478 RVA: 0x00008C8F File Offset: 0x00006E8F
        public static int Min(int a, int b)
        {
            if (a < b)
            {
                return a;
            }
            return b;
        }

        // Token: 0x060001DF RID: 479 RVA: 0x00008C98 File Offset: 0x00006E98
        public static float SmoothStep(float value1, float value2, float amount)
        {
            float num = MathHelper.Clamp(amount, 0f, 1f);
            return MathHelper.Lerp(value1, value2, num * num * (3f - 2f * num));
        }

        // Token: 0x060001E0 RID: 480 RVA: 0x00008CCE File Offset: 0x00006ECE
        public static float RadiansToDegrees(float radians)
        {
            return radians * 57.29578f;
        }

        // Token: 0x060001E1 RID: 481 RVA: 0x00008CD7 File Offset: 0x00006ED7
        public static float DegreesToRadians(float degrees)
        {
            return degrees * 0.01745329f;
        }

        // Token: 0x060001E2 RID: 482 RVA: 0x00008CE0 File Offset: 0x00006EE0
        public static float Sin(float f)
        {
            return (float)Math.Sin((double)f);
        }

        // Token: 0x060001E3 RID: 483 RVA: 0x00008CEA File Offset: 0x00006EEA
        public static float Cos(float f)
        {
            return (float)Math.Cos((double)f);
        }

        // Token: 0x060001E4 RID: 484 RVA: 0x00008CF4 File Offset: 0x00006EF4
        public static float Tan(float f)
        {
            return (float)Math.Tan((double)f);
        }

        // Token: 0x060001E5 RID: 485 RVA: 0x00008CFE File Offset: 0x00006EFE
        public static float ASin(float f)
        {
            return (float)Math.Asin((double)f);
        }

        // Token: 0x060001E6 RID: 486 RVA: 0x00008D08 File Offset: 0x00006F08
        public static float ACos(float f)
        {
            return (float)Math.Acos((double)f);
        }

        // Token: 0x060001E7 RID: 487 RVA: 0x00008D12 File Offset: 0x00006F12
        public static float ATan(float f)
        {
            return (float)Math.Atan((double)f);
        }

        // Token: 0x060001E8 RID: 488 RVA: 0x00008D1C File Offset: 0x00006F1C
        public static float ATan2(float a, float b)
        {
            return (float)Math.Atan2((double)a, (double)b);
        }

        // Token: 0x060001E9 RID: 489 RVA: 0x00008D28 File Offset: 0x00006F28
        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt((double)f);
        }

        // Token: 0x060001EA RID: 490 RVA: 0x00008D32 File Offset: 0x00006F32
        public static float Abs(float f)
        {
            return Math.Abs(f);
        }

        // Token: 0x060001EB RID: 491 RVA: 0x00008D3A File Offset: 0x00006F3A
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        // Token: 0x060001EC RID: 492 RVA: 0x00008D42 File Offset: 0x00006F42
        public static float Pow(float fBase, float fExponent)
        {
            return (float)Math.Pow((double)fBase, (double)fExponent);
        }

        // Token: 0x060001ED RID: 493 RVA: 0x00008D4E File Offset: 0x00006F4E
        public static float Exp(float power)
        {
            return (float)Math.Exp((double)power);
        }

        // Token: 0x060001EE RID: 494 RVA: 0x00008D58 File Offset: 0x00006F58
        public static float Log(float f)
        {
            return (float)Math.Log((double)f);
        }

        // Token: 0x060001EF RID: 495 RVA: 0x00008D62 File Offset: 0x00006F62
        public static float Log10(float f)
        {
            return (float)Math.Log10((double)f);
        }

        // Token: 0x060001F0 RID: 496 RVA: 0x00008D6C File Offset: 0x00006F6C
        public static float Ceil(float f)
        {
            return (float)Math.Ceiling((double)f);
        }

        // Token: 0x060001F1 RID: 497 RVA: 0x00008D76 File Offset: 0x00006F76
        public static float Floor(float f)
        {
            return (float)Math.Floor((double)f);
        }

        // Token: 0x060001F2 RID: 498 RVA: 0x00008D80 File Offset: 0x00006F80
        public static float Round(float f)
        {
            return (float)Math.Round((double)f);
        }

        // Token: 0x060001F3 RID: 499 RVA: 0x00008D8A File Offset: 0x00006F8A
        public static int ICeil(float f)
        {
            return (int)Math.Ceiling((double)f);
        }

        // Token: 0x060001F4 RID: 500 RVA: 0x00008D94 File Offset: 0x00006F94
        public static int IFloor(float f)
        {
            return (int)Math.Floor((double)f);
        }

        // Token: 0x060001F5 RID: 501 RVA: 0x00008D9E File Offset: 0x00006F9E
        public static int IRound(float f)
        {
            return (int)Math.Round((double)f);
        }

        // Token: 0x060001F6 RID: 502 RVA: 0x00008DA8 File Offset: 0x00006FA8
        public static bool IsPowerOfTwo(int value)
        {
            return (value & value - 1) == 0;
        }

        // Token: 0x060001F7 RID: 503 RVA: 0x00008DB2 File Offset: 0x00006FB2
        public static int NextPowerOfTwo(int v)
        {
            v--;
            v |= v >> 16;
            v |= v >> 8;
            v |= v >> 4;
            v |= v >> 2;
            v |= v >> 1;
            return v + 1;
        }

        // Token: 0x04000026 RID: 38
        public const float E = 2.718282f;

        // Token: 0x04000027 RID: 39
        public const float Log10E = 0.4342945f;

        // Token: 0x04000028 RID: 40
        public const float Log2E = 1.442695f;

        // Token: 0x04000029 RID: 41
        public const float Pi = 3.141593f;

        // Token: 0x0400002A RID: 42
        public const float PiOver2 = 1.570796f;

        // Token: 0x0400002B RID: 43
        public const float PiOver4 = 0.7853982f;

        // Token: 0x0400002C RID: 44
        public const float TwoPi = 6.283185f;

        // Token: 0x0400002D RID: 45
        public const float Deg2Rad = 0.01745329f;

        // Token: 0x0400002E RID: 46
        public const float Rad2Deg = 57.29578f;

        // Token: 0x0400002F RID: 47
        public const float Epsilon = 1E-45f;

        // Token: 0x04000030 RID: 48
        public const float Infinity = float.PositiveInfinity;

        // Token: 0x04000031 RID: 49
        public const float NegativeInfinity = float.NegativeInfinity;
    }
}
