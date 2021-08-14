using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DashFireSpatial;

namespace DashFire
{
  public class SpatialBoard : ILosBoard
  {
    public SpatialBoard(ICellMapView cellMapView)
    {
      m_CellMapView = cellMapView;
      m_Width = m_CellMapView.MaxColCount;
      m_Height = m_CellMapView.MaxRowCount;
    }

    public void ResetVisited(int x,int y,int radius)
    {
      m_Left = x - radius;      
      m_Top = y - radius;
      m_Right = x + radius;
      m_Bottom = y + radius;
      if (m_Left < 0) m_Left = 0;
      if (m_Top < 0) m_Top = 0;
      if (m_Right >= m_Width) m_Right = m_Width - 1;
      if (m_Bottom >= m_Height) m_Bottom = m_Height - 1;
      m_Visited = new bool[radius * 2 + 1, radius * 2 + 1];

      SetViewPoint(x, y);
      ResetViewed(radius);

      m_RepeatCountForVisit = 0;
      m_TotalCountForVisit = 0;
    }

    public void ResetViewed(int radius)
    {
      m_Viewed = new bool[radius * 2 + 1, radius * 2 + 1];
      LogSystem.Debug("FovViewed reset.");
    }
    public void View(int x, int y)
    {
      if (null!=m_Viewed && Contains(x, y)) {
        m_Viewed[x - m_Left, y - m_Top] = true;
      }
    }
    public bool IsViewed(int x, int y)
    {
      bool ret = false;
      if (null != m_Viewed && Contains(x, y)) {
        ret = m_Viewed[x - m_Left, y - m_Top];
      }
      return ret;
    }

    public void SetViewPoint(int x, int y)
    {
      m_ViewerX = x;
      m_ViewerY = y;
      m_ViewerLevel = m_CellMapView.GetCellLevel(m_ViewerY, m_ViewerX);
    }

    public bool Contains(int x, int y)
    {
      return x >= m_Left && y >= m_Top && x <= m_Right && y <= m_Bottom;
    }

    public bool IsObstacle(int x, int y)
    {
      if (Contains(x, y)) {
        if (IsViewed(x, y)) {
          return false;
        }
        bool ret = !m_CellMapView.CanSee(y, x);
        if (!ret) {
          byte lvl = m_CellMapView.GetCellLevel(y, x);
          ret = (lvl == BlockType.LEVEL_FLOOR_BLINDAGE || lvl > m_ViewerLevel);
        }
        return ret;
      } else {
        return true;
      }
    }

    public void Visit(int x, int y)
    {
      if (Contains(x, y)) {
        ++m_TotalCountForVisit;
        if (m_Visited[x - m_Left, y - m_Top])
          ++m_RepeatCountForVisit;
        else
          m_Visited[x - m_Left, y - m_Top] = true;
      }
    }

    public bool IsVisited(int x, int y)
    {
      bool ret = false;
      if (null != m_Visited && Contains(x, y)) {
        ret = m_Visited[x - m_Left, y - m_Top];
      }
      return ret;
    }
    public int RepeatCountForVisit
    {
      get { return m_RepeatCountForVisit; }
    }
    public int TotalCountForVisit
    {
      get { return m_TotalCountForVisit; }
    }

    private ICellMapView m_CellMapView = null;
    private int m_Width = 0;
    private int m_Height = 0;
    private int m_Left = 0;
    private int m_Top = 0;
    private int m_Right = 0;
    private int m_Bottom = 0;
    private bool[,] m_Visited = null;
    private bool[,] m_Viewed = null;
    private int m_ViewerX = 0;
    private int m_ViewerY = 0;
    private int m_ViewerLevel = 0;
    private int m_RepeatCountForVisit = 0;
    private int m_TotalCountForVisit = 0;
  }
}
