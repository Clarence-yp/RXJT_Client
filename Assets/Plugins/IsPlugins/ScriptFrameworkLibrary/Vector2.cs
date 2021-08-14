using System;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x02000011 RID: 17
    [Serializable]
    public struct Vector2 : IEquatable<Vector2>
    {
        // Token: 0x17000156 RID: 342
        // (get) Token: 0x0600029A RID: 666 RVA: 0x00010705 File Offset: 0x0000E905
        public static Vector2 Zero {
            get {
                return Vector2._zero;
            }
        }

        // Token: 0x17000157 RID: 343
        // (get) Token: 0x0600029B RID: 667 RVA: 0x0001070C File Offset: 0x0000E90C
        public static Vector2 One {
            get {
                return Vector2._one;
            }
        }

        // Token: 0x17000158 RID: 344
        // (get) Token: 0x0600029C RID: 668 RVA: 0x00010713 File Offset: 0x0000E913
        public static Vector2 UnitX {
            get {
                return Vector2._unitX;
            }
        }

        // Token: 0x17000159 RID: 345
        // (get) Token: 0x0600029D RID: 669 RVA: 0x0001071A File Offset: 0x0000E91A
        public static Vector2 UnitY {
            get {
                return Vector2._unitY;
            }
        }

        // Token: 0x0600029E RID: 670 RVA: 0x00010721 File Offset: 0x0000E921
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        // Token: 0x0600029F RID: 671 RVA: 0x00010734 File Offset: 0x0000E934
        public Vector2(float value)
        {
            this.Y = value;
            this.X = value;
        }

        // Token: 0x1700015A RID: 346
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

        // Token: 0x060002A2 RID: 674 RVA: 0x00010794 File Offset: 0x0000E994
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{0}, {1}", new object[]
            {
                this.X.ToString(currentCulture),
                this.Y.ToString(currentCulture)
            });
        }

        // Token: 0x060002A3 RID: 675 RVA: 0x000107D8 File Offset: 0x0000E9D8
        public bool Equals(Vector2 other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        // Token: 0x060002A4 RID: 676 RVA: 0x000107FC File Offset: 0x0000E9FC
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Vector2)
            {
                result = this.Equals((Vector2)obj);
            }
            return result;
        }

        // Token: 0x060002A5 RID: 677 RVA: 0x00010821 File Offset: 0x0000EA21
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }

        // Token: 0x060002A6 RID: 678 RVA: 0x0001083C File Offset: 0x0000EA3C
        public float Length()
        {
            float num = this.X * this.X + this.Y * this.Y;
            return (float)Math.Sqrt((double)num);
        }

        // Token: 0x060002A7 RID: 679 RVA: 0x0001086D File Offset: 0x0000EA6D
        public float LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }

        // Token: 0x060002A8 RID: 680 RVA: 0x0001088C File Offset: 0x0000EA8C
        public static float Distance(Vector2 value1, Vector2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = num * num + num2 * num2;
            return (float)Math.Sqrt((double)num3);
        }

        // Token: 0x060002A9 RID: 681 RVA: 0x000108CC File Offset: 0x0000EACC
        public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = num * num + num2 * num2;
            result = (float)Math.Sqrt((double)num3);
        }

        // Token: 0x060002AA RID: 682 RVA: 0x00010908 File Offset: 0x0000EB08
        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            return num * num + num2 * num2;
        }

        // Token: 0x060002AB RID: 683 RVA: 0x0001093C File Offset: 0x0000EB3C
        public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            result = num * num + num2 * num2;
        }

        // Token: 0x060002AC RID: 684 RVA: 0x00010970 File Offset: 0x0000EB70
        public void Normalize()
        {
            float num = this.X * this.X + this.Y * this.Y;
            if (num < 1E-05f)
            {
                return;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            this.X *= num2;
            this.Y *= num2;
        }

        // Token: 0x060002AD RID: 685 RVA: 0x000109D0 File Offset: 0x0000EBD0
        public static Vector2 Normalize(Vector2 value)
        {
            float num = value.X * value.X + value.Y * value.Y;
            if (num < 1E-05f)
            {
                return value;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            Vector2 result;
            result.X = value.X * num2;
            result.Y = value.Y * num2;
            return result;
        }

        // Token: 0x060002AE RID: 686 RVA: 0x00010A38 File Offset: 0x0000EC38
        public static void Normalize(ref Vector2 value, out Vector2 result)
        {
            float num = value.X * value.X + value.Y * value.Y;
            if (num < 1E-05f)
            {
                result = value;
                return;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            result.X = value.X * num2;
            result.Y = value.Y * num2;
        }

        // Token: 0x060002AF RID: 687 RVA: 0x00010AA4 File Offset: 0x0000ECA4
        public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y;
            Vector2 result;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            return result;
        }

        // Token: 0x060002B0 RID: 688 RVA: 0x00010B10 File Offset: 0x0000ED10
        public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
        }

        // Token: 0x060002B1 RID: 689 RVA: 0x00010B70 File Offset: 0x0000ED70
        public static Vector2 Min(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            return result;
        }

        // Token: 0x060002B2 RID: 690 RVA: 0x00010BCC File Offset: 0x0000EDCC
        public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
        }

        // Token: 0x060002B3 RID: 691 RVA: 0x00010C20 File Offset: 0x0000EE20
        public static Vector2 Max(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            return result;
        }

        // Token: 0x060002B4 RID: 692 RVA: 0x00010C7C File Offset: 0x0000EE7C
        public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
        }

        // Token: 0x060002B5 RID: 693 RVA: 0x00010CD0 File Offset: 0x0000EED0
        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            Vector2 result;
            result.X = num;
            result.Y = num2;
            return result;
        }

        // Token: 0x060002B6 RID: 694 RVA: 0x00010D54 File Offset: 0x0000EF54
        public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            result.X = num;
            result.Y = num2;
        }

        // Token: 0x060002B7 RID: 695 RVA: 0x00010DCC File Offset: 0x0000EFCC
        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            Vector2 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            return result;
        }

        // Token: 0x060002B8 RID: 696 RVA: 0x00010E1A File Offset: 0x0000F01A
        public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
        }

        // Token: 0x060002B9 RID: 697 RVA: 0x00010E54 File Offset: 0x0000F054
        public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            Vector2 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            return result;
        }

        // Token: 0x060002BA RID: 698 RVA: 0x00010ED8 File Offset: 0x0000F0D8
        public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
        }

        // Token: 0x060002BB RID: 699 RVA: 0x00010F54 File Offset: 0x0000F154
        public static Vector2 Negate(Vector2 value)
        {
            Vector2 result;
            result.X = -value.X;
            result.Y = -value.Y;
            return result;
        }

        // Token: 0x060002BC RID: 700 RVA: 0x00010F80 File Offset: 0x0000F180
        public static void Negate(ref Vector2 value, out Vector2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        // Token: 0x060002BD RID: 701 RVA: 0x00010F9C File Offset: 0x0000F19C
        public static float Dot(Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }

        // Token: 0x060002BE RID: 702 RVA: 0x00010FBD File Offset: 0x0000F1BD
        public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            result = value1.X * value2.X + value1.Y * value2.Y;
        }

        // Token: 0x060002BF RID: 703 RVA: 0x00010FDC File Offset: 0x0000F1DC
        public static float Angle(Vector2 from, Vector2 to)
        {
            from.Normalize();
            to.Normalize();
            float value;
            Vector2.Dot(ref from, ref to, out value);
            return MathHelper.ACos(MathHelper.Clamp(value, -1f, 1f)) * 57.29578f;
        }

        // Token: 0x060002C0 RID: 704 RVA: 0x00011020 File Offset: 0x0000F220
        public static void Angle(ref Vector2 from, ref Vector2 to, out float result)
        {
            from.Normalize();
            to.Normalize();
            float value;
            Vector2.Dot(ref from, ref to, out value);
            result = MathHelper.ACos(MathHelper.Clamp(value, -1f, 1f)) * 57.29578f;
        }

        // Token: 0x060002C1 RID: 705 RVA: 0x00011060 File Offset: 0x0000F260
        public static Vector2 Add(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            return result;
        }

        // Token: 0x060002C2 RID: 706 RVA: 0x0001109A File Offset: 0x0000F29A
        public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }

        // Token: 0x060002C3 RID: 707 RVA: 0x000110C4 File Offset: 0x0000F2C4
        public static Vector2 Sub(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            return result;
        }

        // Token: 0x060002C4 RID: 708 RVA: 0x000110FE File Offset: 0x0000F2FE
        public static void Sub(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }

        // Token: 0x060002C5 RID: 709 RVA: 0x00011128 File Offset: 0x0000F328
        public static Vector2 Multiply(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            return result;
        }

        // Token: 0x060002C6 RID: 710 RVA: 0x00011162 File Offset: 0x0000F362
        public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }

        // Token: 0x060002C7 RID: 711 RVA: 0x0001118C File Offset: 0x0000F38C
        public static Vector2 Multiply(Vector2 value1, float scaleFactor)
        {
            Vector2 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            return result;
        }

        // Token: 0x060002C8 RID: 712 RVA: 0x000111BA File Offset: 0x0000F3BA
        public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }

        // Token: 0x060002C9 RID: 713 RVA: 0x000111D8 File Offset: 0x0000F3D8
        public static Vector2 Divide(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            return result;
        }

        // Token: 0x060002CA RID: 714 RVA: 0x00011212 File Offset: 0x0000F412
        public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }

        // Token: 0x060002CB RID: 715 RVA: 0x0001123C File Offset: 0x0000F43C
        public static Vector2 Divide(Vector2 value1, float divider)
        {
            float num = 1f / divider;
            Vector2 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            return result;
        }

        // Token: 0x060002CC RID: 716 RVA: 0x00011274 File Offset: 0x0000F474
        public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
        {
            float num = 1f / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
        }

        // Token: 0x060002CD RID: 717 RVA: 0x000112A8 File Offset: 0x0000F4A8
        public static Vector2 operator -(Vector2 value)
        {
            Vector2 result;
            result.X = -value.X;
            result.Y = -value.Y;
            return result;
        }

        // Token: 0x060002CE RID: 718 RVA: 0x000112D4 File Offset: 0x0000F4D4
        public static bool operator ==(Vector2 value1, Vector2 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }

        // Token: 0x060002CF RID: 719 RVA: 0x000112F8 File Offset: 0x0000F4F8
        public static bool operator !=(Vector2 value1, Vector2 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }

        // Token: 0x060002D0 RID: 720 RVA: 0x00011320 File Offset: 0x0000F520
        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            return result;
        }

        // Token: 0x060002D1 RID: 721 RVA: 0x0001135C File Offset: 0x0000F55C
        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            return result;
        }

        // Token: 0x060002D2 RID: 722 RVA: 0x00011398 File Offset: 0x0000F598
        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            return result;
        }

        // Token: 0x060002D3 RID: 723 RVA: 0x000113D4 File Offset: 0x0000F5D4
        public static Vector2 operator *(Vector2 value, float scaleFactor)
        {
            Vector2 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            return result;
        }

        // Token: 0x060002D4 RID: 724 RVA: 0x00011404 File Offset: 0x0000F604
        public static Vector2 operator *(float scaleFactor, Vector2 value)
        {
            Vector2 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            return result;
        }

        // Token: 0x060002D5 RID: 725 RVA: 0x00011434 File Offset: 0x0000F634
        public static Vector2 operator /(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            return result;
        }

        // Token: 0x060002D6 RID: 726 RVA: 0x00011470 File Offset: 0x0000F670
        public static Vector2 operator /(Vector2 value1, float divider)
        {
            float num = 1f / divider;
            Vector2 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            return result;
        }

        // Token: 0x04000050 RID: 80
        private const float epsilon = 1E-05f;

        // Token: 0x04000051 RID: 81
        public float X;

        // Token: 0x04000052 RID: 82
        public float Y;

        // Token: 0x04000053 RID: 83
        private static Vector2 _zero = default(Vector2);

        // Token: 0x04000054 RID: 84
        private static Vector2 _one = new Vector2(1f, 1f);

        // Token: 0x04000055 RID: 85
        private static Vector2 _unitX = new Vector2(1f, 0f);

        // Token: 0x04000056 RID: 86
        private static Vector2 _unitY = new Vector2(0f, 1f);
    }
}
