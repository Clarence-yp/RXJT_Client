using System;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x0200000F RID: 15
    [Serializable]
    public struct Quaternion : IEquatable<Quaternion>
    {
        // Token: 0x17000155 RID: 341
        // (get) Token: 0x0600025D RID: 605 RVA: 0x0000E588 File Offset: 0x0000C788
        public static Quaternion Identity {
            get {
                return Quaternion._identity;
            }
        }

        // Token: 0x0600025E RID: 606 RVA: 0x0000E58F File Offset: 0x0000C78F
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        // Token: 0x0600025F RID: 607 RVA: 0x0000E5B0 File Offset: 0x0000C7B0
        public Quaternion(float angle, Vector3 rkAxis)
        {
            float num = angle * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float w = (float)Math.Cos((double)num);
            this.X = rkAxis.X * num2;
            this.Y = rkAxis.Y * num2;
            this.Z = rkAxis.Z * num2;
            this.W = w;
        }

        // Token: 0x06000260 RID: 608 RVA: 0x0000E60C File Offset: 0x0000C80C
        public Quaternion(Vector3 xaxis, Vector3 yaxis, Vector3 zaxis)
        {
            Matrix44 identity = Matrix44.Identity;
            identity[0, 0] = xaxis.X;
            identity[1, 0] = xaxis.Y;
            identity[2, 0] = xaxis.Z;
            identity[0, 1] = yaxis.X;
            identity[1, 1] = yaxis.Y;
            identity[2, 1] = yaxis.Z;
            identity[0, 2] = zaxis.X;
            identity[1, 2] = zaxis.Y;
            identity[2, 2] = zaxis.Z;
            Quaternion.CreateFromRotationMatrix(ref identity, out this);
        }

        // Token: 0x06000261 RID: 609 RVA: 0x0000E6B8 File Offset: 0x0000C8B8
        public Quaternion(float yaw, float pitch, float roll)
        {
            float num = roll * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float num3 = (float)Math.Cos((double)num);
            float num4 = pitch * 0.5f;
            float num5 = (float)Math.Sin((double)num4);
            float num6 = (float)Math.Cos((double)num4);
            float num7 = yaw * 0.5f;
            float num8 = (float)Math.Sin((double)num7);
            float num9 = (float)Math.Cos((double)num7);
            this.X = num9 * num5 * num3 + num8 * num6 * num2;
            this.Y = num8 * num6 * num3 - num9 * num5 * num2;
            this.Z = num9 * num6 * num2 - num8 * num5 * num3;
            this.W = num9 * num6 * num3 + num8 * num5 * num2;
        }

        // Token: 0x06000262 RID: 610 RVA: 0x0000E770 File Offset: 0x0000C970
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

        // Token: 0x06000263 RID: 611 RVA: 0x0000E7D2 File Offset: 0x0000C9D2
        public bool Equals(Quaternion other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
        }

        // Token: 0x06000264 RID: 612 RVA: 0x0000E814 File Offset: 0x0000CA14
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Quaternion)
            {
                result = this.Equals((Quaternion)obj);
            }
            return result;
        }

        // Token: 0x06000265 RID: 613 RVA: 0x0000E839 File Offset: 0x0000CA39
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode() + this.W.GetHashCode();
        }

        // Token: 0x06000266 RID: 614 RVA: 0x0000E86A File Offset: 0x0000CA6A
        public float LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
        }

        // Token: 0x06000267 RID: 615 RVA: 0x0000E8A4 File Offset: 0x0000CAA4
        public float Length()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
            return (float)Math.Sqrt((double)num);
        }

        // Token: 0x06000268 RID: 616 RVA: 0x0000E8F4 File Offset: 0x0000CAF4
        public void Normalize()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            this.X *= num2;
            this.Y *= num2;
            this.Z *= num2;
            this.W *= num2;
        }

        // Token: 0x06000269 RID: 617 RVA: 0x0000E980 File Offset: 0x0000CB80
        public static Quaternion Normalize(Quaternion quaternion)
        {
            float num = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            Quaternion result;
            result.X = quaternion.X * num2;
            result.Y = quaternion.Y * num2;
            result.Z = quaternion.Z * num2;
            result.W = quaternion.W * num2;
            return result;
        }

        // Token: 0x0600026A RID: 618 RVA: 0x0000EA20 File Offset: 0x0000CC20
        public static void Normalize(ref Quaternion quaternion, out Quaternion result)
        {
            float num = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            result.X = quaternion.X * num2;
            result.Y = quaternion.Y * num2;
            result.Z = quaternion.Z * num2;
            result.W = quaternion.W * num2;
        }

        // Token: 0x0600026B RID: 619 RVA: 0x0000EAAC File Offset: 0x0000CCAC
        public static Quaternion Inverse(Quaternion quaternion)
        {
            float num = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
            float num2 = 1f / num;
            Quaternion result;
            result.X = -quaternion.X * num2;
            result.Y = -quaternion.Y * num2;
            result.Z = -quaternion.Z * num2;
            result.W = quaternion.W * num2;
            return result;
        }

        // Token: 0x0600026C RID: 620 RVA: 0x0000EB48 File Offset: 0x0000CD48
        public static void Inverse(ref Quaternion quaternion, out Quaternion result)
        {
            float num = quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z + quaternion.W * quaternion.W;
            float num2 = 1f / num;
            result.X = -quaternion.X * num2;
            result.Y = -quaternion.Y * num2;
            result.Z = -quaternion.Z * num2;
            result.W = quaternion.W * num2;
        }

        // Token: 0x0600026D RID: 621 RVA: 0x0000EBD0 File Offset: 0x0000CDD0
        public static Quaternion CreateFromAxisAngle(Vector3 axis, float angle)
        {
            float num = angle * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float w = (float)Math.Cos((double)num);
            Quaternion result;
            result.X = axis.X * num2;
            result.Y = axis.Y * num2;
            result.Z = axis.Z * num2;
            result.W = w;
            return result;
        }

        // Token: 0x0600026E RID: 622 RVA: 0x0000EC30 File Offset: 0x0000CE30
        public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Quaternion result)
        {
            float num = angle * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float w = (float)Math.Cos((double)num);
            result.X = axis.X * num2;
            result.Y = axis.Y * num2;
            result.Z = axis.Z * num2;
            result.W = w;
        }

        // Token: 0x0600026F RID: 623 RVA: 0x0000EC88 File Offset: 0x0000CE88
        public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            float num = roll * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float num3 = (float)Math.Cos((double)num);
            float num4 = pitch * 0.5f;
            float num5 = (float)Math.Sin((double)num4);
            float num6 = (float)Math.Cos((double)num4);
            float num7 = yaw * 0.5f;
            float num8 = (float)Math.Sin((double)num7);
            float num9 = (float)Math.Cos((double)num7);
            Quaternion result;
            result.X = num9 * num5 * num3 + num8 * num6 * num2;
            result.Y = num8 * num6 * num3 - num9 * num5 * num2;
            result.Z = num9 * num6 * num2 - num8 * num5 * num3;
            result.W = num9 * num6 * num3 + num8 * num5 * num2;
            return result;
        }

        // Token: 0x06000270 RID: 624 RVA: 0x0000ED48 File Offset: 0x0000CF48
        public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Quaternion result)
        {
            float num = roll * 0.5f;
            float num2 = (float)Math.Sin((double)num);
            float num3 = (float)Math.Cos((double)num);
            float num4 = pitch * 0.5f;
            float num5 = (float)Math.Sin((double)num4);
            float num6 = (float)Math.Cos((double)num4);
            float num7 = yaw * 0.5f;
            float num8 = (float)Math.Sin((double)num7);
            float num9 = (float)Math.Cos((double)num7);
            result.X = num9 * num5 * num3 + num8 * num6 * num2;
            result.Y = num8 * num6 * num3 - num9 * num5 * num2;
            result.Z = num9 * num6 * num2 - num8 * num5 * num3;
            result.W = num9 * num6 * num3 + num8 * num5 * num2;
        }

        // Token: 0x06000271 RID: 625 RVA: 0x0000EE00 File Offset: 0x0000D000
        public static Quaternion CreateFromRotationMatrix(Matrix44 matrix)
        {
            float num = matrix.M00 + matrix.M11 + matrix.M22;
            Quaternion result = default(Quaternion);
            if (num > 0f)
            {
                float num2 = (float)Math.Sqrt((double)(num + 1f));
                result.W = num2 * 0.5f;
                num2 = 0.5f / num2;
                result.X = (matrix.M21 - matrix.M12) * num2;
                result.Y = (matrix.M02 - matrix.M20) * num2;
                result.Z = (matrix.M10 - matrix.M01) * num2;
                return result;
            }
            if (matrix.M00 >= matrix.M11 && matrix.M00 >= matrix.M22)
            {
                float num3 = (float)Math.Sqrt((double)(1f + matrix.M00 - matrix.M11 - matrix.M22));
                float num4 = 0.5f / num3;
                result.X = 0.5f * num3;
                result.Y = (matrix.M10 + matrix.M01) * num4;
                result.Z = (matrix.M20 + matrix.M02) * num4;
                result.W = (matrix.M21 - matrix.M12) * num4;
                return result;
            }
            if (matrix.M11 > matrix.M22)
            {
                float num5 = (float)Math.Sqrt((double)(1f + matrix.M11 - matrix.M00 - matrix.M22));
                float num6 = 0.5f / num5;
                result.X = (matrix.M01 + matrix.M10) * num6;
                result.Y = 0.5f * num5;
                result.Z = (matrix.M12 + matrix.M21) * num6;
                result.W = (matrix.M02 - matrix.M20) * num6;
                return result;
            }
            float num7 = (float)Math.Sqrt((double)(1f + matrix.M22 - matrix.M00 - matrix.M11));
            float num8 = 0.5f / num7;
            result.X = (matrix.M02 + matrix.M20) * num8;
            result.Y = (matrix.M12 + matrix.M21) * num8;
            result.Z = 0.5f * num7;
            result.W = (matrix.M10 - matrix.M01) * num8;
            return result;
        }

        // Token: 0x06000272 RID: 626 RVA: 0x0000F080 File Offset: 0x0000D280
        public static void CreateFromRotationMatrix(ref Matrix44 matrix, out Quaternion result)
        {
            float num = matrix.M00 + matrix.M11 + matrix.M22;
            if (num > 0f)
            {
                float num2 = (float)Math.Sqrt((double)(num + 1f));
                result.W = num2 * 0.5f;
                num2 = 0.5f / num2;
                result.X = (matrix.M21 - matrix.M12) * num2;
                result.Y = (matrix.M02 - matrix.M20) * num2;
                result.Z = (matrix.M10 - matrix.M01) * num2;
                return;
            }
            if (matrix.M00 >= matrix.M11 && matrix.M00 >= matrix.M22)
            {
                float num3 = (float)Math.Sqrt((double)(1f + matrix.M00 - matrix.M11 - matrix.M22));
                float num4 = 0.5f / num3;
                result.X = 0.5f * num3;
                result.Y = (matrix.M10 + matrix.M01) * num4;
                result.Z = (matrix.M20 + matrix.M02) * num4;
                result.W = (matrix.M21 - matrix.M12) * num4;
                return;
            }
            if (matrix.M11 > matrix.M22)
            {
                float num5 = (float)Math.Sqrt((double)(1f + matrix.M11 - matrix.M00 - matrix.M22));
                float num6 = 0.5f / num5;
                result.X = (matrix.M01 + matrix.M10) * num6;
                result.Y = 0.5f * num5;
                result.Z = (matrix.M12 + matrix.M21) * num6;
                result.W = (matrix.M02 - matrix.M20) * num6;
                return;
            }
            float num7 = (float)Math.Sqrt((double)(1f + matrix.M22 - matrix.M00 - matrix.M11));
            float num8 = 0.5f / num7;
            result.X = (matrix.M02 + matrix.M20) * num8;
            result.Y = (matrix.M12 + matrix.M21) * num8;
            result.Z = 0.5f * num7;
            result.W = (matrix.M10 - matrix.M01) * num8;
        }

        // Token: 0x06000273 RID: 627 RVA: 0x0000F2B0 File Offset: 0x0000D4B0
        public static float Dot(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        }

        // Token: 0x06000274 RID: 628 RVA: 0x0000F2FC File Offset: 0x0000D4FC
        public static void Dot(ref Quaternion quaternion1, ref Quaternion quaternion2, out float result)
        {
            result = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
        }

        // Token: 0x06000275 RID: 629 RVA: 0x0000F338 File Offset: 0x0000D538
        public static Quaternion Slerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            float num = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            bool flag = false;
            if (num < 0f)
            {
                flag = true;
                num = -num;
            }
            float num2;
            float num3;
            if (num > 0.999999f)
            {
                num2 = 1f - amount;
                num3 = (flag ? (-amount) : amount);
            }
            else
            {
                float num4 = (float)Math.Acos((double)num);
                float num5 = (float)(1.0 / Math.Sin((double)num4));
                num2 = (float)Math.Sin((double)((1f - amount) * num4)) * num5;
                num3 = (flag ? ((float)(-(float)Math.Sin((double)(amount * num4))) * num5) : ((float)Math.Sin((double)(amount * num4)) * num5));
            }
            Quaternion result;
            result.X = num2 * quaternion1.X + num3 * quaternion2.X;
            result.Y = num2 * quaternion1.Y + num3 * quaternion2.Y;
            result.Z = num2 * quaternion1.Z + num3 * quaternion2.Z;
            result.W = num2 * quaternion1.W + num3 * quaternion2.W;
            return result;
        }

        // Token: 0x06000276 RID: 630 RVA: 0x0000F47C File Offset: 0x0000D67C
        public static void Slerp(ref Quaternion quaternion1, ref Quaternion quaternion2, float amount, out Quaternion result)
        {
            float num = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            bool flag = false;
            if (num < 0f)
            {
                flag = true;
                num = -num;
            }
            float num2;
            float num3;
            if (num > 0.999999f)
            {
                num2 = 1f - amount;
                num3 = (flag ? (-amount) : amount);
            }
            else
            {
                float num4 = (float)Math.Acos((double)num);
                float num5 = (float)(1.0 / Math.Sin((double)num4));
                num2 = (float)Math.Sin((double)((1f - amount) * num4)) * num5;
                num3 = (flag ? ((float)(-(float)Math.Sin((double)(amount * num4))) * num5) : ((float)Math.Sin((double)(amount * num4)) * num5));
            }
            result.X = num2 * quaternion1.X + num3 * quaternion2.X;
            result.Y = num2 * quaternion1.Y + num3 * quaternion2.Y;
            result.Z = num2 * quaternion1.Z + num3 * quaternion2.Z;
            result.W = num2 * quaternion1.W + num3 * quaternion2.W;
        }

        // Token: 0x06000277 RID: 631 RVA: 0x0000F5A4 File Offset: 0x0000D7A4
        public static Quaternion Lerp(Quaternion quaternion1, Quaternion quaternion2, float amount)
        {
            float num = 1f - amount;
            Quaternion result = default(Quaternion);
            float num2 = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            if (num2 >= 0f)
            {
                result.X = num * quaternion1.X + amount * quaternion2.X;
                result.Y = num * quaternion1.Y + amount * quaternion2.Y;
                result.Z = num * quaternion1.Z + amount * quaternion2.Z;
                result.W = num * quaternion1.W + amount * quaternion2.W;
            }
            else
            {
                result.X = num * quaternion1.X - amount * quaternion2.X;
                result.Y = num * quaternion1.Y - amount * quaternion2.Y;
                result.Z = num * quaternion1.Z - amount * quaternion2.Z;
                result.W = num * quaternion1.W - amount * quaternion2.W;
            }
            float num3 = result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W;
            float num4 = 1f / (float)Math.Sqrt((double)num3);
            result.X *= num4;
            result.Y *= num4;
            result.Z *= num4;
            result.W *= num4;
            return result;
        }

        // Token: 0x06000278 RID: 632 RVA: 0x0000F770 File Offset: 0x0000D970
        public static void Lerp(ref Quaternion quaternion1, ref Quaternion quaternion2, float amount, out Quaternion result)
        {
            float num = 1f - amount;
            float num2 = quaternion1.X * quaternion2.X + quaternion1.Y * quaternion2.Y + quaternion1.Z * quaternion2.Z + quaternion1.W * quaternion2.W;
            if (num2 >= 0f)
            {
                result.X = num * quaternion1.X + amount * quaternion2.X;
                result.Y = num * quaternion1.Y + amount * quaternion2.Y;
                result.Z = num * quaternion1.Z + amount * quaternion2.Z;
                result.W = num * quaternion1.W + amount * quaternion2.W;
            }
            else
            {
                result.X = num * quaternion1.X - amount * quaternion2.X;
                result.Y = num * quaternion1.Y - amount * quaternion2.Y;
                result.Z = num * quaternion1.Z - amount * quaternion2.Z;
                result.W = num * quaternion1.W - amount * quaternion2.W;
            }
            float num3 = result.X * result.X + result.Y * result.Y + result.Z * result.Z + result.W * result.W;
            float num4 = 1f / (float)Math.Sqrt((double)num3);
            result.X *= num4;
            result.Y *= num4;
            result.Z *= num4;
            result.W *= num4;
        }

        // Token: 0x06000279 RID: 633 RVA: 0x0000F905 File Offset: 0x0000DB05
        public void Conjugate()
        {
            this.X = -this.X;
            this.Y = -this.Y;
            this.Z = -this.Z;
        }

        // Token: 0x0600027A RID: 634 RVA: 0x0000F930 File Offset: 0x0000DB30
        public static Quaternion Conjugate(Quaternion value)
        {
            Quaternion result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = value.W;
            return result;
        }

        // Token: 0x0600027B RID: 635 RVA: 0x0000F979 File Offset: 0x0000DB79
        public static void Conjugate(ref Quaternion value, out Quaternion result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = value.W;
        }

        // Token: 0x0600027C RID: 636 RVA: 0x0000F9AE File Offset: 0x0000DBAE
        private static float Angle(Quaternion a, Quaternion b)
        {
            return (float)Math.Acos((double)Math.Min(Math.Abs(Quaternion.Dot(a, b)), 1f)) * 2f * 57.29578f;
        }

        // Token: 0x0600027D RID: 637 RVA: 0x0000F9DC File Offset: 0x0000DBDC
        private static void Angle(ref Quaternion a, ref Quaternion b, out float result)
        {
            result = (float)Math.Acos((double)Math.Min(Math.Abs(Quaternion.Dot(a, b)), 1f)) * 2f * 57.29578f;
        }

        // Token: 0x0600027E RID: 638 RVA: 0x0000FA18 File Offset: 0x0000DC18
        public static Quaternion Negate(Quaternion quaternion)
        {
            Quaternion result;
            result.X = -quaternion.X;
            result.Y = -quaternion.Y;
            result.Z = -quaternion.Z;
            result.W = -quaternion.W;
            return result;
        }

        // Token: 0x0600027F RID: 639 RVA: 0x0000FA62 File Offset: 0x0000DC62
        public static void Negate(ref Quaternion quaternion, out Quaternion result)
        {
            result.X = -quaternion.X;
            result.Y = -quaternion.Y;
            result.Z = -quaternion.Z;
            result.W = -quaternion.W;
        }

        // Token: 0x06000280 RID: 640 RVA: 0x0000FA98 File Offset: 0x0000DC98
        public static Quaternion Sub(Quaternion quaternion1, Quaternion quaternion2)
        {
            Quaternion result;
            result.X = quaternion1.X - quaternion2.X;
            result.Y = quaternion1.Y - quaternion2.Y;
            result.Z = quaternion1.Z - quaternion2.Z;
            result.W = quaternion1.W - quaternion2.W;
            return result;
        }

        // Token: 0x06000281 RID: 641 RVA: 0x0000FB00 File Offset: 0x0000DD00
        public static void Sub(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            result.X = quaternion1.X - quaternion2.X;
            result.Y = quaternion1.Y - quaternion2.Y;
            result.Z = quaternion1.Z - quaternion2.Z;
            result.W = quaternion1.W - quaternion2.W;
        }

        // Token: 0x06000282 RID: 642 RVA: 0x0000FB5C File Offset: 0x0000DD5C
        public static Vector3 Rotate(Quaternion rotation, Vector3 vector3)
        {
            float num = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            Vector3 result;
            result.X = (1f - (num5 + num6)) * vector3.X + (num7 - num12) * vector3.Y + (num8 + num11) * vector3.Z;
            result.Y = (num7 + num12) * vector3.X + (1f - (num4 + num6)) * vector3.Y + (num9 - num10) * vector3.Z;
            result.Z = (num8 - num11) * vector3.X + (num9 + num10) * vector3.Y + (1f - (num4 + num5)) * vector3.Z;
            return result;
        }

        // Token: 0x06000283 RID: 643 RVA: 0x0000FC98 File Offset: 0x0000DE98
        public static void Rotate(ref Quaternion rotation, ref Vector3 vector3, out Vector3 result)
        {
            float num = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            result.X = (1f - (num5 + num6)) * vector3.X + (num7 - num12) * vector3.Y + (num8 + num11) * vector3.Z;
            result.Y = (num7 + num12) * vector3.X + (1f - (num4 + num6)) * vector3.Y + (num9 - num10) * vector3.Z;
            result.Z = (num8 - num11) * vector3.X + (num9 + num10) * vector3.Y + (1f - (num4 + num5)) * vector3.Z;
        }

        // Token: 0x06000284 RID: 644 RVA: 0x0000FDBC File Offset: 0x0000DFBC
        public static Quaternion Multiply(Quaternion quaternion1, Quaternion quaternion2)
        {
            float x = quaternion1.X;
            float y = quaternion1.Y;
            float z = quaternion1.Z;
            float w = quaternion1.W;
            float x2 = quaternion2.X;
            float y2 = quaternion2.Y;
            float z2 = quaternion2.Z;
            float w2 = quaternion2.W;
            float num = y * z2 - z * y2;
            float num2 = z * x2 - x * z2;
            float num3 = x * y2 - y * x2;
            float num4 = x * x2 + y * y2 + z * z2;
            Quaternion result;
            result.X = x * w2 + x2 * w + num;
            result.Y = y * w2 + y2 * w + num2;
            result.Z = z * w2 + z2 * w + num3;
            result.W = w * w2 - num4;
            return result;
        }

        // Token: 0x06000285 RID: 645 RVA: 0x0000FE8C File Offset: 0x0000E08C
        public static void Multiply(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
        {
            float x = quaternion1.X;
            float y = quaternion1.Y;
            float z = quaternion1.Z;
            float w = quaternion1.W;
            float x2 = quaternion2.X;
            float y2 = quaternion2.Y;
            float z2 = quaternion2.Z;
            float w2 = quaternion2.W;
            float num = y * z2 - z * y2;
            float num2 = z * x2 - x * z2;
            float num3 = x * y2 - y * x2;
            float num4 = x * x2 + y * y2 + z * z2;
            result.X = x * w2 + x2 * w + num;
            result.Y = y * w2 + y2 * w + num2;
            result.Z = z * w2 + z2 * w + num3;
            result.W = w * w2 - num4;
        }

        // Token: 0x06000286 RID: 646 RVA: 0x0000FF4C File Offset: 0x0000E14C
        public static Quaternion operator -(Quaternion quaternion)
        {
            Quaternion result;
            result.X = -quaternion.X;
            result.Y = -quaternion.Y;
            result.Z = -quaternion.Z;
            result.W = -quaternion.W;
            return result;
        }

        // Token: 0x06000287 RID: 647 RVA: 0x0000FF98 File Offset: 0x0000E198
        public static bool operator ==(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1.X == quaternion2.X && quaternion1.Y == quaternion2.Y && quaternion1.Z == quaternion2.Z && quaternion1.W == quaternion2.W;
        }

        // Token: 0x06000288 RID: 648 RVA: 0x0000FFE8 File Offset: 0x0000E1E8
        public static bool operator !=(Quaternion quaternion1, Quaternion quaternion2)
        {
            return quaternion1.X != quaternion2.X || quaternion1.Y != quaternion2.Y || quaternion1.Z != quaternion2.Z || quaternion1.W != quaternion2.W;
        }

        // Token: 0x06000289 RID: 649 RVA: 0x0001003C File Offset: 0x0000E23C
        public static Quaternion operator -(Quaternion quaternion1, Quaternion quaternion2)
        {
            Quaternion result;
            result.X = quaternion1.X - quaternion2.X;
            result.Y = quaternion1.Y - quaternion2.Y;
            result.Z = quaternion1.Z - quaternion2.Z;
            result.W = quaternion1.W - quaternion2.W;
            return result;
        }

        // Token: 0x0600028A RID: 650 RVA: 0x000100A4 File Offset: 0x0000E2A4
        public static Quaternion operator *(Quaternion quaternion1, Quaternion quaternion2)
        {
            float x = quaternion1.X;
            float y = quaternion1.Y;
            float z = quaternion1.Z;
            float w = quaternion1.W;
            float x2 = quaternion2.X;
            float y2 = quaternion2.Y;
            float z2 = quaternion2.Z;
            float w2 = quaternion2.W;
            float num = y * z2 - z * y2;
            float num2 = z * x2 - x * z2;
            float num3 = x * y2 - y * x2;
            float num4 = x * x2 + y * y2 + z * z2;
            Quaternion result;
            result.X = x * w2 + x2 * w + num;
            result.Y = y * w2 + y2 * w + num2;
            result.Z = z * w2 + z2 * w + num3;
            result.W = w * w2 - num4;
            return result;
        }

        // Token: 0x04000049 RID: 73
        public float W;

        // Token: 0x0400004A RID: 74
        public float X;

        // Token: 0x0400004B RID: 75
        public float Y;

        // Token: 0x0400004C RID: 76
        public float Z;

        // Token: 0x0400004D RID: 77
        private static Quaternion _identity = new Quaternion(0f, 0f, 0f, 1f);
    }
}
