using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LongPressRecognizer))]
public class LongPressRecognizerInspector : GestureRecognizerInspector<LongPressRecognizer> 
{
  protected static GUIContent LABEL_RequiredFingerCount = new GUIContent("Required Finger Number", "");
  protected static GUIContent LABEL_Duration = new GUIContent("Press Duration", "");
  protected static GUIContent LABEL_MoveTolerance = new GUIContent("Move Tolerance", "");
  protected static GUIContent LABEL_MaxDuration = new GUIContent("Max Duration", "");

  protected override bool ShowRequiredFingerCount
  {
    get
    {
      return true;
    }
  }

  protected override void OnSettingsUI()
  {
    base.OnSettingsUI();

    Gesture.RequiredFingerCount = EditorGUILayout.IntField(LABEL_RequiredFingerCount, Gesture.RequiredFingerCount);
    Gesture.Duration = EditorGUILayout.FloatField(LABEL_Duration, Gesture.Duration);
    Gesture.MoveTolerance = DistanceField(LABEL_MoveTolerance, Gesture.MoveTolerance);
    Gesture.MaxDuration = EditorGUILayout.FloatField(LABEL_MaxDuration, Gesture.MaxDuration);
  }

  protected override void ValidateValues()
  {
    base.ValidateValues();
    Gesture.Duration = Mathf.Max(0.001f, Gesture.Duration);
    Gesture.MoveTolerance = Mathf.Max(0, Gesture.MoveTolerance);
  }
}
