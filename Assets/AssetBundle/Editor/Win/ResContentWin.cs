using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DashFire;

public class ResContentWin : EditorWindow
{
  private const string c_AssetBundleFormat = "\tID:{0}\n\tResources:{1}\n\tTargetName:{2}\n\tDepends:{3}";
  private const string c_AssetBundleDependenciesFormat = "\tID:{0} \tResources:{1} \tTargetName:{2} \tDepends:{3}";
  private const string c_ContentDetailFormat = "\t{0}(Size:{1}B/{2}KB/{3}M)";
  private static string[] c_AssetOtherPattern = new string[] { ".cs", ".js" };

  enum ResType
  {
    None,
    Asset,
    AssetBundle,
    Directory,
  }

  UnityEngine.Object m_TargetObj;
  ResType m_TargetType;
  string m_TargetPath;
  ResBuildData m_TargetBuildData;
  string m_TargetAssetPath;
  List<string> m_TargetAssetDependenciesList;
  List<string> m_TargetInnerAssetList;
  List<string> m_TargetOuterAssetList;
  List<string> m_TargetOtherAssetList;

  //UnityEngine.AssetBundle m_AssetBundleObj;
  //List<UnityEngine.Object> m_AssetBundleAssetsList;
  //IEnumerator m_LoadAssetBundleEnumerator = null;

  string m_Content;
  Vector2 m_ContentScrollPos;
  string m_TipInfo;

  bool m_IsBaseContent;

  bool m_IsTheoryConfig;
  bool m_IsTheoryAssetBundleContent;
  bool m_IsTheoryAssetBundleDependenciesContent;
  bool m_IsTheoryAssetContent;
  bool m_IsTheoryAssetDependenciesContent;
  bool m_IsTheoryInnerAssetContent;
  bool m_IsTheoryOuterAssetContent;
  bool m_IsTheoryOtherAssetContent;

  //bool m_IsActualConfig;
  //bool m_IsActualAssetBundleContent;
  //bool m_IsActualAssetBundleDependenciesContent;
  //bool m_IsActualAssetContent;
  //bool m_IsActualAssetDependenciesContent;
  //bool m_IsActualInnerAssetContent;
  //bool m_IsActualOuterAssetContent;
  //bool m_IsActualOtherAssetContent;

  private void Initialize()
  {
    m_TargetObj = null;
    m_TargetType = ResType.None;
    m_TargetAssetPath = string.Empty;
    m_TargetAssetDependenciesList = new List<string>();
    m_TargetInnerAssetList = new List<string>();
    m_TargetOuterAssetList = new List<string>();
    m_TargetOtherAssetList = new List<string>();

    m_TipInfo = string.Empty;
    m_Content = string.Empty;
    m_ContentScrollPos = Vector2.zero;

    m_IsBaseContent = true;

    m_IsTheoryConfig = false;
    m_IsTheoryAssetBundleContent = true;
    m_IsTheoryAssetBundleDependenciesContent = false;
    m_IsTheoryAssetContent = false;
    m_IsTheoryAssetDependenciesContent = false;
    m_IsTheoryInnerAssetContent = true;
    m_IsTheoryOuterAssetContent = false;
    m_IsTheoryOtherAssetContent = false;

    //m_IsActualConfig = false;
    //m_IsActualAssetBundleContent = true;
    //m_IsActualAssetBundleDependenciesContent = false;
    //m_IsActualAssetContent = false;
    //m_IsActualAssetDependenciesContent = false;
    //m_IsActualInnerAssetContent = true;
    //m_IsActualOuterAssetContent = false;
    //m_IsActualOtherAssetContent = false;

    ResBuildConfig.Load();
    bool ret = ResBuildGenerator.LoadResConfig();
    if (!ret) {
      m_TipInfo = string.Format("载入ResBuildConfig.txt错误！");
    }

    //if (m_AssetBundleObj != null) {
    //  m_AssetBundleObj.Unload(false);
    //}
    //m_AssetBundleObj = null;
    //m_AssetBundleAssetsList = new List<UnityEngine.Object>();
    //if (m_LoadAssetBundleEnumerator != null) {
    //  m_LoadAssetBundleEnumerator.Reset();
    //}
    //m_LoadAssetBundleEnumerator = null;
  }
  private void OnEnable()
  {
    Initialize();
  }
  private void OnDestroy()
  {
    Initialize();
  }
  private void Update()
  {
    //if (m_LoadAssetBundleEnumerator != null) {
    //  if (!m_LoadAssetBundleEnumerator.MoveNext()) {
    //    m_LoadAssetBundleEnumerator = null;
    //  }
    //}
  }
  private void OnGUI()
  {
    ShowContent();
    this.Repaint();
  }
  private void OnSelectionChange()
  {
    if (Selection.activeObject == null) {
      return;
    }
    UnityEngine.Object firstTarget = Selection.activeObject;
    OnTargetChanged(firstTarget);
    this.Repaint();
  }
  private void ShowContent()
  {
    m_TargetObj = EditorGUILayout.ObjectField("Target:", m_TargetObj, typeof(UnityEngine.Object), true);
    m_TipInfo = EditorGUILayout.TextField(m_TipInfo);

    m_IsBaseContent = GUILayout.Toggle(m_IsBaseContent, "Base Content");

    m_IsTheoryConfig = EditorGUILayout.Foldout(m_IsTheoryConfig, "TheoryConfig");
    if (m_IsTheoryConfig) {
      m_IsTheoryAssetBundleContent = GUILayout.Toggle(m_IsTheoryAssetBundleContent, "AssetBundle Content");
      m_IsTheoryAssetBundleDependenciesContent = GUILayout.Toggle(m_IsTheoryAssetBundleDependenciesContent, "AssetBundle Dependencies Content");
      m_IsTheoryAssetContent = GUILayout.Toggle(m_IsTheoryAssetContent, "Asset Content");
      m_IsTheoryAssetDependenciesContent = GUILayout.Toggle(m_IsTheoryAssetDependenciesContent, "Asset Dependencies Content");
      m_IsTheoryInnerAssetContent = GUILayout.Toggle(m_IsTheoryInnerAssetContent, "Inner Asset Content");
      m_IsTheoryOuterAssetContent = GUILayout.Toggle(m_IsTheoryOuterAssetContent, "Outer Asset Content");
      m_IsTheoryOtherAssetContent = GUILayout.Toggle(m_IsTheoryOtherAssetContent, "Other Asset Content");
    }

    //m_IsActualConfig = EditorGUILayout.Foldout(m_IsActualConfig, "ActualConfig");
    //if (m_IsActualConfig) {
    //  m_IsActualAssetBundleContent = GUILayout.Toggle(m_IsActualAssetBundleContent, "AssetBundle Content");
    //  m_IsActualAssetBundleDependenciesContent = GUILayout.Toggle(m_IsActualAssetBundleDependenciesContent, "AssetBundle Dependencies Content");
    //  m_IsActualAssetContent = GUILayout.Toggle(m_IsActualAssetContent, "Asset Content");
    //  m_IsActualAssetDependenciesContent = GUILayout.Toggle(m_IsActualAssetDependenciesContent, "Asset Dependencies Content");
    //  m_IsActualInnerAssetContent = GUILayout.Toggle(m_IsActualInnerAssetContent, "Inner Asset Content");
    //  m_IsActualOuterAssetContent = GUILayout.Toggle(m_IsActualOuterAssetContent, "Outer Asset Content");
    //  m_IsActualOtherAssetContent = GUILayout.Toggle(m_IsActualOtherAssetContent, "Other Asset Content");
    //}

    if (GUILayout.Button("Refresh", GUILayout.MaxWidth(120))) {
      FreshContent();
    }

    Rect lastRect = GUILayoutUtility.GetLastRect();
    float textAreaHeight = this.position.height - lastRect.y - lastRect.height;
    m_ContentScrollPos = GUILayout.BeginScrollView(m_ContentScrollPos, GUILayout.MaxHeight(textAreaHeight));
    m_Content = GUILayout.TextArea(m_Content, int.MaxValue, GUILayout.MinHeight(textAreaHeight - 10));
    GUILayout.EndScrollView();
  }
  private void OnTargetChanged(UnityEngine.Object target)
  {
    m_TargetObj = target;
    m_TargetPath = AssetDatabase.GetAssetPath(m_TargetObj);
    m_TargetType = GetTargetType(m_TargetPath);
    m_TargetBuildData = null;
    m_TargetAssetPath = string.Empty;
    m_TargetAssetDependenciesList.Clear();
    m_TargetInnerAssetList.Clear();
    m_TargetOuterAssetList.Clear();
    m_TargetOtherAssetList.Clear();

    switch (m_TargetType) {
      case ResType.Directory: {
        } break;
      case ResType.AssetBundle: {
          m_TargetBuildData = FindResBuildDataByABName(m_TargetPath);
          if (m_TargetBuildData != null) {
            m_TargetAssetPath = m_TargetBuildData.m_Resources;
          }
          if (!string.IsNullOrEmpty(m_TargetAssetPath)) {
            string[] pathNames = new string[] { m_TargetAssetPath };
            string[] dependInfo = AssetDatabase.GetDependencies(pathNames);
            m_TargetAssetDependenciesList.AddRange(dependInfo);
            m_TargetAssetDependenciesList = m_TargetAssetDependenciesList.ConvertAll<string>(low => {
              return low.ToLower();
            });
          }
          if (m_TargetAssetDependenciesList.Count > 0) {
            foreach (string assetPath in m_TargetAssetDependenciesList) {
              string[] tResBuildAssetPattern =
                ResBuildConfig.ResBuildAssetPattern.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
              if (tResBuildAssetPattern != null) {
                if (ResBuildHelper.CheckFilePatternEndWith(assetPath, c_AssetOtherPattern)) {
                  m_TargetOtherAssetList.Add(assetPath);
                } else if (ResBuildHelper.CheckFilePatternEndWith(assetPath, tResBuildAssetPattern)
                  && !assetPath.Equals(m_TargetAssetPath)) {
                  m_TargetOuterAssetList.Add(assetPath);
                } else {
                  m_TargetInnerAssetList.Add(assetPath);
                }
              }
            }
          }
          //if (m_AssetBundleObj != null) {
          //  m_AssetBundleObj.Unload(false);
          //}
          //m_AssetBundleAssetsList.Clear();
          //m_LoadAssetBundleEnumerator = LoadAssetBundle(m_TargetPath);
          //CoroutineInsManager.Instance.StartCoroutine(LoadAssetBundle(m_TargetPath));
        } break;
    }

    FreshContent();
  }
  private void FreshContent()
  {
    StringBuilder sb = new StringBuilder();
    if (m_IsBaseContent) {
      sb.AppendLine(ExtractBaseContent());
    }
    if (m_IsTheoryAssetBundleContent) {
      sb.AppendLine(ExtractAssetBundleContent());
    }
    if (m_IsTheoryAssetBundleDependenciesContent) {
      sb.AppendLine(ExtractAssetBundleDependenciesContent());
    }
    if (m_IsTheoryAssetContent) {
      sb.AppendLine(ExtractAssetContent());
    }
    if (m_IsTheoryAssetDependenciesContent) {
      sb.AppendLine(ExtractAssetDependenciesContent());
    }
    if (m_IsTheoryInnerAssetContent) {
      sb.AppendLine(ExtractInnerAssetContent());
    }
    if (m_IsTheoryOuterAssetContent) {
      sb.AppendLine(ExtractOuterAssetContent());
    }
    if (m_IsTheoryOtherAssetContent) {
      sb.AppendLine(ExtractOtherAssetContent());
    }

    m_Content = sb.ToString();
    GUIUtility.keyboardControl = 0;
  }
  private string ExtractBaseContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("TargetPath:" + m_TargetPath);
    sb.AppendLine("TargetType:" + m_TargetType.ToString());
    long size = ResBuildHelper.GetFileSize(ResBuildHelper.GetFilePathAbs(m_TargetPath));
    sb.AppendLine(string.Format("TargetSize:{0}B/{1}KB/{2}M", size, (float)size / (1024), (float)size / (1024 * 1024)));
    return sb.ToString();
  }
  private string ExtractAssetBundleContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Theory:AssetBundle:");
    if (m_TargetBuildData == null) {
      return sb.ToString();
    }

    string abInfo = ExtractAssetBundleInfo(m_TargetBuildData, c_AssetBundleFormat);
    sb.Append(abInfo);
    return sb.ToString();
  }
  private string ExtractAssetBundleDependenciesContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Theory:AssetBundleDependencies:");
    if (m_TargetBuildData == null) {
      return sb.ToString();
    }

    if (m_TargetBuildData.m_Depends != null && m_TargetBuildData.m_Depends.Count > 0) {
      foreach (int dependId in m_TargetBuildData.m_Depends) {
        ResBuildData buildData = ResBuildProvider.Instance.GetDataById(dependId);
        if (buildData != null) {
          string abDependInfo = ExtractAssetBundleInfo(buildData, c_AssetBundleDependenciesFormat);
          sb.Append(abDependInfo);
        }
      }
    }
    return sb.ToString();
  }
  private string ExtractAssetContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Theory:AssetContent:");
    long size = ResBuildHelper.GetFileSize(ResBuildHelper.GetFilePathAbs(m_TargetAssetPath));
    sb.AppendLine(string.Format(c_ContentDetailFormat, m_TargetAssetPath, size, (float)size / (1024), (float)size / (1024 * 1024)));
    return sb.ToString();
  }
  private string ExtractAssetDependenciesContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Theory:AssetDependencies:");
    if (m_TargetAssetDependenciesList == null || m_TargetAssetDependenciesList.Count <= 0) {
      return sb.ToString();
    }

    foreach (string assetPath in m_TargetAssetDependenciesList) {
      long size = ResBuildHelper.GetFileSize(ResBuildHelper.GetFilePathAbs(assetPath));
      sb.AppendLine(string.Format(c_ContentDetailFormat, assetPath, size, (float)size / (1024), (float)size / (1024 * 1024)));
    }
    return sb.ToString();
  }
  private string ExtractAssetBundleInfo(ResBuildData buildData, string format)
  {
    StringBuilder sb = new StringBuilder();
    if (buildData != null) {
      string buildDataInfo = string.Format(format,
        buildData.m_Id,
        buildData.m_Resources,
        buildData.m_TargetName,
        ResBuildHelper.FormatListContent(buildData.m_Depends));
      sb.AppendLine(buildDataInfo);
    } else {
      m_TipInfo = "ExtractAssetBundleInfo ResBuildData null";
    }
    return sb.ToString();
  }
  private string ExtractInnerAssetContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Theory:AssetInnerContent:");
    if (m_TargetInnerAssetList == null || m_TargetInnerAssetList.Count <= 0) {
      return sb.ToString();
    }

    foreach (string assetPath in m_TargetInnerAssetList) {
      long size = ResBuildHelper.GetFileSize(ResBuildHelper.GetFilePathAbs(assetPath));
      sb.AppendLine(string.Format(c_ContentDetailFormat, assetPath.ToLower(), size, (float)size / (1024), (float)size / (1024 * 1024)));
    }
    return sb.ToString();
  }
  private string ExtractOuterAssetContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Theory:AssetOuterContent:");
    if (m_TargetOuterAssetList == null || m_TargetOuterAssetList.Count <= 0) {
      return sb.ToString();
    }

    foreach (string assetPath in m_TargetOuterAssetList) {
      long size = ResBuildHelper.GetFileSize(ResBuildHelper.GetFilePathAbs(assetPath));
      sb.AppendLine(string.Format(c_ContentDetailFormat, assetPath, size, (float)size / (1024), (float)size / (1024 * 1024)));
    }
    return sb.ToString();
  }
  private string ExtractOtherAssetContent()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("Theory:AssetOtherContent:");
    if (m_TargetOtherAssetList == null || m_TargetOtherAssetList.Count <= 0) {
      return sb.ToString();
    }

    foreach (string assetPath in m_TargetOtherAssetList) {
      long size = ResBuildHelper.GetFileSize(ResBuildHelper.GetFilePathAbs(assetPath));
      sb.AppendLine(string.Format(c_ContentDetailFormat, assetPath, size, (float)size / (1024), (float)size / (1024 * 1024)));
    }
    return sb.ToString();
  }
  private ResType GetTargetType(string targetPath)
  {
    ResType type = ResType.None;
    string targetPathAbs = ResBuildHelper.GetFilePathAbs(targetPath);
    FileAttributes fileAttr = File.GetAttributes(targetPathAbs);
    if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory) {
      type = ResType.Directory;
    } else {
      if (targetPath.EndsWith(ResBuildConfig.BuildOptionExtend)) {
        type = ResType.AssetBundle;
      } else {
        type = ResType.Asset;
      }
    }
    return type;
  }
  private ResBuildData FindResBuildDataByABName(string abName)
  {
    string abFileNameFormat = Path.GetFileNameWithoutExtension(abName);
    try {
      int abId = -1;
      if (int.TryParse(abFileNameFormat, out abId)) {
        return ResBuildProvider.Instance.GetDataById(abId);
      }
    } catch (System.Exception ex) {
      m_TipInfo = "Parse abName error! abName:" + abFileNameFormat + "ex:" + ex;
    }
    return null;
  }
}