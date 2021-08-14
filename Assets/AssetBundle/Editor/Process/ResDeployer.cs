using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class ResDeployer
{
  #region Commit
  public static bool CommitBuildInResources()
  {
    CleanBuildInRes();
    string srcDir = ResBuildHelper.GetFilePathAbs(ResBuildHelper.GetPlatformPath(ResBuildConfig.BuildOptionTarget));
    if (!System.IO.Directory.Exists(srcDir)) {
      ResBuildLog.Warn("ResDeployer.CopyBuildInResources failed srcPath:" + srcDir);
      return false;
    }
    string destDir = ResBuildHelper.GetFilePathAbs(ResBuildConfig.ResCommitBuildInPath);
    if (!System.IO.Directory.Exists(destDir)) {
      System.IO.Directory.CreateDirectory(destDir);
    }
    if (!System.IO.Directory.Exists(destDir)) {
      ResBuildLog.Warn("ResDeployer.CopyBuildInResources failed destDir:" + destDir);
      return false;
    }
    if (!ResVersionGenerator.LoadResVersion()) {
      ResBuildLog.Warn("ResDeployer.LoadResVersion failed");
      return false;
    }
    foreach (ResVersionData buildData in ResVersionProvider.Instance.GetData().Values) {
      if (buildData != null && ResBuildHelper.IsResBuildIn(buildData.GetId(), buildData.m_Chapter)) {
        string abPathSource = Path.Combine(srcDir, buildData.m_AssetBundleName);
        string abPathDest = Path.Combine(destDir, buildData.m_AssetBundleName);
        string dir = string.Empty;
        try {
          if (!File.Exists(abPathSource)) {
            ResBuildLog.Warn("ResDeployer.CopyBuildInResources copy file not exist. abPathSource:" + abPathSource);
            continue;
          }
          dir = Path.GetDirectoryName(abPathDest);
          if (!System.IO.Directory.Exists(dir)) {
            System.IO.Directory.CreateDirectory(dir);
          }
          File.Copy(abPathSource, abPathDest, true);
          ResBuildLog.Info("ResDeployer.CopyBuildInResources copy ab from:{0} to:{1}", abPathSource, abPathDest);
        } catch (System.Exception ex) {
          ResBuildLog.Warn("ResDeployer.CopyBuildInResources copy failed ab from:{0} to:{1}", abPathSource, abPathDest);
        }
      }
    }

    string resVersionPathSource = Path.Combine(srcDir, ResBuildConfig.ResVersionZipPath);
    string resVersionPathDest = Path.Combine(destDir, ResBuildConfig.ResVersionZipPath);
    if (!File.Exists(resVersionPathSource)) {
      ResBuildLog.Warn("ResDeployer.CopyBuildInResources copy file not exist. resVersionPathSource:" + resVersionPathSource);
      return false;
    }
    File.Copy(resVersionPathSource, resVersionPathDest, true);
    ResBuildLog.Info("ResDeployer.CopyBuildInResources copy ab from:{0} to:{1}",
      resVersionPathSource, resVersionPathDest);

    string resVersionClientPathSource = Path.Combine(srcDir, ResBuildConfig.ResVersionClientFilePath);
    string resVersionClientPathDest = Path.Combine(destDir, ResBuildConfig.ResVersionClientFilePath);
    if (!File.Exists(resVersionClientPathSource)) {
      ResBuildLog.Warn("ResDeployer.CopyBuildInResources copy file not exist. resVersionClientPathSource:" + resVersionClientPathSource);
      return false;
    }
    File.Copy(resVersionClientPathSource, resVersionClientPathDest, true);
    ResBuildLog.Info("ResDeployer.CopyBuildInResources copy ab from:{0} to:{1}",
      resVersionClientPathSource, resVersionClientPathDest);

    string resCachePathSource = Path.Combine(srcDir, ResBuildConfig.ResCacheZipPath);
    string resCachePathDest = Path.Combine(destDir, ResBuildConfig.ResCacheZipPath);
    if (!File.Exists(resCachePathSource)) {
      ResBuildLog.Warn("ResDeployer.CopyBuildInResources copy file not exist. resCachePathSource:" + resCachePathSource);
      return false;
    }
    File.Copy(resCachePathSource, resCachePathDest, true);
    ResBuildLog.Info("ResDeployer.CopyBuildInResources copy ab from:{0} to:{1}",
      resCachePathSource, resCachePathDest);

    string resSheetPathSource = Path.Combine(srcDir, ResBuildConfig.ResSheetZipPath);
    string resSheetPathDest = Path.Combine(destDir, ResBuildConfig.ResSheetZipPath);
    if (!File.Exists(resSheetPathSource)) {
      ResBuildLog.Warn("ResDeployer.CopyBuildInResources copy file not exist. resSheetPathSource:" + resSheetPathSource);
      return false;
    }
    File.Copy(resSheetPathSource, resSheetPathDest, true);
    ResBuildLog.Info("ResDeployer.CopyBuildInResources copy ab from:{0} to:{1}",
      resSheetPathSource, resSheetPathDest);

    string versionPathSource = Path.Combine(srcDir, ResBuildConfig.VersionClientFile);
    string versionPathDest = Path.Combine(destDir, ResBuildConfig.VersionClientFile);
    if (!File.Exists(versionPathSource)) {
      ResBuildLog.Warn("ResDeployer.CopyBuildInResources copy file not exist. versionPathSource:" + versionPathSource);
      return false;
    }
    File.Copy(versionPathSource, versionPathDest, true);
    ResBuildLog.Info("ResDeployer.CopyBuildInResources copy ab from:{0} to:{1}",
      versionPathSource, versionPathDest);

    AssetDatabase.Refresh();
    ResBuildLog.Info("ResDeployer.CopyBuildInResources Done");
    return true;
  }
  #endregion
  #region Clean
  public static bool CleanCache()
  {
    bool ret = false;
    try {
      Caching.ClearCache();
      ResBuildProvider.Instance.Clear();
      string resCachePath = Path.Combine(Application.persistentDataPath, ResBuildConfig.ResCommitCachePath);
      if (Directory.Exists(resCachePath)) {
        ResBuildHelper.DeleteDirectory(resCachePath);
      }
      string resSheetPath = Path.Combine(Application.persistentDataPath, ResBuildConfig.ResSheetCachePath);
      if (Directory.Exists(resSheetPath)) {
        ResBuildHelper.DeleteDirectory(resSheetPath);
      }
      ret = true;
      ResBuildLog.Info("ResBuildProcesser.CleanCache Done");
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuildProcesser.CleanCache failed! ex:" + ex);
      ret = false;
    }
    return ret;
  }
  public static bool CleanBuildInRes()
  {
    string buildinPath = ResBuildHelper.GetFilePathAbs(ResBuildConfig.ResCommitBuildInPath);
    if (Directory.Exists(buildinPath)) {
      ResBuildHelper.DeleteDirectory(buildinPath);
    }
    ResBuildLog.Info("ResBuildProcesser.CleanBuildInRes Done");
    return true;
  }
  public static bool CleanOutputDir()
  {
    string outputPath = ResBuildHelper.GetFilePathAbs(ResBuildConfig.ResBuildConfigOutputPath);
    if (System.IO.Directory.Exists(outputPath)) {
      ResBuildHelper.DeleteDirectory(outputPath);
    }
    ResBuildLog.Info("ResProcess.CleanOutputDir Done");
    return true;
  }
  public static bool CleanAll()
  {
    if (ResDeployer.CleanCache()
        && ResDeployer.CleanBuildInRes()
        && ResDeployer.CleanOutputDir()) {
      return true;
    }
    return false;
  }
  public static bool CleanOutputResDir()
  {
    string outputPath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.GetPlatformPath(ResBuildConfig.BuildOptionTarget));
    if (System.IO.Directory.Exists(outputPath)) {
      ResBuildHelper.DeleteDirectory(outputPath);
    }
    ResBuildLog.Info("ResProcess.CleanOutputResDir Done");
    return true;
  }
  public static bool CleanOutputPlayerDir()
  {
    string outputPath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.GetPlatformPlayerPath(ResBuildConfig.BuildOptionTarget));
    if (Directory.Exists(outputPath)) {
      ResBuildHelper.DeleteDirectory(outputPath);
    }
    ResBuildLog.Info("ResProcess.CleanOutputPlayerDir Done");
    return true;
  }
  #endregion
}
