using System;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x02000010 RID: 16
    [Serializable]
    public struct Ray : IEquatable<Ray>
    {
        // Token: 0x0600028C RID: 652 RVA: 0x00010193 File Offset: 0x0000E393
        public Ray(Vector3 position, Vector3 direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        // Token: 0x0600028D RID: 653 RVA: 0x000101A4 File Offset: 0x0000E3A4
        public bool Equals(Ray other)
        {
            return this.Position.X == other.Position.X && this.Position.Y == other.Position.Y && this.Position.Z == other.Position.Z && this.Direction.X == other.Direction.X && this.Direction.Y == other.Direction.Y && this.Direction.Z == other.Direction.Z;
        }

        // Token: 0x0600028E RID: 654 RVA: 0x0001024C File Offset: 0x0000E44C
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj != null && obj is Ray)
            {
                result = this.Equals((Ray)obj);
            }
            return result;
        }

        // Token: 0x0600028F RID: 655 RVA: 0x00010274 File Offset: 0x0000E474
        public override int GetHashCode()
        {
            return this.Position.GetHashCode() + this.Direction.GetHashCode();
        }

        // Token: 0x06000290 RID: 656 RVA: 0x0001029C File Offset: 0x0000E49C
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "P:{0}, D:{1}", new object[]
            {
                this.Position.ToString(),
                this.Direction.ToString()
            });
        }

        // Token: 0x06000291 RID: 657 RVA: 0x000102E8 File Offset: 0x0000E4E8
        public bool Intersects(BoundingBox box)
        {
            return box.Intersects(this);
        }

        // Token: 0x06000292 RID: 658 RVA: 0x000102F7 File Offset: 0x0000E4F7
        public bool Intersects(ref BoundingBox box, out float distance)
        {
            return box.Intersects(ref this, out distance);
        }

        // Token: 0x06000293 RID: 659 RVA: 0x00010301 File Offset: 0x0000E501
        public bool Intersects(BoundingFrustum frustum)
        {
            if (frustum == null)
            {
                throw new ArgumentNullException("frustum");
            }
            return frustum.Intersects(this);
        }

        // Token: 0x06000294 RID: 660 RVA: 0x00010324 File Offset: 0x0000E524
        public bool Intersects(BoundingSphere sphere)
        {
            float num = sphere.Center.X - this.Position.X;
            float num2 = sphere.Center.Y - this.Position.Y;
            float num3 = sphere.Center.Z - this.Position.Z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            float num5 = sphere.Radius * sphere.Radius;
            if (num4 <= num5)
            {
                return true;
            }
            float num6 = num * this.Direction.X + num2 * this.Direction.Y + num3 * this.Direction.Z;
            if (num6 < 0f)
            {
                return false;
            }
            float num7 = num4 - num6 * num6;
            return num7 <= num5;
        }

        // Token: 0x06000295 RID: 661 RVA: 0x000103E8 File Offset: 0x0000E5E8
        public bool Intersects(ref BoundingSphere sphere, out float distance)
        {
            distance = 0f;
            float num = sphere.Center.X - this.Position.X;
            float num2 = sphere.Center.Y - this.Position.Y;
            float num3 = sphere.Center.Z - this.Position.Z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            float num5 = sphere.Radius * sphere.Radius;
            if (num4 <= num5)
            {
                return true;
            }
            distance = 0f;
            float num6 = num * this.Direction.X + num2 * this.Direction.Y + num3 * this.Direction.Z;
            if (num6 >= 0f)
            {
                float num7 = num4 - num6 * num6;
                if (num7 <= num5)
                {
                    float num8 = (float)Math.Sqrt((double)(num5 - num7));
                    distance = num6 - num8;
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06000296 RID: 662 RVA: 0x000104C8 File Offset: 0x0000E6C8
        public bool Intersects(Plane plane)
        {
            float num = plane.Normal.X * this.Direction.X + plane.Normal.Y * this.Direction.Y + plane.Normal.Z * this.Direction.Z;
            if (Math.Abs(num) < 1E-05f)
            {
                return false;
            }
            float num2 = plane.Normal.X * this.Position.X + plane.Normal.Y * this.Position.Y + plane.Normal.Z * this.Position.Z;
            float num3 = (-plane.D - num2) / num;
            return num3 >= 0f || num3 >= -1E-05f;
        }

        // Token: 0x06000297 RID: 663 RVA: 0x0001059A File Offset: 0x0000E79A
        public bool Intersects(ref Plane plane, out float distance)
        {
            return plane.Intersects(ref this, out distance);
        }

        // Token: 0x06000298 RID: 664 RVA: 0x000105A4 File Offset: 0x0000E7A4
        public static bool operator ==(Ray a, Ray b)
        {
            return a.Position.X == b.Position.X && a.Position.Y == b.Position.Y && a.Position.Z == b.Position.Z && a.Direction.X == b.Direction.X && a.Direction.Y == b.Direction.Y && a.Direction.Z == b.Direction.Z;
        }

        // Token: 0x06000299 RID: 665 RVA: 0x00010654 File Offset: 0x0000E854
        public static bool operator !=(Ray a, Ray b)
        {
            return a.Position.X != b.Position.X || a.Position.Y != b.Position.Y || a.Position.Z != b.Position.Z || a.Direction.X != b.Direction.X || a.Direction.Y != b.Direction.Y || a.Direction.Z != b.Direction.Z;
        }

        // Token: 0x0400004E RID: 78
        public Vector3 Position;

        // Token: 0x0400004F RID: 79
        public Vector3 Direction;
    }
}
