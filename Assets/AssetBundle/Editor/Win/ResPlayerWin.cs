using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResPlayerWin : EditorWindow
{
  bool IsGameConfig = true;
  bool IsBuildOption = true;
  bool IsBuildOptionPlayer = false;

  private void Initialize()
  {
    ResBuildConfig.Load();
  }
  private void Apply()
  {
    ResBuildConfig.Save();
  }
  private void OnEnable()
  {
    Initialize();
  }
  private void OnGUI()
  {
    EditorGUILayout.Space();
    ShowConfig();
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
      ResBuildConfig.AppName = EditorGUILayout.TextField("AppName:", ResBuildConfig.AppName);
      ResBuildConfig.Channel = EditorGUILayout.TextField("Channel:", ResBuildConfig.Channel);
      ResBuildConfig.ResServerURL = EditorGUILayout.TextField("ResServerURL:", ResBuildConfig.ResServerURL);
    }

    IsBuildOption = EditorGUILayout.Foldout(IsBuildOption, "BuildOption");
    if (IsBuildOption) {
      ResBuildConfig.BuildOptionTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildOptionTarget:", ResBuildConfig.BuildOptionTarget);
      IsBuildOptionPlayer = EditorGUILayout.Foldout(IsBuildOptionPlayer, "BuildOptionPlayer");
      if (IsBuildOptionPlayer) {
        foreach (string name in Enum.GetNames(typeof(BuildOptions))) {
          BuildOptions val = (BuildOptions)Enum.Parse(typeof(BuildOptions), name);
          if (EditorGUILayout.Toggle(name,
            (ResBuildConfig.BuildOptionPlayer & val) != 0)) {
            ResBuildConfig.BuildOptionPlayer |= val;
          } else {
            ResBuildConfig.BuildOptionPlayer &= ~val;
          }
        }
      }
      ResBuildConfig.PlayerBundleIdentifier = EditorGUILayout.TextField(
        "PlayerBundleIdentifier:", ResBuildConfig.PlayerBundleIdentifier);
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
    EditorGUILayout.EndHorizontal();
  }
}