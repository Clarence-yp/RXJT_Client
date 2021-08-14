using UnityEngine;
using System.Collections;

public class FingerDownEvent : FingerEvent {
}

public class FingerDown : FingerEventDetector<FingerDownEvent> {
  public FingerEventHandler OnFingerDown;
  public string MessageName = "OnFingerDown";

  protected override void ProcessFinger(TouchManager.Finger finger)
  {
    if (finger.IsDown && !finger.WasDown) {
      FingerDownEvent e = GetEvent(finger.Index);
      if (null != e) {
        e.Name = MessageName;
        if (OnFingerDown != null) {
          OnFingerDown(e);
        }
        TrySendMessage(e);
      }
    }
  }
}
