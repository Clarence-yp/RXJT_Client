using System;

namespace ScriptRuntime
{
    // Token: 0x02000009 RID: 9
    [Serializable]
    internal class Gjk
    {
        // Token: 0x060001CB RID: 459 RVA: 0x000081F0 File Offset: 0x000063F0
        public Gjk()
        {
            for (int i = 0; i < 16; i++)
            {
                this.det[i] = new float[4];
            }
        }

        // Token: 0x060001CC RID: 460 RVA: 0x000082A8 File Offset: 0x000064A8
        public bool AddSupportPoint(ref Vector3 newPoint)
        {
            int num = (Gjk.BitsToIndices[this.simplexBits ^ 15] & 7) - 1;
            this.y[num] = newPoint;
            this.yLengthSq[num] = newPoint.LengthSquared();
            for (int num2 = Gjk.BitsToIndices[this.simplexBits]; num2 != 0; num2 >>= 3)
            {
                int num3 = (num2 & 7) - 1;
                Vector3 vector = this.y[num3] - newPoint;
                this.edges[num3][num] = vector;
                this.edges[num][num3] = -vector;
                this.edgeLengthSq[num][num3] = (this.edgeLengthSq[num3][num] = vector.LengthSquared());
            }
            this.UpdateDeterminant(num);
            return this.UpdateSimplex(num);
        }

        // Token: 0x060001CD RID: 461 RVA: 0x00008384 File Offset: 0x00006584
        private Vector3 ComputeClosestPoint()
        {
            float num = 0f;
            Vector3 vector = Vector3.Zero;
            this.maxLengthSq = 0f;
            for (int num2 = Gjk.BitsToIndices[this.simplexBits]; num2 != 0; num2 >>= 3)
            {
                int num3 = (num2 & 7) - 1;
                float num4 = this.det[this.simplexBits][num3];
                num += num4;
                vector += this.y[num3] * num4;
                this.maxLengthSq = MathHelper.Max(this.maxLengthSq, this.yLengthSq[num3]);
            }
            return vector / num;
        }

        // Token: 0x060001CE RID: 462 RVA: 0x00008419 File Offset: 0x00006619
        private static float Dot(ref Vector3 a, ref Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        // Token: 0x060001CF RID: 463 RVA: 0x00008444 File Offset: 0x00006644
        private bool IsSatisfiesRule(int xBits, int yBits)
        {
            for (int num = Gjk.BitsToIndices[yBits]; num != 0; num >>= 3)
            {
                int num2 = (num & 7) - 1;
                int num3 = 1 << num2;
                if ((num3 & xBits) != 0)
                {
                    if (this.det[xBits][num2] <= 0f)
                    {
                        return false;
                    }
                }
                else if (this.det[xBits | num3][num2] > 0f)
                {
                    return false;
                }
            }
            return true;
        }

        // Token: 0x060001D0 RID: 464 RVA: 0x0000849D File Offset: 0x0000669D
        public void Reset()
        {
            this.simplexBits = 0;
            this.maxLengthSq = 0f;
        }

        // Token: 0x060001D1 RID: 465 RVA: 0x000084B4 File Offset: 0x000066B4
        private void UpdateDeterminant(int xmIdx)
        {
            int num = 1 << xmIdx;
            this.det[num][xmIdx] = 1f;
            int num2 = Gjk.BitsToIndices[this.simplexBits];
            int num3 = num2;
            int num4 = 0;
            while (num3 != 0)
            {
                int num5 = (num3 & 7) - 1;
                int num6 = 1 << num5;
                int num7 = num6 | num;
                this.det[num7][num5] = Gjk.Dot(ref this.edges[xmIdx][num5], ref this.y[xmIdx]);
                this.det[num7][xmIdx] = Gjk.Dot(ref this.edges[num5][xmIdx], ref this.y[num5]);
                int num8 = num2;
                for (int i = 0; i < num4; i++)
                {
                    int num9 = (num8 & 7) - 1;
                    int num10 = 1 << num9;
                    int num11 = num7 | num10;
                    int num12 = (this.edgeLengthSq[num5][num9] < this.edgeLengthSq[xmIdx][num9]) ? num5 : xmIdx;
                    this.det[num11][num9] = this.det[num7][num5] * Gjk.Dot(ref this.edges[num12][num9], ref this.y[num5]) + this.det[num7][xmIdx] * Gjk.Dot(ref this.edges[num12][num9], ref this.y[xmIdx]);
                    num12 = ((this.edgeLengthSq[num9][num5] < this.edgeLengthSq[xmIdx][num5]) ? num9 : xmIdx);
                    this.det[num11][num5] = this.det[num10 | num][num9] * Gjk.Dot(ref this.edges[num12][num5], ref this.y[num9]) + this.det[num10 | num][xmIdx] * Gjk.Dot(ref this.edges[num12][num5], ref this.y[xmIdx]);
                    num12 = ((this.edgeLengthSq[num5][xmIdx] < this.edgeLengthSq[num9][xmIdx]) ? num5 : num9);
                    this.det[num11][xmIdx] = this.det[num6 | num10][num9] * Gjk.Dot(ref this.edges[num12][xmIdx], ref this.y[num9]) + this.det[num6 | num10][num5] * Gjk.Dot(ref this.edges[num12][xmIdx], ref this.y[num5]);
                    num8 >>= 3;
                }
                num3 >>= 3;
                num4++;
            }
            if ((this.simplexBits | num) == 15)
            {
                int num13 = (this.edgeLengthSq[1][0] < this.edgeLengthSq[2][0]) ? ((this.edgeLengthSq[1][0] < this.edgeLengthSq[3][0]) ? 1 : 3) : ((this.edgeLengthSq[2][0] < this.edgeLengthSq[3][0]) ? 2 : 3);
                this.det[15][0] = this.det[14][1] * Gjk.Dot(ref this.edges[num13][0], ref this.y[1]) + this.det[14][2] * Gjk.Dot(ref this.edges[num13][0], ref this.y[2]) + this.det[14][3] * Gjk.Dot(ref this.edges[num13][0], ref this.y[3]);
                num13 = ((this.edgeLengthSq[0][1] < this.edgeLengthSq[2][1]) ? ((this.edgeLengthSq[0][1] < this.edgeLengthSq[3][1]) ? 0 : 3) : ((this.edgeLengthSq[2][1] < this.edgeLengthSq[3][1]) ? 2 : 3));
                this.det[15][1] = this.det[13][0] * Gjk.Dot(ref this.edges[num13][1], ref this.y[0]) + this.det[13][2] * Gjk.Dot(ref this.edges[num13][1], ref this.y[2]) + this.det[13][3] * Gjk.Dot(ref this.edges[num13][1], ref this.y[3]);
                num13 = ((this.edgeLengthSq[0][2] < this.edgeLengthSq[1][2]) ? ((this.edgeLengthSq[0][2] < this.edgeLengthSq[3][2]) ? 0 : 3) : ((this.edgeLengthSq[1][2] < this.edgeLengthSq[3][2]) ? 1 : 3));
                this.det[15][2] = this.det[11][0] * Gjk.Dot(ref this.edges[num13][2], ref this.y[0]) + this.det[11][1] * Gjk.Dot(ref this.edges[num13][2], ref this.y[1]) + this.det[11][3] * Gjk.Dot(ref this.edges[num13][2], ref this.y[3]);
                num13 = ((this.edgeLengthSq[0][3] < this.edgeLengthSq[1][3]) ? ((this.edgeLengthSq[0][3] < this.edgeLengthSq[2][3]) ? 0 : 2) : ((this.edgeLengthSq[1][3] < this.edgeLengthSq[2][3]) ? 1 : 2));
                this.det[15][3] = this.det[7][0] * Gjk.Dot(ref this.edges[num13][3], ref this.y[0]) + this.det[7][1] * Gjk.Dot(ref this.edges[num13][3], ref this.y[1]) + this.det[7][2] * Gjk.Dot(ref this.edges[num13][3], ref this.y[2]);
            }
        }

        // Token: 0x060001D2 RID: 466 RVA: 0x00008ACC File Offset: 0x00006CCC
        private bool UpdateSimplex(int newIndex)
        {
            int num = this.simplexBits | 1 << newIndex;
            int num2 = 1 << newIndex;
            for (int num3 = this.simplexBits; num3 != 0; num3--)
            {
                if ((num3 & num) == num3 && this.IsSatisfiesRule(num3 | num2, num))
                {
                    this.simplexBits = (num3 | num2);
                    this.closestPoint = this.ComputeClosestPoint();
                    return true;
                }
            }
            bool result = false;
            if (this.IsSatisfiesRule(num2, num))
            {
                this.simplexBits = num2;
                this.closestPoint = this.y[newIndex];
                this.maxLengthSq = this.yLengthSq[newIndex];
                result = true;
            }
            return result;
        }

        // Token: 0x17000149 RID: 329
        // (get) Token: 0x060001D3 RID: 467 RVA: 0x00008B62 File Offset: 0x00006D62
        public Vector3 ClosestPoint {
            get {
                return this.closestPoint;
            }
        }

        // Token: 0x1700014A RID: 330
        // (get) Token: 0x060001D4 RID: 468 RVA: 0x00008B6A File Offset: 0x00006D6A
        public bool FullSimplex {
            get {
                return this.simplexBits == 15;
            }
        }

        // Token: 0x1700014B RID: 331
        // (get) Token: 0x060001D5 RID: 469 RVA: 0x00008B76 File Offset: 0x00006D76
        public float MaxLengthSquared {
            get {
                return this.maxLengthSq;
            }
        }

        // Token: 0x0400001D RID: 29
        private static int[] BitsToIndices = new int[]
        {
            0,
            1,
            2,
            17,
            3,
            25,
            26,
            209,
            4,
            33,
            34,
            273,
            35,
            281,
            282,
            2257
        };

        // Token: 0x0400001E RID: 30
        private Vector3 closestPoint;

        // Token: 0x0400001F RID: 31
        private float[][] det = new float[16][];

        // Token: 0x04000020 RID: 32
        private float[][] edgeLengthSq = new float[][]
        {
            new float[4],
            new float[4],
            new float[4],
            new float[4]
        };

        // Token: 0x04000021 RID: 33
        private Vector3[][] edges = new Vector3[][]
        {
            new Vector3[4],
            new Vector3[4],
            new Vector3[4],
            new Vector3[4]
        };

        // Token: 0x04000022 RID: 34
        private float maxLengthSq;

        // Token: 0x04000023 RID: 35
        private int simplexBits;

        // Token: 0x04000024 RID: 36
        private Vector3[] y = new Vector3[4];

        // Token: 0x04000025 RID: 37
        private float[] yLengthSq = new float[4];
    }
}
