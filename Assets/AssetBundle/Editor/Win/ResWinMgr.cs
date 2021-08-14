using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

public class ResWinMgr
{
  private static Vector2 m_WinMinSize = new Vector2(600.0f, 600.0f);
  private static Rect m_WinPosition = new Rect(300.0f, 200.0f, m_WinMinSize.x, m_WinMinSize.y);

  private static ResMainWin m_MainWin = null;
  private static ResViewWin m_ViewWin = null;
  private static ResPlayerWin m_PlayerWin = null;
  private static ResContentWin m_ContentWin = null;

  #region Config
  [MenuItem("AssetBundle/Config/Resources", false, 1)]
  public static void CreateResMainWin()
  {
    m_MainWin = EditorWindow.GetWindow<ResMainWin>("Config", false, typeof(ResMainWin));
    m_MainWin.position = m_WinPosition;
    m_MainWin.minSize = m_WinMinSize;
    m_MainWin.ShowTab();
    m_MainWin.Focus();
  }
  [MenuItem("AssetBundle/Config/Player", false, 1)]
  public static void CreateResPlayerWin()
  {
    m_PlayerWin = EditorWindow.GetWindow<ResPlayerWin>("Player", false, typeof(ResMainWin));
    m_PlayerWin.position = m_WinPosition;
    m_PlayerWin.minSize = m_WinMinSize;
    m_PlayerWin.ShowTab();
    m_PlayerWin.Focus();
  }
  #endregion

  #region BuildResources
  [MenuItem("AssetBundle/BuildResources/BuildAllResources", false, 2)]
  public static void BuildAllResources()
  {
    if (ResBuildProcesser.BuildAllResources()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResources Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResources Failed!",
        "OK");
    }
  }
  [MenuItem("AssetBundle/BuildResources/BuildConfigFiles", false, 20)]
  public static void BuildConfigFiles()
  {
    if (ResBuildConfig.Load()
          && ResBuildConfig.Check()
          && ResBuildGenerator.GenBuildConfig()
           && ResBuildGenerator.LoadResConfig()
           && ResCacheGenerator.BuildResCacheFile()
           && ResSheetGenerator.BuildResSheetFile()
           && ResVersionGenerator.BuildResVersionFiles()
           && VersionGenerator.GenVersionFile()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildConfigFiles Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildConfigFiles Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildResources/BuildSelectedResources", false, 20)]
  //public static void BuildSelectedResourceAsConfig()
  //{
  //  string targetDir = ResBuildProcesser.GetSelectedResources();
  //  if (string.IsNullOrEmpty(targetDir)) {
  //    EditorUtility.DisplayDialog(
  //      "Confirm",
  //      "BuildSelectedResourceAsConfig Selected nothing!",
  //      "OK");
  //    return;
  //  }
  //  if (EditorUtility.DisplayDialog(
  //    "Confirm",
  //    "BuildSelectedResourceAsConfig Selected:" + targetDir,
  //    "OK", "Cancel")) {
  //    if (ResBuildProcesser.BuildSelectResources(-1, targetDir)) {
  //      EditorUtility.DisplayDialog(
  //        "Confirm",
  //        "BuildSelectedResourceAsConfig Success!",
  //        "OK");
  //    } else {
  //      EditorUtility.DisplayDialog(
  //        "Confirm",
  //        "BuildSelectedResourceAsConfig Failed!",
  //        "OK");
  //    }
  //  }
  //}
  //[MenuItem("AssetBundle/BuildAllResources")]
  public static void BuildAllResourceAsConfig()
  {
    if (ResBuildProcesser.BuildAllResources()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResource Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResource Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildAllResources/BuildAllResources For Windows")]
  public static void BuildAllResourcesForWindows()
  {
    if (ResBuildProcesser.BuildAllResources((int)BuildTarget.StandaloneWindows)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForWindows Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForWindows Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildAllResources/BuildAllResources For Mac")]
  public static void BuildAllResourcesForMac()
  {
    if (ResBuildProcesser.BuildAllResources((int)BuildTarget.StandaloneOSXIntel)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForMac Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForMac Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildAllResources/BuildAllResources For Android")]
  public static void BuildAllResourcesForAndroid()
  {
    if (ResBuildProcesser.BuildAllResources((int)BuildTarget.Android)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForAndroid Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForAndroid Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildAllResources/BuildAllResources For iPhone")]
  public static void BuildAllResourcesForiPhone()
  {
    if (ResBuildProcesser.BuildAllResources((int)BuildTarget.iOS)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForiPhone Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildAllResourcesForiPhone Failed!",
        "OK");
    }
  }
  #endregion

  #region BuildPlayer
  [MenuItem("AssetBundle/BuildPlayer/BuildPlayer", false, 2)]
  public static void BuildPlayerAsConfig()
  {
    if (PlayerGenerator.Build()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerAsConfig Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerAsConfig Failed!",
        "OK");
    }
  }
  [MenuItem("AssetBundle/BuildPlayer/BuildPlayerPrefix", false, 2)]
  public static void BuildPlayerPrefixAsConfig()
  {
    if (PlayerGenerator.BuildPlayerPrefix()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixAsConfig Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixAsConfig Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildPlayer/BuildPlayerPrefix/BuildPlayerPrefix For Windows")]
  public static void BuildPlayerPrefixForWindows()
  {
    if (PlayerGenerator.BuildPlayerPrefix((int)BuildTarget.StandaloneWindows)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForWindows Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForWindows Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildPlayer/BuildPlayerPrefix/BuildPlayerPrefix For Mac")]
  public static void BuildPlayerPrefixForMac()
  {
    if (PlayerGenerator.BuildPlayerPrefix((int)BuildTarget.StandaloneOSXIntel)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForMac Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForMac Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildPlayer/BuildPlayerPrefix/BuildPlayerPrefix For Android")]
  public static void BuildPlayerPrefixForAndroid()
  {
    if (PlayerGenerator.BuildPlayerPrefix((int)BuildTarget.Android)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForAndroid Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForAndroid Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/BuildPlayer/BuildPlayerPrefix/BuildPlayerPrefix For iPhone")]
  public static void BuildPlayerPrefixForiPhone()
  {
    if (PlayerGenerator.BuildPlayerPrefix((int)BuildTarget.iOS)) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForiPhone Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildPlayerPrefixForiPhone Failed!",
        "OK");
    }
  }
  #endregion

  #region Clean
  [MenuItem("AssetBundle/Clean/Clean Cache", false, 10)]
  public static void CleanCache()
  {
    if (ResBuildConfig.Load()
     && ResDeployer.CleanCache()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CleanCache Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CleanCache Failed!",
        "OK");
    }
  }
  [MenuItem("AssetBundle/Clean/Clean Buildin Res", false, 10)]
  public static void CleanBuildInRes()
  {
    if (ResBuildConfig.Load()
      && ResDeployer.CleanBuildInRes()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CleanBuildInRes Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CleanBuildInRes Failed!",
        "OK");
    }
  }
  [MenuItem("AssetBundle/Clean/Clean Output Res", false, 10)]
  public static void CleanOutputRes()
  {
    if (ResBuildConfig.Load()
      && ResDeployer.CleanOutputDir()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CleanOutputRes Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CleanOutputRes Failed!",
        "OK");
    }
  }
  [MenuItem("AssetBundle/Clean/Clean All", false, 10)]
  public static void CleanAll()
  {
    if (EditorUtility.DisplayDialog("Clean All", "Clean All Resources and Cache?", "Confirm", "Cancel")) {
      //TODO:防止资源删除不干净，重复执行3次，不要喷，我知道挫爆了~~~~
      if (ResBuildConfig.Load()
        && ResDeployer.CleanAll()
        && ResDeployer.CleanAll()
        && ResDeployer.CleanAll()) {
        EditorUtility.DisplayDialog(
          "Confirm",
          "CleanAll Success!",
          "OK");
      } else {
        EditorUtility.DisplayDialog(
          "Confirm",
          "CleanAll Failed!",
          "OK");
      }
    }
  }
  #endregion
  #region Commit
  [MenuItem("AssetBundle/Commit/Commit BuildIn Res", false, 11)]
  public static void CommitBuildInRes()
  {
    if (ResBuildConfig.Load()
      && ResBuildGenerator.LoadResConfig()
      && ResDeployer.CommitBuildInResources()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CommitBuildInResources Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "CommitBuildInResources Failed!",
        "OK");
    }
  }
  #endregion
 
  #region Tools
  [MenuItem("AssetBundle/Tools/Content", false, 20)]
  public static void ShowContent()
  {
    m_ContentWin = EditorWindow.GetWindow<ResContentWin>("Content", false, typeof(ResMainWin));
    m_ContentWin.position = m_WinPosition;
    m_ContentWin.minSize = m_WinMinSize;
    m_ContentWin.ShowTab();
    m_ContentWin.Focus();
  }
  [MenuItem("AssetBundle/Tools/ShaderFinder", false, 20)]
  public static void ShaderFinder()
  {
    if (ResShaderFinder.FinderShaderData()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "ShaderFinder Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "ShaderFinder Failed!",
        "OK");
    }
  }
  [MenuItem("AssetBundle/Tools/TypeFinder", false, 20)]
  public static void TypeFinder()
  {
    if (ResTypeFinder.FinderTypeData()) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "TypeFinder Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "TypeFinder Failed!",
        "OK");
    }
  }
  //[MenuItem("AssetBundle/Windows/Tools/View")]
  public static void CreateResViewWin()
  {
    m_ViewWin = EditorWindow.GetWindow<ResViewWin>("View", false, typeof(ResMainWin));
    m_ViewWin.position = m_WinPosition;
    m_ViewWin.minSize = m_WinMinSize;
    m_ViewWin.ShowTab();
    m_ViewWin.Focus();
  }
  #endregion

  private static void CreateWins()
  {
    m_MainWin = EditorWindow.GetWindow<ResMainWin>("Config", false, typeof(ResMainWin));
    m_MainWin.position = m_WinPosition;
    m_MainWin.minSize = m_WinMinSize;

    m_ViewWin = EditorWindow.GetWindow<ResViewWin>("View", false, typeof(ResMainWin));
    m_PlayerWin = EditorWindow.GetWindow<ResPlayerWin>("Player", false, typeof(ResMainWin));
  }
}
