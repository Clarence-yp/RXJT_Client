using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(EasyRegognizer))]
public class EasyRecognizerInspector : GestureRecognizerInspector<EasyRegognizer> 
{
  protected static GUIContent LABEL_HintDistance = new GUIContent("Hint Distance", "");
  protected static GUIContent LABEL_ToleranceDistance = new GUIContent("Tolerance Distance", "");
  protected static GUIContent LABEL_FireDistance = new GUIContent("Fire Distance", "");
  protected static GUIContent LABEL_QteActiveDistance = new GUIContent("Qte Active Distance", "");
  protected static GUIContent LABEL_QteFireDistance = new GUIContent("Qte Fire Distance", "");
  protected static GUIContent LABEL_TimeOut = new GUIContent("TimeOut", "");
  protected static GUIContent LABEL_QteSimilarityDistance = new GUIContent("Qte Similarity Distance", "");

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
    Gesture.HintDistance = EditorGUILayout.FloatField(LABEL_HintDistance, Gesture.HintDistance);
    Gesture.ToleranceDistance = EditorGUILayout.FloatField(LABEL_ToleranceDistance, Gesture.ToleranceDistance);
    Gesture.FireDistance = EditorGUILayout.FloatField(LABEL_FireDistance, Gesture.FireDistance);
    Gesture.QteActiveDistance = EditorGUILayout.FloatField(LABEL_QteActiveDistance, Gesture.QteActiveDistance);
    Gesture.QteFireDistance = EditorGUILayout.FloatField(LABEL_QteFireDistance, Gesture.QteFireDistance);
    Gesture.TimeOut = EditorGUILayout.FloatField(LABEL_TimeOut, Gesture.TimeOut);
    Gesture.QteSimilarityDistance = EditorGUILayout.FloatField(LABEL_QteSimilarityDistance, Gesture.QteSimilarityDistance);
  }
}
