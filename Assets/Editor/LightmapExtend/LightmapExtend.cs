using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightmapExtend : EditorWindow {
  private static Color[] UVColors = new Color[] {
    Color.red,
    Color.green,
    Color.cyan,
    Color.black,
    Color.gray,
  };
  private static int TargetLayerMask = 30;

  private static Vector2 m_WinMinSize = new Vector2(315.0f, 400.0f);
  private static Rect m_WinPosition = new Rect(100.0f, 100.0f, 315.0f, 400.0f);
  private Material lineMaterial;

  public GameObject SceneObject;
  public LocalProcessInfo ProcessInfo;
  public int LightmapIndex;
  public Vector2 LightmapTilling;
  public Vector2 LightmapOffset;
  public Texture2D LightmapTextureFar;
  public string DebugInfo;

  private List<List<UVInfo>> CurUVInfos = null;
  private List<TriangleInfo> CurTriangles = null;
  private Rect TextureControlRect = new Rect();
  private TriangleInfo CurTriInfo = null;

  [MenuItem("Custom/LightmapExtend")]
  private static void Init() {
    LightmapExtend window = EditorWindow.GetWindow<LightmapExtend>("LightmapExtend", true, typeof(EditorWindow));
    window.position = m_WinPosition;
    window.minSize = m_WinMinSize;
    window.wantsMouseMove = true;
    window.Show();

    window.Reset();
  }
  private void Reset() {
    if (SceneObject != null) {
      LightmapHelper.PostProcessObject(SceneObject, ProcessInfo);
    }
    SceneObject = null;
    ProcessInfo = null;
    LightmapIndex = -1;
    LightmapTilling = Vector2.zero;
    LightmapOffset = Vector2.zero;
    LightmapTextureFar = null;
    DebugInfo = string.Empty;

    CurUVInfos = null;
    CurTriangles = null;
    CurTriInfo = null;
  }
  private void OnGUI() {
    PaintMainPanel();
    PaintTextureTriangles();
    ProcessInput();
    this.Repaint();
  }
  private void OnDestroy() {
    Reset();
    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
  }
  private void PaintMainPanel() {
    EditorGUILayout.ObjectField("Scene Object:", SceneObject, typeof(GameObject), false);
    EditorGUILayout.ObjectField("Lightmap:", LightmapTextureFar, typeof(Texture2D), false);
    if (LightmapTextureFar != null) {
      string path = AssetDatabase.GetAssetPath(LightmapTextureFar);
      EditorGUILayout.TextField("Lightmap Path:", path);
      if (GUILayout.Button("Show In Explorer", GUILayout.MaxWidth(200))) {
        GUIHelp.OpenInFileBrowser(path);
      }
    }
    Rect winRect = this.position;
    float textureFildSize = Mathf.Min(winRect.width, winRect.height);
    LightmapTextureFar = EditorGUILayout.ObjectField(
      LightmapTextureFar,
      typeof(Texture),
      false,
      GUILayout.MaxWidth(textureFildSize),
      GUILayout.MaxHeight(textureFildSize)
      ) as Texture2D;
    Rect textureControlRect = GUILayoutUtility.GetLastRect();
    textureControlRect.xMin += 3.0f;
    textureControlRect.yMin += 3.0f;
    textureControlRect.xMax -= 3.0f;
    textureControlRect.yMax -= 3.0f;

    if (IsLightmapValid() && Event.current.type == EventType.Repaint) {
      if (!GUIHelp.IsRectEqual(textureControlRect, TextureControlRect)) {
        OnTextureControlRectChanged(textureControlRect);
      }
    }
  }
  private void PaintTextureTriangles() {
    if (!IsLightmapValid()) {
      return;
    }
    if (Event.current == null || Event.current.type != EventType.Repaint) {
      return;
    }
    LightmapGraphic.PaintTextureTriangles(CurTriangles, TextureControlRect, UVColors[0]);
    LightmapGraphic.PaintTextureTriangle(CurTriInfo, TextureControlRect, UVColors[1]);
  }
  private void ProcessInput() {
    if (!IsLightmapValid() || SceneView.mouseOverWindow != this) {
      return;
    }
    Event current = Event.current;
    if (current != null) {
      TriangleInfo oldTriInfo = CurTriInfo;
      Vector2 mousePos = current.mousePosition;
      if (TextureControlRect.Contains(mousePos)) {
        CurTriInfo = LightmapHelper.FindSelectedTriangle(CurTriangles, mousePos, TextureControlRect);
        if (CurTriInfo != null && oldTriInfo != CurTriInfo) {
          SceneView.RepaintAll();
        }
      }
    }
  }
  /************************************************************************/
  /* Call back                                                            */
  /************************************************************************/
  private void OnSelectionChange() {
    GameObject[] selObjs = Selection.gameObjects;
    if (selObjs == null || selObjs.Length <= 0) {
      return;
    }
    Reset();
    GameObject curObj = selObjs[0];
    if (curObj.GetComponent<Renderer>() != null) {
      if (curObj.GetComponent<Renderer>().lightmapIndex >= 0 && curObj.GetComponent<Renderer>().lightmapIndex < LightmapSettings.lightmaps.Length) {
        OnSceneObjectChanged(curObj);
        OnLightmapChanged();
      }
    }
    this.Repaint();
  }
  private void OnFocus() {
    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    SceneView.onSceneGUIDelegate += this.OnSceneGUI;
  }
  private void OnSceneGUI(SceneView sceneView) {
    if (IsLightmapValid() && SceneView.mouseOverWindow == SceneView.currentDrawingSceneView
      /*&& Event.current != null && Event.current.rawType == EventType.MouseUp*/) {
      Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
      RaycastHit hitInfo;
      if (Physics.Raycast(worldRay, out hitInfo, 1000, (1 << TargetLayerMask))) {
        if (hitInfo.transform.GetInstanceID() == SceneObject.transform.GetInstanceID() || hitInfo.transform.IsChildOf(SceneObject.transform)) {
          TriangleInfo triInfo = LightmapHelper.FindSelectedTriangleByIndex(CurTriangles, hitInfo.triangleIndex);
          if (triInfo != null) {
            CurTriInfo = triInfo;
          }
        }
      }
    }

    LightmapGraphic.PaintSceneTriangle(CurTriInfo);
  }
  private void OnLightmapChanged() {
    if (SceneObject.GetComponent<Renderer>() != null) {
      if (SceneObject.GetComponent<Renderer>().lightmapIndex >= 0 && SceneObject.GetComponent<Renderer>().lightmapIndex < LightmapSettings.lightmaps.Length) {
        Vector4 offset = SceneObject.GetComponent<Renderer>().lightmapScaleOffset;
        LightmapIndex = SceneObject.GetComponent<Renderer>().lightmapIndex;
        Texture2D curLightmapTex = LightmapSettings.lightmaps[LightmapIndex].lightmapColor;
        LightmapTextureFar = curLightmapTex;
        LightmapTilling = new Vector2(offset.x, offset.y);
        LightmapOffset = new Vector2(offset.z, offset.w);
      }
    }
    CurUVInfos = LightmapHelper.CaculateUV(SceneObject);
    CurTriangles = LightmapHelper.CaculateTex(CurUVInfos, TextureControlRect);
  }
  private void OnSceneObjectChanged(GameObject newObj) {
    if (SceneObject != null && newObj != null
      && SceneObject.GetInstanceID() == newObj.GetInstanceID()) {
      SceneObject = newObj;
      return;
    }
    if (ProcessInfo != null) {
      LightmapHelper.PostProcessObject(SceneObject, ProcessInfo);
    }
    ProcessInfo = LightmapHelper.PreProcessObject(newObj, TargetLayerMask);
    SceneObject = newObj;
  }
  private void OnTextureControlRectChanged(Rect textureControlRect) {
    TextureControlRect = textureControlRect;
    OnLightmapChanged();
  }
  private bool IsLightmapValid() {
    if (SceneObject == null || LightmapTextureFar == null || CurTriangles == null) {
      return false;
    }
    return true;
  }

}