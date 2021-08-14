using System;
using System.Collections.Generic;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x02000002 RID: 2
    [Serializable]
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public Vector3[] GetCorners()
        {
            return new Vector3[]
            {
                new Vector3(this.Min.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
            };
        }

        // Token: 0x06000002 RID: 2 RVA: 0x000021F8 File Offset: 0x000003F8
        public void GetCorners(Vector3[] corners)
        {
            if (corners == null)
            {
                throw new ArgumentNullException("corners");
            }
            if (corners.Length < 8)
            {
                throw new ArgumentOutOfRangeException("corners", FrameworkResources.NotEnoughCorners);
            }
            corners[0].X = this.Min.X;
            corners[0].Y = this.Max.Y;
            corners[0].Z = this.Max.Z;
            corners[1].X = this.Max.X;
            corners[1].Y = this.Max.Y;
            corners[1].Z = this.Max.Z;
            corners[2].X = this.Max.X;
            corners[2].Y = this.Min.Y;
            corners[2].Z = this.Max.Z;
            corners[3].X = this.Min.X;
            corners[3].Y = this.Min.Y;
            corners[3].Z = this.Max.Z;
            corners[4].X = this.Min.X;
            corners[4].Y = this.Max.Y;
            corners[4].Z = this.Min.Z;
            corners[5].X = this.Max.X;
            corners[5].Y = this.Max.Y;
            corners[5].Z = this.Min.Z;
            corners[6].X = this.Max.X;
            corners[6].Y = this.Min.Y;
            corners[6].Z = this.Min.Z;
            corners[7].X = this.Min.X;
            corners[7].Y = this.Min.Y;
            corners[7].Z = this.Min.Z;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002451 File Offset: 0x00000651
        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        // Token: 0x06000004 RID: 4 RVA: 0x00002461 File Offset: 0x00000661
        public bool Equals(BoundingBox other)
        {
            return this.Min == other.Min && this.Max == other.Max;
        }

        // Token: 0x06000005 RID: 5 RVA: 0x0000248C File Offset: 0x0000068C
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is BoundingBox)
            {
                result = this.Equals((BoundingBox)obj);
            }
            return result;
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000024B1 File Offset: 0x000006B1
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000024D8 File Offset: 0x000006D8
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Min:{0}, Max{1}", new object[]
            {
                this.Min.ToString(),
                this.Max.ToString()
            });
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002524 File Offset: 0x00000724
        public static BoundingBox CreateFromSphere(BoundingSphere sphere)
        {
            BoundingBox result;
            result.Min.X = sphere.Center.X - sphere.Radius;
            result.Min.Y = sphere.Center.Y - sphere.Radius;
            result.Min.Z = sphere.Center.Z - sphere.Radius;
            result.Max.X = sphere.Center.X + sphere.Radius;
            result.Max.Y = sphere.Center.Y + sphere.Radius;
            result.Max.Z = sphere.Center.Z + sphere.Radius;
            return result;
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000025F4 File Offset: 0x000007F4
        public static void CreateFromSphere(ref BoundingSphere sphere, out BoundingBox result)
        {
            result.Min.X = sphere.Center.X - sphere.Radius;
            result.Min.Y = sphere.Center.Y - sphere.Radius;
            result.Min.Z = sphere.Center.Z - sphere.Radius;
            result.Max.X = sphere.Center.X + sphere.Radius;
            result.Max.Y = sphere.Center.Y + sphere.Radius;
            result.Max.Z = sphere.Center.Z + sphere.Radius;
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000026B0 File Offset: 0x000008B0
        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException();
            }
            bool flag = false;
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);
            foreach (Vector3 vector in points)
            {
                Vector3 vector2 = vector;
                Vector3.Min(ref min, ref vector2, out min);
                Vector3.Max(ref max, ref vector2, out max);
                flag = true;
            }
            if (!flag)
            {
                throw new ArgumentException(FrameworkResources.BoundingBoxZeroPoints);
            }
            return new BoundingBox(min, max);
        }

        // Token: 0x0600000B RID: 11 RVA: 0x00002754 File Offset: 0x00000954
        public bool Intersects(BoundingBox box)
        {
            return this.Max.X >= box.Min.X && this.Min.X <= box.Max.X && this.Max.Y >= box.Min.Y && this.Min.Y <= box.Max.Y && this.Max.Z >= box.Min.Z && this.Min.Z <= box.Max.Z;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002800 File Offset: 0x00000A00
        public void Intersects(ref BoundingBox box, out bool result)
        {
            result = false;
            if (this.Max.X >= box.Min.X && this.Min.X <= box.Max.X && this.Max.Y >= box.Min.Y && this.Min.Y <= box.Max.Y && this.Max.Z >= box.Min.Z && this.Min.Z <= box.Max.Z)
            {
                result = true;
            }
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000028A3 File Offset: 0x00000AA3
        public bool Intersects(BoundingFrustum frustum)
        {
            if (null == frustum)
            {
                throw new ArgumentNullException("frustum", FrameworkResources.NullNotAllowed);
            }
            return frustum.Intersects(this);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x000028CC File Offset: 0x00000ACC
        public PlaneIntersectionStatus Intersects(Plane plane)
        {
            Vector3 vector;
            vector.X = ((plane.Normal.X >= 0f) ? this.Min.X : this.Max.X);
            vector.Y = ((plane.Normal.Y >= 0f) ? this.Min.Y : this.Max.Y);
            vector.Z = ((plane.Normal.Z >= 0f) ? this.Min.Z : this.Max.Z);
            Vector3 vector2;
            vector2.X = ((plane.Normal.X >= 0f) ? this.Max.X : this.Min.X);
            vector2.Y = ((plane.Normal.Y >= 0f) ? this.Max.Y : this.Min.Y);
            vector2.Z = ((plane.Normal.Z >= 0f) ? this.Max.Z : this.Min.Z);
            float num = plane.Normal.X * vector.X + plane.Normal.Y * vector.Y + plane.Normal.Z * vector.Z;
            if (num + plane.D > 0f)
            {
                return PlaneIntersectionStatus.Front;
            }
            num = plane.Normal.X * vector2.X + plane.Normal.Y * vector2.Y + plane.Normal.Z * vector2.Z;
            if (num + plane.D < 0f)
            {
                return PlaneIntersectionStatus.Back;
            }
            return PlaneIntersectionStatus.Intersecting;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00002AA8 File Offset: 0x00000CA8
        public void Intersects(ref Plane plane, out PlaneIntersectionStatus result)
        {
            Vector3 vector;
            vector.X = ((plane.Normal.X >= 0f) ? this.Min.X : this.Max.X);
            vector.Y = ((plane.Normal.Y >= 0f) ? this.Min.Y : this.Max.Y);
            vector.Z = ((plane.Normal.Z >= 0f) ? this.Min.Z : this.Max.Z);
            Vector3 vector2;
            vector2.X = ((plane.Normal.X >= 0f) ? this.Max.X : this.Min.X);
            vector2.Y = ((plane.Normal.Y >= 0f) ? this.Max.Y : this.Min.Y);
            vector2.Z = ((plane.Normal.Z >= 0f) ? this.Max.Z : this.Min.Z);
            float num = plane.Normal.X * vector.X + plane.Normal.Y * vector.Y + plane.Normal.Z * vector.Z;
            if (num + plane.D > 0f)
            {
                result = PlaneIntersectionStatus.Front;
                return;
            }
            num = plane.Normal.X * vector2.X + plane.Normal.Y * vector2.Y + plane.Normal.Z * vector2.Z;
            if (num + plane.D < 0f)
            {
                result = PlaneIntersectionStatus.Back;
                return;
            }
            result = PlaneIntersectionStatus.Intersecting;
        }

        // Token: 0x06000010 RID: 16 RVA: 0x00002C7C File Offset: 0x00000E7C
        public bool Intersects(BoundingSphere sphere)
        {
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            float num;
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num);
            return num <= sphere.Radius * sphere.Radius;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00002CC8 File Offset: 0x00000EC8
        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            float num;
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num);
            result = (num <= sphere.Radius * sphere.Radius);
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002D14 File Offset: 0x00000F14
        public bool Intersects(Ray ray)
        {
            float num = 0f;
            float num2 = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if (ray.Position.X < this.Min.X || ray.Position.X > this.Max.X)
                {
                    return false;
                }
            }
            else
            {
                float num3 = 1f / ray.Direction.X;
                float num4 = (this.Min.X - ray.Position.X) * num3;
                float num5 = (this.Max.X - ray.Position.X) * num3;
                if (num4 > num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num = MathHelper.Max(num4, num);
                num2 = MathHelper.Min(num5, num2);
                if (num > num2)
                {
                    return false;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if (ray.Position.Y < this.Min.Y || ray.Position.Y > this.Max.Y)
                {
                    return false;
                }
            }
            else
            {
                float num7 = 1f / ray.Direction.Y;
                float num8 = (this.Min.Y - ray.Position.Y) * num7;
                float num9 = (this.Max.Y - ray.Position.Y) * num7;
                if (num8 > num9)
                {
                    float num10 = num8;
                    num8 = num9;
                    num9 = num10;
                }
                num = MathHelper.Max(num8, num);
                num2 = MathHelper.Min(num9, num2);
                if (num > num2)
                {
                    return false;
                }
            }
            if (Math.Abs(ray.Direction.Z) < 1E-06f)
            {
                if (ray.Position.Z < this.Min.Z || ray.Position.Z > this.Max.Z)
                {
                    return false;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.Z;
                float num12 = (this.Min.Z - ray.Position.Z) * num11;
                float num13 = (this.Max.Z - ray.Position.Z) * num11;
                if (num12 > num13)
                {
                    float num14 = num12;
                    num12 = num13;
                    num13 = num14;
                }
                num = MathHelper.Max(num12, num);
                num2 = MathHelper.Min(num13, num2);
                if (num > num2)
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00002F74 File Offset: 0x00001174
        public bool Intersects(ref Ray ray, out float distance)
        {
            float num = 0f;
            distance = 0f;
            float num2 = float.MaxValue;
            if (Math.Abs(ray.Direction.X) < 1E-06f)
            {
                if (ray.Position.X < this.Min.X || ray.Position.X > this.Max.X)
                {
                    return false;
                }
            }
            else
            {
                float num3 = 1f / ray.Direction.X;
                float num4 = (this.Min.X - ray.Position.X) * num3;
                float num5 = (this.Max.X - ray.Position.X) * num3;
                if (num4 > num5)
                {
                    float num6 = num4;
                    num4 = num5;
                    num5 = num6;
                }
                num = MathHelper.Max(num4, num);
                num2 = MathHelper.Min(num5, num2);
                if (num > num2)
                {
                    return false;
                }
            }
            if (Math.Abs(ray.Direction.Y) < 1E-06f)
            {
                if (ray.Position.Y < this.Min.Y || ray.Position.Y > this.Max.Y)
                {
                    return false;
                }
            }
            else
            {
                float num7 = 1f / ray.Direction.Y;
                float num8 = (this.Min.Y - ray.Position.Y) * num7;
                float num9 = (this.Max.Y - ray.Position.Y) * num7;
                if (num8 > num9)
                {
                    float num10 = num8;
                    num8 = num9;
                    num9 = num10;
                }
                num = MathHelper.Max(num8, num);
                num2 = MathHelper.Min(num9, num2);
                if (num > num2)
                {
                    return false;
                }
            }
            if (Math.Abs(ray.Direction.Z) < 1E-06f)
            {
                if (ray.Position.Z < this.Min.Z || ray.Position.Z > this.Max.Z)
                {
                    return false;
                }
            }
            else
            {
                float num11 = 1f / ray.Direction.Z;
                float num12 = (this.Min.Z - ray.Position.Z) * num11;
                float num13 = (this.Max.Z - ray.Position.Z) * num11;
                if (num12 > num13)
                {
                    float num14 = num12;
                    num12 = num13;
                    num13 = num14;
                }
                num = MathHelper.Max(num12, num);
                num2 = MathHelper.Min(num13, num2);
                if (num > num2)
                {
                    return false;
                }
            }
            distance = num;
            return true;
        }

        // Token: 0x06000014 RID: 20 RVA: 0x000031CC File Offset: 0x000013CC
        public ClipStatus Contains(BoundingBox box)
        {
            if (this.Max.X < box.Min.X || this.Min.X > box.Max.X)
            {
                return ClipStatus.Outside;
            }
            if (this.Max.Y < box.Min.Y || this.Min.Y > box.Max.Y)
            {
                return ClipStatus.Outside;
            }
            if (this.Max.Z < box.Min.Z || this.Min.Z > box.Max.Z)
            {
                return ClipStatus.Outside;
            }
            if (this.Min.X <= box.Min.X && box.Max.X <= this.Max.X && this.Min.Y <= box.Min.Y && box.Max.Y <= this.Max.Y && this.Min.Z <= box.Min.Z && box.Max.Z <= this.Max.Z)
            {
                return ClipStatus.Inside;
            }
            return ClipStatus.Intersecting;
        }

        // Token: 0x06000015 RID: 21 RVA: 0x00003310 File Offset: 0x00001510
        public void Contains(ref BoundingBox box, out ClipStatus result)
        {
            result = ClipStatus.Outside;
            if (this.Max.X >= box.Min.X && this.Min.X <= box.Max.X && this.Max.Y >= box.Min.Y && this.Min.Y <= box.Max.Y && this.Max.Z >= box.Min.Z && this.Min.Z <= box.Max.Z)
            {
                result = ((this.Min.X <= box.Min.X && box.Max.X <= this.Max.X && this.Min.Y <= box.Min.Y && box.Max.Y <= this.Max.Y && this.Min.Z <= box.Min.Z && box.Max.Z <= this.Max.Z) ? ClipStatus.Inside : ClipStatus.Intersecting);
            }
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00003458 File Offset: 0x00001658
        public ClipStatus Contains(BoundingFrustum frustum)
        {
            if (null == frustum)
            {
                throw new ArgumentNullException("frustum", FrameworkResources.NullNotAllowed);
            }
            if (!frustum.Intersects(this))
            {
                return ClipStatus.Outside;
            }
            foreach (Vector3 point in frustum.cornerArray)
            {
                if (this.Contains(point) == ClipStatus.Outside)
                {
                    return ClipStatus.Intersecting;
                }
            }
            return ClipStatus.Inside;
        }

        // Token: 0x06000017 RID: 23 RVA: 0x000034C0 File Offset: 0x000016C0
        public ClipStatus Contains(Vector3 point)
        {
            if (this.Min.X <= point.X && point.X <= this.Max.X && this.Min.Y <= point.Y && point.Y <= this.Max.Y && this.Min.Z <= point.Z && point.Z <= this.Max.Z)
            {
                return ClipStatus.Inside;
            }
            return ClipStatus.Outside;
        }

        // Token: 0x06000018 RID: 24 RVA: 0x00003548 File Offset: 0x00001748
        public void Contains(ref Vector3 point, out ClipStatus result)
        {
            result = ((this.Min.X <= point.X && point.X <= this.Max.X && this.Min.Y <= point.Y && point.Y <= this.Max.Y && this.Min.Z <= point.Z && point.Z <= this.Max.Z) ? ClipStatus.Inside : ClipStatus.Outside);
        }

        // Token: 0x06000019 RID: 25 RVA: 0x000035D0 File Offset: 0x000017D0
        public ClipStatus Contains(BoundingSphere sphere)
        {
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            float num;
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num);
            float radius = sphere.Radius;
            if (num > radius * radius)
            {
                return ClipStatus.Outside;
            }
            if (this.Min.X + radius <= sphere.Center.X && sphere.Center.X <= this.Max.X - radius && this.Max.X - this.Min.X > radius && this.Min.Y + radius <= sphere.Center.Y && sphere.Center.Y <= this.Max.Y - radius && this.Max.Y - this.Min.Y > radius && this.Min.Z + radius <= sphere.Center.Z && sphere.Center.Z <= this.Max.Z - radius && this.Max.X - this.Min.X > radius)
            {
                return ClipStatus.Inside;
            }
            return ClipStatus.Intersecting;
        }

        // Token: 0x0600001A RID: 26 RVA: 0x00003718 File Offset: 0x00001918
        public void Contains(ref BoundingSphere sphere, out ClipStatus result)
        {
            Vector3 vector;
            Vector3.Clamp(ref sphere.Center, ref this.Min, ref this.Max, out vector);
            float num;
            Vector3.DistanceSquared(ref sphere.Center, ref vector, out num);
            float radius = sphere.Radius;
            if (num > radius * radius)
            {
                result = ClipStatus.Outside;
                return;
            }
            result = ((this.Min.X + radius <= sphere.Center.X && sphere.Center.X <= this.Max.X - radius && this.Max.X - this.Min.X > radius && this.Min.Y + radius <= sphere.Center.Y && sphere.Center.Y <= this.Max.Y - radius && this.Max.Y - this.Min.Y > radius && this.Min.Z + radius <= sphere.Center.Z && sphere.Center.Z <= this.Max.Z - radius && this.Max.X - this.Min.X > radius) ? ClipStatus.Inside : ClipStatus.Intersecting);
        }

        // Token: 0x0600001B RID: 27 RVA: 0x0000385C File Offset: 0x00001A5C
        public BoundingBox Transform(Matrix44 matrix)
        {
            Vector3[] corners = this.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                Matrix44.TransformPosition(ref matrix, ref corners[i], out corners[i]);
            }
            return BoundingBox.CreateFromPoints(corners);
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00003898 File Offset: 0x00001A98
        public void Transform(ref Matrix44 matrix, out BoundingBox result)
        {
            Vector3[] corners = this.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                Matrix44.TransformPosition(ref matrix, ref corners[i], out corners[i]);
            }
            result = BoundingBox.CreateFromPoints(corners);
        }

        // Token: 0x0600001D RID: 29 RVA: 0x000038D8 File Offset: 0x00001AD8
        internal void SupportMapping(ref Vector3 v, out Vector3 result)
        {
            result.X = ((v.X >= 0f) ? this.Max.X : this.Min.X);
            result.Y = ((v.Y >= 0f) ? this.Max.Y : this.Min.Y);
            result.Z = ((v.Z >= 0f) ? this.Max.Z : this.Min.Z);
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00003966 File Offset: 0x00001B66
        public static bool operator ==(BoundingBox a, BoundingBox b)
        {
            return a.Equals(b);
        }

        // Token: 0x0600001F RID: 31 RVA: 0x00003970 File Offset: 0x00001B70
        public static bool operator !=(BoundingBox a, BoundingBox b)
        {
            return a.Min != b.Min || a.Max != b.Max;
        }

        // Token: 0x04000001 RID: 1
        public const int CornerCount = 8;

        // Token: 0x04000002 RID: 2
        public Vector3 Min;

        // Token: 0x04000003 RID: 3
        public Vector3 Max;
    }
}
