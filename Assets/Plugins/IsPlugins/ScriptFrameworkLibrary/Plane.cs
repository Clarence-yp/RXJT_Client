using System;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x0200000D RID: 13
    [Serializable]
    public struct Plane : IEquatable<Plane>
    {
        // Token: 0x0600024A RID: 586 RVA: 0x0000DAF6 File Offset: 0x0000BCF6
        public Plane(float a, float b, float c, float d)
        {
            this.Normal.X = a;
            this.Normal.Y = b;
            this.Normal.Z = c;
            this.D = d;
        }

        // Token: 0x0600024B RID: 587 RVA: 0x0000DB24 File Offset: 0x0000BD24
        public Plane(Vector3 normal, float d)
        {
            this.Normal = normal;
            this.D = d;
        }

        // Token: 0x0600024C RID: 588 RVA: 0x0000DB34 File Offset: 0x0000BD34
        public Plane(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            float num = point2.X - point1.X;
            float num2 = point2.Y - point1.Y;
            float num3 = point2.Z - point1.Z;
            float num4 = point3.X - point1.X;
            float num5 = point3.Y - point1.Y;
            float num6 = point3.Z - point1.Z;
            float num7 = num2 * num6 - num3 * num5;
            float num8 = num3 * num4 - num * num6;
            float num9 = num * num5 - num2 * num4;
            float num10 = num7 * num7 + num8 * num8 + num9 * num9;
            float num11 = 1f / (float)Math.Sqrt((double)num10);
            this.Normal.X = num7 * num11;
            this.Normal.Y = num8 * num11;
            this.Normal.Z = num9 * num11;
            this.D = -(this.Normal.X * point1.X + this.Normal.Y * point1.Y + this.Normal.Z * point1.Z);
        }

        // Token: 0x0600024D RID: 589 RVA: 0x0000DC58 File Offset: 0x0000BE58
        public Plane(Vector3 normal, Vector3 point)
        {
            this.Normal = normal;
            this.D = -(this.Normal.X * point.X + this.Normal.Y * point.Y + this.Normal.Z * point.Z);
        }

        // Token: 0x0600024E RID: 590 RVA: 0x0000DCB0 File Offset: 0x0000BEB0
        public bool Equals(Plane other)
        {
            return this.Normal.X == other.Normal.X && this.Normal.Y == other.Normal.Y && this.Normal.Z == other.Normal.Z && this.D == other.D;
        }

        // Token: 0x0600024F RID: 591 RVA: 0x0000DD1C File Offset: 0x0000BF1C
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Plane)
            {
                result = this.Equals((Plane)obj);
            }
            return result;
        }

        // Token: 0x06000250 RID: 592 RVA: 0x0000DD41 File Offset: 0x0000BF41
        public override int GetHashCode()
        {
            return this.Normal.GetHashCode() + this.D.GetHashCode();
        }

        // Token: 0x06000251 RID: 593 RVA: 0x0000DD60 File Offset: 0x0000BF60
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "Normal:{0} D:{1}", new object[]
            {
                this.Normal.ToString(),
                this.D.ToString(currentCulture)
            });
        }

        // Token: 0x06000252 RID: 594 RVA: 0x0000DDAC File Offset: 0x0000BFAC
        public static Plane Transform(Matrix44 matrix, Plane plane)
        {
            Matrix44 matrix2;
            Matrix44.Invert(ref matrix, out matrix2);
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float d = plane.D;
            Plane result;
            result.Normal.X = x * matrix2.M00 + y * matrix2.M10 + z * matrix2.M20 + d * matrix2.M30;
            result.Normal.Y = x * matrix2.M01 + y * matrix2.M11 + z * matrix2.M21 + d * matrix2.M31;
            result.Normal.Z = x * matrix2.M02 + y * matrix2.M12 + z * matrix2.M22 + d * matrix2.M32;
            result.D = x * matrix2.M03 + y * matrix2.M13 + z * matrix2.M23 + d * matrix2.M33;
            return result;
        }

        // Token: 0x06000253 RID: 595 RVA: 0x0000DEC4 File Offset: 0x0000C0C4
        public static void Transform(ref Matrix44 matrix, ref Plane plane, out Plane result)
        {
            Matrix44 matrix2;
            Matrix44.Invert(ref matrix, out matrix2);
            float x = plane.Normal.X;
            float y = plane.Normal.Y;
            float z = plane.Normal.Z;
            float d = plane.D;
            result.Normal.X = x * matrix2.M00 + y * matrix2.M10 + z * matrix2.M20 + d * matrix2.M30;
            result.Normal.Y = x * matrix2.M01 + y * matrix2.M11 + z * matrix2.M21 + d * matrix2.M31;
            result.Normal.Z = x * matrix2.M02 + y * matrix2.M12 + z * matrix2.M22 + d * matrix2.M32;
            result.D = x * matrix2.M03 + y * matrix2.M13 + z * matrix2.M23 + d * matrix2.M33;
        }

        // Token: 0x06000254 RID: 596 RVA: 0x0000DFCC File Offset: 0x0000C1CC
        public PlaneIntersectionStatus Intersects(BoundingBox box)
        {
            Vector3 vector;
            vector.X = ((this.Normal.X >= 0f) ? box.Min.X : box.Max.X);
            vector.Y = ((this.Normal.Y >= 0f) ? box.Min.Y : box.Max.Y);
            vector.Z = ((this.Normal.Z >= 0f) ? box.Min.Z : box.Max.Z);
            Vector3 vector2;
            vector2.X = ((this.Normal.X >= 0f) ? box.Max.X : box.Min.X);
            vector2.Y = ((this.Normal.Y >= 0f) ? box.Max.Y : box.Min.Y);
            vector2.Z = ((this.Normal.Z >= 0f) ? box.Max.Z : box.Min.Z);
            float num = this.Normal.X * vector.X + this.Normal.Y * vector.Y + this.Normal.Z * vector.Z;
            if (num + this.D > 0f)
            {
                return PlaneIntersectionStatus.Front;
            }
            num = this.Normal.X * vector2.X + this.Normal.Y * vector2.Y + this.Normal.Z * vector2.Z;
            if (num + this.D < 0f)
            {
                return PlaneIntersectionStatus.Back;
            }
            return PlaneIntersectionStatus.Intersecting;
        }

        // Token: 0x06000255 RID: 597 RVA: 0x0000E1A8 File Offset: 0x0000C3A8
        public void Intersects(ref BoundingBox box, out PlaneIntersectionStatus result)
        {
            Vector3 vector;
            vector.X = ((this.Normal.X >= 0f) ? box.Min.X : box.Max.X);
            vector.Y = ((this.Normal.Y >= 0f) ? box.Min.Y : box.Max.Y);
            vector.Z = ((this.Normal.Z >= 0f) ? box.Min.Z : box.Max.Z);
            Vector3 vector2;
            vector2.X = ((this.Normal.X >= 0f) ? box.Max.X : box.Min.X);
            vector2.Y = ((this.Normal.Y >= 0f) ? box.Max.Y : box.Min.Y);
            vector2.Z = ((this.Normal.Z >= 0f) ? box.Max.Z : box.Min.Z);
            float num = this.Normal.X * vector.X + this.Normal.Y * vector.Y + this.Normal.Z * vector.Z;
            if (num + this.D > 0f)
            {
                result = PlaneIntersectionStatus.Front;
                return;
            }
            num = this.Normal.X * vector2.X + this.Normal.Y * vector2.Y + this.Normal.Z * vector2.Z;
            if (num + this.D < 0f)
            {
                result = PlaneIntersectionStatus.Back;
                return;
            }
            result = PlaneIntersectionStatus.Intersecting;
        }

        // Token: 0x06000256 RID: 598 RVA: 0x0000E37C File Offset: 0x0000C57C
        public PlaneIntersectionStatus Intersects(BoundingFrustum frustum)
        {
            if (null == frustum)
            {
                throw new ArgumentNullException("frustum", FrameworkResources.NullNotAllowed);
            }
            return frustum.Intersects(this);
        }

        // Token: 0x06000257 RID: 599 RVA: 0x0000E3A4 File Offset: 0x0000C5A4
        public PlaneIntersectionStatus Intersects(BoundingSphere sphere)
        {
            float num = sphere.Center.X * this.Normal.X + sphere.Center.Y * this.Normal.Y + sphere.Center.Z * this.Normal.Z;
            float num2 = num + this.D;
            if (num2 > sphere.Radius)
            {
                return PlaneIntersectionStatus.Front;
            }
            if (num2 < -sphere.Radius)
            {
                return PlaneIntersectionStatus.Back;
            }
            return PlaneIntersectionStatus.Intersecting;
        }

        // Token: 0x06000258 RID: 600 RVA: 0x0000E41F File Offset: 0x0000C61F
        public bool Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }

        // Token: 0x06000259 RID: 601 RVA: 0x0000E430 File Offset: 0x0000C630
        public bool Intersects(ref Ray ray, out float result)
        {
            float num = Vector3.Dot(ray.Direction, this.Normal);
            float num2 = -Vector3.Dot(ray.Position, this.Normal) - this.D;
            result = 0f;
            float num3 = Math.Abs(num);
            if (num3 < 1E-06f)
            {
                return false;
            }
            result = num2 / num;
            return result > 0f;
        }

        // Token: 0x0600025A RID: 602 RVA: 0x0000E490 File Offset: 0x0000C690
        public void Intersects(ref BoundingSphere sphere, out PlaneIntersectionStatus result)
        {
            float num = sphere.Center.X * this.Normal.X + sphere.Center.Y * this.Normal.Y + sphere.Center.Z * this.Normal.Z;
            float num2 = num + this.D;
            if (num2 > sphere.Radius)
            {
                result = PlaneIntersectionStatus.Front;
                return;
            }
            if (num2 < -sphere.Radius)
            {
                result = PlaneIntersectionStatus.Back;
                return;
            }
            result = PlaneIntersectionStatus.Intersecting;
        }

        // Token: 0x0600025B RID: 603 RVA: 0x0000E50C File Offset: 0x0000C70C
        public static bool operator ==(Plane lhs, Plane rhs)
        {
            return lhs.Equals(rhs);
        }

        // Token: 0x0600025C RID: 604 RVA: 0x0000E518 File Offset: 0x0000C718
        public static bool operator !=(Plane lhs, Plane rhs)
        {
            return lhs.Normal.X != rhs.Normal.X || lhs.Normal.Y != rhs.Normal.Y || lhs.Normal.Z != rhs.Normal.Z || lhs.D != rhs.D;
        }

        // Token: 0x04000043 RID: 67
        public Vector3 Normal;

        // Token: 0x04000044 RID: 68
        public float D;
    }
}
