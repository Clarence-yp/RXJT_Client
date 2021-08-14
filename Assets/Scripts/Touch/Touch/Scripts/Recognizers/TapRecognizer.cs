using UnityEngine;
using System.Collections;

public class TapGesture : Gesture {
  /// 点击次数
  int taps = 0;

  public int Taps
  {
    get
    {
      return taps;
    }
    set
    {
      taps = value;
    }
  }

  bool down = false;
  bool wasdown = false;
  float lastdowntime = 0;
  float lasttaptime = 0;

  public bool Down
  {
    get
    {
      return down;
    }
    set
    {
      down = value;
    }
  }

  public bool WasDown
  {
    get
    {
      return wasdown;
    }
    set
    {
      wasdown = value;
    }
  }

  public float LastDownTime
  {
    get
    {
      return lastdowntime;
    }
    set
    {
      lastdowntime = value;
    }
  }

  public float LastTapTime
  {
    get
    {
      return lasttaptime;
    }
    set
    {
      lasttaptime = value;
    }
  }
}

public class TapRecognizer : GestureRecognizerBase<TapGesture> {
  public int RequiredTaps = 1;
  /// 手势允许移动的距离
  public float MoveTolerance = 0.5f;
  /// 手指按压不会导致手势失败的最大时间，0为无限长
  public float MaxDuration = 0;
  public float MaxDelayBetweenTaps = 0.5f;

  bool IsMultiTap
  {
    get
    {
      return RequiredTaps > 1;
    }
  }

  bool HasTimedOut(TapGesture gesture)
  {
    if (MaxDuration > 0 && (gesture.ElapsedTime > MaxDuration)) {
      return true;
    }
    if (IsMultiTap && MaxDelayBetweenTaps > 0 && (Time.time - gesture.LastTapTime > MaxDelayBetweenTaps)) {
      return true;
    }
    return false;
  }

  protected override void Reset(TapGesture gesture, bool isPressed = false)
  {
    gesture.Taps = 0;
    gesture.Down = false;
    gesture.WasDown = false;
    base.Reset(gesture);
  }

  public override bool SupportFingerClustering
  {
    get
    {
      if (IsMultiTap) {
        return false;
      }
      return base.SupportFingerClustering;
    }
  }

  GestureRecognitionState RecognizeSingleTap(TapGesture gesture, TouchManager.IFingerList touches)
  {
    if (touches.Count != RequiredFingerCount) {
      // 所有手指抬起，触发Recognized事件
      if (touches.Count == 0) {
        return GestureRecognitionState.Recognized;
      }
      return GestureRecognitionState.Failed;
    }
    if (HasTimedOut(gesture)) {
      return GestureRecognitionState.Failed;
    }
    // 手指移动距离
    float sqrDist = Vector3.SqrMagnitude(touches.GetAveragePosition() - gesture.StartPosition);
    if (sqrDist >= ToSqrPixels(MoveTolerance)) {
      return GestureRecognitionState.Failed;
    }
    return GestureRecognitionState.InProgress;
  }

  GestureRecognitionState RecognizeMultiTap(TapGesture gesture, TouchManager.IFingerList touches)
  {
    gesture.WasDown = gesture.Down;
    gesture.Down = false;

    if (touches.Count == RequiredFingerCount) {
      gesture.Down = true;
      gesture.LastDownTime = Time.time;
    } else if (touches.Count == 0) {
      gesture.Down = false;
    } else {
      if (touches.Count < RequiredFingerCount) {
        if (Time.time - gesture.LastDownTime > 0.25f) {
          return GestureRecognitionState.Failed;
        }
      } else {
        if (!Young(touches)) {
          return GestureRecognitionState.Failed;
        }
      }
    }

    if (HasTimedOut(gesture)) {
      return GestureRecognitionState.Failed;
    }

    if (gesture.Down) {
      float sqrDist = Vector3.SqrMagnitude(touches.GetAveragePosition() - gesture.StartPosition);
      if (sqrDist >= ToSqrPixels(MoveTolerance)) {
        return GestureRecognitionState.FailAndRetry;
      }
    }

    if (gesture.WasDown != gesture.Down) {
      if (!gesture.Down) {
        ++gesture.Taps;
        gesture.LastTapTime = Time.time;
        if (gesture.Taps >= RequiredTaps) {
          return GestureRecognitionState.Recognized;
        }
      }
    }

    return GestureRecognitionState.InProgress;
  }

  public override string GetDefaultEventMessageName()
  {
    return string.IsNullOrEmpty(EventMessageName) ? "OnSingleTap" : EventMessageName;
  }

  protected override void OnBegin(TapGesture gesture, TouchManager.IFingerList touches)
  {
    gesture.Position = touches.GetAveragePosition();
    gesture.StartPosition = gesture.Position;
    gesture.LastTapTime = Time.time;
  }

  protected override GestureRecognitionState OnRecognize(TapGesture gesture, TouchManager.IFingerList touches)
  {
    return IsMultiTap ? RecognizeMultiTap(gesture, touches) : RecognizeSingleTap(gesture, touches);
  }
}
