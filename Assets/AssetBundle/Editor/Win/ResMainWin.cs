using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResMainWin : EditorWindow
{
  bool IsGameConfig = true;
  bool IsBuildConfig = true;
  bool IsBuildOption = true;

  bool IsAdvancedConfig = false;
  bool IsADBuildConfig = false;
  bool IsADResGraph = false;
  bool IsADResVersion = false;
  bool IsADAssetDB = false;
  bool IsADResCache = false;
  bool IsADVersion = false;
  bool IsADResLog = false;
  bool IsADResCommit = false;
  bool IsADExtend = false;

  bool IsBuildOptionRes = false;

  private void Initialize()
  {
    bool ret = ResBuildConfig.Load();
    if (!ret){
      EditorUtility.DisplayDialog(
        "Confirm",
        "LoadConfig Failed!",
        "OK");
    }
  }
  private void Apply()
  {
    bool ret = ResBuildConfig.Save();
    if (ret) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "SaveConfig Success!",
        "OK");
    } else {
      EditorUtility.DisplayDialog(
        "Confirm",
        "SaveConfig Failed!",
        "OK");
    }
  }
  private void OnEnable()
  {
    Initialize();
  }
  private void OnGUI()
  {
    EditorGUILayout.Space();
    ShowConfig();
    ShowAdvancedConfig();
    ConfirmOption();
    this.Repaint();
  }
  private void ShowConfig()
  {
    IsGameConfig = EditorGUILayout.Foldout(IsGameConfig, "GameConfig");
    if (IsGameConfig) {
      ResBuildConfig.ClientVersion = EditorGUILayout.TextField("ClientVersion:", ResBuildConfig.ClientVersion);
      ResBuildConfig.ServerVersion = EditorGUILayout.TextField("ServerVersion:", ResBuildConfig.ServerVersion);
      EditorGUILayout.LabelField("Platform:", ResBuildHelper.GetPlatformName(ResBuildConfig.BuildOptionTarget));
      EditorGUILayout.LabelField("AppName:", ResBuildConfig.AppName);
      ResBuildConfig.Channel = EditorGUILayout.TextField("Channel:", ResBuildConfig.Channel);
      ResBuildConfig.ResServerURL = EditorGUILayout.TextField("ResServerURL:", ResBuildConfig.ResServerURL);
    }

    IsBuildConfig = EditorGUILayout.Foldout(IsBuildConfig, "BuildConfig");
    if (IsBuildConfig) {
      ResBuildConfig.ResBuildConfigOutputPath = EditorGUILayout.TextField("ResBuildConfigOutputPath:", ResBuildConfig.ResBuildConfigOutputPath);
      ResBuildConfig.ResBuildDirs = EditorGUILayout.TextField("ResBuildDirs:", ResBuildConfig.ResBuildDirs);
      ResBuildConfig.ResBuildPattern = EditorGUILayout.TextField("ResBuildPattern:", ResBuildConfig.ResBuildPattern);
      ResBuildConfig.ResBuildAssetPattern = EditorGUILayout.TextField("ResBuildAssetPattern:", ResBuildConfig.ResBuildAssetPattern);
      ResBuildConfig.ResBuildPlayerLevel = EditorGUILayout.TextField("ResBuildPlayerLevel:", ResBuildConfig.ResBuildPlayerLevel);
      ResBuildConfig.ResBuildPlayerPath = EditorGUILayout.TextField("ResBuildPlayerPath:", ResBuildConfig.ResBuildPlayerPath);
      ResBuildConfig.ResBuildInChapter = EditorGUILayout.IntField("ResBuildInChapter:", ResBuildConfig.ResBuildInChapter);
      //EditorGUILayout.LabelField("ResCurChapter:", ResBuildConfig.ResCurChapter.ToString());
      //EditorGUILayout.LabelField("ResChapterCount:", ResBuildConfig.ResChapterRes.Count.ToString());
      //IsResChapter = EditorGUILayout.Foldout(IsResChapter, "ResChapter");
      //if (IsResChapter) {
      //  for (int index = 0; index < ResBuildConfig.ResChapterRes.Count; index++) {
      //    EditorGUILayout.LabelField(string.Format("ResChapterRes{0}:", index + 1), ResBuildConfig.ResChapterRes[index]);
      //  }
      //}
      //EditorGUILayout.LabelField("ResBuildIn:", ResBuildConfig.ResBuildIn);
    }

    IsBuildOption = EditorGUILayout.Foldout(IsBuildOption, "BuildOption");
    if (IsBuildOption) {
      ResBuildConfig.BuildOptionTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildOptionTarget:", ResBuildConfig.BuildOptionTarget);
      ResBuildConfig.BuildOptionExtend = EditorGUILayout.TextField("BuildOptionExtend:", ResBuildConfig.BuildOptionExtend);
      IsBuildOptionRes = EditorGUILayout.Foldout(IsBuildOptionRes, "BuildOptionRes");
      if (IsBuildOptionRes) {
        foreach (string name in Enum.GetNames(typeof(BuildAssetBundleOptions))) {
          BuildAssetBundleOptions val = (BuildAssetBundleOptions)Enum.Parse(typeof(BuildAssetBundleOptions), name);
          if (EditorGUILayout.Toggle(name,
            (ResBuildConfig.BuildOptionRes & val) != 0)) {
            ResBuildConfig.BuildOptionRes |= val;
          } else {
            ResBuildConfig.BuildOptionRes &= ~val;
          }
        }
      }
    }
    EditorGUILayout.Space();
  }
  private void ShowAdvancedConfig()
  {
    IsAdvancedConfig = EditorGUILayout.Foldout(IsAdvancedConfig, "AdvancedConfig");
    if (IsAdvancedConfig) {
      IsADBuildConfig = EditorGUILayout.Foldout(IsADBuildConfig, "BuildConfig");
      if (IsADBuildConfig) {
        EditorGUILayout.LabelField(
          "ResBuildConfigFilePath:", ResBuildConfig.ResBuildConfigFilePath);
        EditorGUILayout.LabelField(
          "ResBuildConfigFormat:", ResBuildConfig.ResBuildConfigFormat);
        EditorGUILayout.LabelField(
          "ResBuildConfigHeader:", ResBuildConfig.ResBuildConfigHeader);
        EditorGUILayout.LabelField("ResChapterCount:", ResBuildConfig.ResChapterCount.ToString());
        foreach (ResChapterInfo info in ResBuildConfig.ResChapterRes){
          EditorGUILayout.TextField(string.Format("ResChapterRes{0}:", info.ResChapterIndex, info.ResChapterIdContent));
        }
      }

      IsADResGraph = EditorGUILayout.Foldout(IsADResGraph, "ResGraph");
      if (IsADResGraph) {
        EditorGUILayout.LabelField("ResGraphNodeFormat:", ResBuildConfig.ResGraphNodeFormat);
        EditorGUILayout.LabelField("ResGraphNodeHeader:", ResBuildConfig.ResGraphNodeHeader);
        EditorGUILayout.LabelField("ResGraphNodeInfoFormat:", ResBuildConfig.ResGraphNodeInfoFormat);
      }

      IsADResVersion = EditorGUILayout.Foldout(IsADResVersion, "ResVersion");
      if (IsADResVersion) {
        EditorGUILayout.LabelField("ResVersionFilePath:", ResBuildConfig.ResVersionFilePath);
        EditorGUILayout.LabelField("ResVersionZipPath:", ResBuildConfig.ResVersionZipPath);
        EditorGUILayout.LabelField("ResVersionFormat:", ResBuildConfig.ResVersionFormat);
        EditorGUILayout.LabelField("ResVersionHeader:", ResBuildConfig.ResVersionHeader);

        EditorGUILayout.LabelField("ResVersionClientFilePath:", ResBuildConfig.ResVersionClientFilePath);
        EditorGUILayout.LabelField("ResVersionClientFormat:", ResBuildConfig.ResVersionClientFormat);
        EditorGUILayout.LabelField("ResVersionClientHeader:", ResBuildConfig.ResVersionClientHeader);

        EditorGUILayout.LabelField("ResVersionIncrementalEnable:", ResBuildConfig.ResVersionIncrementalEnable.ToString());
        EditorGUILayout.LabelField("ResVersionIncrementalFilePath:", ResBuildConfig.ResVersionIncrementalFilePath);
        EditorGUILayout.LabelField("ResVersionIncrementalFormat:", ResBuildConfig.ResVersionIncrementalFormat);
        EditorGUILayout.LabelField("ResVersionIncrementalHeader:", ResBuildConfig.ResVersionIncrementalHeader);
      }
      
      IsADAssetDB = EditorGUILayout.Foldout(IsADAssetDB, "AssetDB");
      if (IsADAssetDB) {
        EditorGUILayout.LabelField("AssetDBFilePath:", ResBuildConfig.AssetDBFilePath);
        EditorGUILayout.LabelField("AssetDBZipPath:", ResBuildConfig.AssetDBZipPath);
        EditorGUILayout.LabelField("AssetDBFormat:", ResBuildConfig.AssetDBFormat);
        EditorGUILayout.LabelField("AssetDBHeader:", ResBuildConfig.AssetDBHeader);
        EditorGUILayout.LabelField("AssetDBPattern:", ResBuildConfig.AssetDBPattern);
      }

      IsADResCache = EditorGUILayout.Foldout(IsADResCache, "ResCache");
      if (IsADResCache) {
        EditorGUILayout.LabelField("ResCacheFilePath:", ResBuildConfig.ResCacheFilePath);
        EditorGUILayout.LabelField("ResCacheZipPath:", ResBuildConfig.ResCacheZipPath);
        EditorGUILayout.LabelField("ResCacheFormat:", ResBuildConfig.ResCacheFormat);
        EditorGUILayout.LabelField("ResCacheHeader:", ResBuildConfig.ResCacheHeader);
        EditorGUILayout.LabelField("ResCacheScenesPattern:", ResBuildConfig.ResCacheScenesPattern);
        EditorGUILayout.LabelField("ResCacheResourcesPattern:", ResBuildConfig.ResCacheResourcesPattern);
        EditorGUILayout.LabelField("ResCacheResExclude:", ResBuildConfig.ResCacheResExclude);
      }

      IsADVersion = EditorGUILayout.Foldout(IsADVersion, "Version");
      if (IsADVersion) {
        EditorGUILayout.LabelField("VersionClientFile:", ResBuildConfig.VersionClientFile);
        EditorGUILayout.LabelField("VersionServerFile:", ResBuildConfig.VersionServerFile);
      }

      IsADResLog = EditorGUILayout.Foldout(IsADResLog, "ResLog");
      if (IsADResLog) {
        EditorGUILayout.LabelField("ResLogFilePath:", ResBuildConfig.ResLogFilePath);
      }

      IsADResCommit = EditorGUILayout.Foldout(IsADResCommit, "ResCommit");
      if (IsADResCommit) {
        EditorGUILayout.LabelField("ResCommitSearchPattern:", ResBuildConfig.ResCommitSearchPattern);
        EditorGUILayout.LabelField("ResCommitBuildInPath:", ResBuildConfig.ResCommitBuildInPath);
        EditorGUILayout.LabelField("ResCommitCachePath:", ResBuildConfig.ResCommitCachePath);
      }

      IsADExtend = EditorGUILayout.Foldout(IsADExtend, "Extend");
      if (IsADExtend) {
        EditorGUILayout.LabelField("ResFinderShaderFilePath:", ResBuildConfig.ResFinderShaderFilePath);
        EditorGUILayout.LabelField("ResFinderTypeFilePath:", ResBuildConfig.ResFinderTypeFilePath);
        EditorGUILayout.LabelField("ResFinderTypeTargetDir:", ResBuildConfig.ResFinderTypeTargetDir);
        EditorGUILayout.LabelField("ResFinderTypeExcludePattern:", ResBuildConfig.ResFinderTypeExcludePattern);
        EditorGUILayout.LabelField("ResFinderTypeIncludePattern:", ResBuildConfig.ResFinderTypeIncludePattern);
      }
    }
    EditorGUILayout.Space();
  }
  private void ConfirmOption()
  {
    EditorGUILayout.BeginHorizontal();
    if (GUILayout.Button("Apply", GUILayout.MaxWidth(80))) {
      Apply();
    }
    if (GUILayout.Button("Reset", GUILayout.MaxWidth(80))) {
      Initialize();
    }
    if (GUILayout.Button("Check", GUILayout.MaxWidth(80))) {
      if (ResBuildConfig.Check()) {
        EditorUtility.DisplayDialog(
        "Confirm",
        "CheckConfig Success!",
        "OK");
      } else {
        EditorUtility.DisplayDialog(
          "Confirm",
          "CheckConfig Failed! Check log plz!",
          "OK");
      }
    }
    EditorGUILayout.EndHorizontal();
  }
}