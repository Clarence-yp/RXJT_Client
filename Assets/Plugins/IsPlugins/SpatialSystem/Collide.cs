/**
 * @file   Collide.cs
 * @author carl <zhangnaisheng@cyou-inc.com>
 * @date   Fri Apr 26 16:40:34 2013
 * 
 * @brief  碰撞检测算法类，提供多边形，圆等凸多边形的碰撞检测算法
 * 
 * 
 */


using System;
using System.Collections.Generic;
using ScriptRuntime;

namespace DashFireSpatial
{
    public sealed class Collide
    {
        //判断两个多边形是否分离
        public bool SeperatePolygon(Vector3 axis_vector, Polygon one, Polygon two)
        {
            float p00 = 1 - (float)Math.Pow(axis_vector.X, 2);
            float p01 = -axis_vector.X * axis_vector.Z;
            float p10 = p01;
            float p11 = 1 - (float)Math.Pow(axis_vector.Z, 2);
            List<Vector3> one_list = one.world_vertex();
            Vector3 one_min = new Vector3(0, 0, 0);
            Vector3 one_max = new Vector3(0, 0, 0);
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.X = (float)(one_list[i].X * p00 + one_list[i].Z * p10);
                pass_pos.Z = (float)(one_list[i].X * p01 + one_list[i].Z * p11);
                if (i == 0)
                {
                    one_min = pass_pos;
                    one_max = pass_pos;
                }
                if (pass_pos.X < one_min.X ||
                    (pass_pos.X == one_min.X && pass_pos.Z < one_min.Z))
                {
                    one_min = pass_pos;
                }
                if (pass_pos.X > one_max.X ||
                    (pass_pos.X == one_max.X && pass_pos.Z > one_max.Z))
                {
                    one_max = pass_pos;
                }
            }

            List<Vector3> two_list = two.world_vertex();
            Vector3 two_min = new Vector3(0, 0, 0);
            Vector3 two_max = new Vector3(0, 0, 0);
            for (int i = 0; i < two_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.X = (float)(two_list[i].X * p00 + two_list[i].Z * p10);
                pass_pos.Z = (float)(two_list[i].X * p01 + two_list[i].Z * p11);
                if (i == 0)
                {
                    two_min = pass_pos;
                    two_max = pass_pos;
                }
                if (pass_pos.X < two_min.X ||
                    (pass_pos.X == two_min.X && pass_pos.Z < two_min.Z))
                {
                    two_min = pass_pos;
                }
                if (pass_pos.X > two_max.X ||
                    (pass_pos.X == two_max.X && pass_pos.Z > two_max.Z))
                {
                    two_max = pass_pos;
                }
            }

            if (one_min.X > two_max.X ||
                (one_min.X == two_max.X && one_min.Z > two_max.Z))
            {
                return true;
            }
            if (one_max.X < two_min.X ||
                (one_max.X == two_min.X && one_max.Z < two_min.Z))
            {
                return true;
            }
            return false;
        }

        //判断矩形和圆是否分离
        public bool SeperatePolygonCircle(Vector3 axis_vector, Polygon rect, Circle circle)
        {
            float p00 = 1 - (float)Math.Pow(axis_vector.X, 2);
            float p01 = -axis_vector.X * axis_vector.Z;
            float p10 = p01;
            float p11 = 1 - (float)Math.Pow(axis_vector.Z, 2);

            //计算多边形的最大最小透影
            List<Vector3> one_list = rect.world_vertex();
            Vector3 one_min = new Vector3(0, 0, 0);
            Vector3 one_max = new Vector3(0, 0, 0);
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 pass_pos = new Vector3(0, 0, 0);
                pass_pos.X = (float)(one_list[i].X * p00 + one_list[i].Z * p10);
                pass_pos.Z = (float)(one_list[i].X * p01 + one_list[i].Z * p11);
                if (i == 0)
                {
                    one_min = pass_pos;
                    one_max = pass_pos;
                }
                if (pass_pos.X < one_min.X ||
                    (pass_pos.X == one_min.X && pass_pos.Z < one_min.Z))
                {
                    one_min = pass_pos;
                }
                if (pass_pos.X > one_max.X ||
                    (pass_pos.X == one_max.X && pass_pos.Z > one_max.Z))
                {
                    one_max = pass_pos;
                }
            }

            //计算圆的最大最小透影
            float two_min_x = 0;
            float two_min_y = 0;
            float two_max_x = 0;
            float two_max_y = 0;
            Vector3 center = circle.world_center_pos();
            Vector3 center_pass = new Vector3(0, 0, 0);
            center_pass.X = (float)(center.X * p00 + center.Z * p10);
            center_pass.Z = (float)(center.X * p01 + center.Z * p11);
            float half_x = circle.radius() * Math.Abs(axis_vector.Z);
            two_min_x = center_pass.X - half_x;
            two_max_x = center_pass.X + half_x;
            float half_y = circle.radius() * Math.Abs(axis_vector.X);
            two_min_y = center_pass.Z - half_y;
            two_max_y = center_pass.Z + half_y;

            //判断是否分离
            if (one_min.X > two_max_x ||
                (one_min.X == two_max_x && one_min.Z > two_max_y))
            {
                return true;
            }
            if (one_max.X < two_min_x ||
                (one_max.X == two_min_x && one_max.Z < two_min_y))
            {
                return true;
            }
            return false;
        }

        //判断两个圆是否碰撞
        public bool IntersectCircle(Circle one, Circle two)
        {
            Vector3 one_center = one.world_center_pos();
            Vector3 two_center = two.world_center_pos();
            float center_distance_square = DashFire.Geometry.DistanceSquare(one_center, two_center);
            float radius = one.radius() + two.radius();
            if (radius * radius < center_distance_square)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //取得两点的垂直单位向量
        public Vector3 GetVerticalVector(Vector3 one, Vector3 two)
        {
            Vector3 vertical_vect = new Vector3(0, 0, 0);
            float length = (float)Math.Sqrt(
                                      Math.Pow((one.X - two.X), 2) +
                                      Math.Pow((one.Z - two.Z), 2));
            vertical_vect.X = (float)((two.Z - one.Z) / length);
            vertical_vect.Z = (float)((one.X - two.X) / length);
            return vertical_vect;
        }

        public bool IsLineCrossShape(Line line, Shape shape)
        {
            Vector3 axis = GetVerticalVector(line.GetStartPos(), line.GetEndPos());
            if (IsSeperateByAxis(axis, line, shape))
            {
                return false;
            }
            axis = GetVerticalVector(new Vector3(0, 0, 0), axis);
            if (IsSeperateByAxis(axis, line, shape))
            {
                return false;
            }
            if (shape.GetShapeType() == ShapeType.kShapeCircle)
            {
                Vector3 center = ((Circle)shape).world_center_pos();
                axis = GetVerticalVector(center, line.GetStartPos());
                if (IsSeperateByAxis(axis, line, shape))
                {
                    return false;
                }
                axis = GetVerticalVector(line.GetEndPos(), center);
                if (IsSeperateByAxis(axis, line, shape))
                {
                    return false;
                }
            }
            if (shape.GetShapeType() == ShapeType.kShapePolygon)
            {
                List<Vector3> rect_list = ((Polygon)shape).world_vertex();
                for (int i = 0; i < rect_list.Count; i++)
                {
                    if (i == rect_list.Count - 1)
                    {
                        axis = GetVerticalVector(rect_list[i], rect_list[0]);
                    }
                    else
                    {
                        axis = GetVerticalVector(rect_list[i], rect_list[i + 1]);
                    }
                    if (IsSeperateByAxis(axis, line, shape))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<Vector3> GetCollidePoint(Line line, Shape shape)
        {
            List<Vector3> collide_points = new List<Vector3>();
            Vector3 pos = new Vector3();
            if (shape.GetShapeType() == ShapeType.kShapePolygon)
            {
                List<Vector3> vect_list = ((Polygon)shape).world_vertex();
                for (int i = 0; i < vect_list.Count; i++)
                {
                    Line edge;
                    if (i == vect_list.Count - 1)
                    {
                        edge = new Line(vect_list[i], vect_list[0]);
                    }
                    else
                    {
                        edge = new Line(vect_list[i], vect_list[i + 1]);
                    }
                    if (GetHitPoint(line, edge, out pos))
                    {
                        collide_points.Add(pos);
                    }
                }
            }
            Vector3 startpos = line.GetStartPos();
            collide_points.Sort((one, two) =>
            {
                if (Math.Abs(one.X - startpos.X) < Math.Abs(two.X - startpos.X) ||
                  (Math.Abs(one.X - startpos.X) <= 0.0001 && Math.Abs(one.Z - startpos.Z) < Math.Abs(two.Z - startpos.Z)))
                {
                    return -1;
                }
                else if (Math.Abs(one.X - two.X) < 0.0001 && Math.Abs(one.Z - two.Z) < 0.0001)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            });
            return collide_points;
        }


        public bool Intersect(Shape one, Shape two)
        {
            if (one.GetShapeType() == ShapeType.kShapeCircle &&
                two.GetShapeType() == ShapeType.kShapeCircle)
            {//两个都是圆
                return IntersectCircle((Circle)one, (Circle)two);
            }
            else if (one.GetShapeType() == ShapeType.kLine)
            {
                return IsLineCrossShape((Line)one, two);
            }
            else if (two.GetShapeType() == ShapeType.kLine)
            {
                return IsLineCrossShape((Line)two, one);
            }
            else if (one.GetShapeType() == ShapeType.kShapeCircle)
            {//第一个是圆，第二个是矩形
                List<Vector3> rect_list = ((Polygon)two).world_vertex();
                Vector3 center_pos = ((Circle)one).world_center_pos();
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    if (i == rect_list.Count - 1)
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[0]);
                    }
                    else
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[i + 1]);
                    }
                    if (SeperatePolygonCircle(vector, (Polygon)two, (Circle)one))
                    {
                        return false;
                    }
                }
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    vector = GetVerticalVector(center_pos, rect_list[i]);
                    if (SeperatePolygonCircle(vector, (Polygon)two, (Circle)one))
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (two.GetShapeType() == ShapeType.kShapeCircle)
            {//第一个是矩形，第二个是圆
                List<Vector3> rect_list = ((Polygon)one).world_vertex();
                Vector3 center_pos = ((Circle)two).world_center_pos();
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    if (i == rect_list.Count - 1)
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[0]);
                    }
                    else
                    {
                        vector = GetVerticalVector(rect_list[i], rect_list[i + 1]);
                    }
                    if (SeperatePolygonCircle(vector, (Polygon)one, (Circle)two))
                    {
                        return false;
                    }
                }
                for (int i = 0; i < rect_list.Count; i++)
                {
                    Vector3 vector = new Vector3(0, 0, 0);
                    vector = GetVerticalVector(center_pos, rect_list[i]);
                    if (SeperatePolygonCircle(vector, (Polygon)one, (Circle)two))
                    {
                        return false;
                    }
                }
                return true;
            }

            //两个都矩形
            //判断第一个矩形边的透影
            List<Vector3> one_list = ((Polygon)one).world_vertex();
            for (int i = 0; i < one_list.Count; i++)
            {
                Vector3 vector = new Vector3(0, 0, 0);
                if (i == one_list.Count - 1)
                {
                    vector = GetVerticalVector(one_list[i], one_list[0]);
                }
                else
                {
                    vector = GetVerticalVector(one_list[i], one_list[i + 1]);
                }
                if (SeperatePolygon(vector, (Polygon)one, (Polygon)two))
                {
                    return false;
                }
            }

            //判断第二个矩形边的透影
            List<Vector3> two_list = ((Polygon)two).world_vertex();
            for (int i = 0; i < two_list.Count; i++)
            {
                Vector3 vector = new Vector3(0, 0, 0);
                if (i == two_list.Count - 1)
                {
                    vector = GetVerticalVector(two_list[i], two_list[0]);
                }
                else
                {
                    vector = GetVerticalVector(two_list[i], two_list[i + 1]);
                }
                if (SeperatePolygon(vector, (Polygon)one, (Polygon)two))
                {
                    return false;
                }
            }
            return true;
        }

        // y = kx + c
        // ax + by = d
        private bool GetHitPoint(Line one, Line two, out Vector3 hitpos)
        {
            hitpos = new Vector3();
            Vector3 vd1 = new Vector3(one.GetEndPos().X - one.GetStartPos().X, 0, one.GetEndPos().Z - one.GetStartPos().Z);
            float a1 = vd1.Z;
            float b1 = -vd1.X;
            float d1 = one.GetStartPos().X * vd1.Z - one.GetStartPos().Z * vd1.X;
            Vector3 vd2 = new Vector3(two.GetEndPos().X - two.GetStartPos().X, 0, two.GetEndPos().Z - two.GetStartPos().Z);
            float a2 = vd2.Z;
            float b2 = -vd2.X;
            float d2 = two.GetStartPos().X * vd2.Z - two.GetStartPos().Z * vd2.X;
            float denominator = a1 * b2 - a2 * b1;
            if (Math.Abs(denominator) >= 0.0001)
            {
                hitpos.X = (b2 * d1 - b1 * d2) / denominator;
                hitpos.Z = (a1 * d2 - a2 * d1) / denominator;
                if (IsPointInLine(hitpos, one) && IsPointInLine(hitpos, two))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            float c1, c2;
            if (Math.Abs(a1) <= 0.0001)
            {
                c1 = d1 / b1;
                c2 = d2 / b2;
            }
            else
            {
                c1 = d1 / a1;
                c2 = d2 / a2;
            }
            // parallel
            if (Math.Abs(c1 - c2) > 0.0001)
            {
                return false;
            }
            //overlaped
            bool isstartin = IsPointInLine(two.GetStartPos(), one);
            bool isendin = IsPointInLine(two.GetEndPos(), one);
            if (isstartin && isendin)
            {
                if (Math.Abs(two.GetStartPos().X - one.GetStartPos().X) < Math.Abs(two.GetEndPos().X - one.GetStartPos().X) ||
                  (Math.Abs(one.GetStartPos().X - one.GetEndPos().X) <= 0.0001 &&
                  Math.Abs(two.GetStartPos().Z - one.GetStartPos().Z) < Math.Abs(two.GetEndPos().Z - one.GetStartPos().Z)))
                {
                    hitpos = two.GetStartPos();
                    return true;
                }
                else
                {
                    hitpos = two.GetEndPos();
                    return true;
                }
            }
            else if (isstartin)
            {
                hitpos = two.GetStartPos();
                return true;
            }
            else if (isendin)
            {
                hitpos = two.GetEndPos();
                return true;
            }
            else
            {
                return false;
            }
        }

        // constraint: the point must on the straight line but maybe not bettwen the line.start and line.end
        // this is judge whether it's bettwen or not
        private bool IsPointInLine(Vector3 pos, Line line)
        {
            Vector3 min, max;
            if (line.GetStartPos().X < line.GetEndPos().X ||
                (Math.Abs(line.GetStartPos().X - line.GetEndPos().X) <= 0.0001 &&
                line.GetStartPos().Z < line.GetEndPos().Z))
            {
                min = line.GetStartPos();
                max = line.GetEndPos();
            }
            else
            {
                min = line.GetEndPos();
                max = line.GetStartPos();
            }
            return !IsShadowSeperate(pos, pos, min, max);
        }

        private bool IsSeperateByAxis(Vector3 axis, Line line, Shape shape)
        {
            Vector3 line_shadow_min, line_shadow_max;
            Vector3 shape_shadow_min, shape_shadow_max;
            line.GetShadowOnAxis(axis, out line_shadow_min, out line_shadow_max);
            shape.GetShadowOnAxis(axis, out shape_shadow_min, out shape_shadow_max);
            return IsShadowSeperate(line_shadow_min, line_shadow_max, shape_shadow_min, shape_shadow_max);
        }

        private bool IsShadowSeperate(Vector3 one_min, Vector3 one_max, Vector3 two_min, Vector3 two_max)
        {
            //判断是否分离
            if (one_min.X > two_max.X ||
                (one_min.X == two_max.X && one_min.Z > two_max.Z))
            {
                return true;
            }
            if (one_max.X < two_min.X ||
                (one_max.X == two_min.X && one_max.Z < two_min.Z))
            {
                return true;
            }
            return false;
        }
    }
}
