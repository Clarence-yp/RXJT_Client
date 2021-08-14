using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ScriptRuntime;
using DashFire;

namespace DashFireSpatial
{
    public sealed class CellManager
    {
        // constructors-----------------------------------------------------------
        public CellManager()
        {
        }

        public void Reset()
        {
            cell_map_views_.Clear();
        }

        // public functions-------------------------------------------------------
        // 初始化一张空的地图
        public void Init(float width, float height, float cellwidth)
        {
            map_width_ = width;
            map_height_ = height;
            cell_width_ = cellwidth;
            GetCell(new Vector3(width, 0, height), out max_row_, out max_col_);
            max_row_++;
            max_col_++;
            if (max_col_ % 2 == 0)
            {
                max_row_++;
            }
            cells_arr_ = new byte[max_row_, max_col_];
        }

        // 从文件读取
        public bool Init(string filename)
        {
            if (!FileReaderProxy.Exists(filename))
            {
                return false;
            }
            try
            {
                using (MemoryStream ms = FileReaderProxy.ReadFileAsMemoryStream(filename))
                {
                    using (BinaryReader br = new BinaryReader(ms))
                    {
                        map_width_ = (float)br.ReadDouble();
                        map_height_ = (float)br.ReadDouble();
                        cell_width_ = (float)br.ReadDouble();
                        GetCell(new Vector3(map_width_, 0, map_height_), out max_row_, out max_col_);
                        max_row_++;
                        max_col_++;
                        if (max_col_ % 2 == 0)
                        {
                            max_row_++;
                        }
                        cells_arr_ = new byte[max_row_, max_col_];
                        int row = 0;
                        int col = 0;
                        while (ms.Position < ms.Length && row < max_row_)
                        {
                            cells_arr_[row, col] = br.ReadByte();
                            if (++col >= max_col_)
                            {
                                col = 0;
                                ++row;
                            }
                        }
                        br.Close();
                    }
                }
            }
            catch (Exception e)
            {
                LogSystem.Error("{0}\n{1}", e.Message, e.StackTrace);
                return false;
            }
            return true;
        }

        // 保存到文件
        public bool Save(string filename)
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write((float)map_width_);
                bw.Write((float)map_height_);
                bw.Write((float)cell_width_);
                bw.Flush();
                for (int row = 0; row < max_row_; row++)
                {
                    for (int col = 0; col < max_col_; col++)
                    {
                        bw.Write(cells_arr_[row, col]);
                    }
                }
                bw.Flush();
                bw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                LogSystem.Error("{0}\n{1}", e.Message, e.StackTrace);
                return false;
            }
            return true;
        }

        public void Scale(float scale)
        {
            map_width_ *= scale;
            map_height_ *= scale;
            cell_width_ *= scale;
        }

        // 查询cell
        public bool GetCell(Vector3 pos, out int cell_row, out int cell_col)
        {
            cell_row = (int)(pos.Z / cell_width_);
            float y = pos.Z - cell_row * cell_width_;
            if (y >= cell_width_ + 0.001f)
                ++cell_row;

            cell_col = (int)(pos.X / cell_width_);
            float x = pos.X - cell_col * cell_width_;
            if (x >= cell_width_ + 0.001f)
                ++cell_col;

            return cell_row >= 0 && cell_row < max_row_ && cell_col >= 0 && cell_col < max_col_;
        }

        public bool RayCast(Vector3 start, Vector3 end, bool includeCanShoot, bool includeCanLeap, bool includeCantLeap, bool includeLevel, out CellPos cell)
        {
            bool ret = false;
            byte cmpLevel = BlockType.LEVEL_UNDERFLOOR_2;
            if (includeLevel)
            {
                byte status = GetCellStatus(start);
                cmpLevel = BlockType.GetBlockLevel(status);
            }
            CellPos retCell = new CellPos();
            VisitCellsCrossByLine(start, end, (row, col) =>
            {
                if (IsCellMatch(row, col, includeCanShoot, includeCantLeap, includeCantLeap, includeLevel, cmpLevel))
                {
                    retCell.row = row;
                    retCell.col = col;
                    ret = true;
                    return false;
                }
                else
                {
                    return true;
                }
            });
            cell = retCell;
            return ret;
        }

        private bool IsCellMatch(int row, int col, bool includeCanShoot, bool includeCanLeap, bool includeCantLeap, bool includeLevel, byte cmpLevel)
        {
            bool isMatch = false;
            byte status = cells_arr_[row, col];
            byte blockType = BlockType.GetBlockType(status);
            byte subtype = BlockType.GetBlockSubType(status);
            byte blinding = BlockType.GetBlockBlinding(status);
            byte level = BlockType.GetBlockLevel(status);
            if (blockType != BlockType.NOT_BLOCK)
            {
                if (includeCanShoot && (subtype == BlockType.SUBTYPE_ROADBLOCK))
                    isMatch = true;
                if (includeCanLeap && (subtype == BlockType.SUBTYPE_SHOTPROOF || subtype == BlockType.SUBTYPE_ENERGYWALL))
                    isMatch = true;
                if (includeCantLeap && subtype == BlockType.SUBTYPE_OBSTACLE)
                    isMatch = true;
            }
            if (includeLevel && level > cmpLevel)
                isMatch = true;
            return isMatch;
        }

        public List<CellPos> GetCellsCrossByLine(Vector3 start, Vector3 end)
        {
            List<CellPos> result = new List<CellPos>();
            VisitCellsCrossByLine(start, end, (int row, int col) =>
            {
                result.Add(new CellPos(row, col));
            });
            return result;
        }
        public void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyAction<int, int> visitor)
        {
            VisitCellsCrossByLine(start, end, (row, col) => { visitor(row, col); return true; });
        }
        public void VisitCellsCrossByLine(Vector3 start, Vector3 end, MyFunc<int, int, bool> visitor)
        {
            int startRow, startCol, endRow, endCol;
            bool ltValid = GetCell(start, out startRow, out startCol);
            bool rbValid = GetCell(end, out endRow, out endCol);
            if (ltValid && rbValid)
            {
                const float c_Scale = 1000.0f;//防止tx/deltaTx/tz/deltaTz过小的倍率（防止小于浮点精度）
                int c = (int)(start.X / cell_width_);
                int r = (int)(start.Z / cell_width_);
                int cend = (int)(end.X / cell_width_);
                int rend = (int)(end.Z / cell_width_);

                int dc = ((c < cend) ? 1 : ((c > cend) ? -1 : 0));
                int dr = ((r < rend) ? 1 : ((r > rend) ? -1 : 0));

                float minX = cell_width_ * (float)Math.Floor(start.X / cell_width_);
                float maxX = minX + cell_width_;
                float tx = (dc == 0 ? float.MaxValue : (dc > 0 ? (maxX - start.X) : (start.X - minX)) * c_Scale / Math.Abs(end.X - start.X));
                float minZ = cell_width_ * (float)Math.Floor(start.Z / cell_width_);
                float maxZ = minZ + cell_width_;
                float tz = (dr == 0 ? float.MaxValue : (dr > 0 ? (maxZ - start.Z) : (start.Z - minZ)) * c_Scale / Math.Abs(end.Z - start.Z));

                float deltaTx = (dc == 0 ? float.MaxValue : cell_width_ * c_Scale / Math.Abs(end.X - start.X));
                float deltaTz = (dr == 0 ? float.MaxValue : cell_width_ * c_Scale / Math.Abs(end.Z - start.Z));

                if (dr == 0 && dc == 0)
                {
                    visitor(r, c);
                }
                else
                {
                    const int c_ErrorCount = 500;
                    int ct = 0;
                    for (; ct < c_ErrorCount; ++ct)
                    {
                        if (!visitor(r, c))
                            break;
                        if (tx <= tz)
                        {
                            if (c == cend)
                                break;
                            tx += deltaTx;
                            c += dc;
                        }
                        else
                        {
                            if (r == rend)
                                break;
                            tz += deltaTz;
                            r += dr;
                        }
                    }
                    if (ct >= c_ErrorCount)
                    {
                        LogSystem.Error("VisitCellsCrossByLine({0} -> {1}) c:{2} cend:{3} dc:{4} r:{5} rend:{6} dr:{7} deltaTx:{8} deltaTz:{9} minX:{10} maxX:{11} minZ:{12} maxZ:{13} tx:{14} tz:{15}", start.ToString(), end.ToString(),
                          c, cend, dc, r, rend, dr, deltaTx, deltaTz, minX, maxX, minZ, maxZ, tx, tz);
                    }
                }
            }
        }

        public List<CellPos> GetCellsCrossByPolyline(IList<Vector3> pts)
        {
            List<CellPos> cells = new List<CellPos>();
            VisitCellsCrossByPolyline(pts, (int row, int col) =>
            {
                cells.Add(new CellPos(row, col));
            });
            return cells;
        }
        public void VisitCellsCrossByPolyline(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            for (int vi = 0; vi < pts.Count - 1; ++vi)
            {
                VisitCellsCrossByLine(pts[vi], pts[vi + 1], visitor);
            }
        }
        public List<CellPos> GetCellsCrossByPolygon(IList<Vector3> pts)
        {
            List<CellPos> cells = new List<CellPos>();
            VisitCellsCrossByPolygon(pts, (int row, int col) =>
            {
                cells.Add(new CellPos(row, col));
            });
            return cells;
        }
        public void VisitCellsCrossByPolygon(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            for (int vi = 0; vi < pts.Count; ++vi)
            {
                VisitCellsCrossByLine(pts[vi], pts[(vi + 1) % pts.Count], visitor);
            }
        }

        public List<CellPos> GetCellsInPolygon(IList<Vector3> pts)
        {
            List<CellPos> ret = new List<CellPos>();
            VisitCellsInPolygon(pts, (int row, int col) =>
            {
                ret.Add(new CellPos(row, col));
            });
            return ret;
        }
        public void VisitCellsInPolygon(IList<Vector3> pts, MyAction<int, int> visitor)
        {
            float maxXval = pts[0].X;
            float minXval = pts[0].X;
            float maxZval = pts[0].Z;
            float minZval = pts[0].Z;
            for (int i = 1; i < pts.Count; ++i)
            {
                int ix = i;
                float xv = pts[ix].X;
                float zv = pts[ix].Z;
                if (maxXval < xv)
                    maxXval = xv;
                else if (minXval > xv)
                    minXval = xv;
                if (maxZval < zv)
                    maxZval = zv;
                else if (minZval > zv)
                    minZval = zv;
            }
            int startRow = 0, endRow = 0;
            int startCol = 0, endCol = 0;
            bool ltValid = GetCell(new Vector3(minXval, 0, minZval), out startRow, out startCol);
            bool rbValid = GetCell(new Vector3(maxXval, 0, maxZval), out endRow, out endCol);
            if (ltValid && rbValid)
            {
                for (int row = startRow; row <= endRow; ++row)
                {
                    for (int col = startCol; col <= endCol; ++col)
                    {
                        Vector3 pt = GetCellCenter(row, col);
                        int pos = Geometry.PointInPolygon(pt, pts, 0, pts.Count, minXval, maxXval, minZval, maxZval);
                        if (pos > 0)
                        {
                            visitor(row, col);
                        }
                    }
                }
            }
        }

        public List<CellPos> GetCellsInCircle(Vector3 center, float radius)
        {
            List<CellPos> ret = new List<CellPos>();
            VisitCellsInCircle(center, radius, (int row, int col) =>
            {
                ret.Add(new CellPos(row, col));
            });
            return ret;
        }
        public void VisitCellsInCircle(Vector3 center, float radius, MyAction<int, int> visitor)
        {
            float maxXval = center.X + radius;
            float minXval = center.X - radius;
            float maxZval = center.Z + radius;
            float minZval = center.Z - radius;
            int startRow = 0, endRow = 0;
            int startCol = 0, endCol = 0;
            bool ltValid = GetCell(new Vector3(minXval, 0, minZval), out startRow, out startCol);
            bool rbValid = GetCell(new Vector3(maxXval, 0, maxZval), out endRow, out endCol);
            if (ltValid && rbValid)
            {
                float powRadius = radius * radius;
                for (int row = startRow; row <= endRow; ++row)
                {
                    for (int col = startCol; col <= endCol; ++col)
                    {
                        Vector3 pt = GetCellCenter(row, col);
                        if ((pt - center).LengthSquared() <= powRadius)
                        {
                            visitor(row, col);
                        }
                    }
                }
            }
        }

        public List<CellPos> GetCellsCrossByCircle(Vector3 center, float radius)
        {
            List<CellPos> ret = new List<CellPos>();
            VisitCellsCrossByCircle(center, radius, (int row, int col) =>
            {
                ret.Add(new CellPos(row, col));
            });
            return ret;
        }
        public void VisitCellsCrossByCircle(Vector3 center, float radius, MyAction<int, int> visitor)
        {
            float maxXval = center.X + radius;
            float minXval = center.X - radius;
            float maxZval = center.Z + radius;
            float minZval = center.Z - radius;
            int startRow = 0, endRow = 0;
            int startCol = 0, endCol = 0;
            bool ltValid = GetCell(new Vector3(minXval, 0, minZval), out startRow, out startCol);
            bool rbValid = GetCell(new Vector3(maxXval, 0, maxZval), out endRow, out endCol);
            if (ltValid && rbValid)
            {
                float minPowRadius = (radius - 0.707f * cell_width_) * (radius - 0.707f * cell_width_);
                float maxPowRadius = (radius + 0.707f * cell_width_) * (radius + 0.707f * cell_width_);
                for (int row = startRow; row <= endRow; ++row)
                {
                    for (int col = startCol; col <= endCol; ++col)
                    {
                        Vector3 pt = GetCellCenter(row, col);
                        float powDist = (pt - center).LengthSquared();
                        if (powDist >= minPowRadius && powDist <= maxPowRadius)
                        {
                            visitor(row, col);
                        }
                    }
                }
            }
        }

        public Vector3 GetCellCenter(int row, int col)
        {
            Vector3 center = new Vector3();
            center.X = col * cell_width_ + cell_width_ / 2;
            center.Z = row * cell_width_ + cell_width_ / 2;
            return center;
        }

        public byte GetCellStatus(int row, int col)
        {
            if (row >= max_row_ || col >= max_col_ || row < 0 || col < 0)
            {
                return BlockType.OUT_OF_BLOCK;
            }
            return cells_arr_[row, col];
        }

        public byte GetCellStatus(Vector3 pos)
        {
            int row, col;
            GetCell(pos, out row, out col);
            if (row >= max_row_ || col >= max_col_ || row < 0 || col < 0)
            {
                return BlockType.OUT_OF_BLOCK;
            }
            return cells_arr_[row, col];
        }

        public void SetCellStatus(Vector3 pos, byte status)
        {
            int row, col;
            GetCell(pos, out row, out col);
            if (row >= max_row_ || col >= max_col_ || row < 0 || col < 0)
            {
                return;
            }
            cells_arr_[row, col] = status;
        }

        public void SetCellStatus(int row, int col, byte status)
        {
            if (row >= max_row_ || col >= max_col_ || row < 0 || col < 0)
            {
                return;
            }
            cells_arr_[row, col] = status;
        }

        public List<CellPos> GetCellAdjacent(CellPos center)
        {
            return CellMapView.GetCellAdjacent(center, max_row_, max_col_);
        }

        public ICellMapView GetCellMapView(int radius)
        {
            ICellMapView view = null;
            if (cell_map_views_.ContainsKey(radius))
            {
                view = cell_map_views_[radius];
            }
            else
            {
                view = new CellMapView(this, radius);
                cell_map_views_.Add(radius, view);
            }
            return view;
        }

        public int GetMaxRow() { return max_row_; }
        public int GetMaxCol() { return max_col_; }
        public float GetMapWidth() { return map_width_; }
        public float GetMapHeight() { return map_height_; }
        public float GetCellWidth() { return cell_width_; }

        // private functions------------------------------------------------------

        public Polygon GetCellShape(Vector3 center)
        {
            Vector3[] v = new Vector3[4];
            v[0].X = center.X - cell_width_ / 2;
            v[0].Z = center.Z - cell_width_ / 2;
            v[1].X = center.X + cell_width_ / 2;
            v[1].Z = center.Z - cell_width_ / 2;
            v[2].X = center.X + cell_width_ / 2;
            v[2].Z = center.Z + cell_width_ / 2;
            v[3].X = center.X - cell_width_ / 2;
            v[3].Z = center.Z + cell_width_ / 2;
            Polygon polygon = new Polygon();
            for (int i = 0; i < 4; i++)
            {
                polygon.AddVertex(v[i]);
            }
            polygon.IdenticalTransform();
            return polygon;
        }

        // private attribute------------------------------------------------------
        private float map_width_;   // 地图的宽度
        private float map_height_;  // 地图的高度
        private float cell_width_; // 阻挡区域的一边边长
        private int max_row_;
        private int max_col_;
        private Collide collide_detector_ = new Collide();
        private MyDictionary<int, ICellMapView> cell_map_views_ = new MyDictionary<int, ICellMapView>();

        internal byte[,] cells_arr_;
    }
}
