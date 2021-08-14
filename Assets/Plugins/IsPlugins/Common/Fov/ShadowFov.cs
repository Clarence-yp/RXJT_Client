using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class ShadowFov
  {
    public ShadowFov(ILosBoard board)
    {
      m_Board = board;
    }

    public void CalculateFOV(int startx, int starty, float force, float decay, RadiusStrategy rStrat)
    {
      m_StartX = startx;
      m_StartY = starty;
      m_Force = force;
      m_Decay = decay;
      m_RadiusStrategy = rStrat;

      m_Radius = (force / decay);

      m_Board.Visit(startx, starty);
      foreach (Direction d in Direction.DIAGONALS) {
        CastLight(1, 1.0f, 0.0f, 0, d.m_DeltaX, d.m_DeltaY, 0);
        CastLight(1, 1.0f, 0.0f, d.m_DeltaX, 0, 0, d.m_DeltaY);
      }
    }

    private void CastLight(int row, float start, float end, int xx, int xy, int yx, int yy)
    {

      float newStart = 0.0f;
      if (start < end) {
        return;
      }
      bool blocked = false;
      for (int distance = row; distance <= m_Radius && !blocked; distance++) {
        int deltaY = -distance;
        for (int deltaX = -distance; deltaX <= 0; deltaX++) {
          int currentX = m_StartX + deltaX * xx + deltaY * xy;
          int currentY = m_StartY + deltaX * yx + deltaY * yy;
          float leftSlope = (deltaX - 0.5f) / (deltaY + 0.5f);
          float rightSlope = (deltaX + 0.5f) / (deltaY - 0.5f);

          if (!m_Board.Contains(currentX,currentY) || start < rightSlope) {
            continue;
          } else if (end > leftSlope) {
            break;
          }

          //check if it's within the lightable area and light if needed
          if (m_RadiusStrategy.Radius(deltaX, deltaY) <= m_Radius) {
            float bright = (float)(1 - (m_Decay * m_RadiusStrategy.Radius(deltaX, deltaY) / m_Force));
            if (bright > 0)
              m_Board.Visit(currentX, currentY);
          }

          if (blocked) { //previous cell was a blocking one
            if (m_Board.IsObstacle(currentX, currentY)) {//hit a wall
              newStart = rightSlope;
              continue;
            } else {
              blocked = false;
              start = newStart;
            }
          } else {
            if (m_Board.IsObstacle(currentX, currentY) && distance < m_Radius) {//hit a wall within sight line
              blocked = true;
              CastLight(distance + 1, start, leftSlope, xx, xy, yx, yy);
              newStart = rightSlope;
            }
          }
        }
      }
    }

    public void CalculateFOV(int startx, int starty, float radius)
    {
      CalculateFOV(startx, starty, 1, 1 / radius, BasicRadiusStrategy.s_Circle);
    }

    private ILosBoard m_Board = null;
    private int m_StartX, m_StartY;
    private float m_Force, m_Decay, m_Radius;
    private RadiusStrategy m_RadiusStrategy;
  }
}
