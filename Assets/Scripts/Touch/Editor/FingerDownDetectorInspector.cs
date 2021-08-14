using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FingerDown))]
public class FingerDownDetectorInspector : FingerEventDetectorInspector<FingerDown> 
{
  protected override void MessageEventsGUI()
  {
    Detector.MessageName = EditorGUILayout.TextField("Message Name", Detector.MessageName);
  }
}
