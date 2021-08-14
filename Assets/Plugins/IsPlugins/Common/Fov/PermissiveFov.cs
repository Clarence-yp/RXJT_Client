using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptRuntime;

namespace DashFire
{
  public class PrecisePermissive
  {
    public class permissiveMaskT
    {
      public int m_North;
      public int m_South;
      public int m_East;
      public int m_West;
      public int[] m_Mask;
      public FovType m_FovType;
      public int m_DistPlusOneSq;
      public ILosBoard m_Board;
    }
    public class fovStateT
    {
      public Point2I m_Source;
      public permissiveMaskT m_Mask;
      public Object m_Context;
      public Point2I m_Quadrant;
      public Point2I m_Extent;
      public int m_QuadrantIndex;
      public ILosBoard m_Board;
      public bool m_IsLos = false;
    }
    public class bumpT
    {
      public bumpT()
      {
      }
      public override String ToString()
      {
        return m_Location.ToString() + " p( " + m_Parent + " ) ";
      }

      public Point2I m_Location;
      public bumpT m_Parent = null;
    }
    public class fieldT
    {
      public fieldT(fieldT f)
      {
        m_Steep = new Line2I(new Point2I(f.m_Steep.m_Near.m_X, f.m_Steep.m_Near.m_Y),
            new Point2I(f.m_Steep.m_Far.m_X, f.m_Steep.m_Far.m_Y));
        m_Shallow = new Line2I(
            new Point2I(f.m_Shallow.m_Near.m_X, f.m_Shallow.m_Near.m_Y),
            new Point2I(f.m_Shallow.m_Far.m_X, f.m_Shallow.m_Far.m_Y));
        m_SteepBump = f.m_SteepBump;
        m_ShallowBump = f.m_ShallowBump;
      }
      public fieldT()
      {
      }
      public override String ToString()
      {
        return "[ steep " + m_Steep + ",  shallow " + m_Shallow + "]";
      }

      public Line2I m_Steep = new Line2I(new Point2I(0, 0), new Point2I(0, 0));
      public Line2I m_Shallow = new Line2I(new Point2I(0, 0), new Point2I(0, 0));
      public bumpT m_SteepBump;
      public bumpT m_ShallowBump;
    }

    public PrecisePermissive(ILosBoard board)
    {
      m_Board = board;
    }
    public void VisitFieldOfView(int x, int y, int distance)
    {
      permissiveMaskT mask = new permissiveMaskT();
      mask.m_East = mask.m_North = mask.m_South = mask.m_West = distance;
      mask.m_Mask = null;
      mask.m_FovType = FovType.CIRCLE;
      mask.m_DistPlusOneSq = (distance + 1) * (distance + 1);
      mask.m_Board = m_Board;
      PermissiveFov(x, y, mask);
    }
    private int Max(int i, int j)
    {
      return i > j ? i : j;
    }
    private int Min(int i, int j)
    {
      return i < j ? i : j;
    }
    private void VisitSquare(fovStateT state, Point2I dest,
        CLikeIterator<fieldT> currentField, LinkedList<bumpT> steepBumps,
        LinkedList<bumpT> shallowBumps, LinkedList<fieldT> activeFields)
    {
      // The top-left and bottom-right corners of the destination square.
      Point2I topLeft = new Point2I(dest.m_X, dest.m_Y + 1);
      Point2I bottomRight = new Point2I(dest.m_X + 1, dest.m_Y);

      while (!currentField.IsAtEnd()
          && currentField.GetCurrent().m_Steep
              .IsBelowOrContains(bottomRight)) {
        // case ABOVE
        // The square is in case 'above'. This means that it is ignored
        // for the currentField. But the steeper fields might need it.
        currentField.GotoNext();
      }
      if (currentField.IsAtEnd()) {
        // The square was in case 'above' for all fields. This means that
        // we no longer care about it or any squares in its diagonal rank.
        return;
      }

      // Now we check for other cases.
      if (currentField.GetCurrent().m_Shallow.IsAboveOrContains(topLeft)) {
        // case BELOW
        // The shallow line is above the extremity of the square, so that
        // square is ignored.
        return;
      }
      // The square is between the lines in some way. This means that we
      // need to visit it and determine whether it is blocked.
      bool isBlocked = ActIsBlocked(state, dest);
      if (!isBlocked) {
        // We don't care what case might be left, because this square does
        // not obstruct.
        return;
      }

      if (currentField.GetCurrent().m_Shallow.IsAbove(bottomRight)
          && currentField.GetCurrent().m_Steep.IsBelow(topLeft)) {
        // case BLOCKING
        // Both lines intersect the square. This current field has ended.
        currentField.RemoveCurrent();
      } else if (currentField.GetCurrent().m_Shallow.IsAbove(bottomRight)) {
        // case SHALLOW BUMP
        // The square intersects only the shallow line.
        AddShallowBump(topLeft, currentField.GetCurrent(), steepBumps,
            shallowBumps);
        CheckField(currentField);
      } else if (currentField.GetCurrent().m_Steep.IsBelow(topLeft)) {
        // case STEEP BUMP
        // The square intersects only the steep line.
        AddSteepBump(bottomRight, currentField.GetCurrent(), steepBumps,
            shallowBumps);
        CheckField(currentField);
      } else {
        // case BETWEEN
        // The square intersects neither line. We need to split into two
        // fields.
        fieldT steeperField = currentField.GetCurrent();
        fieldT shallowerField = new fieldT(currentField.GetCurrent());
        currentField.InsertBeforeCurrent(shallowerField);
        AddSteepBump(bottomRight, shallowerField, steepBumps, shallowBumps);
        currentField.GotoPrevious();
        if (!CheckField(currentField)) // did not remove
          currentField.GotoNext();// point to the original element
        AddShallowBump(topLeft, steeperField, steepBumps, shallowBumps);
        CheckField(currentField);
      }
    }
    private bool CheckField(CLikeIterator<fieldT> currentField)
    {
      // If the two slopes are colinear, and if they pass through either
      // extremity, remove the field of view.
      fieldT currFld = currentField.GetCurrent();
      bool ret = false;

      if (currFld.m_Shallow.DoesContain(currFld.m_Steep.m_Near)
          && currFld.m_Shallow.DoesContain(currFld.m_Steep.m_Far)
          && (currFld.m_Shallow.DoesContain(new Point2I(0, 1)) || currFld.m_Shallow
              .DoesContain(new Point2I(1, 0)))) {
        currentField.RemoveCurrent();
        ret = true;
      }
      return ret;
    }
    private void AddShallowBump(Point2I point, fieldT currFld,
        LinkedList<bumpT> steepBumps, LinkedList<bumpT> shallowBumps)
    {
      // First, the far point of shallow is set to the new point.
      currFld.m_Shallow.m_Far = point;
      // Second, we need to add the new bump to the shallow bump list for
      // future steep bump handling.
      shallowBumps.AddLast(new bumpT());
      shallowBumps.Last.Value.m_Location = point;
      shallowBumps.Last.Value.m_Parent = currFld.m_ShallowBump;
      currFld.m_ShallowBump = shallowBumps.Last.Value;
      // Now we have too look through the list of steep bumps and see if
      // any of them are below the line.
      // If there are, we need to replace near point too.
      bumpT currentBump = currFld.m_SteepBump;
      while (currentBump != null) {
        if (currFld.m_Shallow.IsAbove(currentBump.m_Location)) {
          currFld.m_Shallow.m_Near = currentBump.m_Location;
        }
        currentBump = currentBump.m_Parent;
      }
    }
    private void AddSteepBump(Point2I point, fieldT currFld,
        LinkedList<bumpT> steepBumps, LinkedList<bumpT> shallowBumps)
    {
      currFld.m_Steep.m_Far = point;
      steepBumps.AddLast(new bumpT());
      steepBumps.Last.Value.m_Location = point;
      steepBumps.Last.Value.m_Parent = currFld.m_SteepBump;
      currFld.m_SteepBump = steepBumps.Last.Value;
      // Now look through the list of shallow bumps and see if any of them
      // are below the line.
      bumpT currentBump = currFld.m_ShallowBump;
      while (currentBump != null) {
        if (currFld.m_Steep.IsBelow(currentBump.m_Location)) {
          currFld.m_Steep.m_Near = currentBump.m_Location;
        }
        currentBump = currentBump.m_Parent;
      }
    }
    private bool ActIsBlocked(fovStateT state, Point2I pos)
    {
      Point2I adjustedPos = new Point2I(pos.m_X * state.m_Quadrant.m_X
          + state.m_Source.m_X, pos.m_Y * state.m_Quadrant.m_Y + state.m_Source.m_Y);

      if (!state.m_Board.Contains(adjustedPos.m_X, adjustedPos.m_Y))
        return false;//we are getting outside the board

      if (state.m_IsLos // In LOS calculation all visits allowed
          || state.m_QuadrantIndex == 0 // can visit anything from Q1
          || (state.m_QuadrantIndex == 1 && pos.m_X != 0) // Q2 : no Y axis
          || (state.m_QuadrantIndex == 2 && pos.m_Y != 0) // Q3 : no X axis
          || (state.m_QuadrantIndex == 3 && pos.m_X != 0 && pos.m_Y != 0)) // Q4
        // no X
        // or Y
        // axis
        if (DoesPermissiveVisit(state.m_Mask, pos.m_X * state.m_Quadrant.m_X, pos.m_Y
            * state.m_Quadrant.m_Y) == 1) {
          state.m_Board.Visit(adjustedPos.m_X, adjustedPos.m_Y);
        }
      return state.m_Board.IsObstacle(adjustedPos.m_X, adjustedPos.m_Y);
    }
    private void PermissiveFov(int sourceX, int sourceY, permissiveMaskT mask)
    {
      fovStateT state = new fovStateT();
      state.m_Source = new Point2I(sourceX, sourceY);
      state.m_Mask = mask;
      state.m_Board = mask.m_Board;
      // state.isBlocked = isBlocked;
      // state.visit = visit;
      // state.context = context;

      int quadrantCount = 4;
      Point2I[] quadrants = { new Point2I(1, 1), new Point2I(-1, 1),
				new Point2I(-1, -1), new Point2I(1, -1) };

      Point2I[] extents = { new Point2I(mask.m_East, mask.m_North),
				new Point2I(mask.m_West, mask.m_North),
				new Point2I(mask.m_West, mask.m_South),
				new Point2I(mask.m_East, mask.m_South) };
      int quadrantIndex = 0;
      for (; quadrantIndex < quadrantCount; ++quadrantIndex) {
        state.m_Quadrant = quadrants[quadrantIndex];
        state.m_Extent = extents[quadrantIndex];
        state.m_QuadrantIndex = quadrantIndex;
        CalculateFovQuadrant(state);
      }
    }
    private int DoesPermissiveVisit(permissiveMaskT mask, int x, int y)
    {
      if (mask.m_FovType == FovType.SQUARE)
        return 1;
      else if (mask.m_FovType == FovType.CIRCLE) {
        if (x * x + y * y < mask.m_DistPlusOneSq)
          return 1;
        else
          return 0;
      }
      return 1;
    }
    private void CalculateFovQuadrant(fovStateT state)
    {
      LinkedList<bumpT> steepBumps = new LinkedList<bumpT>();
      LinkedList<bumpT> shallowBumps = new LinkedList<bumpT>();
      // activeFields is sorted from shallow-to-steep.
      LinkedList<fieldT> activeFields = new LinkedList<fieldT>();
      activeFields.AddLast(new fieldT());
      activeFields.Last.Value.m_Shallow.m_Near = new Point2I(0, 1);
      activeFields.Last.Value.m_Shallow.m_Far = new Point2I(state.m_Extent.m_X, 0);
      activeFields.Last.Value.m_Steep.m_Near = new Point2I(1, 0);
      activeFields.Last.Value.m_Steep.m_Far = new Point2I(0, state.m_Extent.m_Y);

      Point2I dest = new Point2I(0, 0);

      // Visit the source square exactly once (in quadrant 1).
      if (state.m_Quadrant.m_X == 1 && state.m_Quadrant.m_Y == 1) {
        ActIsBlocked(state, dest);
      }

      CLikeIterator<fieldT> currentField = new CLikeIterator<fieldT>(
          activeFields.First);
      int i = 0;
      int j = 0;
      int maxI = state.m_Extent.m_X + state.m_Extent.m_Y;
      // For each square outline
      for (i = 1; i <= maxI && activeFields.Count > 0; ++i) {
        int startJ = Max(0, i - state.m_Extent.m_X);
        int maxJ = Min(i, state.m_Extent.m_Y);
        // Visit the nodes in the outline
        for (j = startJ; j <= maxJ && !currentField.IsAtEnd(); ++j) {
          dest.m_X = i - j;
          dest.m_Y = j;
          VisitSquare(state, dest, currentField, steepBumps,
              shallowBumps, activeFields);
        }
        currentField = new CLikeIterator<fieldT>(activeFields.First);
      }
    }

    private ILosBoard m_Board;
  }
}
