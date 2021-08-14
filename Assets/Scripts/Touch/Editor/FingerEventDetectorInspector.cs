using UnityEditor;
using UnityEngine;
using System.Reflection; // for clipboard stuff

public abstract class FingerEventDetectorInspector<T> : Editor where T : FingerEventDetector 
{
  protected static GUIContent LABEL_EventMessageName = new GUIContent("Message Name", "");
  protected static GUIStyle SectionTitleStyle;

  static bool stylesInitialized = false;

  static void InitStyles()
  {
    SectionTitleStyle = new GUIStyle(GUI.skin.label);
    SectionTitleStyle.fontStyle = FontStyle.Bold;
  }

  T detector;

  protected virtual void ValidateValues()
  {

  }

  public T Detector
  {
    get
    {
      return detector;
    }
  }

  public override void OnInspectorGUI()
  {
    if (!stylesInitialized) {
      InitStyles();
      stylesInitialized = true;
    }
    detector = (T)target;
    OnSettingsUI();
    OnMessagingUI();
    if (GUI.changed) {
      ValidateValues();
      EditorUtility.SetDirty(target);
    }
  }

  protected virtual void OnSettingsUI()
  {
  }

  protected virtual void OnMessagingUI()
  {
    MessageEventsGUI();
    GUI.enabled = true;
  }
  protected abstract void MessageEventsGUI();
}
