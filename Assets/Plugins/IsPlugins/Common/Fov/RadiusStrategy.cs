using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public interface RadiusStrategy
  {
    /**
     * Returns the Radius between the two points provided.
     *
     * @param startx
     * @param starty
     * @param endx
     * @param endy
     * @return
     */
    float Radius(int startx, int starty, int endx, int endy);

    /**
     * Returns the Radius calculated using the two distances provided.
     * 
     * @param dx
     * @param dy
     * @return 
     */
    float Radius(int dx, int dy);

    /**
     * Returns the Radius between the two points provided.
     *
     * @param startx
     * @param starty
     * @param endx
     * @param endy
     * @return
     */
    float Radius(float startx, float starty, float endx, float endy);

    /**
     * Returns the Radius calculated based on the two distances provided.
     * 
     * @param dx
     * @param dy
     * @return 
     */
    float Radius(float dx, float dy);
  }
  public class BasicRadiusStrategy : RadiusStrategy
  {
    public float Radius(int startx, int starty, int endx, int endy)
    {
      return Radius((float)startx, (float)starty, (float)endx, (float)endy);
    }

    public float Radius(float startx, float starty, float endx, float endy)
    {
      float dx = Math.Abs(startx - endx);
      float dy = Math.Abs(starty - endy);
      return Radius(dx, dy);
    }

    public float Radius(int dx, int dy)
    {
      return Radius((float)dx, (float)dy);
    }

    public float Radius(float dx, float dy)
    {
      dx = Math.Abs(dx);
      dy = Math.Abs(dy);
      float radius = 0f;
      switch (m_Strategy) {
        case StratetyEnum.SQUARE:
          radius = Math.Max(dx, dy);//Radius is longest axial distance
          break;
        case StratetyEnum.DIAMOND:
          radius = dx + dy;//Radius is the manhattan distance
          break;
        case StratetyEnum.CIRCLE:
          radius = (float)Math.Sqrt(dx * dx + dy * dy);//standard circular Radius
          break;
      }
      return radius;
    }

    private BasicRadiusStrategy(StratetyEnum type)
    {
      m_Strategy = type;
    }

    private StratetyEnum m_Strategy;

    private enum StratetyEnum : int
    {
      /**
       * In an unobstructed area the FOV would be a square.
       *
       * This is the shape that would represent movement Radius in an 8-way
       * movement scheme with no additional cost for diagonal movement.
       */
      SQUARE,
      /**
       * In an unobstructed area the FOV would be a diamond.
       *
       * This is the shape that would represent movement Radius in a 4-way
       * movement scheme.
       */
      DIAMOND,
      /**
       * In an unobstructed area the FOV would be a circle.
       *
       * This is the shape that would represent movement Radius in an 8-way
       * movement scheme with all movement cost the same based on distance from
       * the source
       */
      CIRCLE
    }

    public static RadiusStrategy s_Square = new BasicRadiusStrategy(StratetyEnum.SQUARE);
    public static RadiusStrategy s_Diamond = new BasicRadiusStrategy(StratetyEnum.DIAMOND);
    public static RadiusStrategy s_Circle = new BasicRadiusStrategy(StratetyEnum.CIRCLE);
  }
  public class Direction
  {
    public static Direction UP = new Direction(0, -1, DirectionEnum.UP);
    public static Direction DOWN = new Direction(0, 1, DirectionEnum.DOWN);
    public static Direction LEFT = new Direction(-1, 0, DirectionEnum.LEFT);
    public static Direction RIGHT = new Direction(1, 0, DirectionEnum.RIGHT);
    public static Direction UP_LEFT = new Direction(-1, -1, DirectionEnum.UP_LEFT);
    public static Direction UP_RIGHT = new Direction(1, -1, DirectionEnum.UP_RIGHT);
    public static Direction DOWN_LEFT = new Direction(-1, 1, DirectionEnum.DOWN_LEFT);
    public static Direction DOWN_RIGHT = new Direction(1, 1, DirectionEnum.DOWN_RIGHT);
    public static Direction NONE = new Direction(0, 0, DirectionEnum.NONE);
    /**
     * An array which holds only the four cardinal directions.
     */
    public static Direction[] CARDINALS = { UP, DOWN, LEFT, RIGHT };
    /**
     * An array which holds only the four diagonal directions.
     */
    public static Direction[] DIAGONALS = { UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT };
    /**
     * An array which holds all eight OUTWARDS directions.
     */
    public static Direction[] OUTWARDS = { UP, DOWN, LEFT, RIGHT, UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT };
    /**
     * The x coordinate difference for this direction.
     */
    public int m_DeltaX;
    /**
     * The y coordinate difference for this direction.
     */
    public int m_DeltaY;

    /**
     * Returns the direction that most closely matches the input.
     *
     * @param x
     * @param y
     * @return
     */
    static public Direction GetDirection(int x, int y)
    {
      if (x < 0) {
        if (y < 0) {
          return UP_LEFT;
        } else if (y == 0) {
          return LEFT;
        } else {
          return DOWN_LEFT;
        }
      } else if (x == 0) {
        if (y < 0) {
          return UP;
        } else if (y == 0) {
          return NONE;
        } else {
          return DOWN;
        }
      } else {
        if (y < 0) {
          return UP_RIGHT;
        } else if (y == 0) {
          return RIGHT;
        } else {
          return DOWN_RIGHT;
        }
      }
    }

    /**
     * Returns the Direction one step clockwise including diagonals.
     *
     * @param dir
     * @return
     */
    public Direction Clockwise()
    {
      switch (m_Direction) {
        case DirectionEnum.UP:
          return UP_RIGHT;
        case DirectionEnum.DOWN:
          return DOWN_LEFT;
        case DirectionEnum.LEFT:
          return UP_LEFT;
        case DirectionEnum.RIGHT:
          return DOWN_RIGHT;
        case DirectionEnum.UP_LEFT:
          return UP;
        case DirectionEnum.UP_RIGHT:
          return RIGHT;
        case DirectionEnum.DOWN_LEFT:
          return LEFT;
        case DirectionEnum.DOWN_RIGHT:
          return DOWN;
        case DirectionEnum.NONE:
        default:
          return NONE;
      }
    }

    /**
     * Returns the Direction one step counterclockwise including diagonals.
     *
     * @param dir
     * @return
     */
    public Direction CounterClockwise()
    {
      switch (m_Direction) {
        case DirectionEnum.UP:
          return UP_LEFT;
        case DirectionEnum.DOWN:
          return DOWN_RIGHT;
        case DirectionEnum.LEFT:
          return DOWN_LEFT;
        case DirectionEnum.RIGHT:
          return UP_RIGHT;
        case DirectionEnum.UP_LEFT:
          return LEFT;
        case DirectionEnum.UP_RIGHT:
          return UP;
        case DirectionEnum.DOWN_LEFT:
          return DOWN;
        case DirectionEnum.DOWN_RIGHT:
          return RIGHT;
        case DirectionEnum.NONE:
        default:
          return NONE;
      }
    }

    /**
     * Returns the direction directly opposite of this one.
     *
     * @return
     */
    public Direction Opposite()
    {
      switch (m_Direction) {
        case DirectionEnum.UP:
          return DOWN;
        case DirectionEnum.DOWN:
          return UP;
        case DirectionEnum.LEFT:
          return RIGHT;
        case DirectionEnum.RIGHT:
          return LEFT;
        case DirectionEnum.UP_LEFT:
          return DOWN_RIGHT;
        case DirectionEnum.UP_RIGHT:
          return DOWN_LEFT;
        case DirectionEnum.DOWN_LEFT:
          return UP_RIGHT;
        case DirectionEnum.DOWN_RIGHT:
          return UP_LEFT;
        case DirectionEnum.NONE:
        default:
          return NONE;
      }
    }

    private Direction(int x, int y, DirectionEnum dir)
    {
      this.m_DeltaX = x;
      this.m_DeltaY = y;
      m_Direction = dir;
    }

    private DirectionEnum m_Direction;

    private enum DirectionEnum : int
    {
      UP,
      DOWN,
      LEFT,
      RIGHT,
      UP_LEFT,
      UP_RIGHT,
      DOWN_LEFT,
      DOWN_RIGHT,
      NONE
    }
  }
}
