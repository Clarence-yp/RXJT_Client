using System;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x02000012 RID: 18
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        // Token: 0x1700015B RID: 347
        // (get) Token: 0x060002D8 RID: 728 RVA: 0x000114FC File Offset: 0x0000F6FC
        public static Vector3 Zero {
            get {
                return Vector3._zero;
            }
        }

        // Token: 0x1700015C RID: 348
        // (get) Token: 0x060002D9 RID: 729 RVA: 0x00011503 File Offset: 0x0000F703
        public static Vector3 One {
            get {
                return Vector3._one;
            }
        }

        // Token: 0x1700015D RID: 349
        // (get) Token: 0x060002DA RID: 730 RVA: 0x0001150A File Offset: 0x0000F70A
        public static Vector3 UnitX {
            get {
                return Vector3._unitX;
            }
        }

        // Token: 0x1700015E RID: 350
        // (get) Token: 0x060002DB RID: 731 RVA: 0x00011511 File Offset: 0x0000F711
        public static Vector3 UnitY {
            get {
                return Vector3._unitY;
            }
        }

        // Token: 0x1700015F RID: 351
        // (get) Token: 0x060002DC RID: 732 RVA: 0x00011518 File Offset: 0x0000F718
        public static Vector3 UnitZ {
            get {
                return Vector3._unitZ;
            }
        }

        // Token: 0x17000160 RID: 352
        // (get) Token: 0x060002DD RID: 733 RVA: 0x0001151F File Offset: 0x0000F71F
        public static Vector3 Up {
            get {
                return Vector3._up;
            }
        }

        // Token: 0x17000161 RID: 353
        // (get) Token: 0x060002DE RID: 734 RVA: 0x00011526 File Offset: 0x0000F726
        public static Vector3 Down {
            get {
                return Vector3._down;
            }
        }

        // Token: 0x17000162 RID: 354
        // (get) Token: 0x060002DF RID: 735 RVA: 0x0001152D File Offset: 0x0000F72D
        public static Vector3 Right {
            get {
                return Vector3._right;
            }
        }

        // Token: 0x17000163 RID: 355
        // (get) Token: 0x060002E0 RID: 736 RVA: 0x00011534 File Offset: 0x0000F734
        public static Vector3 Left {
            get {
                return Vector3._left;
            }
        }

        // Token: 0x17000164 RID: 356
        // (get) Token: 0x060002E1 RID: 737 RVA: 0x0001153B File Offset: 0x0000F73B
        public static Vector3 Forward {
            get {
                return Vector3._forward;
            }
        }

        // Token: 0x17000165 RID: 357
        // (get) Token: 0x060002E2 RID: 738 RVA: 0x00011542 File Offset: 0x0000F742
        public static Vector3 Backward {
            get {
                return Vector3._backward;
            }
        }

        // Token: 0x060002E3 RID: 739 RVA: 0x00011549 File Offset: 0x0000F749
        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        // Token: 0x060002E4 RID: 740 RVA: 0x00011560 File Offset: 0x0000F760
        public Vector3(float value)
        {
            this.Z = value;
            this.Y = value;
            this.X = value;
        }

        // Token: 0x060002E5 RID: 741 RVA: 0x00011586 File Offset: 0x0000F786
        public Vector3(Vector2 value, float z)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }

        // Token: 0x17000166 RID: 358
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

        // Token: 0x060002E8 RID: 744 RVA: 0x000115EC File Offset: 0x0000F7EC
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{0}, {1}, {2}", new object[]
            {
                this.X.ToString(currentCulture),
                this.Y.ToString(currentCulture),
                this.Z.ToString(currentCulture)
            });
        }

        // Token: 0x060002E9 RID: 745 RVA: 0x0001163F File Offset: 0x0000F83F
        public bool Equals(Vector3 other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }

        // Token: 0x060002EA RID: 746 RVA: 0x00011670 File Offset: 0x0000F870
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Vector3)
            {
                result = this.Equals((Vector3)obj);
            }
            return result;
        }

        // Token: 0x060002EB RID: 747 RVA: 0x00011695 File Offset: 0x0000F895
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode();
        }

        // Token: 0x060002EC RID: 748 RVA: 0x000116BC File Offset: 0x0000F8BC
        public float Length()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            return (float)Math.Sqrt((double)num);
        }

        // Token: 0x060002ED RID: 749 RVA: 0x000116FB File Offset: 0x0000F8FB
        public float LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }

        // Token: 0x060002EE RID: 750 RVA: 0x00011728 File Offset: 0x0000F928
        public static float Distance(Vector3 value1, Vector3 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            return (float)Math.Sqrt((double)num4);
        }

        // Token: 0x060002EF RID: 751 RVA: 0x0001177C File Offset: 0x0000F97C
        public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            result = (float)Math.Sqrt((double)num4);
        }

        // Token: 0x060002F0 RID: 752 RVA: 0x000117CC File Offset: 0x0000F9CC
        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            return num * num + num2 * num2 + num3 * num3;
        }

        // Token: 0x060002F1 RID: 753 RVA: 0x00011814 File Offset: 0x0000FA14
        public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            result = num * num + num2 * num2 + num3 * num3;
        }

        // Token: 0x060002F2 RID: 754 RVA: 0x00011858 File Offset: 0x0000FA58
        public static float Dot(Vector3 vector1, Vector3 vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        // Token: 0x060002F3 RID: 755 RVA: 0x00011889 File Offset: 0x0000FA89
        public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
        {
            result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }

        // Token: 0x060002F4 RID: 756 RVA: 0x000118B8 File Offset: 0x0000FAB8
        public void Normalize()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            if (num < Vector3.epsilon)
            {
                return;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            this.X *= num2;
            this.Y *= num2;
            this.Z *= num2;
        }

        // Token: 0x060002F5 RID: 757 RVA: 0x00011934 File Offset: 0x0000FB34
        public static Vector3 Normalize(Vector3 value)
        {
            float num = value.X * value.X + value.Y * value.Y + value.Z * value.Z;
            if (num < Vector3.epsilon)
            {
                return value;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            Vector3 result;
            result.X = value.X * num2;
            result.Y = value.Y * num2;
            result.Z = value.Z * num2;
            return result;
        }

        // Token: 0x060002F6 RID: 758 RVA: 0x000119BC File Offset: 0x0000FBBC
        public static void Normalize(ref Vector3 value, out Vector3 result)
        {
            float num = value.X * value.X + value.Y * value.Y + value.Z * value.Z;
            if (num < Vector3.epsilon)
            {
                result = value;
                return;
            }
            float num2 = 1f / (float)Math.Sqrt((double)num);
            result.X = value.X * num2;
            result.Y = value.Y * num2;
            result.Z = value.Z * num2;
        }

        // Token: 0x060002F7 RID: 759 RVA: 0x00011A44 File Offset: 0x0000FC44
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            Vector3 result;
            result.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            result.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return result;
        }

        // Token: 0x060002F8 RID: 760 RVA: 0x00011AC4 File Offset: 0x0000FCC4
        public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
        {
            float x = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            float y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            float z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        // Token: 0x060002F9 RID: 761 RVA: 0x00011B3C File Offset: 0x0000FD3C
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            Vector3 result;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            result.Z = vector.Z - 2f * num * normal.Z;
            return result;
        }

        // Token: 0x060002FA RID: 762 RVA: 0x00011BD4 File Offset: 0x0000FDD4
        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            result.Z = vector.Z - 2f * num * normal.Z;
        }

        // Token: 0x060002FB RID: 763 RVA: 0x00011C5C File Offset: 0x0000FE5C
        public static Vector3 Min(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
            return result;
        }

        // Token: 0x060002FC RID: 764 RVA: 0x00011CE0 File Offset: 0x0000FEE0
        public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
        }

        // Token: 0x060002FD RID: 765 RVA: 0x00011D54 File Offset: 0x0000FF54
        public static Vector3 Max(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
            return result;
        }

        // Token: 0x060002FE RID: 766 RVA: 0x00011DD8 File Offset: 0x0000FFD8
        public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
        }

        // Token: 0x060002FF RID: 767 RVA: 0x00011E4C File Offset: 0x0001004C
        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
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
            Vector3 result;
            result.X = num;
            result.Y = num2;
            result.Z = num3;
            return result;
        }

        // Token: 0x06000300 RID: 768 RVA: 0x00011F08 File Offset: 0x00010108
        public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
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
            result.X = num;
            result.Y = num2;
            result.Z = num3;
        }

        // Token: 0x06000301 RID: 769 RVA: 0x00011FB4 File Offset: 0x000101B4
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            Vector3 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return result;
        }

        // Token: 0x06000302 RID: 770 RVA: 0x00012024 File Offset: 0x00010224
        public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }

        // Token: 0x06000303 RID: 771 RVA: 0x00012088 File Offset: 0x00010288
        public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            Vector3 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return result;
        }

        // Token: 0x06000304 RID: 772 RVA: 0x0001212C File Offset: 0x0001032C
        public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }

        // Token: 0x06000305 RID: 773 RVA: 0x000121C4 File Offset: 0x000103C4
        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            Vector3 result;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
            return result;
        }

        // Token: 0x06000306 RID: 774 RVA: 0x000122AC File Offset: 0x000104AC
        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
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
        }

        // Token: 0x06000307 RID: 775 RVA: 0x00012384 File Offset: 0x00010584
        public static Vector3 Negate(Vector3 value)
        {
            Vector3 result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            return result;
        }

        // Token: 0x06000308 RID: 776 RVA: 0x000123BF File Offset: 0x000105BF
        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }

        // Token: 0x06000309 RID: 777 RVA: 0x000123E8 File Offset: 0x000105E8
        public static Vector3 Add(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            return result;
        }

        // Token: 0x0600030A RID: 778 RVA: 0x00012438 File Offset: 0x00010638
        public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }

        // Token: 0x0600030B RID: 779 RVA: 0x00012474 File Offset: 0x00010674
        public static Vector3 Sub(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            return result;
        }

        // Token: 0x0600030C RID: 780 RVA: 0x000124C4 File Offset: 0x000106C4
        public static void Sub(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }

        // Token: 0x0600030D RID: 781 RVA: 0x00012500 File Offset: 0x00010700
        public static Vector3 Multiply(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            return result;
        }

        // Token: 0x0600030E RID: 782 RVA: 0x00012550 File Offset: 0x00010750
        public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }

        // Token: 0x0600030F RID: 783 RVA: 0x0001258C File Offset: 0x0001078C
        public static Vector3 Multiply(Vector3 value1, float scaleFactor)
        {
            Vector3 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            return result;
        }

        // Token: 0x06000310 RID: 784 RVA: 0x000125CA File Offset: 0x000107CA
        public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }

        // Token: 0x06000311 RID: 785 RVA: 0x000125F8 File Offset: 0x000107F8
        public static Vector3 Divide(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
            return result;
        }

        // Token: 0x06000312 RID: 786 RVA: 0x00012648 File Offset: 0x00010848
        public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }

        // Token: 0x06000313 RID: 787 RVA: 0x00012684 File Offset: 0x00010884
        public static Vector3 Divide(Vector3 value1, float divider)
        {
            float num = 1f / divider;
            Vector3 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
            return result;
        }

        // Token: 0x06000314 RID: 788 RVA: 0x000126CC File Offset: 0x000108CC
        public static void Divide(ref Vector3 value1, float divider, out Vector3 result)
        {
            float num = 1f / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
        }

        // Token: 0x06000315 RID: 789 RVA: 0x0001270B File Offset: 0x0001090B
        private static float magnitude(ref Vector3 inV)
        {
            return (float)Math.Sqrt((double)Vector3.Dot(inV, inV));
        }

        // Token: 0x06000316 RID: 790 RVA: 0x00012728 File Offset: 0x00010928
        private static Vector3 orthoNormalVectorFast(ref Vector3 n)
        {
            Vector3 result;
            if (Math.Abs(n.Z) > Vector3.k1OverSqrt2)
            {
                float num = n.Y * n.Y + n.Z * n.Z;
                float num2 = 1f / (float)Math.Sqrt((double)num);
                result.X = 0f;
                result.Y = -n.Z * num2;
                result.Z = n.Y * num2;
            }
            else
            {
                float num3 = n.X * n.X + n.Y * n.Y;
                float num4 = 1f / (float)Math.Sqrt((double)num3);
                result.X = -n.Y * num4;
                result.Y = n.X * num4;
                result.Z = 0f;
            }
            return result;
        }

        // Token: 0x06000317 RID: 791 RVA: 0x000127FC File Offset: 0x000109FC
        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent)
        {
            float num = Vector3.magnitude(ref normal);
            if (num > Vector3.epsilon)
            {
                normal /= num;
            }
            else
            {
                normal = new Vector3(1f, 0f, 0f);
            }
            float scaleFactor = Vector3.Dot(normal, tangent);
            tangent -= scaleFactor * normal;
            num = Vector3.magnitude(ref tangent);
            if (num < Vector3.epsilon)
            {
                tangent = Vector3.orthoNormalVectorFast(ref normal);
                return;
            }
            tangent /= num;
        }

        // Token: 0x06000318 RID: 792 RVA: 0x000128A4 File Offset: 0x00010AA4
        public static void OrthoNormalize(ref Vector3 normal, ref Vector3 tangent, ref Vector3 binormal)
        {
            float num = Vector3.magnitude(ref normal);
            if (num > Vector3.epsilon)
            {
                normal /= num;
            }
            else
            {
                normal = new Vector3(1f, 0f, 0f);
            }
            float scaleFactor = Vector3.Dot(normal, tangent);
            tangent -= scaleFactor * normal;
            num = Vector3.magnitude(ref tangent);
            if (num > Vector3.epsilon)
            {
                tangent /= num;
            }
            else
            {
                tangent = Vector3.orthoNormalVectorFast(ref normal);
            }
            float scaleFactor2 = Vector3.Dot(tangent, binormal);
            scaleFactor = Vector3.Dot(normal, binormal);
            binormal -= scaleFactor * normal + scaleFactor2 * tangent;
            num = Vector3.magnitude(ref binormal);
            if (num > Vector3.epsilon)
            {
                binormal /= num;
                return;
            }
            binormal = Vector3.Cross(normal, tangent);
        }

        // Token: 0x06000319 RID: 793 RVA: 0x000129D7 File Offset: 0x00010BD7
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            return onNormal * Vector3.Dot(vector, onNormal) / Vector3.Dot(onNormal, onNormal);
        }

        // Token: 0x0600031A RID: 794 RVA: 0x000129F2 File Offset: 0x00010BF2
        public static void Project(ref Vector3 vector, ref Vector3 onNormal, out Vector3 result)
        {
            result = onNormal * Vector3.Dot(vector, onNormal) / Vector3.Dot(onNormal, onNormal);
        }

        // Token: 0x0600031B RID: 795 RVA: 0x00012A2C File Offset: 0x00010C2C
        public static float Angle(Vector3 from, Vector3 to)
        {
            from.Normalize();
            to.Normalize();
            float value;
            Vector3.Dot(ref from, ref to, out value);
            return MathHelper.ACos(MathHelper.Clamp(value, -1f, 1f)) * 57.29578f;
        }

        // Token: 0x0600031C RID: 796 RVA: 0x00012A70 File Offset: 0x00010C70
        public static void Angle(ref Vector3 from, ref Vector3 to, out float result)
        {
            from.Normalize();
            to.Normalize();
            float value;
            Vector3.Dot(ref from, ref to, out value);
            result = MathHelper.ACos(MathHelper.Clamp(value, -1f, 1f)) * 57.29578f;
        }

        // Token: 0x0600031D RID: 797 RVA: 0x00012AB0 File Offset: 0x00010CB0
        public static Vector3 operator -(Vector3 value)
        {
            Vector3 result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            return result;
        }

        // Token: 0x0600031E RID: 798 RVA: 0x00012AEB File Offset: 0x00010CEB
        public static bool operator ==(Vector3 value1, Vector3 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
        }

        // Token: 0x0600031F RID: 799 RVA: 0x00012B1F File Offset: 0x00010D1F
        public static bool operator !=(Vector3 value1, Vector3 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z;
        }

        // Token: 0x06000320 RID: 800 RVA: 0x00012B58 File Offset: 0x00010D58
        public static Vector3 operator +(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            return result;
        }

        // Token: 0x06000321 RID: 801 RVA: 0x00012BA8 File Offset: 0x00010DA8
        public static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            return result;
        }

        // Token: 0x06000322 RID: 802 RVA: 0x00012BF8 File Offset: 0x00010DF8
        public static Vector3 operator *(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            return result;
        }

        // Token: 0x06000323 RID: 803 RVA: 0x00012C48 File Offset: 0x00010E48
        public static Vector3 operator *(Vector3 value, float scaleFactor)
        {
            Vector3 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            result.Z = value.Z * scaleFactor;
            return result;
        }

        // Token: 0x06000324 RID: 804 RVA: 0x00012C88 File Offset: 0x00010E88
        public static Vector3 operator *(float scaleFactor, Vector3 value)
        {
            Vector3 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            result.Z = value.Z * scaleFactor;
            return result;
        }

        // Token: 0x06000325 RID: 805 RVA: 0x00012CC8 File Offset: 0x00010EC8
        public static Vector3 operator /(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
            return result;
        }

        // Token: 0x06000326 RID: 806 RVA: 0x00012D18 File Offset: 0x00010F18
        public static Vector3 operator /(Vector3 value, float divider)
        {
            float num = 1f / divider;
            Vector3 result;
            result.X = value.X * num;
            result.Y = value.Y * num;
            result.Z = value.Z * num;
            return result;
        }

        // Token: 0x06000327 RID: 807 RVA: 0x00012D60 File Offset: 0x00010F60
        static Vector3()
        {
            Vector3._zero = default(Vector3);
            Vector3._one = new Vector3(1f, 1f, 1f);
            Vector3._unitX = new Vector3(1f, 0f, 0f);
            Vector3._unitY = new Vector3(0f, 1f, 0f);
            Vector3._unitZ = new Vector3(0f, 0f, 1f);
            Vector3._up = new Vector3(0f, 1f, 0f);
            Vector3._down = new Vector3(0f, -1f, 0f);
            Vector3._right = new Vector3(1f, 0f, 0f);
            Vector3._left = new Vector3(-1f, 0f, 0f);
            Vector3._forward = new Vector3(0f, 0f, -1f);
            Vector3._backward = new Vector3(0f, 0f, 1f);
        }

        // Token: 0x04000057 RID: 87
        public float X;

        // Token: 0x04000058 RID: 88
        public float Y;

        // Token: 0x04000059 RID: 89
        public float Z;

        // Token: 0x0400005A RID: 90
        private static Vector3 _zero;

        // Token: 0x0400005B RID: 91
        private static Vector3 _one;

        // Token: 0x0400005C RID: 92
        private static Vector3 _unitX;

        // Token: 0x0400005D RID: 93
        private static Vector3 _unitY;

        // Token: 0x0400005E RID: 94
        private static Vector3 _unitZ;

        // Token: 0x0400005F RID: 95
        private static Vector3 _up;

        // Token: 0x04000060 RID: 96
        private static Vector3 _down;

        // Token: 0x04000061 RID: 97
        private static Vector3 _right;

        // Token: 0x04000062 RID: 98
        private static Vector3 _left;

        // Token: 0x04000063 RID: 99
        private static Vector3 _forward;

        // Token: 0x04000064 RID: 100
        private static Vector3 _backward;

        // Token: 0x04000065 RID: 101
        private static float k1OverSqrt2 = 0.70710677f;

        // Token: 0x04000066 RID: 102
        private static readonly float epsilon = 1E-05f;
    }
}
