using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SceneLocator : EditorWindow {
  private const string DEFAULT_PATH = @"D:\Unit.txt";
  private const string INFO_FORMAT = "{0}	{1} {2} {3}	{4}\n";
  private const string POLE_PATH = "Assets/Editor/SceneLocator/Res/Pole.prefab";
  private const string POLE_IDENTIFILER = "XJ";

  private static Vector2 m_WinMinSize = new Vector2(600.0f, 400.0f);
  private static Rect m_WinPosition = new Rect(300.0f, 200.0f, m_WinMinSize.x, m_WinMinSize.y);

  public GameObject RootObject { get; set; }
  public GameObject PoleObject { get; set; }
  public int SelectedLayer { get; set; }
  public Vector3 PoleOffset { get; set; }
  public string SerializeFile { get; set; }
  public KeyCode LocateKey { get; set; }
  public Vector3 MouseHitPos { get; set; }
  public string PoleIdentifiler;

  public bool IsLocating { get; set; }

  public string DebugInfo { get; set; }

  [MenuItem("Custom/SceneLocator")]
  private static void Init() {
    SceneLocator window = EditorWindow.GetWindow<SceneLocator>("SceneLocator", true, typeof(EditorWindow));
    window.position = m_WinPosition;
    window.minSize = m_WinMinSize;
    window.Show();

    window.OnInitialize();
  }
  private void OnGUI() {
    // Layouts
    RootObject = EditorGUILayout.ObjectField("Root Object:", RootObject, typeof(GameObject), true) as GameObject;
    PoleObject = EditorGUILayout.ObjectField("Pole Object:", PoleObject, typeof(GameObject), true) as GameObject;
    SelectedLayer = EditorGUILayout.LayerField("Layer:", SelectedLayer);
    PoleIdentifiler = EditorGUILayout.TextField("PoleIdentifiler:", PoleIdentifiler);
    PoleOffset = EditorGUILayout.Vector3Field("Offset:", PoleOffset);
    LocateKey = (KeyCode)EditorGUILayout.EnumPopup("LocateKey: shift +", LocateKey);
    EditorGUILayout.BeginHorizontal();
    SerializeFile = EditorGUILayout.TextField("SerizlizeFile:", SerializeFile);
    if (GUILayout.Button("Select", GUILayout.MaxWidth(50))) {
      SerializeFile = EditorUtility.OpenFilePanel(
      "Select File to Write",
      DEFAULT_PATH,
      "txt");
    }
    EditorGUILayout.EndHorizontal();
    EditorGUILayout.BeginHorizontal();
    if (!IsLocating && GUILayout.Button("Start", GUILayout.MaxWidth(200))) {
      OnStartLocate();
    } else if (IsLocating && GUILayout.Button("Stop", GUILayout.MaxWidth(200))) {
      OnStopLocate();
    } else if (GUILayout.Button("Serialize", GUILayout.MaxWidth(200))) {
      OnSerializeLocate();
    }
    EditorGUILayout.EndHorizontal();
    //if (GUILayout.Button("ResetLog", GUILayout.MaxWidth(200))) {
    //  DebugInfo = "";
    //}
    //DebugInfo = EditorGUILayout.TextField("DebugInfo:", DebugInfo);

    this.Repaint();
  }
  private void OnSelectionChange() {
    this.Repaint();
  }

  private void OnInitialize() {
    SelectedLayer = LayerMask.NameToLayer("Terrains");
    PoleOffset = new Vector3(0, 0.5f, 0);
    IsLocating = false;
    LocateKey = KeyCode.C;
    DebugInfo = "";
    MouseHitPos = Vector3.zero;
    SerializeFile = DEFAULT_PATH;
    PoleObject = AssetDatabase.LoadAssetAtPath(POLE_PATH, typeof(GameObject)) as GameObject;
    PoleIdentifiler = POLE_IDENTIFILER;
  }
  private void OnStartLocate() {
    if (RootObject == null || PoleObject == null) {
      EditorUtility.DisplayDialog(
      "Locator Warn",
      "RootObject or PoleObject is null!",
      "OK");
      return;
    }

    IsLocating = true;
    SceneView.currentDrawingSceneView.Focus();
    RootObject.transform.position = Vector3.zero;
  }
  private void OnStopLocate() {
    IsLocating = false;
  }
  private void OnSerializeLocate() {
    IsLocating = false;
    string filePath = "";
    if (!string.IsNullOrEmpty(SerializeFile)) {
      try {
#if UNITY_STANDALONE_WIN
        filePath = SerializeFile.Replace("/", "\\");
#else
        filePath = SerializeFile.Replace("\\", "/");
#endif
        if (!File.Exists(filePath)) {
          FileStream fs = File.Create(filePath);
          fs.Close();
        }
      } catch (System.Exception ex) {
        DebugInfo = "OnSerializeLocate file create failed!" + ex;
      }
    }
    if (!File.Exists(filePath)) {
      EditorUtility.DisplayDialog(
        "Locator Warn",
        "SerializeFile is null!",
        "OK");
      return;
    }

    string content = ExtractLocatorInfo(RootObject);
    File.WriteAllText(filePath, content);
  }
  private void OnFocus() {
    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    SceneView.onSceneGUIDelegate += this.OnSceneGUI;
  }
  private void OnDestroy() {
    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
  }
  private void OnSceneGUI(SceneView sceneView) {
    if (!IsLocateReady()) {
      return;
    }

    bool isHit = false;
    if (SceneView.mouseOverWindow == SceneView.currentDrawingSceneView) {
      Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
      RaycastHit hitInfo;
      if (Physics.Raycast(worldRay, out hitInfo, 1000, (1 << SelectedLayer))) {
        MouseHitPos = hitInfo.point;
        isHit = true;
      }
    }

    if (isHit) {
      Event eEvent = Event.current;
      if (eEvent != null && eEvent.type == EventType.KeyDown
        && eEvent.shift && eEvent.keyCode == LocateKey) {
        if (RootObject != null && PoleObject != null) {
          GameObject tPole = GameObject.Instantiate(PoleObject) as GameObject;
          tPole.transform.parent = RootObject.transform;
          tPole.transform.rotation = Quaternion.identity;
          tPole.transform.position = MouseHitPos + PoleOffset;
          Selection.activeGameObject = tPole;
        } else {
          DebugInfo = "RootObject or PoleObject is null!";
        }
      }
    }
  }
  private bool IsLocateReady() {
    return IsLocating;
  }
  private string ExtractLocatorInfo(GameObject rootObj, int index = 0) {
    if (rootObj == null)
      return "";

    string formatInfoPre = "";
    for (int formatIndex = 0; formatIndex < index; formatIndex++) {
      formatInfoPre += "  ";
    }
    string content = string.Format(formatInfoPre + INFO_FORMAT, rootObj.name,
      rootObj.transform.position.x, rootObj.transform.position.y, rootObj.transform.position.z,
      rootObj.transform.rotation.eulerAngles.y
      );
    int childCount = rootObj.transform.childCount;
    for (int tIndex = 0; tIndex < childCount; tIndex++) {
      Transform childTrans = rootObj.transform.GetChild(tIndex);
      if (childTrans != null && childTrans.gameObject != null && childTrans.gameObject.name.StartsWith(PoleIdentifiler)) {
        content += ExtractLocatorInfo(childTrans.gameObject, (index + 1));
      }
    }

    return content;
  }
}