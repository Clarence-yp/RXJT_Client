using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class PlayerGenerator
{
  public static bool BuildPlayerPrefix(int buildTarget = -1)
  {
    try {
      if (!ResBuildConfig.Load()) {
        ResBuildLog.Warn("PlayerGenerator.Load failed!");
        return false;
      }
      if ((buildTarget >= 0 && !ResBuildConfig.SetBuildTargetPlatform((BuildTarget)buildTarget))) {
        ResBuildLog.Warn("PlayerGenerator.SetBuildTargetPlatform failed!");
        return false;
      }
      if (!ResBuildLog.SetFileLog(true, ResBuildHelper.FormatResPlayerLogFilePath())) {
        ResBuildLog.Warn("PlayerGenerator.SetFileLog failed!");
        return false;
      }
      if (!ResDeployer.CleanBuildInRes()) {
        ResBuildLog.Warn("PlayerGenerator.CleanBuildInRes failed!");
        return false;
      }
      if (!ProcessResourcesDir()) {
        ResBuildLog.Warn("PlayerGenerator.ProcessResourcesDir failed!");
        return false;
      }
      if (!(ResDeployer.CommitBuildInResources())) {
        ResBuildLog.Warn("PlayerGenerator.CommitBuildInResources failed!");
        return false;
      }
      if (!(ApplyPlayerConfig())) {
        ResBuildLog.Warn("PlayerGenerator.ApplyPlayerConfig failed!");
        return false;
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("PlayerGenerator.Build failed! ex:" + ex);
      return false;
    }
    ResBuildLog.Info("PlayerGenerator.BuildPlayerPrefix Success");
    return true;
  }

  public static bool Build(int buildTarget = -1)
  {
    try {
      if (!BuildPlayerPrefix(buildTarget)) {
        ResBuildLog.Warn("PlayerGenerator.BuildPlayerPrefix failed!");
        return false;
      }
      if (!ResDeployer.CleanOutputPlayerDir()) {
        ResBuildLog.Warn("PlayerGenerator.CleanOutputPlayerDir failed!");
        return false;
      }
      if (!BuildPlayer()) {
        ResBuildLog.Warn("PlayerGenerator.BuildPlayer failed!");
        return false;
      }
      ResBuildLog.Info("PlayerGenerator.Build Success");
    } catch (System.Exception ex) {
      ResBuildLog.Warn("PlayerGenerator.Build failed! ex:" + ex);
      return false;
    }
    return true;
  }
  private static bool ProcessResourcesDir()
  {
    // Process StreamingAssets Dir
    string srcDir = "assets/streamingAssets";
    if (Directory.Exists(srcDir)) {
      AssetDatabase.MoveAssetToTrash(srcDir);
    }

    string[] tResBuildDirs = ResBuildConfig.ResBuildDirs.Split(ResBuildConfig.ConfigSplit,
    StringSplitOptions.RemoveEmptyEntries);
    if (tResBuildDirs != null && tResBuildDirs.Length > 0) {
      foreach (string rbd in tResBuildDirs) {
        if (rbd.ToLower().StartsWith("assets/resources")) {
          srcDir = rbd;
          if (srcDir.EndsWith("/")) {
            srcDir = rbd.Substring(0, srcDir.Length - 1);
          }
          if (Directory.Exists(srcDir)) {
            AssetDatabase.MoveAssetToTrash(srcDir);
          }
        }
      }
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("PlayerGenerator.ProcessResourcesDir Success.");

    return true;
  }
  private static bool ApplyPlayerConfig()
  {
    PlayerSettings.bundleVersion = ResBuildConfig.ClientVersion;
    ResBuildLog.Info("PlayerGenerator.ApplyPlayerConfig Success.");
    return true;
  }
  private static bool BuildPlayer()
  {
    try {
      AssetDatabase.Refresh();

      string extend = ResBuildHelper.GetPlatformExtend(ResBuildConfig.BuildOptionTarget);
      string targetPath = ResBuildHelper.GetFilePathAbs(ResBuildConfig.ResBuildPlayerPath);
      targetPath = Path.Combine(targetPath, ResBuildHelper.GetChannelPlatformName(ResBuildConfig.BuildOptionTarget));
      if (!Directory.Exists(targetPath)) {
        Directory.CreateDirectory(targetPath);
      }
      targetPath = Path.Combine(targetPath, ResBuildConfig.AppName + extend);
      string[] tResBuildPlayerLevel = ResBuildConfig.ResBuildPlayerLevel.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
      BuildPipeline.BuildPlayer(
        tResBuildPlayerLevel,
        targetPath,
        ResBuildConfig.BuildOptionTarget,
        ResBuildConfig.BuildOptionPlayer);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("PlayerGenerator.BuildPlayer failed. ex" + ex);
      return false;
    }
    ResBuildLog.Info("PlayerGenerator.BuildPlayer Success.");
    return true;
  }
  private static string GetBackupPath()
  {
    string targetPath = ResBuildHelper.GetFilePathAbs(ResBuildConfig.ResBuildPlayerPath);
    targetPath = Path.Combine(targetPath, "backup");
    targetPath = Path.Combine(targetPath, ResBuildHelper.GetChannelPlatformName(ResBuildConfig.BuildOptionTarget));
    return targetPath;
  }
  private static string GetResourcesBackupPath()
  {
    return Path.Combine(GetBackupPath(), "Resources");
  }
  private static string GetStreamingAssetsBackupPath()
  {
    return Path.Combine(GetBackupPath(), "StreamingAssets");
  }
}
