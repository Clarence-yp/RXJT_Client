using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public sealed class Point2I
  {
    public Point2I(int x, int y)
    {
      this.m_X = x; this.m_Y = y;
    }

    /**
     * Uses x+y as hash
     */
    public int HashCode()
    {
      return m_X << 7 - m_X + m_Y;//x*prime+y
    }

    public override String ToString()
    {
      return "Point2I[ " + m_X + ", " + m_Y + " ]";
    }

    public int m_X = 0;
    public int m_Y = 0;
  }
  public sealed class Line2I
  {
    public Line2I(Point2I newNear, Point2I newFar)
    {
      m_Near = newNear;
      m_Far = newFar;
    }

    public bool IsBelow(Point2I point)
    {
      return RelativeSlope(point) > 0;
    }

    public bool IsBelowOrContains(Point2I point)
    {
      return RelativeSlope(point) >= 0;
    }

    public bool IsAbove(Point2I point)
    {
      return RelativeSlope(point) < 0;
    }

    public bool IsAboveOrContains(Point2I point)
    {
      return RelativeSlope(point) <= 0;
    }

    public bool DoesContain(Point2I point)
    {
      return RelativeSlope(point) == 0;
    }

    // negative if the line is above the point.
    // positive if the line is below the point.
    // 0 if the line is on the point.
    public int RelativeSlope(Point2I point)
    {
      return (m_Far.m_Y - m_Near.m_Y) * (m_Far.m_X - point.m_X) - (m_Far.m_Y - point.m_Y)
          * (m_Far.m_X - m_Near.m_X);
    }

    public override String ToString()
    {
      return "( " + m_Near + " -> " + m_Far + " )";
    }

    public Point2I m_Near;
    public Point2I m_Far;
  }
  public enum FovType : int
  {
    SQUARE,
    CIRCLE
  }
  public sealed class GenericCalculateProjection
  {
    public static List<Point2I> CalculateProjecton(int startX, int startY, int x1, int y1,
        VisitedBoard fb)
    {
      List<Point2I> path = new List<Point2I>();

      // calculate usual Bresenham values required.
      int dx = x1 - startX;
      int dy = y1 - startY;
      int signX, signY;
      int adx, ady;
      if (dx > 0) {
        adx = dx;
        signX = 1;
      } else {
        adx = -dx;
        signX = -1;
      }
      if (dy > 0) {
        ady = dy;
        signY = 1;
      } else {
        ady = -dy;
        signY = -1;
      }
      bool axesSwapped = false;
      if (adx < ady) {
        axesSwapped = true;
        int tmp = adx;
        adx = ady;
        ady = tmp;
      }

      //calculate the two error values.
      int incE = 2 * ady; //error diff if x++
      int incNE = 2 * ady - 2 * adx; // error diff if x++ and y++
      int d = 2 * ady - adx; // starting error 
      Point2I p = new Point2I(0, 0);
      int lasti = 0, lastj = 0;
      int j = 0;
      for (int i = 0; i <= adx; ) {
        if (axesSwapped) {
          path.Add(new Point2I(
              (j * signX + startX),
              (i * signY + startY)));
        } else {
          path.Add(new Point2I(
          (i * signX + startX),
          (j * signY + startY)));
        }
        lasti = i;
        lastj = j;
        bool ippNotrecommended = false;//whether i++ is recommended
        if (d <= 0) {
          // try to just inc x
          if (axesSwapped) {
            p.m_Y = i + 1;
            p.m_X = j;
          } else {
            p.m_X = i + 1;
            p.m_Y = j;
          }
          if (fb.WasVisited(p.m_X, p.m_Y)) {
            d += incE;
            i++;
            continue;
          }
        } else {
          ippNotrecommended = true;
        }

        // try to inc x and y
        if (axesSwapped) {
          p.m_Y = i + 1;
          p.m_X = j + 1;
        } else {
          p.m_X = i + 1;
          p.m_Y = j + 1;
        }
        if (fb.WasVisited(p.m_X, p.m_Y)) {
          d += incNE;
          j++;
          i++;
          continue;
        }
        if (ippNotrecommended) { // try it even if not recommended
          if (axesSwapped) {
            p.m_Y = i + 1;
            p.m_X = j;
          } else {
            p.m_X = i + 1;
            p.m_Y = j;
          }
          if (fb.WasVisited(p.m_X, p.m_Y)) {
            d += incE;
            i++;
            continue;
          }
        }
        // last resort
        // try to inc just y
        if (axesSwapped) {
          p.m_Y = i;
          p.m_X = j + 1;
        } else {
          p.m_X = i;
          p.m_Y = j + 1;
        }
        if (fb.WasVisited(p.m_X, p.m_Y)) {
          d += -incE + incNE;
          j++;
          continue;
        }
        // no path, end here, after adding last point.
        if (axesSwapped) {
          path.Add(new Point2I(
              (j * signX + startX),
              (i * signY + startY)));
        } else {
          path.Add(new Point2I(
          (i * signX + startX),
          (j * signY + startY)));
        }
        break;
      }

      return path;
    }

    public interface VisitedBoard
    {
      bool WasVisited(int x, int y);
    }
  }
  public sealed class CLikeIterator<T>
  {
    public CLikeIterator(LinkedListNode<T> it)
    {
      m_Iterator = it;
      if (null != m_Iterator && null != m_Iterator.Value)
        m_Current = m_Iterator.Value;
      else {
        m_Current = default(T);
        m_IsAtEnd = true;
      }
    }

    public T GetCurrent()
    {
      return m_Current;
    }

    public void GotoNext()
    {
      if (null != m_Iterator && null != m_Iterator.Next) {
        m_Iterator = m_Iterator.Next;
        m_Current = m_Iterator.Value;
      } else {
        m_IsAtEnd = true;
        m_Current = default(T);
      }
    }

    public void GotoPrevious()
    {
      if (null != m_Iterator && null != m_Iterator.Previous) {
        m_Iterator = m_Iterator.Previous;
        m_Current = m_Iterator.Value;
      } else {
        m_IsAtEnd = true;
        m_Current = default(T);
      }
    }

    public bool IsAtEnd()
    {
      return m_IsAtEnd;
    }

    public void RemoveCurrent()
    {
      if (null != m_Iterator) {
        LinkedListNode<T> t = m_Iterator;
        GotoNext();
        t.List.Remove(t);
      }
    }

    public void InsertBeforeCurrent(T t)
    {
      if (null != m_Iterator) {
        m_Iterator.List.AddBefore(m_Iterator, t);
      }
    }

    private LinkedListNode<T> m_Iterator;
    private T m_Current;
    private bool m_IsAtEnd = false;
  }
}
