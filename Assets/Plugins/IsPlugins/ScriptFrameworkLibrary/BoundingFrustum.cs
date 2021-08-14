using System;
using System.ComponentModel;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x02000003 RID: 3
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class BoundingFrustum : IEquatable<BoundingFrustum>
    {
        // Token: 0x06000020 RID: 32 RVA: 0x0000399C File Offset: 0x00001B9C
        private BoundingFrustum()
        {
            this.planes = new Plane[6];
            this.cornerArray = new Vector3[8];
        }

        // Token: 0x06000021 RID: 33 RVA: 0x000039BC File Offset: 0x00001BBC
        public BoundingFrustum(Matrix44 value)
        {
            this.planes = new Plane[6];
            this.cornerArray = new Vector3[8];
            this.SetMatrix(ref value);
        }

        // Token: 0x06000022 RID: 34 RVA: 0x000039E4 File Offset: 0x00001BE4
        public ClipStatus Contains(BoundingBox box)
        {
            bool flag = false;
            foreach (Plane plane in this.planes)
            {
                switch (box.Intersects(plane))
                {
                    case PlaneIntersectionStatus.Front:
                        return ClipStatus.Outside;
                    case PlaneIntersectionStatus.Intersecting:
                        flag = true;
                        break;
                }
            }
            if (!flag)
            {
                return ClipStatus.Inside;
            }
            return ClipStatus.Intersecting;
        }

        // Token: 0x06000023 RID: 35 RVA: 0x00003A48 File Offset: 0x00001C48
        public ClipStatus Contains(BoundingFrustum frustum)
        {
            if (frustum == null)
            {
                throw new ArgumentNullException("frustum");
            }
            ClipStatus result = ClipStatus.Outside;
            if (this.Intersects(frustum))
            {
                result = ClipStatus.Inside;
                for (int i = 0; i < this.cornerArray.Length; i++)
                {
                    if (this.Contains(frustum.cornerArray[i]) == ClipStatus.Outside)
                    {
                        return ClipStatus.Intersecting;
                    }
                }
            }
            return result;
        }

        // Token: 0x06000024 RID: 36 RVA: 0x00003AA8 File Offset: 0x00001CA8
        public ClipStatus Contains(BoundingSphere sphere)
        {
            Vector3 center = sphere.Center;
            float radius = sphere.Radius;
            int num = 0;
            foreach (Plane plane in this.planes)
            {
                float num2 = plane.Normal.X * center.X + plane.Normal.Y * center.Y + plane.Normal.Z * center.Z;
                float num3 = num2 + plane.D;
                if (num3 > radius)
                {
                    return ClipStatus.Outside;
                }
                if (num3 < -radius)
                {
                    num++;
                }
            }
            if (num != 6)
            {
                return ClipStatus.Intersecting;
            }
            return ClipStatus.Inside;
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00003B5C File Offset: 0x00001D5C
        public ClipStatus Contains(Vector3 point)
        {
            foreach (Plane plane in this.planes)
            {
                float num = plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.D;
                if (num > 1E-05f)
                {
                    return ClipStatus.Outside;
                }
            }
            return ClipStatus.Inside;
        }

        // Token: 0x06000026 RID: 38 RVA: 0x00003BE8 File Offset: 0x00001DE8
        public void Contains(ref BoundingBox box, out ClipStatus result)
        {
            bool flag = false;
            foreach (Plane plane in this.planes)
            {
                switch (box.Intersects(plane))
                {
                    case PlaneIntersectionStatus.Front:
                        result = ClipStatus.Outside;
                        return;
                    case PlaneIntersectionStatus.Intersecting:
                        flag = true;
                        break;
                }
            }
            result = (flag ? ClipStatus.Intersecting : ClipStatus.Inside);
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00003C48 File Offset: 0x00001E48
        public void Contains(ref BoundingSphere sphere, out ClipStatus result)
        {
            Vector3 center = sphere.Center;
            float radius = sphere.Radius;
            int num = 0;
            foreach (Plane plane in this.planes)
            {
                float num2 = plane.Normal.X * center.X + plane.Normal.Y * center.Y + plane.Normal.Z * center.Z;
                float num3 = num2 + plane.D;
                if (num3 > radius)
                {
                    result = ClipStatus.Outside;
                    return;
                }
                if (num3 < -radius)
                {
                    num++;
                }
            }
            result = ((num == 6) ? ClipStatus.Inside : ClipStatus.Intersecting);
        }

        // Token: 0x06000028 RID: 40 RVA: 0x00003CFC File Offset: 0x00001EFC
        public void Contains(ref Vector3 point, out ClipStatus result)
        {
            foreach (Plane plane in this.planes)
            {
                float num = plane.Normal.X * point.X + plane.Normal.Y * point.Y + plane.Normal.Z * point.Z + plane.D;
                if (num > 1E-05f)
                {
                    result = ClipStatus.Outside;
                    return;
                }
            }
            result = ClipStatus.Inside;
        }

        // Token: 0x06000029 RID: 41 RVA: 0x00003D7F File Offset: 0x00001F7F
        public bool Equals(BoundingFrustum other)
        {
            return !(other == null) && this.matrix == other.matrix;
        }

        // Token: 0x0600002A RID: 42 RVA: 0x00003DA0 File Offset: 0x00001FA0
        public override bool Equals(object obj)
        {
            bool result = false;
            BoundingFrustum boundingFrustum = obj as BoundingFrustum;
            if (boundingFrustum != null)
            {
                result = (this.matrix == boundingFrustum.matrix);
            }
            return result;
        }

        // Token: 0x0600002B RID: 43 RVA: 0x00003DD2 File Offset: 0x00001FD2
        public Vector3[] GetCorners()
        {
            return (Vector3[])this.cornerArray.Clone();
        }

        // Token: 0x0600002C RID: 44 RVA: 0x00003DE4 File Offset: 0x00001FE4
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
            this.cornerArray.CopyTo(corners, 0);
        }

        // Token: 0x0600002D RID: 45 RVA: 0x00003E17 File Offset: 0x00002017
        public override int GetHashCode()
        {
            return this.matrix.GetHashCode();
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00003E2C File Offset: 0x0000202C
        public bool Intersects(BoundingBox box)
        {
            bool result;
            this.Intersects(ref box, out result);
            return result;
        }

        // Token: 0x0600002F RID: 47 RVA: 0x00003E44 File Offset: 0x00002044
        public bool Intersects(BoundingFrustum frustum)
        {
            if (frustum == null)
            {
                throw new ArgumentNullException("frustum");
            }
            if (this.gjk == null)
            {
                this.gjk = new Gjk();
            }
            this.gjk.Reset();
            Vector3 closestPoint;
            Vector3.Sub(ref this.cornerArray[0], ref frustum.cornerArray[0], out closestPoint);
            if (closestPoint.LengthSquared() < 1E-05f)
            {
                Vector3.Sub(ref this.cornerArray[0], ref frustum.cornerArray[1], out closestPoint);
            }
            float num = float.MaxValue;
            for (; ; )
            {
                Vector3 vector;
                vector.X = -closestPoint.X;
                vector.Y = -closestPoint.Y;
                vector.Z = -closestPoint.Z;
                Vector3 vector2;
                this.SupportMapping(ref vector, out vector2);
                Vector3 vector3;
                frustum.SupportMapping(ref closestPoint, out vector3);
                Vector3 vector4;
                Vector3.Sub(ref vector2, ref vector3, out vector4);
                float num2 = closestPoint.X * vector4.X + closestPoint.Y * vector4.Y + closestPoint.Z * vector4.Z;
                if (num2 > 0f)
                {
                    break;
                }
                this.gjk.AddSupportPoint(ref vector4);
                closestPoint = this.gjk.ClosestPoint;
                float num3 = num;
                num = closestPoint.LengthSquared();
                float num4 = 4E-05f * this.gjk.MaxLengthSquared;
                if (num3 - num <= 1E-05f * num3)
                {
                    return false;
                }
                if (this.gjk.FullSimplex || num < num4)
                {
                    return true;
                }
            }
            return false;
        }

        // Token: 0x06000030 RID: 48 RVA: 0x00003FC0 File Offset: 0x000021C0
        public bool Intersects(BoundingSphere sphere)
        {
            bool result;
            this.Intersects(ref sphere, out result);
            return result;
        }

        // Token: 0x06000031 RID: 49 RVA: 0x00003FD8 File Offset: 0x000021D8
        public PlaneIntersectionStatus Intersects(Plane plane)
        {
            int num = 0;
            for (int i = 0; i < 8; i++)
            {
                float num2;
                Vector3.Dot(ref this.cornerArray[i], ref plane.Normal, out num2);
                if (num2 + plane.D > 0f)
                {
                    num |= 1;
                }
                else
                {
                    num |= 2;
                }
                if (num == 3)
                {
                    return PlaneIntersectionStatus.Intersecting;
                }
            }
            if (num != 1)
            {
                return PlaneIntersectionStatus.Back;
            }
            return PlaneIntersectionStatus.Front;
        }

        // Token: 0x06000032 RID: 50 RVA: 0x00004034 File Offset: 0x00002234
        public bool Intersects(Ray ray)
        {
            ClipStatus clipStatus;
            this.Contains(ref ray.Position, out clipStatus);
            if (clipStatus == ClipStatus.Inside)
            {
                return true;
            }
            float num = float.MinValue;
            float num2 = float.MaxValue;
            foreach (Plane plane in this.planes)
            {
                Vector3 normal = plane.Normal;
                float num3;
                Vector3.Dot(ref ray.Direction, ref normal, out num3);
                float num4;
                Vector3.Dot(ref ray.Position, ref normal, out num4);
                num4 += plane.D;
                if (Math.Abs(num3) < 1E-05f)
                {
                    if (num4 > 0f)
                    {
                        return true;
                    }
                }
                else
                {
                    float num5 = -num4 / num3;
                    if (num3 < 0f)
                    {
                        if (num5 > num2)
                        {
                            return true;
                        }
                        if (num5 > num)
                        {
                            num = num5;
                        }
                    }
                    else
                    {
                        if (num5 < num)
                        {
                            return true;
                        }
                        if (num5 < num2)
                        {
                            num2 = num5;
                        }
                    }
                }
            }
            float num6 = (num >= 0f) ? num : num2;
            return num6 >= 0f;
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00004138 File Offset: 0x00002338
        public void Intersects(ref BoundingBox box, out bool result)
        {
            if (this.gjk == null)
            {
                this.gjk = new Gjk();
            }
            this.gjk.Reset();
            Vector3 closestPoint;
            Vector3.Sub(ref this.cornerArray[0], ref box.Min, out closestPoint);
            if (closestPoint.LengthSquared() < 1E-05f)
            {
                Vector3.Sub(ref this.cornerArray[0], ref box.Max, out closestPoint);
            }
            float num = float.MaxValue;
            result = false;
            float num4;
            do
            {
                Vector3 vector;
                vector.X = -closestPoint.X;
                vector.Y = -closestPoint.Y;
                vector.Z = -closestPoint.Z;
                Vector3 vector2;
                this.SupportMapping(ref vector, out vector2);
                Vector3 vector3;
                box.SupportMapping(ref closestPoint, out vector3);
                Vector3 vector4;
                Vector3.Sub(ref vector2, ref vector3, out vector4);
                float num2 = closestPoint.X * vector4.X + closestPoint.Y * vector4.Y + closestPoint.Z * vector4.Z;
                if (num2 > 0f)
                {
                    return;
                }
                this.gjk.AddSupportPoint(ref vector4);
                closestPoint = this.gjk.ClosestPoint;
                float num3 = num;
                num = closestPoint.LengthSquared();
                if (num3 - num <= 1E-05f * num3)
                {
                    return;
                }
                num4 = 4E-05f * this.gjk.MaxLengthSquared;
            }
            while (!this.gjk.FullSimplex && num >= num4);
            result = true;
        }

        // Token: 0x06000034 RID: 52 RVA: 0x0000429C File Offset: 0x0000249C
        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            if (this.gjk == null)
            {
                this.gjk = new Gjk();
            }
            this.gjk.Reset();
            Vector3 vector;
            Vector3.Sub(ref this.cornerArray[0], ref sphere.Center, out vector);
            if (vector.LengthSquared() < 1E-05f)
            {
                vector = Vector3.UnitX;
            }
            float num = float.MaxValue;
            result = false;
            float num4;
            do
            {
                Vector3 vector2;
                vector2.X = -vector.X;
                vector2.Y = -vector.Y;
                vector2.Z = -vector.Z;
                Vector3 vector3;
                this.SupportMapping(ref vector2, out vector3);
                Vector3 vector4;
                sphere.SupportMapping(ref vector, out vector4);
                Vector3 vector5;
                Vector3.Sub(ref vector3, ref vector4, out vector5);
                float num2 = vector.X * vector5.X + vector.Y * vector5.Y + vector.Z * vector5.Z;
                if (num2 > 0f)
                {
                    return;
                }
                this.gjk.AddSupportPoint(ref vector5);
                vector = this.gjk.ClosestPoint;
                float num3 = num;
                num = vector.LengthSquared();
                if (num3 - num <= 1E-05f * num3)
                {
                    return;
                }
                num4 = 4E-05f * this.gjk.MaxLengthSquared;
            }
            while (!this.gjk.FullSimplex && num >= num4);
            result = true;
        }

        // Token: 0x06000035 RID: 53 RVA: 0x000043F0 File Offset: 0x000025F0
        public void Intersects(ref Plane plane, out PlaneIntersectionStatus result)
        {
            int num = 0;
            for (int i = 0; i < 8; i++)
            {
                float num2;
                Vector3.Dot(ref this.cornerArray[i], ref plane.Normal, out num2);
                if (num2 + plane.D > 0f)
                {
                    num |= 1;
                }
                else
                {
                    num |= 2;
                }
                if (num == 3)
                {
                    result = PlaneIntersectionStatus.Intersecting;
                    return;
                }
            }
            result = ((num == 1) ? PlaneIntersectionStatus.Front : PlaneIntersectionStatus.Back);
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00004450 File Offset: 0x00002650
        public bool Intersects(ref Ray ray, out float result)
        {
            ClipStatus clipStatus;
            this.Contains(ref ray.Position, out clipStatus);
            if (clipStatus == ClipStatus.Inside)
            {
                result = 0f;
                return true;
            }
            float num = float.MinValue;
            float num2 = float.MaxValue;
            result = 0f;
            foreach (Plane plane in this.planes)
            {
                Vector3 normal = plane.Normal;
                float num3;
                Vector3.Dot(ref ray.Direction, ref normal, out num3);
                float num4;
                Vector3.Dot(ref ray.Position, ref normal, out num4);
                num4 += plane.D;
                if (Math.Abs(num3) < 1E-05f)
                {
                    if (num4 > 0f)
                    {
                        return true;
                    }
                }
                else
                {
                    float num5 = -num4 / num3;
                    if (num3 < 0f)
                    {
                        if (num5 > num2)
                        {
                            return true;
                        }
                        if (num5 > num)
                        {
                            num = num5;
                        }
                    }
                    else
                    {
                        if (num5 < num)
                        {
                            return true;
                        }
                        if (num5 < num2)
                        {
                            num2 = num5;
                        }
                    }
                }
            }
            float num6 = (num >= 0f) ? num : num2;
            if (num6 >= 0f)
            {
                result = num6;
                return true;
            }
            return false;
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00004562 File Offset: 0x00002762
        public static bool operator ==(BoundingFrustum a, BoundingFrustum b)
        {
            return object.Equals(a, b);
        }

        // Token: 0x06000038 RID: 56 RVA: 0x0000456B File Offset: 0x0000276B
        public static bool operator !=(BoundingFrustum a, BoundingFrustum b)
        {
            return !object.Equals(a, b);
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00004578 File Offset: 0x00002778
        private void SetMatrix(ref Matrix44 value)
        {
            this.matrix = value;
            this.planes[2].Normal.X = -value.M30 - value.M00;
            this.planes[2].Normal.Y = -value.M31 - value.M01;
            this.planes[2].Normal.Z = -value.M32 - value.M02;
            this.planes[2].D = -value.M33 - value.M03;
            this.planes[3].Normal.X = -value.M30 + value.M00;
            this.planes[3].Normal.Y = -value.M31 + value.M01;
            this.planes[3].Normal.Z = -value.M32 + value.M02;
            this.planes[3].D = -value.M33 + value.M03;
            this.planes[4].Normal.X = -value.M30 + value.M10;
            this.planes[4].Normal.Y = -value.M31 + value.M11;
            this.planes[4].Normal.Z = -value.M32 + value.M12;
            this.planes[4].D = -value.M33 + value.M13;
            this.planes[5].Normal.X = -value.M30 - value.M10;
            this.planes[5].Normal.Y = -value.M31 - value.M11;
            this.planes[5].Normal.Z = -value.M32 - value.M12;
            this.planes[5].D = -value.M33 - value.M13;
            this.planes[0].Normal.X = -value.M20;
            this.planes[0].Normal.Y = -value.M21;
            this.planes[0].Normal.Z = -value.M22;
            this.planes[0].D = -value.M23;
            this.planes[1].Normal.X = -value.M30 + value.M20;
            this.planes[1].Normal.Y = -value.M31 + value.M21;
            this.planes[1].Normal.Z = -value.M32 + value.M22;
            this.planes[1].D = -value.M33 + value.M23;
            for (int i = 0; i < 6; i++)
            {
                float num = this.planes[i].Normal.Length();
                this.planes[i].Normal = this.planes[i].Normal / num;
                Plane[] array = this.planes;
                int num2 = i;
                array[num2].D = array[num2].D / num;
            }
            Ray ray = BoundingFrustum.ComputeIntersectionLine(ref this.planes[0], ref this.planes[2]);
            this.cornerArray[0] = BoundingFrustum.ComputeIntersection(ref this.planes[4], ref ray);
            this.cornerArray[3] = BoundingFrustum.ComputeIntersection(ref this.planes[5], ref ray);
            ray = BoundingFrustum.ComputeIntersectionLine(ref this.planes[3], ref this.planes[0]);
            this.cornerArray[1] = BoundingFrustum.ComputeIntersection(ref this.planes[4], ref ray);
            this.cornerArray[2] = BoundingFrustum.ComputeIntersection(ref this.planes[5], ref ray);
            ray = BoundingFrustum.ComputeIntersectionLine(ref this.planes[2], ref this.planes[1]);
            this.cornerArray[4] = BoundingFrustum.ComputeIntersection(ref this.planes[4], ref ray);
            this.cornerArray[7] = BoundingFrustum.ComputeIntersection(ref this.planes[5], ref ray);
            ray = BoundingFrustum.ComputeIntersectionLine(ref this.planes[1], ref this.planes[3]);
            this.cornerArray[5] = BoundingFrustum.ComputeIntersection(ref this.planes[4], ref ray);
            this.cornerArray[6] = BoundingFrustum.ComputeIntersection(ref this.planes[5], ref ray);
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00004AB4 File Offset: 0x00002CB4
        private static Vector3 ComputeIntersection(ref Plane plane, ref Ray ray)
        {
            float scaleFactor = (-plane.D - Vector3.Dot(plane.Normal, ray.Position)) / Vector3.Dot(plane.Normal, ray.Direction);
            return ray.Position + ray.Direction * scaleFactor;
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00004B04 File Offset: 0x00002D04
        private static Ray ComputeIntersectionLine(ref Plane p1, ref Plane p2)
        {
            Vector3 vector = Vector3.Cross(p1.Normal, p2.Normal);
            float divider = vector.LengthSquared();
            Vector3 direction = Vector3.Cross(-p1.D * p2.Normal + p2.D * p1.Normal, vector) / divider;
            return new Ray(vector, direction);
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00004B68 File Offset: 0x00002D68
        internal void SupportMapping(ref Vector3 v, out Vector3 result)
        {
            int num = 0;
            float num2;
            Vector3.Dot(ref this.cornerArray[0], ref v, out num2);
            for (int i = 1; i < this.cornerArray.Length; i++)
            {
                float num3;
                Vector3.Dot(ref this.cornerArray[i], ref v, out num3);
                if (num3 > num2)
                {
                    num = i;
                    num2 = num3;
                }
            }
            result = this.cornerArray[num];
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00004BD4 File Offset: 0x00002DD4
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "N:{0} F:{1} L:{2} R:{3} T:{4} B:{5}", new object[]
            {
                this.Near.ToString(),
                this.Far.ToString(),
                this.Left.ToString(),
                this.Right.ToString(),
                this.Top.ToString(),
                this.Bottom.ToString()
            });
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x0600003E RID: 62 RVA: 0x00004C85 File Offset: 0x00002E85
        public Plane Bottom {
            get {
                return this.planes[5];
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x0600003F RID: 63 RVA: 0x00004C98 File Offset: 0x00002E98
        public Plane Far {
            get {
                return this.planes[1];
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000040 RID: 64 RVA: 0x00004CAB File Offset: 0x00002EAB
        public Plane Left {
            get {
                return this.planes[2];
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000041 RID: 65 RVA: 0x00004CBE File Offset: 0x00002EBE
        // (set) Token: 0x06000042 RID: 66 RVA: 0x00004CC6 File Offset: 0x00002EC6
        public Matrix44 Matrix {
            get {
                return this.matrix;
            }
            set {
                this.SetMatrix(ref value);
            }
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x06000043 RID: 67 RVA: 0x00004CD0 File Offset: 0x00002ED0
        public Plane Near {
            get {
                return this.planes[0];
            }
        }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000044 RID: 68 RVA: 0x00004CE3 File Offset: 0x00002EE3
        public Plane Right {
            get {
                return this.planes[3];
            }
        }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000045 RID: 69 RVA: 0x00004CF6 File Offset: 0x00002EF6
        public Plane Top {
            get {
                return this.planes[4];
            }
        }

        // Token: 0x04000004 RID: 4
        public const int CornerCount = 8;

        // Token: 0x04000005 RID: 5
        private const int BottomPlaneIndex = 5;

        // Token: 0x04000006 RID: 6
        private const int FarPlaneIndex = 1;

        // Token: 0x04000007 RID: 7
        private const int LeftPlaneIndex = 2;

        // Token: 0x04000008 RID: 8
        private const int NearPlaneIndex = 0;

        // Token: 0x04000009 RID: 9
        private const int NumPlanes = 6;

        // Token: 0x0400000A RID: 10
        private const int RightPlaneIndex = 3;

        // Token: 0x0400000B RID: 11
        private const int TopPlaneIndex = 4;

        // Token: 0x0400000C RID: 12
        internal Vector3[] cornerArray;

        // Token: 0x0400000D RID: 13
        private Gjk gjk;

        // Token: 0x0400000E RID: 14
        private Matrix44 matrix;

        // Token: 0x0400000F RID: 15
        private Plane[] planes;
    }
}
