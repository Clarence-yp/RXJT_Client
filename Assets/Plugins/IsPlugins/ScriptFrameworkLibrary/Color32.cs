using System;
using System.Globalization;
using ScriptRuntime.Graphics.PackedVector;

namespace ScriptRuntime
{
    // Token: 0x02000006 RID: 6
    [Serializable]
    public struct Color32 : IEquatable<Color32>
    {
        // Token: 0x06000064 RID: 100 RVA: 0x00005D4B File Offset: 0x00003F4B
        internal Color32(uint packedValue)
        {
            this.packedValue = packedValue;
        }

        // Token: 0x06000065 RID: 101 RVA: 0x00005D54 File Offset: 0x00003F54
        public Color32(int r, int g, int b, int a)
        {
            if (((r | g | b | a) & -256) != 0)
            {
                r = Color32.ClampToByte32(r);
                g = Color32.ClampToByte32(g);
                b = Color32.ClampToByte32(b);
                a = Color32.ClampToByte32(a);
            }
            g <<= 8;
            b <<= 16;
            a <<= 24;
            this.packedValue = (uint)(r | g | b | a);
        }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000066 RID: 102 RVA: 0x00005DB2 File Offset: 0x00003FB2
        // (set) Token: 0x06000067 RID: 103 RVA: 0x00005DBB File Offset: 0x00003FBB
        public byte R {
            get {
                return (byte)this.packedValue;
            }
            set {
                this.packedValue = ((this.packedValue & 4294967040U) | (uint)value);
            }
        }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000068 RID: 104 RVA: 0x00005DD1 File Offset: 0x00003FD1
        // (set) Token: 0x06000069 RID: 105 RVA: 0x00005DDC File Offset: 0x00003FDC
        public byte G {
            get {
                return (byte)(this.packedValue >> 8);
            }
            set {
                this.packedValue = ((this.packedValue & 4294902015U) | (uint)((uint)value << 8));
            }
        }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x0600006A RID: 106 RVA: 0x00005DF4 File Offset: 0x00003FF4
        // (set) Token: 0x0600006B RID: 107 RVA: 0x00005E00 File Offset: 0x00004000
        public byte B {
            get {
                return (byte)(this.packedValue >> 16);
            }
            set {
                this.packedValue = ((this.packedValue & 4278255615U) | (uint)((uint)value << 16));
            }
        }

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x0600006C RID: 108 RVA: 0x00005E19 File Offset: 0x00004019
        // (set) Token: 0x0600006D RID: 109 RVA: 0x00005E25 File Offset: 0x00004025
        public byte A {
            get {
                return (byte)(this.packedValue >> 24);
            }
            set {
                this.packedValue = ((this.packedValue & 16777215U) | (uint)((uint)value << 24));
            }
        }

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x0600006E RID: 110 RVA: 0x00005E3E File Offset: 0x0000403E
        // (set) Token: 0x0600006F RID: 111 RVA: 0x00005E46 File Offset: 0x00004046
        public uint PackedValue {
            get {
                return this.packedValue;
            }
            set {
                this.packedValue = value;
            }
        }

        // Token: 0x06000070 RID: 112 RVA: 0x00005E50 File Offset: 0x00004050
        public static Color32 Lerp(Color32 value1, Color32 value2, float amount)
        {
            uint num = value1.packedValue;
            uint num2 = value2.packedValue;
            int num3 = (int)((byte)num);
            int num4 = (int)((byte)(num >> 8));
            int num5 = (int)((byte)(num >> 16));
            int num6 = (int)((byte)(num >> 24));
            int num7 = (int)((byte)num2);
            int num8 = (int)((byte)(num2 >> 8));
            int num9 = (int)((byte)(num2 >> 16));
            int num10 = (int)((byte)(num2 >> 24));
            int num11 = (int)PackUtils.PackUNorm(65536f, amount);
            int num12 = num3 + ((num7 - num3) * num11 >> 16);
            int num13 = num4 + ((num8 - num4) * num11 >> 16);
            int num14 = num5 + ((num9 - num5) * num11 >> 16);
            int num15 = num6 + ((num10 - num6) * num11 >> 16);
            Color32 result;
            result.packedValue = (uint)(num12 | num13 << 8 | num14 << 16 | num15 << 24);
            return result;
        }

        // Token: 0x06000071 RID: 113 RVA: 0x00005F04 File Offset: 0x00004104
        public static Color32 Multiply(Color32 value, float scale)
        {
            uint num = value.packedValue;
            uint num2 = (uint)((byte)num);
            uint num3 = (uint)((byte)(num >> 8));
            uint num4 = (uint)((byte)(num >> 16));
            uint num5 = (uint)((byte)(num >> 24));
            scale *= 65536f;
            uint num6;
            if (scale < 0f)
            {
                num6 = 0U;
            }
            else if (scale > 16777220f)
            {
                num6 = 16777215U;
            }
            else
            {
                num6 = (uint)scale;
            }
            num2 = num2 * num6 >> 16;
            num3 = num3 * num6 >> 16;
            num4 = num4 * num6 >> 16;
            num5 = num5 * num6 >> 16;
            if (num2 > 255U)
            {
                num2 = 255U;
            }
            if (num3 > 255U)
            {
                num3 = 255U;
            }
            if (num4 > 255U)
            {
                num4 = 255U;
            }
            if (num5 > 255U)
            {
                num5 = 255U;
            }
            Color32 result;
            result.packedValue = (num2 | num3 << 8 | num4 << 16 | num5 << 24);
            return result;
        }

        // Token: 0x06000072 RID: 114 RVA: 0x00005FD4 File Offset: 0x000041D4
        public static Color32 operator *(Color32 value, float scale)
        {
            uint num = value.packedValue;
            uint num2 = (uint)((byte)num);
            uint num3 = (uint)((byte)(num >> 8));
            uint num4 = (uint)((byte)(num >> 16));
            uint num5 = (uint)((byte)(num >> 24));
            scale *= 65536f;
            uint num6;
            if (scale < 0f)
            {
                num6 = 0U;
            }
            else if (scale > 16777220f)
            {
                num6 = 16777215U;
            }
            else
            {
                num6 = (uint)scale;
            }
            num2 = num2 * num6 >> 16;
            num3 = num3 * num6 >> 16;
            num4 = num4 * num6 >> 16;
            num5 = num5 * num6 >> 16;
            if (num2 > 255U)
            {
                num2 = 255U;
            }
            if (num3 > 255U)
            {
                num3 = 255U;
            }
            if (num4 > 255U)
            {
                num4 = 255U;
            }
            if (num5 > 255U)
            {
                num5 = 255U;
            }
            Color32 result;
            result.packedValue = (num2 | num3 << 8 | num4 << 16 | num5 << 24);
            return result;
        }

        // Token: 0x06000073 RID: 115 RVA: 0x000060A4 File Offset: 0x000042A4
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{R:{0} G:{1} B:{2} A:{3}}", new object[]
            {
                this.R,
                this.G,
                this.B,
                this.A
            });
        }

        // Token: 0x06000074 RID: 116 RVA: 0x00006100 File Offset: 0x00004300
        public override int GetHashCode()
        {
            return this.packedValue.GetHashCode();
        }

        // Token: 0x06000075 RID: 117 RVA: 0x0000610D File Offset: 0x0000430D
        public override bool Equals(object obj)
        {
            return obj is Color32 && this.Equals((Color32)obj);
        }

        // Token: 0x06000076 RID: 118 RVA: 0x00006125 File Offset: 0x00004325
        public bool Equals(Color32 other)
        {
            return this.packedValue.Equals(other.packedValue);
        }

        // Token: 0x06000077 RID: 119 RVA: 0x00006139 File Offset: 0x00004339
        public static bool operator ==(Color32 a, Color32 b)
        {
            return a.Equals(b);
        }

        // Token: 0x06000078 RID: 120 RVA: 0x00006143 File Offset: 0x00004343
        public static bool operator !=(Color32 a, Color32 b)
        {
            return !a.Equals(b);
        }

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x06000079 RID: 121 RVA: 0x00006150 File Offset: 0x00004350
        public static Color32 Black {
            get {
                return new Color32(4278190080U);
            }
        }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x0600007A RID: 122 RVA: 0x0000615C File Offset: 0x0000435C
        public static Color32 Blue {
            get {
                return new Color32(4294901760U);
            }
        }

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x0600007B RID: 123 RVA: 0x00006168 File Offset: 0x00004368
        public static Color32 Green {
            get {
                return new Color32(4278222848U);
            }
        }

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x0600007C RID: 124 RVA: 0x00006174 File Offset: 0x00004374
        public static Color32 Red {
            get {
                return new Color32(4278190335U);
            }
        }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x0600007D RID: 125 RVA: 0x00006180 File Offset: 0x00004380
        public static Color32 White {
            get {
                return new Color32(uint.MaxValue);
            }
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00006188 File Offset: 0x00004388
        public ColorF ToColorF()
        {
            ColorF result;
            result.R = PackUtils.UnpackUNorm(255U, this.packedValue);
            result.G = PackUtils.UnpackUNorm(255U, this.packedValue >> 8);
            result.B = PackUtils.UnpackUNorm(255U, this.packedValue >> 16);
            result.A = PackUtils.UnpackUNorm(255U, this.packedValue >> 24);
            return result;
        }

        // Token: 0x0600007F RID: 127 RVA: 0x000061FA File Offset: 0x000043FA
        private static int ClampToByte32(int value)
        {
            if (value < 0)
            {
                return 0;
            }
            if (value > 255)
            {
                return 255;
            }
            return value;
        }

        // Token: 0x04000016 RID: 22
        private uint packedValue;
    }
}
