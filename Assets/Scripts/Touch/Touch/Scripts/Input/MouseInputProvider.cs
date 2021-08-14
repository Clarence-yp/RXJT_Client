using UnityEngine;
using System.Collections;

public class MouseInputProvider : InputProvider {
  public int maxFingers = 2;
  public override int MaxSimultaneousFingers
  {
    get
    {
      return maxFingers;
    }
  }
  public override void GetInputState(int fingerIndex, out bool down, out Vector2 position)
  {
    down = Input.GetMouseButton(fingerIndex);
    position = Input.mousePosition;
  }
}
