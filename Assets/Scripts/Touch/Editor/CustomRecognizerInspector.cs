using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(CustomRegognizer))]
public class CustomRecognizerInspector : GestureRecognizerInspector<CustomRegognizer> 
{
  protected static GUIContent LABEL_Templates = new GUIContent("Gesture Templates List", "");
  protected static GUIContent LABEL_MinDistanceBetweenSamples = new GUIContent("Sampling Distance", "");
  protected static GUIContent LABEL_MaxMatchDistance = new GUIContent("Max Match Distance", "");

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
    Gesture.MaxMatchDistance = EditorGUILayout.FloatField(LABEL_MaxMatchDistance, Gesture.MaxMatchDistance);
    Gesture.MinDistanceBetweenSamples = EditorGUILayout.FloatField(LABEL_MinDistanceBetweenSamples, Gesture.MinDistanceBetweenSamples);
    
    serializedObject.Update();
    if (Gesture.Templates == null) {
      Gesture.Templates = new List<CustomGestureTemplate>();
      EditorUtility.SetDirty(Gesture);
    }

    EditorGUILayout.PropertyField(serializedObject.FindProperty("Templates"), LABEL_Templates, true);
    serializedObject.ApplyModifiedProperties();
  }

  protected override void ValidateValues()
  {
    base.ValidateValues();
    Gesture.MinDistanceBetweenSamples = Mathf.Max(1.0f, Gesture.MinDistanceBetweenSamples);
    Gesture.MaxMatchDistance = Mathf.Max(0.1f, Gesture.MaxMatchDistance);
  }
}
