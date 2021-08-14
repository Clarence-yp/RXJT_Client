using System;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x02000013 RID: 19
    [Serializable]
    public struct Vector4 : IEquatable<Vector4>
    {
        // Token: 0x17000167 RID: 359
        // (get) Token: 0x06000328 RID: 808 RVA: 0x00012E86 File Offset: 0x00011086
        public static Vector4 Zero {
            get {
                return Vector4._zero;
            }
        }

        // Token: 0x17000168 RID: 360
        // (get) Token: 0x06000329 RID: 809 RVA: 0x00012E8D File Offset: 0x0001108D
        public static Vector4 One {
            get {
                return Vector4._one;
            }
        }

        // Token: 0x17000169 RID: 361
        // (get) Token: 0x0600032A RID: 810 RVA: 0x00012E94 File Offset: 0x00011094
        public static Vector4 UnitX {
            get {
                return Vector4._unitX;
            }
        }

        // Token: 0x1700016A RID: 362
        // (get) Token: 0x0600032B RID: 811 RVA: 0x00012E9B File Offset: 0x0001109B
        public static Vector4 UnitY {
            get {
                return Vector4._unitY;
            }
        }

        // Token: 0x1700016B RID: 363
        // (get) Token: 0x0600032C RID: 812 RVA: 0x00012EA2 File Offset: 0x000110A2
        public static Vector4 UnitZ {
            get {
                return Vector4._unitZ;
            }
        }

        // Token: 0x1700016C RID: 364
        // (get) Token: 0x0600032D RID: 813 RVA: 0x00012EA9 File Offset: 0x000110A9
        public static Vector4 UnitW {
            get {
                return Vector4._unitW;
            }
        }

        // Token: 0x0600032E RID: 814 RVA: 0x00012EB0 File Offset: 0x000110B0
        public Vector4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        // Token: 0x0600032F RID: 815 RVA: 0x00012ECF File Offset: 0x000110CF
        public Vector4(Vector2 value, float z, float w)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
            this.W = w;
        }

        // Token: 0x06000330 RID: 816 RVA: 0x00012EF9 File Offset: 0x000110F9
        public Vector4(Vector3 value, float w)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = value.Z;
            this.W = w;
        }

        // Token: 0x06000331 RID: 817 RVA: 0x00012F2C File Offset: 0x0001112C
        public Vector4(float value)
        {
            this.W = value;
            this.Z = value;
            this.Y = value;
            this.X = value;
        }

        // Token: 0x1700016D RID: 365
        public unsafe float this[int index] {
            get {
                fixed (float* ptr = &this.X)
                {
                    return ptr[index];
                }
            }
            set {
                fixed (float* ptr = &this.X)
                {
                    ptr[index] = value;
                }
            }
        }

        // Token: 0x06000334 RID: 820 RVA: 0x00012F9C File Offset: 0x0001119C
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{0}, {1}, {2}, {3}", new object[]
            {
                this.X.ToString(currentCulture),
                this.Y.ToString(currentCulture),
                this.Z.ToString(currentCulture),
                this.W.ToString(currentCulture)
            });
        }

        // Token: 0x06000335 RID: 821 RVA: 0x00012FFE File Offset: 0x000111FE
        public bool Equals(Vector4 other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
        }

        // Token: 0x06000336 RID: 822 RVA: 0x00013040 File Offset: 0x00011240
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Vector4)
            {
                result = this.Equals((Vector4)obj);
            }
            return result;
        }

        // Token: 0x06000337 RID: 823 RVA: 0x00013065 File Offset: 0x00011265
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode() + this.W.GetHashCode();
        }

        // Token: 0x06000338 RID: 824 RVA: 0x00013098 File Offset: 0x00011298
        public float Length()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
            return (float)Math.Sqrt((double)num);
        }

        // Token: 0x06000339 RID: 825 RVA: 0x000130E5 File Offset: 0x000112E5
        public float LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
        }

        // Token: 0x0600033A RID: 826 RVA: 0x00013120 File Offset: 0x00011320
        public static float Distance(Vector4 value1, Vector4 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = value1.W - value2.W;
            float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
            return (float)Math.Sqrt((double)num5);
        }

        // Token: 0x0600033B RID: 827 RVA: 0x00013188 File Offset: 0x00011388
        public static void Distance(ref Vector4 value1, ref Vector4 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = value1.W - value2.W;
            float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
            result = (float)Math.Sqrt((double)num5);
        }

        // Token: 0x0600033C RID: 828 RVA: 0x000131EC File Offset: 0x000113EC
        public static float DistanceSquared(Vector4 value1, Vector4 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = value1.W - value2.W;
            return num * num + num2 * num2 + num3 * num3 + num4 * num4;
        }

        // Token: 0x0600033D RID: 829 RVA: 0x00013248 File Offset: 0x00011448
        public static void DistanceSquared(ref Vector4 value1, ref Vector4 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = value1.W - value2.W;
            result = num * num + num2 * num2 + num3 * num3 + num4 * num4;
        }

        // Token: 0x0600033E RID: 830 RVA: 0x000132A0 File Offset: 0x000114A0
        public static float Dot(Vector4 vector1, Vector4 vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z + vector1.W * vector2.W;
        }

        // Token: 0x0600033F RID: 831 RVA: 0x000132EC File Offset: 0x000114EC
        public static void Dot(ref Vector4 vector1, ref Vector4 vector2, out float result)
        {
            result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z + vector1.W * vector2.W;
        }

        // Token: 0x06000340 RID: 832 RVA: 0x00013328 File Offset: 0x00011528
        public void Normalize()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
            if (num < Vector4.epsilon)
            {
                return;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            this.X *= num2;
            this.Y *= num2;
            this.Z *= num2;
            this.W *= num2;
        }

        // Token: 0x06000341 RID: 833 RVA: 0x000133C0 File Offset: 0x000115C0
        public static Vector4 Normalize(Vector4 vector)
        {
            float num = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;
            if (num < Vector4.epsilon)
            {
                return vector;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            Vector4 result;
            result.X = vector.X * num2;
            result.Y = vector.Y * num2;
            result.Z = vector.Z * num2;
            result.W = vector.W * num2;
            return result;
        }

        // Token: 0x06000342 RID: 834 RVA: 0x00013468 File Offset: 0x00011668
        public static void Normalize(ref Vector4 vector, out Vector4 result)
        {
            float num = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;
            if (num < Vector4.epsilon)
            {
                result = vector;
                return;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            result.X = vector.X * num2;
            result.Y = vector.Y * num2;
            result.Z = vector.Z * num2;
            result.W = vector.W * num2;
        }

        // Token: 0x06000343 RID: 835 RVA: 0x0001350C File Offset: 0x0001170C
        public static Vector4 Min(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
            result.W = ((value1.W < value2.W) ? value1.W : value2.W);
            return result;
        }

        // Token: 0x06000344 RID: 836 RVA: 0x000135B8 File Offset: 0x000117B8
        public static void Min(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
            result.W = ((value1.W < value2.W) ? value1.W : value2.W);
        }

        // Token: 0x06000345 RID: 837 RVA: 0x00013650 File Offset: 0x00011850
        public static Vector4 Max(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
            result.W = ((value1.W > value2.W) ? value1.W : value2.W);
            return result;
        }

        // Token: 0x06000346 RID: 838 RVA: 0x000136FC File Offset: 0x000118FC
        public static void Max(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
            result.W = ((value1.W > value2.W) ? value1.W : value2.W);
        }

        // Token: 0x06000347 RID: 839 RVA: 0x00013794 File Offset: 0x00011994
        public static Vector4 Clamp(Vector4 value1, Vector4 min, Vector4 max)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            float num3 = value1.Z;
            num3 = ((num3 > max.Z) ? max.Z : num3);
            num3 = ((num3 < min.Z) ? min.Z : num3);
            float num4 = value1.W;
            num4 = ((num4 > max.W) ? max.W : num4);
            num4 = ((num4 < min.W) ? min.W : num4);
            Vector4 result;
            result.X = num;
            result.Y = num2;
            result.Z = num3;
            result.W = num4;
            return result;
        }

        // Token: 0x06000348 RID: 840 RVA: 0x00013894 File Offset: 0x00011A94
        public static void Clamp(ref Vector4 value1, ref Vector4 min, ref Vector4 max, out Vector4 result)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            float num3 = value1.Z;
            num3 = ((num3 > max.Z) ? max.Z : num3);
            num3 = ((num3 < min.Z) ? min.Z : num3);
            float num4 = value1.W;
            num4 = ((num4 > max.W) ? max.W : num4);
            num4 = ((num4 < min.W) ? min.W : num4);
            result.X = num;
            result.Y = num2;
            result.Z = num3;
            result.W = num4;
        }

        // Token: 0x06000349 RID: 841 RVA: 0x00013974 File Offset: 0x00011B74
        public static Vector4 Lerp(Vector4 value1, Vector4 value2, float amount)
        {
            Vector4 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            result.W = value1.W + (value2.W - value1.W) * amount;
            return result;
        }

        // Token: 0x0600034A RID: 842 RVA: 0x00013A04 File Offset: 0x00011C04
        public static void Lerp(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            result.W = value1.W + (value2.W - value1.W) * amount;
        }

        // Token: 0x0600034B RID: 843 RVA: 0x00013A84 File Offset: 0x00011C84
        public static Vector4 SmoothStep(Vector4 value1, Vector4 value2, float amount)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            Vector4 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            result.W = value1.W + (value2.W - value1.W) * amount;
            return result;
        }

        // Token: 0x0600034C RID: 844 RVA: 0x00013B48 File Offset: 0x00011D48
        public static void SmoothStep(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            result.W = value1.W + (value2.W - value1.W) * amount;
        }

        // Token: 0x0600034D RID: 845 RVA: 0x00013BFC File Offset: 0x00011DFC
        public static Vector4 Hermite(Vector4 value1, Vector4 tangent1, Vector4 value2, Vector4 tangent2, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            Vector4 result;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
            result.W = value1.W * num3 + value2.W * num4 + tangent1.W * num5 + tangent2.W * num6;
            return result;
        }

        // Token: 0x0600034E RID: 846 RVA: 0x00013D14 File Offset: 0x00011F14
        public static void Hermite(ref Vector4 value1, ref Vector4 tangent1, ref Vector4 value2, ref Vector4 tangent2, float amount, out Vector4 result)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
            result.W = value1.W * num3 + value2.W * num4 + tangent1.W * num5 + tangent2.W * num6;
        }

        // Token: 0x0600034F RID: 847 RVA: 0x00013E15 File Offset: 0x00012015
        public static Vector4 Project(Vector4 vector, Vector4 onNormal)
        {
            return onNormal * Vector4.Dot(vector, onNormal) / Vector4.Dot(onNormal, onNormal);
        }

        // Token: 0x06000350 RID: 848 RVA: 0x00013E30 File Offset: 0x00012030
        public static void Project(ref Vector4 vector, ref Vector4 onNormal, out Vector4 result)
        {
            result = onNormal * Vector4.Dot(vector, onNormal) / Vector4.Dot(onNormal, onNormal);
        }

        // Token: 0x06000351 RID: 849 RVA: 0x00013E6C File Offset: 0x0001206C
        public static Vector4 Negate(Vector4 value)
        {
            Vector4 result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
            return result;
        }

        // Token: 0x06000352 RID: 850 RVA: 0x00013EB6 File Offset: 0x000120B6
        public static void Negate(ref Vector4 value, out Vector4 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
        }

        // Token: 0x06000353 RID: 851 RVA: 0x00013EEC File Offset: 0x000120EC
        public static Vector4 Add(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            result.W = value1.W + value2.W;
            return result;
        }

        // Token: 0x06000354 RID: 852 RVA: 0x00013F54 File Offset: 0x00012154
        public static void Add(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            result.W = value1.W + value2.W;
        }

        // Token: 0x06000355 RID: 853 RVA: 0x00013FB0 File Offset: 0x000121B0
        public static Vector4 Sub(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            result.W = value1.W - value2.W;
            return result;
        }

        // Token: 0x06000356 RID: 854 RVA: 0x00014018 File Offset: 0x00012218
        public static void Sub(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            result.W = value1.W - value2.W;
        }

        // Token: 0x06000357 RID: 855 RVA: 0x00014074 File Offset: 0x00012274
        public static Vector4 Multiply(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            result.W = value1.W * value2.W;
            return result;
        }

        // Token: 0x06000358 RID: 856 RVA: 0x000140DC File Offset: 0x000122DC
        public static void Multiply(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            result.W = value1.W * value2.W;
        }

        // Token: 0x06000359 RID: 857 RVA: 0x00014138 File Offset: 0x00012338
        public static Vector4 Multiply(Vector4 value1, float scaleFactor)
        {
            Vector4 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            result.W = value1.W * scaleFactor;
            return result;
        }

        // Token: 0x0600035A RID: 858 RVA: 0x00014186 File Offset: 0x00012386
        public static void Multiply(ref Vector4 value1, float scaleFactor, out Vector4 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            result.W = value1.W * scaleFactor;
        }

        // Token: 0x0600035B RID: 859 RVA: 0x000141C0 File Offset: 0x000123C0
        public static Vector4 Divide(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
            result.W = value1.W / value2.W;
            return result;
        }

        // Token: 0x0600035C RID: 860 RVA: 0x00014228 File Offset: 0x00012428
        public static void Divide(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
            result.W = value1.W / value2.W;
        }

        // Token: 0x0600035D RID: 861 RVA: 0x00014284 File Offset: 0x00012484
        public static Vector4 Divide(Vector4 value1, float divider)
        {
            float num = 1f / divider;
            Vector4 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
            result.W = value1.W * num;
            return result;
        }

        // Token: 0x0600035E RID: 862 RVA: 0x000142DC File Offset: 0x000124DC
        public static void Divide(ref Vector4 value1, float divider, out Vector4 result)
        {
            float num = 1f / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
            result.W = value1.W * num;
        }

        // Token: 0x0600035F RID: 863 RVA: 0x0001432C File Offset: 0x0001252C
        public static Vector4 operator -(Vector4 value)
        {
            Vector4 result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
            return result;
        }

        // Token: 0x06000360 RID: 864 RVA: 0x00014378 File Offset: 0x00012578
        public static bool operator ==(Vector4 value1, Vector4 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z && value1.W == value2.W;
        }

        // Token: 0x06000361 RID: 865 RVA: 0x000143C8 File Offset: 0x000125C8
        public static bool operator !=(Vector4 value1, Vector4 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z || value1.W != value2.W;
        }

        // Token: 0x06000362 RID: 866 RVA: 0x0001441C File Offset: 0x0001261C
        public static Vector4 operator +(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            result.W = value1.W + value2.W;
            return result;
        }

        // Token: 0x06000363 RID: 867 RVA: 0x00014484 File Offset: 0x00012684
        public static Vector4 operator -(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            result.W = value1.W - value2.W;
            return result;
        }

        // Token: 0x06000364 RID: 868 RVA: 0x000144EC File Offset: 0x000126EC
        public static Vector4 operator *(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            result.W = value1.W * value2.W;
            return result;
        }

        // Token: 0x06000365 RID: 869 RVA: 0x00014554 File Offset: 0x00012754
        public static Vector4 operator *(Vector4 value1, float scaleFactor)
        {
            Vector4 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            result.W = value1.W * scaleFactor;
            return result;
        }

        // Token: 0x06000366 RID: 870 RVA: 0x000145A4 File Offset: 0x000127A4
        public static Vector4 operator *(float scaleFactor, Vector4 value1)
        {
            Vector4 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            result.W = value1.W * scaleFactor;
            return result;
        }

        // Token: 0x06000367 RID: 871 RVA: 0x000145F4 File Offset: 0x000127F4
        public static Vector4 operator /(Vector4 value1, Vector4 value2)
        {
            Vector4 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
            result.W = value1.W / value2.W;
            return result;
        }

        // Token: 0x06000368 RID: 872 RVA: 0x0001465C File Offset: 0x0001285C
        public static Vector4 operator /(Vector4 value1, float divider)
        {
            float num = 1f / divider;
            Vector4 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
            result.W = value1.W * num;
            return result;
        }

        // Token: 0x06000369 RID: 873 RVA: 0x000146B4 File Offset: 0x000128B4
        static Vector4()
        {
            Vector4._zero = default(Vector4);
            Vector4._one = new Vector4(1f, 1f, 1f, 1f);
            Vector4._unitX = new Vector4(1f, 0f, 0f, 0f);
            Vector4._unitY = new Vector4(0f, 1f, 0f, 0f);
            Vector4._unitZ = new Vector4(0f, 0f, 1f, 0f);
            Vector4._unitW = new Vector4(0f, 0f, 0f, 1f);
        }

        // Token: 0x04000067 RID: 103
        public float X;

        // Token: 0x04000068 RID: 104
        public float Y;

        // Token: 0x04000069 RID: 105
        public float Z;

        // Token: 0x0400006A RID: 106
        public float W;

        // Token: 0x0400006B RID: 107
        private static Vector4 _zero;

        // Token: 0x0400006C RID: 108
        private static Vector4 _one;

        // Token: 0x0400006D RID: 109
        private static Vector4 _unitX;

        // Token: 0x0400006E RID: 110
        private static Vector4 _unitY;

        // Token: 0x0400006F RID: 111
        private static Vector4 _unitZ;

        // Token: 0x04000070 RID: 112
        private static Vector4 _unitW;

        // Token: 0x04000071 RID: 113
        private static readonly float epsilon = 1E-05f;
    }
}
