using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public interface ILosBoard
  {
    bool Contains(int x, int y);
    bool IsObstacle(int x, int y);
    void Visit(int x, int y);
  }
}
