using System;
using System.Collections.Generic;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x02000004 RID: 4
    [Serializable]
    public struct BoundingSphere : IEquatable<BoundingSphere>
    {
        // Token: 0x06000046 RID: 70 RVA: 0x00004D09 File Offset: 0x00002F09
        public BoundingSphere(Vector3 center, float radius)
        {
            if (radius < 0f)
            {
                throw new ArgumentException(FrameworkResources.NegativeRadius);
            }
            this.Center = center;
            this.Radius = radius;
        }

        // Token: 0x06000047 RID: 71 RVA: 0x00004D2C File Offset: 0x00002F2C
        public bool Equals(BoundingSphere other)
        {
            return this.Center == other.Center && this.Radius == other.Radius;
        }

        // Token: 0x06000048 RID: 72 RVA: 0x00004D54 File Offset: 0x00002F54
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is BoundingSphere)
            {
                result = this.Equals((BoundingSphere)obj);
            }
            return result;
        }

        // Token: 0x06000049 RID: 73 RVA: 0x00004D79 File Offset: 0x00002F79
        public override int GetHashCode()
        {
            return this.Center.GetHashCode() + this.Radius.GetHashCode();
        }

        // Token: 0x0600004A RID: 74 RVA: 0x00004D98 File Offset: 0x00002F98
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "C:{0}, R:{1}", new object[]
            {
                this.Center.ToString(),
                this.Radius.ToString(currentCulture)
            });
        }

        // Token: 0x0600004B RID: 75 RVA: 0x00004DE4 File Offset: 0x00002FE4
        public static BoundingSphere CreateFromBoundingBox(BoundingBox box)
        {
            BoundingSphere result;
            Vector3.Lerp(ref box.Min, ref box.Max, 0.5f, out result.Center);
            float num;
            Vector3.Distance(ref box.Min, ref box.Max, out num);
            result.Radius = num * 0.5f;
            return result;
        }

        // Token: 0x0600004C RID: 76 RVA: 0x00004E34 File Offset: 0x00003034
        public static void CreateFromBoundingBox(ref BoundingBox box, out BoundingSphere result)
        {
            Vector3.Lerp(ref box.Min, ref box.Max, 0.5f, out result.Center);
            float num;
            Vector3.Distance(ref box.Min, ref box.Max, out num);
            result.Radius = num * 0.5f;
        }

        // Token: 0x0600004D RID: 77 RVA: 0x00004E80 File Offset: 0x00003080
        public static BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            IEnumerator<Vector3> enumerator = points.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw new ArgumentException(FrameworkResources.BoundingSphereZeroPoints);
            }
            Vector3 vector6;
            Vector3 vector5;
            Vector3 vector4;
            Vector3 vector3;
            Vector3 vector2;
            Vector3 vector = vector2 = (vector3 = (vector4 = (vector5 = (vector6 = enumerator.Current))));
            foreach (Vector3 vector7 in points)
            {
                if (vector7.X < vector2.X)
                {
                    vector2 = vector7;
                }
                if (vector7.X > vector.X)
                {
                    vector = vector7;
                }
                if (vector7.Y < vector3.Y)
                {
                    vector3 = vector7;
                }
                if (vector7.Y > vector4.Y)
                {
                    vector4 = vector7;
                }
                if (vector7.Z < vector5.Z)
                {
                    vector5 = vector7;
                }
                if (vector7.Z > vector6.Z)
                {
                    vector6 = vector7;
                }
            }
            float num;
            Vector3.Distance(ref vector, ref vector2, out num);
            float num2;
            Vector3.Distance(ref vector4, ref vector3, out num2);
            float num3;
            Vector3.Distance(ref vector6, ref vector5, out num3);
            Vector3 vector8;
            float num4;
            if (num > num2)
            {
                if (num > num3)
                {
                    Vector3.Lerp(ref vector, ref vector2, 0.5f, out vector8);
                    num4 = num * 0.5f;
                }
                else
                {
                    Vector3.Lerp(ref vector6, ref vector5, 0.5f, out vector8);
                    num4 = num3 * 0.5f;
                }
            }
            else if (num2 > num3)
            {
                Vector3.Lerp(ref vector4, ref vector3, 0.5f, out vector8);
                num4 = num2 * 0.5f;
            }
            else
            {
                Vector3.Lerp(ref vector6, ref vector5, 0.5f, out vector8);
                num4 = num3 * 0.5f;
            }
            foreach (Vector3 vector9 in points)
            {
                Vector3 value;
                value.X = vector9.X - vector8.X;
                value.Y = vector9.Y - vector8.Y;
                value.Z = vector9.Z - vector8.Z;
                float num5 = value.Length();
                if (num5 > num4)
                {
                    num4 = (num4 + num5) * 0.5f;
                    vector8 += (1f - num4 / num5) * value;
                }
            }
            BoundingSphere result;
            result.Center = vector8;
            result.Radius = num4;
            return result;
        }

        // Token: 0x0600004E RID: 78 RVA: 0x000050DC File Offset: 0x000032DC
        public static BoundingSphere CreateFromFrustum(BoundingFrustum frustum)
        {
            if (frustum == null)
            {
                throw new ArgumentNullException("frustum");
            }
            return BoundingSphere.CreateFromPoints(frustum.cornerArray);
        }

        // Token: 0x0600004F RID: 79 RVA: 0x00005100 File Offset: 0x00003300
        public bool Intersects(BoundingBox box)
        {
            Vector3 vector;
            Vector3.Clamp(ref this.Center, ref box.Min, ref box.Max, out vector);
            float num;
            Vector3.DistanceSquared(ref this.Center, ref vector, out num);
            return num <= this.Radius * this.Radius;
        }

        // Token: 0x06000050 RID: 80 RVA: 0x0000514C File Offset: 0x0000334C
        public void Intersects(ref BoundingBox box, out bool result)
        {
            Vector3 vector;
            Vector3.Clamp(ref this.Center, ref box.Min, ref box.Max, out vector);
            float num;
            Vector3.DistanceSquared(ref this.Center, ref vector, out num);
            result = (num <= this.Radius * this.Radius);
        }

        // Token: 0x06000051 RID: 81 RVA: 0x00005198 File Offset: 0x00003398
        public bool Intersects(BoundingFrustum frustum)
        {
            if (null == frustum)
            {
                throw new ArgumentNullException("frustum", FrameworkResources.NullNotAllowed);
            }
            bool result;
            frustum.Intersects(ref this, out result);
            return result;
        }

        // Token: 0x06000052 RID: 82 RVA: 0x000051C8 File Offset: 0x000033C8
        public PlaneIntersectionStatus Intersects(Plane plane)
        {
            return plane.Intersects(this);
        }

        // Token: 0x06000053 RID: 83 RVA: 0x000051D7 File Offset: 0x000033D7
        public void Intersects(ref Plane plane, out PlaneIntersectionStatus result)
        {
            plane.Intersects(ref this, out result);
        }

        // Token: 0x06000054 RID: 84 RVA: 0x000051E1 File Offset: 0x000033E1
        public bool Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }

        // Token: 0x06000055 RID: 85 RVA: 0x000051F0 File Offset: 0x000033F0
        public bool Intersects(ref Ray ray, out float distance)
        {
            return ray.Intersects(ref this, out distance);
        }

        // Token: 0x06000056 RID: 86 RVA: 0x000051FC File Offset: 0x000033FC
        public bool Intersects(BoundingSphere sphere)
        {
            float num;
            Vector3.DistanceSquared(ref this.Center, ref sphere.Center, out num);
            float radius = this.Radius;
            float radius2 = sphere.Radius;
            return radius * radius + 2f * radius * radius2 + radius2 * radius2 > num;
        }

        // Token: 0x06000057 RID: 87 RVA: 0x00005244 File Offset: 0x00003444
        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            float num;
            Vector3.DistanceSquared(ref this.Center, ref sphere.Center, out num);
            float radius = this.Radius;
            float radius2 = sphere.Radius;
            result = (radius * radius + 2f * radius * radius2 + radius2 * radius2 > num);
        }

        // Token: 0x06000058 RID: 88 RVA: 0x00005288 File Offset: 0x00003488
        public ClipStatus Contains(BoundingBox box)
        {
            if (!box.Intersects(this))
            {
                return ClipStatus.Outside;
            }
            float num = this.Radius * this.Radius;
            Vector3 vector;
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            vector.X = this.Center.X - box.Max.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Min.Y;
            vector.Z = this.Center.Z - box.Min.Z;
            if (vector.LengthSquared() > num)
            {
                return ClipStatus.Intersecting;
            }
            return ClipStatus.Inside;
        }

        // Token: 0x06000059 RID: 89 RVA: 0x00005600 File Offset: 0x00003800
        public void Contains(ref BoundingBox box, out ClipStatus result)
        {
            bool flag;
            box.Intersects(ref this, out flag);
            if (!flag)
            {
                result = ClipStatus.Outside;
                return;
            }
            float num = this.Radius * this.Radius;
            result = ClipStatus.Intersecting;
            Vector3 vector;
            vector.X = this.Center.X - box.Min.X;
            vector.Y = this.Center.Y - box.Max.Y;
            vector.Z = this.Center.Z - box.Max.Z;
            if (vector.LengthSquared() <= num)
            {
                vector.X = this.Center.X - box.Max.X;
                vector.Y = this.Center.Y - box.Max.Y;
                vector.Z = this.Center.Z - box.Max.Z;
                if (vector.LengthSquared() <= num)
                {
                    vector.X = this.Center.X - box.Max.X;
                    vector.Y = this.Center.Y - box.Min.Y;
                    vector.Z = this.Center.Z - box.Max.Z;
                    if (vector.LengthSquared() <= num)
                    {
                        vector.X = this.Center.X - box.Min.X;
                        vector.Y = this.Center.Y - box.Min.Y;
                        vector.Z = this.Center.Z - box.Max.Z;
                        if (vector.LengthSquared() <= num)
                        {
                            vector.X = this.Center.X - box.Min.X;
                            vector.Y = this.Center.Y - box.Max.Y;
                            vector.Z = this.Center.Z - box.Min.Z;
                            if (vector.LengthSquared() <= num)
                            {
                                vector.X = this.Center.X - box.Max.X;
                                vector.Y = this.Center.Y - box.Max.Y;
                                vector.Z = this.Center.Z - box.Min.Z;
                                if (vector.LengthSquared() <= num)
                                {
                                    vector.X = this.Center.X - box.Max.X;
                                    vector.Y = this.Center.Y - box.Min.Y;
                                    vector.Z = this.Center.Z - box.Min.Z;
                                    if (vector.LengthSquared() <= num)
                                    {
                                        vector.X = this.Center.X - box.Min.X;
                                        vector.Y = this.Center.Y - box.Min.Y;
                                        vector.Z = this.Center.Z - box.Min.Z;
                                        if (vector.LengthSquared() <= num)
                                        {
                                            result = ClipStatus.Inside;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x0600005A RID: 90 RVA: 0x00005964 File Offset: 0x00003B64
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
            float num = this.Radius * this.Radius;
            foreach (Vector3 vector in frustum.cornerArray)
            {
                Vector3 vector2;
                vector2.X = vector.X - this.Center.X;
                vector2.Y = vector.Y - this.Center.Y;
                vector2.Z = vector.Z - this.Center.Z;
                if (vector2.LengthSquared() > num)
                {
                    return ClipStatus.Intersecting;
                }
            }
            return ClipStatus.Inside;
        }

        // Token: 0x0600005B RID: 91 RVA: 0x00005A31 File Offset: 0x00003C31
        public ClipStatus Contains(Vector3 point)
        {
            if (Vector3.DistanceSquared(point, this.Center) >= this.Radius * this.Radius)
            {
                return ClipStatus.Outside;
            }
            return ClipStatus.Inside;
        }

        // Token: 0x0600005C RID: 92 RVA: 0x00005A54 File Offset: 0x00003C54
        public void Contains(ref Vector3 point, out ClipStatus result)
        {
            float num;
            Vector3.DistanceSquared(ref point, ref this.Center, out num);
            result = ((num < this.Radius * this.Radius) ? ClipStatus.Inside : ClipStatus.Outside);
        }

        // Token: 0x0600005D RID: 93 RVA: 0x00005A88 File Offset: 0x00003C88
        public ClipStatus Contains(BoundingSphere sphere)
        {
            float num;
            Vector3.Distance(ref this.Center, ref sphere.Center, out num);
            float radius = this.Radius;
            float radius2 = sphere.Radius;
            if (radius + radius2 < num)
            {
                return ClipStatus.Outside;
            }
            if (radius - radius2 < num)
            {
                return ClipStatus.Intersecting;
            }
            return ClipStatus.Inside;
        }

        // Token: 0x0600005E RID: 94 RVA: 0x00005ACC File Offset: 0x00003CCC
        public void Contains(ref BoundingSphere sphere, out ClipStatus result)
        {
            float num;
            Vector3.Distance(ref this.Center, ref sphere.Center, out num);
            float radius = this.Radius;
            float radius2 = sphere.Radius;
            result = ((radius + radius2 >= num) ? ((radius - radius2 >= num) ? ClipStatus.Inside : ClipStatus.Intersecting) : ClipStatus.Outside);
        }

        // Token: 0x0600005F RID: 95 RVA: 0x00005B10 File Offset: 0x00003D10
        internal void SupportMapping(ref Vector3 v, out Vector3 result)
        {
            float num = v.Length();
            float num2 = this.Radius / num;
            result.X = this.Center.X + v.X * num2;
            result.Y = this.Center.Y + v.Y * num2;
            result.Z = this.Center.Z + v.Z * num2;
        }

        // Token: 0x06000060 RID: 96 RVA: 0x00005B7C File Offset: 0x00003D7C
        public BoundingSphere Transform(Matrix44 matrix)
        {
            Vector3 center = Matrix44.TransformPosition(matrix, this.Center);
            float val = matrix.M00 * matrix.M00 + matrix.M01 * matrix.M01 + matrix.M02 * matrix.M02;
            float val2 = matrix.M10 * matrix.M10 + matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12;
            float val3 = matrix.M20 * matrix.M20 + matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22;
            float num = Math.Max(val, Math.Max(val2, val3));
            float radius = this.Radius * (float)Math.Sqrt((double)num);
            return new BoundingSphere(center, radius);
        }

        // Token: 0x06000061 RID: 97 RVA: 0x00005C50 File Offset: 0x00003E50
        public void Transform(ref Matrix44 matrix, out BoundingSphere result)
        {
            result.Center = Matrix44.TransformPosition(matrix, this.Center);
            float val = matrix.M00 * matrix.M00 + matrix.M01 * matrix.M01 + matrix.M02 * matrix.M02;
            float val2 = matrix.M10 * matrix.M10 + matrix.M11 * matrix.M11 + matrix.M12 * matrix.M12;
            float val3 = matrix.M20 * matrix.M20 + matrix.M21 * matrix.M21 + matrix.M22 * matrix.M22;
            float num = Math.Max(val, Math.Max(val2, val3));
            result.Radius = this.Radius * (float)Math.Sqrt((double)num);
        }

        // Token: 0x06000062 RID: 98 RVA: 0x00005D15 File Offset: 0x00003F15
        public static bool operator ==(BoundingSphere a, BoundingSphere b)
        {
            return a.Equals(b);
        }

        // Token: 0x06000063 RID: 99 RVA: 0x00005D1F File Offset: 0x00003F1F
        public static bool operator !=(BoundingSphere a, BoundingSphere b)
        {
            return a.Center != b.Center || a.Radius != b.Radius;
        }

        // Token: 0x04000010 RID: 16
        public Vector3 Center;

        // Token: 0x04000011 RID: 17
        public float Radius;
    }
}
