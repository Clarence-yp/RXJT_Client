using UnityEngine;
using System.Collections;

public class LongPressGesture : Gesture {
}

public class LongPressRecognizer : GestureRecognizerBase<LongPressGesture> {
  public float Duration = 1.0f;
  public float MoveTolerance = 0.5f;
  public float MaxDuration = 0;

  public override string GetDefaultEventMessageName()
  {
    return string.IsNullOrEmpty(EventMessageName) ? "OnLongPress" : EventMessageName;
  }

  private bool HasTimedOut(LongPressGesture gesture)
  {
    if (MaxDuration > 0 && (gesture.ElapsedTime > MaxDuration)) {
      return true;
    }
    return false;
  }

  protected override void OnBegin(LongPressGesture gesture, TouchManager.IFingerList touches)
  {
    gesture.Position = touches.GetAveragePosition();
    gesture.StartPosition = gesture.Position;
  }

  protected override void Reset(LongPressGesture gesture, bool isPressed = false)
  {
  }

  protected override void Refresh(LongPressGesture gesture)
  {
    ReleaseFingers(gesture);

    gesture.ClusterId = 0;
    gesture.Fingers.Clear();
    gesture.IsHint = false;
    gesture.IsActive = false;
    gesture.SelectedID = -1;
    gesture.SectionNum = -1;
    gesture.HintFlag = HintType.None;
    gesture.State = GestureRecognitionState.Ready;
  }

  protected override GestureRecognitionState OnRecognize(LongPressGesture gesture, TouchManager.IFingerList touches)
  {
    if (touches.Count != RequiredFingerCount) {
      return GestureRecognitionState.Failed;
    }
    if (HasTimedOut(gesture)) {
      return GestureRecognitionState.Failed;
    }
    if (gesture.ElapsedTime >= Duration) {
      return GestureRecognitionState.Recognized;
    }
    // Æ«Àë³õÊ¼Î»ÖÃÌ«Ô¶
    if (touches.GetAverageDistanceFromStart() > ToPixels(MoveTolerance)) {
      return GestureRecognitionState.Failed;
    }
    return GestureRecognitionState.InProgress;
  }
}
