using System;
using System.Globalization;
using ScriptRuntime.Graphics.PackedVector;

namespace ScriptRuntime
{
    // Token: 0x02000007 RID: 7
    [Serializable]
    public struct ColorF : IEquatable<ColorF>
    {
        // Token: 0x06000080 RID: 128 RVA: 0x00006211 File Offset: 0x00004411
        public ColorF(float r, float g, float b, float a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        // Token: 0x06000081 RID: 129 RVA: 0x00006230 File Offset: 0x00004430
        public ColorF(float r, float g, float b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = 1f;
        }

        // Token: 0x06000082 RID: 130 RVA: 0x00006254 File Offset: 0x00004454
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

        // Token: 0x06000083 RID: 131 RVA: 0x000062B0 File Offset: 0x000044B0
        public override int GetHashCode()
        {
            return this.R.GetHashCode() + this.G.GetHashCode() + this.B.GetHashCode() + this.A.GetHashCode();
        }

        // Token: 0x06000084 RID: 132 RVA: 0x000062E1 File Offset: 0x000044E1
        public override bool Equals(object obj)
        {
            return obj is ColorF && this.Equals((ColorF)obj);
        }

        // Token: 0x06000085 RID: 133 RVA: 0x000062F9 File Offset: 0x000044F9
        public bool Equals(ColorF other)
        {
            return this.R == other.R && this.G == other.G && this.B == other.B && this.A == other.A;
        }

        // Token: 0x06000086 RID: 134 RVA: 0x0000633C File Offset: 0x0000453C
        public static bool operator ==(ColorF value1, ColorF value2)
        {
            return value1.R == value2.R && value1.G == value2.G && value1.B == value2.B && value1.A == value2.A;
        }

        // Token: 0x06000087 RID: 135 RVA: 0x0000638C File Offset: 0x0000458C
        public static bool operator !=(ColorF value1, ColorF value2)
        {
            return value1.R != value2.R || value1.G != value2.G || value1.B != value2.B || value1.A != value2.A;
        }

        // Token: 0x06000088 RID: 136 RVA: 0x000063E0 File Offset: 0x000045E0
        public static ColorF operator +(ColorF value1, ColorF value2)
        {
            ColorF result;
            result.R = value1.R + value2.R;
            result.G = value1.G + value2.G;
            result.B = value1.B + value2.B;
            result.A = value1.A + value2.A;
            return result;
        }

        // Token: 0x06000089 RID: 137 RVA: 0x00006448 File Offset: 0x00004648
        public static ColorF operator -(ColorF value1, ColorF value2)
        {
            ColorF result;
            result.R = value1.R - value2.R;
            result.G = value1.G - value2.G;
            result.B = value1.B - value2.B;
            result.A = value1.A - value2.A;
            return result;
        }

        // Token: 0x0600008A RID: 138 RVA: 0x000064B0 File Offset: 0x000046B0
        public static ColorF operator *(ColorF value1, ColorF value2)
        {
            ColorF result;
            result.R = value1.R * value2.R;
            result.G = value1.G * value2.G;
            result.B = value1.B * value2.B;
            result.A = value1.A * value2.A;
            return result;
        }

        // Token: 0x0600008B RID: 139 RVA: 0x00006518 File Offset: 0x00004718
        public static ColorF operator *(ColorF value1, float scaleFactor)
        {
            ColorF result;
            result.R = value1.R * scaleFactor;
            result.G = value1.G * scaleFactor;
            result.B = value1.B * scaleFactor;
            result.A = value1.A * scaleFactor;
            return result;
        }

        // Token: 0x0600008C RID: 140 RVA: 0x00006568 File Offset: 0x00004768
        public static ColorF operator *(float scaleFactor, ColorF value)
        {
            ColorF result;
            result.R = value.R * scaleFactor;
            result.G = value.G * scaleFactor;
            result.B = value.B * scaleFactor;
            result.A = value.A * scaleFactor;
            return result;
        }

        // Token: 0x0600008D RID: 141 RVA: 0x000065B8 File Offset: 0x000047B8
        public static ColorF operator /(ColorF value1, ColorF value2)
        {
            ColorF result;
            result.R = value1.R / value2.R;
            result.G = value1.G / value2.G;
            result.B = value1.B / value2.B;
            result.A = value1.A / value2.A;
            return result;
        }

        // Token: 0x0600008E RID: 142 RVA: 0x00006620 File Offset: 0x00004820
        public static ColorF operator /(ColorF value1, float divider)
        {
            float num = 1f / divider;
            ColorF result;
            result.R = value1.R * num;
            result.G = value1.G * num;
            result.B = value1.B * num;
            result.A = value1.A * num;
            return result;
        }

        // Token: 0x0600008F RID: 143 RVA: 0x00006676 File Offset: 0x00004876
        public Color32 ToColor32()
        {
            return new Color32(ColorF.PackHelper(this.R, this.G, this.B, this.A));
        }

        // Token: 0x06000090 RID: 144 RVA: 0x0000669A File Offset: 0x0000489A
        public Vector4 ToVector4()
        {
            return new Vector4(this.R, this.G, this.B, this.A);
        }

        // Token: 0x06000091 RID: 145 RVA: 0x000066BC File Offset: 0x000048BC
        private static uint PackHelper(float vectorX, float vectorY, float vectorZ, float vectorW)
        {
            uint num = PackUtils.PackUNorm(255f, vectorX);
            uint num2 = PackUtils.PackUNorm(255f, vectorY) << 8;
            uint num3 = PackUtils.PackUNorm(255f, vectorZ) << 16;
            uint num4 = PackUtils.PackUNorm(255f, vectorW) << 24;
            return num | num2 | num3 | num4;
        }

        // Token: 0x04000017 RID: 23
        public float R;

        // Token: 0x04000018 RID: 24
        public float G;

        // Token: 0x04000019 RID: 25
        public float B;

        // Token: 0x0400001A RID: 26
        public float A;
    }
}
