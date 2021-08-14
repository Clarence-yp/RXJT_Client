using UnityEngine;
using System.Collections.Generic;

public class TouchInputProvider : InputProvider {
  public int maxTouches = 5;

  void Start()
  {
    finger2touchMap = new int[maxTouches];
  }

  void Update()
  {
    UpdateFingerTouchMap();
  }

  /// touch -> finger map
  UnityEngine.Touch nullTouch = new UnityEngine.Touch();
  // finger.index -> touch index map
  int[] finger2touchMap;

  void UpdateFingerTouchMap()
  {
    for (int i = 0; i < finger2touchMap.Length; ++i) {
      finger2touchMap[i] = -1;
    }
    for (int i = 0; i < Input.touchCount; ++i) {
      int fingerIndex = Input.touches[i].fingerId;
      if (fingerIndex < finger2touchMap.Length) {
        finger2touchMap[fingerIndex] = i;
      }
    }
  }

  bool HasValidTouch(int fingerIndex)
  {
    return finger2touchMap[fingerIndex] != -1;
  }

  UnityEngine.Touch GetTouch(int fingerIndex)
  {
    int touchIndex = finger2touchMap[fingerIndex];
    if (touchIndex == -1) {
      return nullTouch;
    }
    return Input.touches[touchIndex];
  }

  public override int MaxSimultaneousFingers
  {
    get
    {
      return maxTouches;
    }
  }

  public override void GetInputState(int fingerIndex, out bool down, out Vector2 position)
  {
    down = false;
    position = Vector2.zero;
    if (HasValidTouch(fingerIndex)) {
      UnityEngine.Touch touch = GetTouch(fingerIndex);
      if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
        down = false;
      } else {
        down = true;
        position = touch.position;
      }
    }
  }
}
