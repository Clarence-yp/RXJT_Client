using System;
using System.Globalization;

namespace ScriptRuntime
{
    // Token: 0x0200000B RID: 11
    [Serializable]
    public struct Matrix44 : IEquatable<Matrix44>
    {
        // Token: 0x1700014C RID: 332
        // (get) Token: 0x060001F8 RID: 504 RVA: 0x00008DE0 File Offset: 0x00006FE0
        public static Matrix44 Identity {
            get {
                return Matrix44._identity;
            }
        }

        // Token: 0x1700014D RID: 333
        // (get) Token: 0x060001F9 RID: 505 RVA: 0x00008DE8 File Offset: 0x00006FE8
        // (set) Token: 0x060001FA RID: 506 RVA: 0x00008E1D File Offset: 0x0000701D
        public Vector3 Up {
            get {
                Vector3 result;
                result.X = this.M01;
                result.Y = this.M11;
                result.Z = this.M21;
                return result;
            }
            set {
                this.M01 = value.X;
                this.M11 = value.Y;
                this.M21 = value.Z;
            }
        }

        // Token: 0x1700014E RID: 334
        // (get) Token: 0x060001FB RID: 507 RVA: 0x00008E48 File Offset: 0x00007048
        // (set) Token: 0x060001FC RID: 508 RVA: 0x00008E80 File Offset: 0x00007080
        public Vector3 Down {
            get {
                Vector3 result;
                result.X = -this.M01;
                result.Y = -this.M11;
                result.Z = -this.M21;
                return result;
            }
            set {
                this.M01 = -value.X;
                this.M11 = -value.Y;
                this.M21 = -value.Z;
            }
        }

        // Token: 0x1700014F RID: 335
        // (get) Token: 0x060001FD RID: 509 RVA: 0x00008EAC File Offset: 0x000070AC
        // (set) Token: 0x060001FE RID: 510 RVA: 0x00008EE1 File Offset: 0x000070E1
        public Vector3 Right {
            get {
                Vector3 result;
                result.X = this.M00;
                result.Y = this.M10;
                result.Z = this.M20;
                return result;
            }
            set {
                this.M00 = value.X;
                this.M10 = value.Y;
                this.M20 = value.Z;
            }
        }

        // Token: 0x17000150 RID: 336
        // (get) Token: 0x060001FF RID: 511 RVA: 0x00008F0C File Offset: 0x0000710C
        // (set) Token: 0x06000200 RID: 512 RVA: 0x00008F44 File Offset: 0x00007144
        public Vector3 Left {
            get {
                Vector3 result;
                result.X = -this.M00;
                result.Y = -this.M10;
                result.Z = -this.M20;
                return result;
            }
            set {
                this.M00 = -value.X;
                this.M10 = -value.Y;
                this.M20 = -value.Z;
            }
        }

        // Token: 0x17000151 RID: 337
        // (get) Token: 0x06000201 RID: 513 RVA: 0x00008F70 File Offset: 0x00007170
        // (set) Token: 0x06000202 RID: 514 RVA: 0x00008FA8 File Offset: 0x000071A8
        public Vector3 Forward {
            get {
                Vector3 result;
                result.X = -this.M02;
                result.Y = -this.M12;
                result.Z = -this.M22;
                return result;
            }
            set {
                this.M02 = -value.X;
                this.M12 = -value.Y;
                this.M22 = -value.Z;
            }
        }

        // Token: 0x17000152 RID: 338
        // (get) Token: 0x06000203 RID: 515 RVA: 0x00008FD4 File Offset: 0x000071D4
        // (set) Token: 0x06000204 RID: 516 RVA: 0x00009009 File Offset: 0x00007209
        public Vector3 Backward {
            get {
                Vector3 result;
                result.X = this.M02;
                result.Y = this.M12;
                result.Z = this.M22;
                return result;
            }
            set {
                this.M02 = value.X;
                this.M12 = value.Y;
                this.M22 = value.Z;
            }
        }

        // Token: 0x17000153 RID: 339
        public unsafe float this[int row, int col] {
            get {
                fixed (float* ptr = &this.M00)
                {
                    return ptr[row * 4 + col];
                }
            }
            set {
                fixed (float* ptr = &this.M00)
                {
                    ptr[row * 4 + col] = value;
                }
            }
        }

        // Token: 0x17000154 RID: 340
        public unsafe float this[int index] {
            get {
                fixed (float* ptr = &this.M00)
                {
                    return ptr[index];
                }
            }
            set {
                fixed (float* ptr = &this.M00)
                {
                    ptr[index] = value;
                }
            }
        }

        // Token: 0x06000209 RID: 521 RVA: 0x000090BC File Offset: 0x000072BC
        public Vector4 GetRow(int index)
        {
            Vector4 result;
            result.X = this[index, 0];
            result.Y = this[index, 1];
            result.Z = this[index, 2];
            result.W = this[index, 3];
            return result;
        }

        // Token: 0x0600020A RID: 522 RVA: 0x00009106 File Offset: 0x00007306
        public void SetRow(int index, Vector4 value)
        {
            this[index, 0] = value.X;
            this[index, 1] = value.Y;
            this[index, 2] = value.Z;
            this[index, 3] = value.W;
        }

        // Token: 0x0600020B RID: 523 RVA: 0x00009144 File Offset: 0x00007344
        public Vector4 GetColumn(int index)
        {
            Vector4 result;
            result.X = this[0, index];
            result.Y = this[1, index];
            result.Z = this[2, index];
            result.W = this[3, index];
            return result;
        }

        // Token: 0x0600020C RID: 524 RVA: 0x0000918E File Offset: 0x0000738E
        public void SetColumn(int index, Vector4 value)
        {
            this[0, index] = value.X;
            this[1, index] = value.Y;
            this[2, index] = value.Z;
            this[3, index] = value.W;
        }

        // Token: 0x0600020D RID: 525 RVA: 0x000091CC File Offset: 0x000073CC
        public Matrix44(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
        {
            this.M00 = m00;
            this.M01 = m01;
            this.M02 = m02;
            this.M03 = m03;
            this.M10 = m10;
            this.M11 = m11;
            this.M12 = m12;
            this.M13 = m13;
            this.M20 = m20;
            this.M21 = m21;
            this.M22 = m22;
            this.M23 = m23;
            this.M30 = m30;
            this.M31 = m31;
            this.M32 = m32;
            this.M33 = m33;
        }

        // Token: 0x0600020E RID: 526 RVA: 0x00009258 File Offset: 0x00007458
        public static Matrix44 CreateTranslation(Vector3 position)
        {
            Matrix44 result;
            result.M00 = 1f;
            result.M01 = 0f;
            result.M02 = 0f;
            result.M03 = position.X;
            result.M10 = 0f;
            result.M11 = 1f;
            result.M12 = 0f;
            result.M13 = position.Y;
            result.M20 = 0f;
            result.M21 = 0f;
            result.M22 = 1f;
            result.M23 = position.Z;
            result.M30 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 1f;
            return result;
        }

        // Token: 0x0600020F RID: 527 RVA: 0x0000932C File Offset: 0x0000752C
        public static void CreateTranslation(ref Vector3 position, out Matrix44 matrix)
        {
            matrix.M00 = 1f;
            matrix.M01 = 0f;
            matrix.M02 = 0f;
            matrix.M03 = position.X;
            matrix.M10 = 0f;
            matrix.M11 = 1f;
            matrix.M12 = 0f;
            matrix.M13 = position.Y;
            matrix.M20 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = 1f;
            matrix.M23 = position.Z;
            matrix.M30 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 1f;
        }

        // Token: 0x06000210 RID: 528 RVA: 0x000093EC File Offset: 0x000075EC
        public static Matrix44 CreateScale(Vector3 scales)
        {
            Matrix44 result;
            result.M00 = scales.X;
            result.M01 = 0f;
            result.M02 = 0f;
            result.M03 = 0f;
            result.M10 = 0f;
            result.M11 = scales.Y;
            result.M12 = 0f;
            result.M13 = 0f;
            result.M20 = 0f;
            result.M21 = 0f;
            result.M22 = scales.Z;
            result.M23 = 0f;
            result.M30 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 1f;
            return result;
        }

        // Token: 0x06000211 RID: 529 RVA: 0x000094C0 File Offset: 0x000076C0
        public static void CreateScale(ref Vector3 scales, out Matrix44 matrix)
        {
            matrix.M00 = scales.X;
            matrix.M01 = 0f;
            matrix.M02 = 0f;
            matrix.M03 = 0f;
            matrix.M10 = 0f;
            matrix.M11 = scales.Y;
            matrix.M12 = 0f;
            matrix.M13 = 0f;
            matrix.M20 = 0f;
            matrix.M21 = 0f;
            matrix.M22 = scales.Z;
            matrix.M23 = 0f;
            matrix.M30 = 0f;
            matrix.M31 = 0f;
            matrix.M32 = 0f;
            matrix.M33 = 1f;
        }

        // Token: 0x06000212 RID: 530 RVA: 0x00009580 File Offset: 0x00007780
        public static Matrix44 CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            if (fieldOfView <= 0f || fieldOfView >= 3.141593f)
            {
                throw new ArgumentOutOfRangeException("fieldOfView", string.Format(CultureInfo.CurrentCulture, FrameworkResources.OutRangeFieldOfView, new object[]
                {
                    "fieldOfView"
                }));
            }
            if (nearPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "nearPlaneDistance"
                }));
            }
            if (farPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "farPlaneDistance"
                }));
            }
            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", FrameworkResources.OppositePlanes);
            }
            float num = 1f / (float)Math.Tan((double)(fieldOfView * 0.5f));
            float m = num / aspectRatio;
            Matrix44 result;
            result.M00 = m;
            result.M10 = (result.M20 = (result.M30 = 0f));
            result.M11 = num;
            result.M01 = (result.M21 = (result.M31 = 0f));
            result.M02 = (result.M12 = 0f);
            result.M22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M32 = -1f;
            result.M03 = (result.M13 = (result.M33 = 0f));
            result.M23 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            return result;
        }

        // Token: 0x06000213 RID: 531 RVA: 0x0000971C File Offset: 0x0000791C
        public static void CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, out Matrix44 matrix)
        {
            if (fieldOfView <= 0f || fieldOfView >= 3.141593f)
            {
                throw new ArgumentOutOfRangeException("fieldOfView", string.Format(CultureInfo.CurrentCulture, FrameworkResources.OutRangeFieldOfView, new object[]
                {
                    "fieldOfView"
                }));
            }
            if (nearPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "nearPlaneDistance"
                }));
            }
            if (farPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "farPlaneDistance"
                }));
            }
            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", FrameworkResources.OppositePlanes);
            }
            float num = 1f / (float)Math.Tan((double)(fieldOfView * 0.5f));
            float m = num / aspectRatio;
            matrix.M00 = m;
            matrix.M10 = (matrix.M20 = (matrix.M30 = 0f));
            matrix.M11 = num;
            matrix.M01 = (matrix.M21 = (matrix.M31 = 0f));
            matrix.M02 = (matrix.M12 = 0f);
            matrix.M22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            matrix.M32 = -1f;
            matrix.M03 = (matrix.M13 = (matrix.M33 = 0f));
            matrix.M23 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        }

        // Token: 0x06000214 RID: 532 RVA: 0x000098B4 File Offset: 0x00007AB4
        public static Matrix44 CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
        {
            if (nearPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "nearPlaneDistance"
                }));
            }
            if (farPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "farPlaneDistance"
                }));
            }
            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", FrameworkResources.OppositePlanes);
            }
            Matrix44 result;
            result.M00 = 2f * nearPlaneDistance / width;
            result.M10 = (result.M20 = (result.M30 = 0f));
            result.M11 = 2f * nearPlaneDistance / height;
            result.M01 = (result.M21 = (result.M31 = 0f));
            result.M22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M02 = (result.M12 = 0f);
            result.M32 = -1f;
            result.M03 = (result.M13 = (result.M33 = 0f));
            result.M23 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            return result;
        }

        // Token: 0x06000215 RID: 533 RVA: 0x00009A04 File Offset: 0x00007C04
        public static void CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance, out Matrix44 matrix)
        {
            if (nearPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "nearPlaneDistance"
                }));
            }
            if (farPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("farPlaneDistance", string.Format(CultureInfo.CurrentCulture, FrameworkResources.NegativePlaneDistance, new object[]
                {
                    "farPlaneDistance"
                }));
            }
            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance", FrameworkResources.OppositePlanes);
            }
            matrix.M00 = 2f * nearPlaneDistance / width;
            matrix.M10 = (matrix.M20 = (matrix.M30 = 0f));
            matrix.M11 = 2f * nearPlaneDistance / height;
            matrix.M01 = (matrix.M21 = (matrix.M31 = 0f));
            matrix.M22 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            matrix.M02 = (matrix.M12 = 0f);
            matrix.M32 = -1f;
            matrix.M03 = (matrix.M13 = (matrix.M33 = 0f));
            matrix.M23 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
        }

        // Token: 0x06000216 RID: 534 RVA: 0x00009B50 File Offset: 0x00007D50
        public static Matrix44 CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
        {
            Matrix44 result;
            result.M00 = 2f / width;
            result.M10 = (result.M20 = (result.M30 = 0f));
            result.M11 = 2f / height;
            result.M01 = (result.M21 = (result.M31 = 0f));
            result.M22 = 1f / (zNearPlane - zFarPlane);
            result.M02 = (result.M12 = (result.M32 = 0f));
            result.M03 = (result.M13 = 0f);
            result.M23 = zNearPlane / (zNearPlane - zFarPlane);
            result.M33 = 1f;
            return result;
        }

        // Token: 0x06000217 RID: 535 RVA: 0x00009C20 File Offset: 0x00007E20
        public static void CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane, out Matrix44 matrix)
        {
            matrix.M00 = 2f / width;
            matrix.M10 = (matrix.M20 = (matrix.M30 = 0f));
            matrix.M11 = 2f / height;
            matrix.M01 = (matrix.M21 = (matrix.M31 = 0f));
            matrix.M22 = 1f / (zNearPlane - zFarPlane);
            matrix.M02 = (matrix.M12 = (matrix.M32 = 0f));
            matrix.M03 = (matrix.M13 = 0f);
            matrix.M23 = zNearPlane / (zNearPlane - zFarPlane);
            matrix.M33 = 1f;
        }

        // Token: 0x06000218 RID: 536 RVA: 0x00009CF0 File Offset: 0x00007EF0
        public static Matrix44 CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Vector3 vector = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            Matrix44 result;
            result.M00 = vector2.X;
            result.M10 = vector3.X;
            result.M20 = vector.X;
            result.M30 = 0f;
            result.M01 = vector2.Y;
            result.M11 = vector3.Y;
            result.M21 = vector.Y;
            result.M31 = 0f;
            result.M02 = vector2.Z;
            result.M12 = vector3.Z;
            result.M22 = vector.Z;
            result.M32 = 0f;
            result.M03 = -Vector3.Dot(vector2, cameraPosition);
            result.M13 = -Vector3.Dot(vector3, cameraPosition);
            result.M23 = -Vector3.Dot(vector, cameraPosition);
            result.M33 = 1f;
            return result;
        }

        // Token: 0x06000219 RID: 537 RVA: 0x00009DFC File Offset: 0x00007FFC
        public static void CreateLookAt(ref Vector3 cameraPosition, ref Vector3 cameraTarget, ref Vector3 cameraUpVector, out Matrix44 matrix)
        {
            Vector3 vector = Vector3.Normalize(cameraPosition - cameraTarget);
            Vector3 vector2 = Vector3.Normalize(Vector3.Cross(cameraUpVector, vector));
            Vector3 vector3 = Vector3.Cross(vector, vector2);
            matrix.M00 = vector2.X;
            matrix.M10 = vector3.X;
            matrix.M20 = vector.X;
            matrix.M30 = 0f;
            matrix.M01 = vector2.Y;
            matrix.M11 = vector3.Y;
            matrix.M21 = vector.Y;
            matrix.M31 = 0f;
            matrix.M02 = vector2.Z;
            matrix.M12 = vector3.Z;
            matrix.M22 = vector.Z;
            matrix.M32 = 0f;
            matrix.M03 = -Vector3.Dot(vector2, cameraPosition);
            matrix.M13 = -Vector3.Dot(vector3, cameraPosition);
            matrix.M23 = -Vector3.Dot(vector, cameraPosition);
            matrix.M33 = 1f;
        }

        // Token: 0x0600021A RID: 538 RVA: 0x00009F14 File Offset: 0x00008114
        public static Matrix44 CreateFromQuaternion(Quaternion quaternion)
        {
            float num = quaternion.X * quaternion.X;
            float num2 = quaternion.Y * quaternion.Y;
            float num3 = quaternion.Z * quaternion.Z;
            float num4 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num6 = quaternion.Z * quaternion.X;
            float num7 = quaternion.Y * quaternion.W;
            float num8 = quaternion.Y * quaternion.Z;
            float num9 = quaternion.X * quaternion.W;
            Matrix44 result;
            result.M00 = 1f - 2f * (num2 + num3);
            result.M10 = 2f * (num4 + num5);
            result.M20 = 2f * (num6 - num7);
            result.M30 = 0f;
            result.M01 = 2f * (num4 - num5);
            result.M11 = 1f - 2f * (num3 + num);
            result.M21 = 2f * (num8 + num9);
            result.M31 = 0f;
            result.M02 = 2f * (num6 + num7);
            result.M12 = 2f * (num8 - num9);
            result.M22 = 1f - 2f * (num2 + num);
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
            return result;
        }

        // Token: 0x0600021B RID: 539 RVA: 0x0000A0BC File Offset: 0x000082BC
        public static void CreateFromQuaternion(ref Quaternion quaternion, out Matrix44 matrix)
        {
            float num = quaternion.X * quaternion.X;
            float num2 = quaternion.Y * quaternion.Y;
            float num3 = quaternion.Z * quaternion.Z;
            float num4 = quaternion.X * quaternion.Y;
            float num5 = quaternion.Z * quaternion.W;
            float num6 = quaternion.Z * quaternion.X;
            float num7 = quaternion.Y * quaternion.W;
            float num8 = quaternion.Y * quaternion.Z;
            float num9 = quaternion.X * quaternion.W;
            matrix.M00 = 1f - 2f * (num2 + num3);
            matrix.M10 = 2f * (num4 + num5);
            matrix.M20 = 2f * (num6 - num7);
            matrix.M30 = 0f;
            matrix.M01 = 2f * (num4 - num5);
            matrix.M11 = 1f - 2f * (num3 + num);
            matrix.M21 = 2f * (num8 + num9);
            matrix.M31 = 0f;
            matrix.M02 = 2f * (num6 + num7);
            matrix.M12 = 2f * (num8 - num9);
            matrix.M22 = 1f - 2f * (num2 + num);
            matrix.M32 = 0f;
            matrix.M03 = 0f;
            matrix.M13 = 0f;
            matrix.M23 = 0f;
            matrix.M33 = 1f;
        }

        // Token: 0x0600021C RID: 540 RVA: 0x0000A23C File Offset: 0x0000843C
        public static Matrix44 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Quaternion quaternion;
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            return Matrix44.CreateFromQuaternion(quaternion);
        }

        // Token: 0x0600021D RID: 541 RVA: 0x0000A25C File Offset: 0x0000845C
        public static void CreateFromYawPitchRoll(float yaw, float pitch, float roll, out Matrix44 result)
        {
            Quaternion quaternion;
            Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
            result = Matrix44.CreateFromQuaternion(quaternion);
        }

        // Token: 0x0600021E RID: 542 RVA: 0x0000A280 File Offset: 0x00008480
        public static Matrix44 CreateRotationX(float radians)
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            Matrix44 result;
            result.M00 = 1f;
            result.M10 = 0f;
            result.M20 = 0f;
            result.M30 = 0f;
            result.M01 = 0f;
            result.M11 = num;
            result.M21 = num2;
            result.M31 = 0f;
            result.M02 = 0f;
            result.M12 = -num2;
            result.M22 = num;
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
            return result;
        }

        // Token: 0x0600021F RID: 543 RVA: 0x0000A354 File Offset: 0x00008554
        public static void CreateRotationX(float radians, out Matrix44 result)
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            result.M00 = 1f;
            result.M10 = 0f;
            result.M20 = 0f;
            result.M30 = 0f;
            result.M01 = 0f;
            result.M11 = num;
            result.M21 = num2;
            result.M31 = 0f;
            result.M02 = 0f;
            result.M12 = -num2;
            result.M22 = num;
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
        }

        // Token: 0x06000220 RID: 544 RVA: 0x0000A414 File Offset: 0x00008614
        public static Matrix44 CreateRotationY(float radians)
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            Matrix44 result;
            result.M00 = num;
            result.M10 = 0f;
            result.M20 = -num2;
            result.M30 = 0f;
            result.M01 = 0f;
            result.M11 = 1f;
            result.M21 = 0f;
            result.M31 = 0f;
            result.M02 = num2;
            result.M12 = 0f;
            result.M22 = num;
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
            return result;
        }

        // Token: 0x06000221 RID: 545 RVA: 0x0000A4E8 File Offset: 0x000086E8
        public static void CreateRotationY(float radians, out Matrix44 result)
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            result.M00 = num;
            result.M10 = 0f;
            result.M20 = -num2;
            result.M30 = 0f;
            result.M01 = 0f;
            result.M11 = 1f;
            result.M21 = 0f;
            result.M31 = 0f;
            result.M02 = num2;
            result.M12 = 0f;
            result.M22 = num;
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
        }

        // Token: 0x06000222 RID: 546 RVA: 0x0000A5A8 File Offset: 0x000087A8
        public static Matrix44 CreateRotationZ(float radians)
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            Matrix44 result;
            result.M00 = num;
            result.M10 = num2;
            result.M20 = 0f;
            result.M30 = 0f;
            result.M01 = -num2;
            result.M11 = num;
            result.M21 = 0f;
            result.M31 = 0f;
            result.M02 = 0f;
            result.M12 = 0f;
            result.M22 = 1f;
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
            return result;
        }

        // Token: 0x06000223 RID: 547 RVA: 0x0000A67C File Offset: 0x0000887C
        public static void CreateRotationZ(float radians, out Matrix44 result)
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            result.M00 = num;
            result.M10 = num2;
            result.M20 = 0f;
            result.M30 = 0f;
            result.M01 = -num2;
            result.M11 = num;
            result.M21 = 0f;
            result.M31 = 0f;
            result.M02 = 0f;
            result.M12 = 0f;
            result.M22 = 1f;
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
        }

        // Token: 0x06000224 RID: 548 RVA: 0x0000A73C File Offset: 0x0000893C
        public static Matrix44 CreateFromAxisAngle(Vector3 axis, float angle)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num = (float)Math.Sin((double)angle);
            float num2 = (float)Math.Cos((double)angle);
            float num3 = x * x;
            float num4 = y * y;
            float num5 = z * z;
            float num6 = x * y;
            float num7 = x * z;
            float num8 = y * z;
            Matrix44 result;
            result.M00 = num3 + num2 * (1f - num3);
            result.M10 = num6 - num2 * num6 + num * z;
            result.M20 = num7 - num2 * num7 - num * y;
            result.M30 = 0f;
            result.M01 = num6 - num2 * num6 - num * z;
            result.M11 = num4 + num2 * (1f - num4);
            result.M21 = num8 - num2 * num8 + num * x;
            result.M31 = 0f;
            result.M02 = num7 - num2 * num7 + num * y;
            result.M12 = num8 - num2 * num8 - num * x;
            result.M22 = num5 + num2 * (1f - num5);
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
            return result;
        }

        // Token: 0x06000225 RID: 549 RVA: 0x0000A8A0 File Offset: 0x00008AA0
        public static void CreateFromAxisAngle(ref Vector3 axis, float angle, out Matrix44 result)
        {
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            float num = (float)Math.Sin((double)angle);
            float num2 = (float)Math.Cos((double)angle);
            float num3 = x * x;
            float num4 = y * y;
            float num5 = z * z;
            float num6 = x * y;
            float num7 = x * z;
            float num8 = y * z;
            result.M00 = num3 + num2 * (1f - num3);
            result.M10 = num6 - num2 * num6 + num * z;
            result.M20 = num7 - num2 * num7 - num * y;
            result.M30 = 0f;
            result.M01 = num6 - num2 * num6 - num * z;
            result.M11 = num4 + num2 * (1f - num4);
            result.M21 = num8 - num2 * num8 + num * x;
            result.M31 = 0f;
            result.M02 = num7 - num2 * num7 + num * y;
            result.M12 = num8 - num2 * num8 - num * x;
            result.M22 = num5 + num2 * (1f - num5);
            result.M32 = 0f;
            result.M03 = 0f;
            result.M13 = 0f;
            result.M23 = 0f;
            result.M33 = 1f;
        }

        // Token: 0x06000226 RID: 550 RVA: 0x0000A9E8 File Offset: 0x00008BE8
        public void Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation)
        {
            Matrix44 identity = Matrix44.Identity;
            float num = 1f / (float)Math.Sqrt((double)(this[0, 0] * this[0, 0] + this[1, 0] * this[1, 0] + this[2, 0] * this[2, 0]));
            identity[0, 0] = this[0, 0] * num;
            identity[1, 0] = this[1, 0] * num;
            identity[2, 0] = this[2, 0] * num;
            float num2 = identity[0, 0] * this[0, 1] + identity[1, 0] * this[1, 1] + identity[2, 0] * this[2, 1];
            identity[0, 1] = this[0, 1] - num2 * identity[0, 0];
            identity[1, 1] = this[1, 1] - num2 * identity[1, 0];
            identity[2, 1] = this[2, 1] - num2 * identity[2, 0];
            num = 1f / (float)Math.Sqrt((double)(identity[0, 1] * identity[0, 1] + identity[1, 1] * identity[1, 1] + identity[2, 1] * identity[2, 1]));
            ref Matrix44 ptr = ref identity;
            identity[0, 1] = ptr[0, 1] * num;
            ref Matrix44 ptr2 = ref identity;
            identity[1, 1] = ptr2[1, 1] * num;
            ref Matrix44 ptr3 = ref identity;
            identity[2, 1] = ptr3[2, 1] * num;
            num2 = identity[0, 0] * this[0, 2] + identity[1, 0] * this[1, 2] + identity[2, 0] * this[2, 2];
            identity[0, 2] = this[0, 2] - num2 * identity[0, 0];
            identity[1, 2] = this[1, 2] - num2 * identity[1, 0];
            identity[2, 2] = this[2, 2] - num2 * identity[2, 0];
            num2 = identity[0, 1] * this[0, 2] + identity[1, 1] * this[1, 2] + identity[2, 1] * this[2, 2];
            ref Matrix44 ptr4 = ref identity;
            identity[0, 2] = ptr4[0, 2] - num2 * identity[0, 1];
            ref Matrix44 ptr5 = ref identity;
            identity[1, 2] = ptr5[1, 2] - num2 * identity[1, 1];
            ref Matrix44 ptr6 = ref identity;
            identity[2, 2] = ptr6[2, 2] - num2 * identity[2, 1];
            num = 1f / (float)Math.Sqrt((double)(identity[0, 2] * identity[0, 2] + identity[1, 2] * identity[1, 2] + identity[2, 2] * identity[2, 2]));
            ref Matrix44 ptr7 = ref identity;
            identity[0, 2] = ptr7[0, 2] * num;
            ref Matrix44 ptr8 = ref identity;
            identity[1, 2] = ptr8[1, 2] * num;
            ref Matrix44 ptr9 = ref identity;
            identity[2, 2] = ptr9[2, 2] * num;
            float num3 = identity[0, 0] * identity[1, 1] * identity[2, 2] + identity[0, 1] * identity[1, 2] * identity[2, 0] + identity[0, 2] * identity[1, 0] * identity[2, 1] - identity[0, 2] * identity[1, 1] * identity[2, 0] - identity[0, 1] * identity[1, 0] * identity[2, 2] - identity[0, 0] * identity[1, 2] * identity[2, 1];
            if ((double)num3 < 0.0)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        identity[i, j] = -identity[i, j];
                    }
                }
            }
            scale = new Vector3(identity[0, 0] * this[0, 0] + identity[1, 0] * this[1, 0] + identity[2, 0] * this[2, 0], identity[0, 1] * this[0, 1] + identity[1, 1] * this[1, 1] + identity[2, 1] * this[2, 1], identity[0, 2] * this[0, 2] + identity[1, 2] * this[1, 2] + identity[2, 2] * this[2, 2]);
            rotation = Quaternion.CreateFromRotationMatrix(identity);
            translation = new Vector3(this[0, 3], this[1, 3], this[2, 3]);
        }

        // Token: 0x06000227 RID: 551 RVA: 0x0000AF2C File Offset: 0x0000912C
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{0}, {1}, {2}, {3}; ", new object[]
            {
                this.M00.ToString(currentCulture),
                this.M01.ToString(currentCulture),
                this.M02.ToString(currentCulture),
                this.M03.ToString(currentCulture)
            }) + string.Format(currentCulture, "{0}, {1}, {2}, {3}; ", new object[]
            {
                this.M10.ToString(currentCulture),
                this.M11.ToString(currentCulture),
                this.M12.ToString(currentCulture),
                this.M13.ToString(currentCulture)
            }) + string.Format(currentCulture, "{0}, {1}, {2}, {3}; ", new object[]
            {
                this.M20.ToString(currentCulture),
                this.M21.ToString(currentCulture),
                this.M22.ToString(currentCulture),
                this.M23.ToString(currentCulture)
            }) + string.Format(currentCulture, "{0}, {1}, {2}, {3}", new object[]
            {
                this.M30.ToString(currentCulture),
                this.M31.ToString(currentCulture),
                this.M32.ToString(currentCulture),
                this.M33.ToString(currentCulture)
            });
        }

        // Token: 0x06000228 RID: 552 RVA: 0x0000B088 File Offset: 0x00009288
        public bool Equals(Matrix44 other)
        {
            return this.M00 == other.M00 && this.M11 == other.M11 && this.M22 == other.M22 && this.M33 == other.M33 && this.M01 == other.M01 && this.M02 == other.M02 && this.M03 == other.M03 && this.M10 == other.M10 && this.M12 == other.M12 && this.M13 == other.M13 && this.M20 == other.M20 && this.M21 == other.M21 && this.M23 == other.M23 && this.M30 == other.M30 && this.M31 == other.M31 && this.M32 == other.M32;
        }

        // Token: 0x06000229 RID: 553 RVA: 0x0000B19C File Offset: 0x0000939C
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Matrix44)
            {
                result = this.Equals((Matrix44)obj);
            }
            return result;
        }

        // Token: 0x0600022A RID: 554 RVA: 0x0000B1C4 File Offset: 0x000093C4
        public override int GetHashCode()
        {
            return this.M00.GetHashCode() + this.M01.GetHashCode() + this.M02.GetHashCode() + this.M03.GetHashCode() + this.M10.GetHashCode() + this.M11.GetHashCode() + this.M12.GetHashCode() + this.M13.GetHashCode() + this.M20.GetHashCode() + this.M21.GetHashCode() + this.M22.GetHashCode() + this.M23.GetHashCode() + this.M30.GetHashCode() + this.M31.GetHashCode() + this.M32.GetHashCode() + this.M33.GetHashCode();
        }

        // Token: 0x0600022B RID: 555 RVA: 0x0000B290 File Offset: 0x00009490
        public static Matrix44 Transpose(Matrix44 matrix)
        {
            Matrix44 result;
            result.M00 = matrix.M00;
            result.M01 = matrix.M10;
            result.M02 = matrix.M20;
            result.M03 = matrix.M30;
            result.M10 = matrix.M01;
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M20 = matrix.M02;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M30 = matrix.M03;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            return result;
        }

        // Token: 0x0600022C RID: 556 RVA: 0x0000B380 File Offset: 0x00009580
        public static void Transpose(ref Matrix44 matrix, out Matrix44 result)
        {
            result.M00 = matrix.M00;
            result.M01 = matrix.M10;
            result.M02 = matrix.M20;
            result.M03 = matrix.M30;
            result.M10 = matrix.M01;
            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M20 = matrix.M02;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M30 = matrix.M03;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
        }

        // Token: 0x0600022D RID: 557 RVA: 0x0000B450 File Offset: 0x00009650
        public float Determinant()
        {
            float m = this.M00;
            float m2 = this.M10;
            float m3 = this.M20;
            float m4 = this.M30;
            float m5 = this.M01;
            float m6 = this.M11;
            float m7 = this.M21;
            float m8 = this.M31;
            float m9 = this.M02;
            float m10 = this.M12;
            float m11 = this.M22;
            float m12 = this.M32;
            float m13 = this.M03;
            float m14 = this.M13;
            float m15 = this.M23;
            float m16 = this.M33;
            float num = m11 * m16 - m12 * m15;
            float num2 = m10 * m16 - m12 * m14;
            float num3 = m10 * m15 - m11 * m14;
            float num4 = m9 * m16 - m12 * m13;
            float num5 = m9 * m15 - m11 * m13;
            float num6 = m9 * m14 - m10 * m13;
            return m * (m6 * num - m7 * num2 + m8 * num3) - m2 * (m5 * num - m7 * num4 + m8 * num5) + m3 * (m5 * num2 - m6 * num4 + m8 * num6) - m4 * (m5 * num3 - m6 * num5 + m7 * num6);
        }

        // Token: 0x0600022E RID: 558 RVA: 0x0000B578 File Offset: 0x00009778
        public static Matrix44 Invert(Matrix44 matrix)
        {
            float m = matrix.M00;
            float m2 = matrix.M10;
            float m3 = matrix.M20;
            float m4 = matrix.M30;
            float m5 = matrix.M01;
            float m6 = matrix.M11;
            float m7 = matrix.M21;
            float m8 = matrix.M31;
            float m9 = matrix.M02;
            float m10 = matrix.M12;
            float m11 = matrix.M22;
            float m12 = matrix.M32;
            float m13 = matrix.M03;
            float m14 = matrix.M13;
            float m15 = matrix.M23;
            float m16 = matrix.M33;
            float num = m11 * m16 - m12 * m15;
            float num2 = m10 * m16 - m12 * m14;
            float num3 = m10 * m15 - m11 * m14;
            float num4 = m9 * m16 - m12 * m13;
            float num5 = m9 * m15 - m11 * m13;
            float num6 = m9 * m14 - m10 * m13;
            float num7 = m6 * num - m7 * num2 + m8 * num3;
            float num8 = -(m5 * num - m7 * num4 + m8 * num5);
            float num9 = m5 * num2 - m6 * num4 + m8 * num6;
            float num10 = -(m5 * num3 - m6 * num5 + m7 * num6);
            float num11 = 1f / (m * num7 + m2 * num8 + m3 * num9 + m4 * num10);
            Matrix44 result;
            result.M00 = num7 * num11;
            result.M01 = num8 * num11;
            result.M02 = num9 * num11;
            result.M03 = num10 * num11;
            result.M10 = -(m2 * num - m3 * num2 + m4 * num3) * num11;
            result.M11 = (m * num - m3 * num4 + m4 * num5) * num11;
            result.M12 = -(m * num2 - m2 * num4 + m4 * num6) * num11;
            result.M13 = (m * num3 - m2 * num5 + m3 * num6) * num11;
            float num12 = m7 * m16 - m8 * m15;
            float num13 = m6 * m16 - m8 * m14;
            float num14 = m6 * m15 - m7 * m14;
            float num15 = m5 * m16 - m8 * m13;
            float num16 = m5 * m15 - m7 * m13;
            float num17 = m5 * m14 - m6 * m13;
            result.M20 = (m2 * num12 - m3 * num13 + m4 * num14) * num11;
            result.M21 = -(m * num12 - m3 * num15 + m4 * num16) * num11;
            result.M22 = (m * num13 - m2 * num15 + m4 * num17) * num11;
            result.M23 = -(m * num14 - m2 * num16 + m3 * num17) * num11;
            float num18 = m7 * m12 - m8 * m11;
            float num19 = m6 * m12 - m8 * m10;
            float num20 = m6 * m11 - m7 * m10;
            float num21 = m5 * m12 - m8 * m9;
            float num22 = m5 * m11 - m7 * m9;
            float num23 = m5 * m10 - m6 * m9;
            result.M30 = -(m2 * num18 - m3 * num19 + m4 * num20) * num11;
            result.M31 = (m * num18 - m3 * num21 + m4 * num22) * num11;
            result.M32 = -(m * num19 - m2 * num21 + m4 * num23) * num11;
            result.M33 = (m * num20 - m2 * num22 + m3 * num23) * num11;
            return result;
        }

        // Token: 0x0600022F RID: 559 RVA: 0x0000B8C8 File Offset: 0x00009AC8
        public static void Invert(ref Matrix44 matrix, out Matrix44 result)
        {
            float m = matrix.M00;
            float m2 = matrix.M10;
            float m3 = matrix.M20;
            float m4 = matrix.M30;
            float m5 = matrix.M01;
            float m6 = matrix.M11;
            float m7 = matrix.M21;
            float m8 = matrix.M31;
            float m9 = matrix.M02;
            float m10 = matrix.M12;
            float m11 = matrix.M22;
            float m12 = matrix.M32;
            float m13 = matrix.M03;
            float m14 = matrix.M13;
            float m15 = matrix.M23;
            float m16 = matrix.M33;
            float num = m11 * m16 - m12 * m15;
            float num2 = m10 * m16 - m12 * m14;
            float num3 = m10 * m15 - m11 * m14;
            float num4 = m9 * m16 - m12 * m13;
            float num5 = m9 * m15 - m11 * m13;
            float num6 = m9 * m14 - m10 * m13;
            float num7 = m6 * num - m7 * num2 + m8 * num3;
            float num8 = -(m5 * num - m7 * num4 + m8 * num5);
            float num9 = m5 * num2 - m6 * num4 + m8 * num6;
            float num10 = -(m5 * num3 - m6 * num5 + m7 * num6);
            float num11 = 1f / (m * num7 + m2 * num8 + m3 * num9 + m4 * num10);
            result.M00 = num7 * num11;
            result.M01 = num8 * num11;
            result.M02 = num9 * num11;
            result.M03 = num10 * num11;
            result.M10 = -(m2 * num - m3 * num2 + m4 * num3) * num11;
            result.M11 = (m * num - m3 * num4 + m4 * num5) * num11;
            result.M12 = -(m * num2 - m2 * num4 + m4 * num6) * num11;
            result.M13 = (m * num3 - m2 * num5 + m3 * num6) * num11;
            float num12 = m7 * m16 - m8 * m15;
            float num13 = m6 * m16 - m8 * m14;
            float num14 = m6 * m15 - m7 * m14;
            float num15 = m5 * m16 - m8 * m13;
            float num16 = m5 * m15 - m7 * m13;
            float num17 = m5 * m14 - m6 * m13;
            result.M20 = (m2 * num12 - m3 * num13 + m4 * num14) * num11;
            result.M21 = -(m * num12 - m3 * num15 + m4 * num16) * num11;
            result.M22 = (m * num13 - m2 * num15 + m4 * num17) * num11;
            result.M23 = -(m * num14 - m2 * num16 + m3 * num17) * num11;
            float num18 = m7 * m12 - m8 * m11;
            float num19 = m6 * m12 - m8 * m10;
            float num20 = m6 * m11 - m7 * m10;
            float num21 = m5 * m12 - m8 * m9;
            float num22 = m5 * m11 - m7 * m9;
            float num23 = m5 * m10 - m6 * m9;
            result.M30 = -(m2 * num18 - m3 * num19 + m4 * num20) * num11;
            result.M31 = (m * num18 - m3 * num21 + m4 * num22) * num11;
            result.M32 = -(m * num19 - m2 * num21 + m4 * num23) * num11;
            result.M33 = (m * num20 - m2 * num22 + m3 * num23) * num11;
        }

        // Token: 0x06000230 RID: 560 RVA: 0x0000BBEC File Offset: 0x00009DEC
        public static Matrix44 Add(Matrix44 matrix1, Matrix44 matrix2)
        {
            Matrix44 result;
            result.M00 = matrix1.M00 + matrix2.M00;
            result.M01 = matrix1.M01 + matrix2.M01;
            result.M02 = matrix1.M02 + matrix2.M02;
            result.M03 = matrix1.M03 + matrix2.M03;
            result.M10 = matrix1.M10 + matrix2.M10;
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M20 = matrix1.M20 + matrix2.M20;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M30 = matrix1.M30 + matrix2.M30;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            return result;
        }

        // Token: 0x06000231 RID: 561 RVA: 0x0000BD5C File Offset: 0x00009F5C
        public static void Add(ref Matrix44 matrix1, ref Matrix44 matrix2, out Matrix44 result)
        {
            result.M00 = matrix1.M00 + matrix2.M00;
            result.M01 = matrix1.M01 + matrix2.M01;
            result.M02 = matrix1.M02 + matrix2.M02;
            result.M03 = matrix1.M03 + matrix2.M03;
            result.M10 = matrix1.M10 + matrix2.M10;
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M20 = matrix1.M20 + matrix2.M20;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M30 = matrix1.M30 + matrix2.M30;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
        }

        // Token: 0x06000232 RID: 562 RVA: 0x0000BE9C File Offset: 0x0000A09C
        public static Matrix44 Sub(Matrix44 matrix1, Matrix44 matrix2)
        {
            Matrix44 result;
            result.M00 = matrix1.M00 - matrix2.M00;
            result.M01 = matrix1.M01 - matrix2.M01;
            result.M02 = matrix1.M02 - matrix2.M02;
            result.M03 = matrix1.M03 - matrix2.M03;
            result.M10 = matrix1.M10 - matrix2.M10;
            result.M11 = matrix1.M11 - matrix2.M11;
            result.M12 = matrix1.M12 - matrix2.M12;
            result.M13 = matrix1.M13 - matrix2.M13;
            result.M20 = matrix1.M20 - matrix2.M20;
            result.M21 = matrix1.M21 - matrix2.M21;
            result.M22 = matrix1.M22 - matrix2.M22;
            result.M23 = matrix1.M23 - matrix2.M23;
            result.M30 = matrix1.M30 - matrix2.M30;
            result.M31 = matrix1.M31 - matrix2.M31;
            result.M32 = matrix1.M32 - matrix2.M32;
            result.M33 = matrix1.M33 - matrix2.M33;
            return result;
        }

        // Token: 0x06000233 RID: 563 RVA: 0x0000C00C File Offset: 0x0000A20C
        public static void Sub(ref Matrix44 matrix1, ref Matrix44 matrix2, out Matrix44 result)
        {
            result.M00 = matrix1.M00 - matrix2.M00;
            result.M01 = matrix1.M01 - matrix2.M01;
            result.M02 = matrix1.M02 - matrix2.M02;
            result.M03 = matrix1.M03 - matrix2.M03;
            result.M10 = matrix1.M10 - matrix2.M10;
            result.M11 = matrix1.M11 - matrix2.M11;
            result.M12 = matrix1.M12 - matrix2.M12;
            result.M13 = matrix1.M13 - matrix2.M13;
            result.M20 = matrix1.M20 - matrix2.M20;
            result.M21 = matrix1.M21 - matrix2.M21;
            result.M22 = matrix1.M22 - matrix2.M22;
            result.M23 = matrix1.M23 - matrix2.M23;
            result.M30 = matrix1.M30 - matrix2.M30;
            result.M31 = matrix1.M31 - matrix2.M31;
            result.M32 = matrix1.M32 - matrix2.M32;
            result.M33 = matrix1.M33 - matrix2.M33;
        }

        // Token: 0x06000234 RID: 564 RVA: 0x0000C14C File Offset: 0x0000A34C
        public static Matrix44 Multiply(Matrix44 matrix1, Matrix44 matrix2)
        {
            Matrix44 result;
            result.M00 = matrix1.M00 * matrix2.M00 + matrix1.M01 * matrix2.M10 + matrix1.M02 * matrix2.M20 + matrix1.M03 * matrix2.M30;
            result.M01 = matrix1.M00 * matrix2.M01 + matrix1.M01 * matrix2.M11 + matrix1.M02 * matrix2.M21 + matrix1.M03 * matrix2.M31;
            result.M02 = matrix1.M00 * matrix2.M02 + matrix1.M01 * matrix2.M12 + matrix1.M02 * matrix2.M22 + matrix1.M03 * matrix2.M32;
            result.M03 = matrix1.M00 * matrix2.M03 + matrix1.M01 * matrix2.M13 + matrix1.M02 * matrix2.M23 + matrix1.M03 * matrix2.M33;
            result.M10 = matrix1.M10 * matrix2.M00 + matrix1.M11 * matrix2.M10 + matrix1.M12 * matrix2.M20 + matrix1.M13 * matrix2.M30;
            result.M11 = matrix1.M10 * matrix2.M01 + matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31;
            result.M12 = matrix1.M10 * matrix2.M02 + matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32;
            result.M13 = matrix1.M10 * matrix2.M03 + matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33;
            result.M20 = matrix1.M20 * matrix2.M00 + matrix1.M21 * matrix2.M10 + matrix1.M22 * matrix2.M20 + matrix1.M23 * matrix2.M30;
            result.M21 = matrix1.M20 * matrix2.M01 + matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31;
            result.M22 = matrix1.M20 * matrix2.M02 + matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32;
            result.M23 = matrix1.M20 * matrix2.M03 + matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33;
            result.M30 = matrix1.M30 * matrix2.M00 + matrix1.M31 * matrix2.M10 + matrix1.M32 * matrix2.M20 + matrix1.M33 * matrix2.M30;
            result.M31 = matrix1.M30 * matrix2.M01 + matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31;
            result.M32 = matrix1.M30 * matrix2.M02 + matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32;
            result.M33 = matrix1.M30 * matrix2.M03 + matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33;
            return result;
        }

        // Token: 0x06000235 RID: 565 RVA: 0x0000C5BC File Offset: 0x0000A7BC
        public static void Multiply(ref Matrix44 matrix1, ref Matrix44 matrix2, out Matrix44 result)
        {
            float m = matrix1.M00 * matrix2.M00 + matrix1.M01 * matrix2.M10 + matrix1.M02 * matrix2.M20 + matrix1.M03 * matrix2.M30;
            float m2 = matrix1.M00 * matrix2.M01 + matrix1.M01 * matrix2.M11 + matrix1.M02 * matrix2.M21 + matrix1.M03 * matrix2.M31;
            float m3 = matrix1.M00 * matrix2.M02 + matrix1.M01 * matrix2.M12 + matrix1.M02 * matrix2.M22 + matrix1.M03 * matrix2.M32;
            float m4 = matrix1.M00 * matrix2.M03 + matrix1.M01 * matrix2.M13 + matrix1.M02 * matrix2.M23 + matrix1.M03 * matrix2.M33;
            float m5 = matrix1.M10 * matrix2.M00 + matrix1.M11 * matrix2.M10 + matrix1.M12 * matrix2.M20 + matrix1.M13 * matrix2.M30;
            float m6 = matrix1.M10 * matrix2.M01 + matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31;
            float m7 = matrix1.M10 * matrix2.M02 + matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32;
            float m8 = matrix1.M10 * matrix2.M03 + matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33;
            float m9 = matrix1.M20 * matrix2.M00 + matrix1.M21 * matrix2.M10 + matrix1.M22 * matrix2.M20 + matrix1.M23 * matrix2.M30;
            float m10 = matrix1.M20 * matrix2.M01 + matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31;
            float m11 = matrix1.M20 * matrix2.M02 + matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32;
            float m12 = matrix1.M20 * matrix2.M03 + matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33;
            float m13 = matrix1.M30 * matrix2.M00 + matrix1.M31 * matrix2.M10 + matrix1.M32 * matrix2.M20 + matrix1.M33 * matrix2.M30;
            float m14 = matrix1.M30 * matrix2.M01 + matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31;
            float m15 = matrix1.M30 * matrix2.M02 + matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32;
            float m16 = matrix1.M30 * matrix2.M03 + matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33;
            result.M00 = m;
            result.M01 = m2;
            result.M02 = m3;
            result.M03 = m4;
            result.M10 = m5;
            result.M11 = m6;
            result.M12 = m7;
            result.M13 = m8;
            result.M20 = m9;
            result.M21 = m10;
            result.M22 = m11;
            result.M23 = m12;
            result.M30 = m13;
            result.M31 = m14;
            result.M32 = m15;
            result.M33 = m16;
        }

        // Token: 0x06000236 RID: 566 RVA: 0x0000C9D4 File Offset: 0x0000ABD4
        public static Vector4 TransformVector4(Matrix44 matrix, Vector4 vector)
        {
            float x = vector.X * matrix.M00 + vector.Y * matrix.M01 + vector.Z * matrix.M02 + vector.W * matrix.M03;
            float y = vector.X * matrix.M10 + vector.Y * matrix.M11 + vector.Z * matrix.M12 + vector.W * matrix.M13;
            float z = vector.X * matrix.M20 + vector.Y * matrix.M21 + vector.Z * matrix.M22 + vector.W * matrix.M23;
            float w = vector.X * matrix.M30 + vector.Y * matrix.M31 + vector.Z * matrix.M32 + vector.W * matrix.M33;
            Vector4 result;
            result.X = x;
            result.Y = y;
            result.Z = z;
            result.W = w;
            return result;
        }

        // Token: 0x06000237 RID: 567 RVA: 0x0000CB04 File Offset: 0x0000AD04
        public static void TransformVector4(ref Matrix44 matrix, ref Vector4 vector, out Vector4 result)
        {
            float x = vector.X * matrix.M00 + vector.Y * matrix.M01 + vector.Z * matrix.M02 + vector.W * matrix.M03;
            float y = vector.X * matrix.M10 + vector.Y * matrix.M11 + vector.Z * matrix.M12 + vector.W * matrix.M13;
            float z = vector.X * matrix.M20 + vector.Y * matrix.M21 + vector.Z * matrix.M22 + vector.W * matrix.M23;
            float w = vector.X * matrix.M30 + vector.Y * matrix.M31 + vector.Z * matrix.M32 + vector.W * matrix.M33;
            result.X = x;
            result.Y = y;
            result.Z = z;
            result.W = w;
        }

        // Token: 0x06000238 RID: 568 RVA: 0x0000CC10 File Offset: 0x0000AE10
        public static Vector3 TransformPosition(Matrix44 matrix, Vector3 position)
        {
            float x = position.X * matrix.M00 + position.Y * matrix.M01 + position.Z * matrix.M02 + matrix.M03;
            float y = position.X * matrix.M10 + position.Y * matrix.M11 + position.Z * matrix.M12 + matrix.M13;
            float z = position.X * matrix.M20 + position.Y * matrix.M21 + position.Z * matrix.M22 + matrix.M23;
            Vector3 result;
            result.X = x;
            result.Y = y;
            result.Z = z;
            return result;
        }

        // Token: 0x06000239 RID: 569 RVA: 0x0000CCE0 File Offset: 0x0000AEE0
        public static void TransformPosition(ref Matrix44 matrix, ref Vector3 position, out Vector3 result)
        {
            float x = position.X * matrix.M00 + position.Y * matrix.M01 + position.Z * matrix.M02 + matrix.M03;
            float y = position.X * matrix.M10 + position.Y * matrix.M11 + position.Z * matrix.M12 + matrix.M13;
            float z = position.X * matrix.M20 + position.Y * matrix.M21 + position.Z * matrix.M22 + matrix.M23;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        // Token: 0x0600023A RID: 570 RVA: 0x0000CD98 File Offset: 0x0000AF98
        public static Vector3 TransformDirection(Matrix44 matrix, Vector3 direction)
        {
            float x = direction.X * matrix.M00 + direction.Y * matrix.M01 + direction.Z * matrix.M02;
            float y = direction.X * matrix.M10 + direction.Y * matrix.M11 + direction.Z * matrix.M12;
            float z = direction.X * matrix.M20 + direction.Y * matrix.M21 + direction.Z * matrix.M22;
            Vector3 result;
            result.X = x;
            result.Y = y;
            result.Z = z;
            return result;
        }

        // Token: 0x0600023B RID: 571 RVA: 0x0000CE50 File Offset: 0x0000B050
        public static void TransformDirection(ref Matrix44 matrix, ref Vector3 direction, out Vector3 result)
        {
            float x = direction.X * matrix.M00 + direction.Y * matrix.M01 + direction.Z * matrix.M02;
            float y = direction.X * matrix.M10 + direction.Y * matrix.M11 + direction.Z * matrix.M12;
            float z = direction.X * matrix.M20 + direction.Y * matrix.M21 + direction.Z * matrix.M22;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        // Token: 0x0600023C RID: 572 RVA: 0x0000CEF0 File Offset: 0x0000B0F0
        public static Matrix44 operator -(Matrix44 matrix1)
        {
            Matrix44 result;
            result.M00 = -matrix1.M00;
            result.M01 = -matrix1.M01;
            result.M02 = -matrix1.M02;
            result.M03 = -matrix1.M03;
            result.M10 = -matrix1.M10;
            result.M11 = -matrix1.M11;
            result.M12 = -matrix1.M12;
            result.M13 = -matrix1.M13;
            result.M20 = -matrix1.M20;
            result.M21 = -matrix1.M21;
            result.M22 = -matrix1.M22;
            result.M23 = -matrix1.M23;
            result.M30 = -matrix1.M30;
            result.M31 = -matrix1.M31;
            result.M32 = -matrix1.M32;
            result.M33 = -matrix1.M33;
            return result;
        }

        // Token: 0x0600023D RID: 573 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
        public static bool operator ==(Matrix44 matrix1, Matrix44 matrix2)
        {
            return matrix1.M00 == matrix2.M00 && matrix1.M11 == matrix2.M11 && matrix1.M22 == matrix2.M22 && matrix1.M33 == matrix2.M33 && matrix1.M01 == matrix2.M01 && matrix1.M02 == matrix2.M02 && matrix1.M03 == matrix2.M03 && matrix1.M10 == matrix2.M10 && matrix1.M12 == matrix2.M12 && matrix1.M13 == matrix2.M13 && matrix1.M20 == matrix2.M20 && matrix1.M21 == matrix2.M21 && matrix1.M23 == matrix2.M23 && matrix1.M30 == matrix2.M30 && matrix1.M31 == matrix2.M31 && matrix1.M32 == matrix2.M32;
        }

        // Token: 0x0600023E RID: 574 RVA: 0x0000D118 File Offset: 0x0000B318
        public static bool operator !=(Matrix44 matrix1, Matrix44 matrix2)
        {
            return matrix1.M00 != matrix2.M00 || matrix1.M01 != matrix2.M01 || matrix1.M02 != matrix2.M02 || matrix1.M03 != matrix2.M03 || matrix1.M10 != matrix2.M10 || matrix1.M11 != matrix2.M11 || matrix1.M12 != matrix2.M12 || matrix1.M13 != matrix2.M13 || matrix1.M20 != matrix2.M20 || matrix1.M21 != matrix2.M21 || matrix1.M22 != matrix2.M22 || matrix1.M23 != matrix2.M23 || matrix1.M30 != matrix2.M30 || matrix1.M31 != matrix2.M31 || matrix1.M32 != matrix2.M32 || matrix1.M33 != matrix2.M33;
        }

        // Token: 0x0600023F RID: 575 RVA: 0x0000D244 File Offset: 0x0000B444
        public static Matrix44 operator +(Matrix44 matrix1, Matrix44 matrix2)
        {
            Matrix44 result;
            result.M00 = matrix1.M00 + matrix2.M00;
            result.M01 = matrix1.M01 + matrix2.M01;
            result.M02 = matrix1.M02 + matrix2.M02;
            result.M03 = matrix1.M03 + matrix2.M03;
            result.M10 = matrix1.M10 + matrix2.M10;
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M20 = matrix1.M20 + matrix2.M20;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M30 = matrix1.M30 + matrix2.M30;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            return result;
        }

        // Token: 0x06000240 RID: 576 RVA: 0x0000D3B4 File Offset: 0x0000B5B4
        public static Matrix44 operator -(Matrix44 matrix1, Matrix44 matrix2)
        {
            Matrix44 result;
            result.M00 = matrix1.M00 - matrix2.M00;
            result.M01 = matrix1.M01 - matrix2.M01;
            result.M02 = matrix1.M02 - matrix2.M02;
            result.M03 = matrix1.M03 - matrix2.M03;
            result.M10 = matrix1.M10 - matrix2.M10;
            result.M11 = matrix1.M11 - matrix2.M11;
            result.M12 = matrix1.M12 - matrix2.M12;
            result.M13 = matrix1.M13 - matrix2.M13;
            result.M20 = matrix1.M20 - matrix2.M20;
            result.M21 = matrix1.M21 - matrix2.M21;
            result.M22 = matrix1.M22 - matrix2.M22;
            result.M23 = matrix1.M23 - matrix2.M23;
            result.M30 = matrix1.M30 - matrix2.M30;
            result.M31 = matrix1.M31 - matrix2.M31;
            result.M32 = matrix1.M32 - matrix2.M32;
            result.M33 = matrix1.M33 - matrix2.M33;
            return result;
        }

        // Token: 0x06000241 RID: 577 RVA: 0x0000D524 File Offset: 0x0000B724
        public static Matrix44 operator *(Matrix44 matrix1, Matrix44 matrix2)
        {
            Matrix44 result;
            result.M00 = matrix1.M00 * matrix2.M00 + matrix1.M01 * matrix2.M10 + matrix1.M02 * matrix2.M20 + matrix1.M03 * matrix2.M30;
            result.M01 = matrix1.M00 * matrix2.M01 + matrix1.M01 * matrix2.M11 + matrix1.M02 * matrix2.M21 + matrix1.M03 * matrix2.M31;
            result.M02 = matrix1.M00 * matrix2.M02 + matrix1.M01 * matrix2.M12 + matrix1.M02 * matrix2.M22 + matrix1.M03 * matrix2.M32;
            result.M03 = matrix1.M00 * matrix2.M03 + matrix1.M01 * matrix2.M13 + matrix1.M02 * matrix2.M23 + matrix1.M03 * matrix2.M33;
            result.M10 = matrix1.M10 * matrix2.M00 + matrix1.M11 * matrix2.M10 + matrix1.M12 * matrix2.M20 + matrix1.M13 * matrix2.M30;
            result.M11 = matrix1.M10 * matrix2.M01 + matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31;
            result.M12 = matrix1.M10 * matrix2.M02 + matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32;
            result.M13 = matrix1.M10 * matrix2.M03 + matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33;
            result.M20 = matrix1.M20 * matrix2.M00 + matrix1.M21 * matrix2.M10 + matrix1.M22 * matrix2.M20 + matrix1.M23 * matrix2.M30;
            result.M21 = matrix1.M20 * matrix2.M01 + matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31;
            result.M22 = matrix1.M20 * matrix2.M02 + matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32;
            result.M23 = matrix1.M20 * matrix2.M03 + matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33;
            result.M30 = matrix1.M30 * matrix2.M00 + matrix1.M31 * matrix2.M10 + matrix1.M32 * matrix2.M20 + matrix1.M33 * matrix2.M30;
            result.M31 = matrix1.M30 * matrix2.M01 + matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31;
            result.M32 = matrix1.M30 * matrix2.M02 + matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32;
            result.M33 = matrix1.M30 * matrix2.M03 + matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33;
            return result;
        }

        // Token: 0x04000032 RID: 50
        public float M00;

        // Token: 0x04000033 RID: 51
        public float M01;

        // Token: 0x04000034 RID: 52
        public float M02;

        // Token: 0x04000035 RID: 53
        public float M03;

        // Token: 0x04000036 RID: 54
        public float M10;

        // Token: 0x04000037 RID: 55
        public float M11;

        // Token: 0x04000038 RID: 56
        public float M12;

        // Token: 0x04000039 RID: 57
        public float M13;

        // Token: 0x0400003A RID: 58
        public float M20;

        // Token: 0x0400003B RID: 59
        public float M21;

        // Token: 0x0400003C RID: 60
        public float M22;

        // Token: 0x0400003D RID: 61
        public float M23;

        // Token: 0x0400003E RID: 62
        public float M30;

        // Token: 0x0400003F RID: 63
        public float M31;

        // Token: 0x04000040 RID: 64
        public float M32;

        // Token: 0x04000041 RID: 65
        public float M33;

        // Token: 0x04000042 RID: 66
        private static Matrix44 _identity = new Matrix44(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
    }
}
