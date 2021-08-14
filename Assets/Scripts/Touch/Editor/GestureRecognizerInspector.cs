using UnityEditor;
using UnityEngine;
using System.Reflection; // for clipboard stuff

public abstract class GestureRecognizerInspector<T> : Editor where T : GestureRecognizer 
{
  protected static GUIContent LABEL_EventMessageName = new GUIContent("Message Name", "");
  protected static GUIStyle SectionTitleStyle;

  static bool stylesInitialized = false;

  static void InitStyles()
  {
    SectionTitleStyle = new GUIStyle(GUI.skin.label);
    SectionTitleStyle.fontStyle = FontStyle.Bold;
  }

  T gesture;

  protected abstract bool ShowRequiredFingerCount
  {
    get;
  }
  protected virtual void ValidateValues()
  {
    Gesture.RequiredFingerCount = Mathf.Clamp(Gesture.RequiredFingerCount, 1, 50);
  }

  public T Gesture
  {
    get
    {
      return gesture;
    }
  }

  public override void OnInspectorGUI()
  {
    if (!stylesInitialized) {
      InitStyles();
      stylesInitialized = true;
    }

    gesture = (T)target;
    OnSettingsUI();
    OnMessagingUI();
    OnComponentsUI();
    DisplayNotices();
    if (GUI.changed) {
      ValidateValues();
      EditorUtility.SetDirty(target);
    }
  }

  void DisplayNotices()
  {
    GUILayout.Space(5);
    EditorGUI.indentLevel--;
    OnNotices();
    EditorGUI.indentLevel++;
  }

  protected virtual void OnNotices()
  {
  }

  protected void UISectionTitle(string title)
  {
  }

  protected void UISectionTitle(GUIContent title)
  {
  }

  public static readonly Color DistanceFieldColor = new Color(0.5f, 0.9f, 1.0f);

  static Color GetUnitColor(DistanceUnit unit)
  {
    return DistanceFieldColor;
  }

  public float DistanceField(GUIContent content, float value, string suffix = "")
  {
    GUILayout.BeginHorizontal();
    Color oldColor = GUI.contentColor;
    GUI.contentColor = GetUnitColor(gesture.DistanceUnit);

    float val = EditorGUILayout.FloatField(content, value);
    gesture.DistanceUnit = (DistanceUnit)EditorGUILayout.EnumPopup(gesture.DistanceUnit, GUILayout.Width(125));

    GUI.contentColor = oldColor;
    GUILayout.EndHorizontal();

    return val;
  }

  protected static readonly GUIContent NotAvailable = new GUIContent("-");

  protected virtual void OnSettingsUI()
  {
  }

  protected virtual void OnMessagingUI()
  {
    string eventName = string.IsNullOrEmpty(Gesture.EventMessageName) ? Gesture.GetDefaultEventMessageName() : Gesture.EventMessageName;
    Gesture.EventMessageName = EditorGUILayout.TextField(LABEL_EventMessageName, eventName);
    GUI.enabled = true;
  }

  protected virtual void OnComponentsUI()
  {
  }
}
