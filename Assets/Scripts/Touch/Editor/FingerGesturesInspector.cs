using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TouchManager))]
public class FingerGesturesInspector : Editor 
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    if (Application.isPlaying) {
      GUILayout.Label("Registered Gesture Recognizers: " + TouchManager.RegisteredGestureRecognizers.Count);
      foreach (GestureRecognizer recognizer in TouchManager.RegisteredGestureRecognizers)
        EditorGUILayout.ObjectField(recognizer.EventMessageName + " - " + recognizer.GetType().Name, recognizer, typeof(GestureRecognizer), true);
    }
  }
}
