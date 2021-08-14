using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FingerUp))]
public class FingerUpDetectorInspector : FingerEventDetectorInspector<FingerUp> 
{
  protected override void MessageEventsGUI()
  {
    Detector.MessageName = EditorGUILayout.TextField("Message Name", Detector.MessageName);
  }
}
