using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResViewWin : EditorWindow {
  UnityEngine.Object TargetAssetBundle;
  string TargetResUrl = "";
  float TargetResPercent = 0;

  UnityEngine.Object TargetTestAsset;
  UnityEngine.Object TargetTestObj;
  Vector3 TargetTestPos;
  string TargetTestName;
  bool m_Loading = false;

  string m_ResContent;
  bool m_Running = false;
  IEnumerator m_Enumerator = null;
  WWW m_WWW;
  Vector2 m_ScrollPos;

  private void Initiaze() {
    TargetAssetBundle = null;
    TargetResUrl = "";
    m_ResContent = "";
    m_Enumerator = null;
    m_WWW = null;
    TargetTestName = "";
    m_Loading = false;
    m_Running = false;
  }
  private void OnEnable() {
    Initiaze();
  }
  private void OnGUI() {
    EditorGUILayout.Space();
    ShowResContent();
    ShowTest();

    this.Repaint();
  }
  private void ShowResContent() {
    TargetAssetBundle = EditorGUILayout.ObjectField("TargetRes:", TargetAssetBundle, typeof(UnityEngine.Object), true);
    EditorGUILayout.TextField("TargetResUrl:", TargetResUrl);
    EditorGUILayout.TextField("TargetResPercent:", string.Format("{0}%", TargetResPercent * 100));

    GUILayout.Label("Res Content:", EditorStyles.boldLabel);
    Rect lastRect = GUILayoutUtility.GetLastRect();
    m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos, GUILayout.MaxHeight(this.position.height - lastRect.yMax - 200));
    m_ResContent = EditorGUILayout.TextArea(m_ResContent, GUILayout.MinHeight(this.position.height - lastRect.yMax - 210));
    EditorGUILayout.EndScrollView();
  }
  private void ShowTest() {
    TargetTestAsset = EditorGUILayout.ObjectField("Target Test:", TargetTestAsset, typeof(UnityEngine.Object), false);
    if (TargetTestAsset != null && TargetTestObj == null) {
      TargetTestObj = GameObject.Instantiate(TargetTestAsset, TargetTestPos, Quaternion.identity);
    }
    TargetTestName = EditorGUILayout.TextField("Target Test Name:", TargetTestName);
    TargetTestPos = EditorGUILayout.Vector3Field("Target Test Pos:", TargetTestPos);
    EditorGUILayout.BeginHorizontal();
    if (GUILayout.Button("Load", GUILayout.MaxWidth(120))) {
      LoadAssetBundle(TargetAssetBundle);
      if (TargetTestObj != null) {
        GameObject.DestroyImmediate(TargetTestObj);
        TargetTestObj = null;
      }
      //Resources.UnloadAsset(TargetTestAsset);
      TargetTestAsset = null;
      Resources.UnloadUnusedAssets();
      m_Loading = true;
    }
    if (GUILayout.Button("UnLoad", GUILayout.MaxWidth(120))) {
      if (TargetTestObj != null) {
        GameObject.DestroyImmediate(TargetTestObj);
        TargetTestObj = null;
      }
      //Resources.UnloadAsset(TargetTestAsset);
      TargetTestAsset = null;
      Resources.UnloadUnusedAssets();
    }
    EditorGUILayout.EndHorizontal();
  }
  private void OnSelectionChange() {
    if (Selection.activeObject == null) {
      return;
    }
    UnityEngine.Object firstTarget = Selection.activeObject;
    string targetPath = AssetDatabase.GetAssetPath(firstTarget);
    if (targetPath.ToLower().EndsWith(ResBuildConfig.BuildOptionExtend)
      || targetPath.ToLower().EndsWith(".assets")
      ) {
      TargetAssetBundle = firstTarget;
      LoadAssetBundle(TargetAssetBundle);
    } else {
      TargetAssetBundle = null;
      TargetResUrl = "";
      m_ResContent = "";
      m_Enumerator = null;
      m_WWW = null;
      m_Loading = false;
      m_Running = false;
    }
    this.Repaint();
  }
  private void Update() {
    if (!m_Running) {
      if (m_Enumerator != null)
        m_Enumerator = null;
      return;
    }
    if (m_Enumerator == null) {
      m_Enumerator = LoadAsset(TargetResUrl);
    } else {
      TargetResPercent = m_WWW.progress;
    }
    if (!m_Enumerator.MoveNext()) {
      m_Running = false;
    }
  }
  private void LoadAssetBundle(UnityEngine.Object target) {
    TargetResUrl = ResBuildHelper.GetAssetURL(target);
    m_ResContent = "";
    m_Enumerator = null;
    m_Loading = false;
    m_Running = true;
  }
  private IEnumerator LoadAsset(string url) {
    ResBuildLog.Warn("LoadAsset url:" + url + " ...");
    m_WWW = new WWW(url);
    while (!m_WWW.isDone)
      yield return "";
    if (m_WWW.error != null)
      ResBuildLog.Warn(m_WWW.error);

    ExtractResInfo(m_WWW.assetBundle);
    if (m_Loading && !string.IsNullOrEmpty(TargetTestName)) {
      TargetTestAsset = m_WWW.assetBundle.LoadAsset(TargetTestName);
    }
    m_WWW.assetBundle.Unload(false);
    m_WWW = null;

  }
  private void ExtractResInfo(AssetBundle abTarget) {
    if (abTarget != null) {
      m_ResContent = string.Format("Name:{0}\n", TargetAssetBundle.name);
      m_ResContent += string.Format("Main Asset:{0}\n", (abTarget.mainAsset != null) ? abTarget.mainAsset.name : "null");
      UnityEngine.Object[] contentObjs = abTarget.LoadAllAssets(typeof(UnityEngine.Object));
      if (contentObjs != null) {
        m_ResContent += string.Format("Size:{0}\n", contentObjs.Length);
        for (int index = 0; index < contentObjs.Length; index++) {
          m_ResContent += string.Format("{0}\n", contentObjs[index].name);
        }
      }
    }
  }
}