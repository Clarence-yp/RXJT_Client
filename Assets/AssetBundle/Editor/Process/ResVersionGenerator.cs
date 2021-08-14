using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;

public class ResVersionGenerator
{
  public static bool BuildResVersionFiles()
  {
    Dictionary<int, ResBuildData> buildDict = ResBuildProvider.Instance.GetDict();
    if (buildDict == null || buildDict.Count == 0) {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersionFiles ResBuildProvider is null or empty.");
      return false;
    }
    if (!GenerateProperty()) {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersionFiles GenerateProperty failed.");
      return false;
    }
    if (!BuildResVersion()) {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersion failed!");
      return false;
    }
    if (!BuildClientResVersion()) {
      ResBuildLog.Warn("ResVersionGenerator.BuildClientResVersion failed!");
      return false;
    }
    if (!BuildIncrementalResVersion()) {
      ResBuildLog.Warn("ResVersionGenerator.BuildIncrementalResVersion failed!");
      return false;
    }
    ResBuildLog.Info("ResVersionGenerator.BuildResVersionFiles Success!");
    return true;
  }
  #region ResVersion
  private static bool BuildResVersion()
  {
    string filePath = ResBuildHelper.FormatResListFilePath();
    if (!OutputResVersionFile(filePath)) {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersion OutputResVersionFile failed.");
      return false;
    }
    if (!BuildResVersionFile()) {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersion OutputResVersionFile failed.");
      return false;
    }
    ResBuildLog.Info("ResVersionGenerator.BuildResVersion Success");
    return true;
  }
  private static bool OutputResVersionFile(string filePath)
  {
    string fileContent = ResBuildConfig.ResVersionHeader + "\n";
    foreach (ResBuildData config in ResBuildProvider.Instance.GetDict().Values) {
      if (config != null) {
        string abInfo = string.Format(ResBuildConfig.ResVersionFormat + "\n",
          config.GetId(),
          ResBuildHelper.FormatResNameFromConfig(config),
          config.m_Resources,
          ResBuildHelper.FormatListContent(config.m_Depends),
          config.m_MD5,
          config.m_Chapter,
          config.m_Size);
        fileContent += abInfo;
      }
    }
    try {
      if (!ResBuildHelper.CheckFilePath(filePath)) {
        ResBuildLog.Warn("ResVersionGenerator.OutputResVersionFile file not exist.");
        return false;
      }
      File.WriteAllText(filePath, fileContent);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResVersionGenerator.OutputResVersionFile failed!" + ex);
      return false;
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("ResVersionGenerator.OutputResVersionFile Success");
    return true;
  }
  private static bool BuildResVersionFile()
  {
    string outputPath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.GetPlatformPath(ResBuildConfig.BuildOptionTarget));
    try {
      if (!System.IO.Directory.Exists(outputPath)) {
        System.IO.Directory.CreateDirectory(outputPath);
      }
      if (!System.IO.Directory.Exists(outputPath)) {
        ResBuildLog.Warn("ResVersionGenerator.BuildResVersionFile directory create failed Path:" + outputPath);
        return false;
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResVersionGenerator.RecordAssetList directory check failed! ex:" + ex);
      return false;
    }
    UnityEngine.TextAsset resVersionObj = AssetDatabase.LoadAssetAtPath(
      ResBuildHelper.FormatResListFilePath(), typeof(UnityEngine.TextAsset)) as TextAsset;
    if (resVersionObj != null) {
      UnityEngine.Object[] assets = { resVersionObj };
      string[] assetNames = { ResBuildConfig.ResVersionFilePath };
      BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames,
        ResBuildHelper.FormatResVersionZipPath(),
        ResBuildConfig.BuildOptionRes,
        ResBuildConfig.BuildOptionTarget);
    } else {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersionFile failed:");
      return false;
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("ResVersionGenerator.BuildResVersionFile Success");
    return true;
  }
  #endregion
  #region ClientResVersion
  private static bool BuildClientResVersion()
  {
    string filePath = ResBuildHelper.FormatClientResListFilePath();
    if (!OutputClientResVersionFile(filePath)) {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersion OutputResVersionFile failed.");
      return false;
    }
    ResBuildLog.Info("ResVersionGenerator.BuildClientResVersion Success");
    return true;
  }
  private static bool OutputClientResVersionFile(string filePath)
  {
    string fileContent = ResBuildConfig.ResVersionClientHeader + "\n";
    foreach (ResBuildData config in ResBuildProvider.Instance.GetDict().Values) {
      if (config != null && ResBuildHelper.IsResBuildIn(config.GetId(), config.m_Chapter)) {
        string abInfo = string.Format(ResBuildConfig.ResVersionClientFormat + "\n",
          ResBuildHelper.FormatResNameFromConfig(config),
          config.m_MD5,
          true);
        fileContent += abInfo;
      }
    }
    try {
      if (!ResBuildHelper.CheckFilePath(filePath)) {
        ResBuildLog.Warn("ResVersionGenerator.OutputClientResVersionFile file not exist.");
        return false;
      }
      File.WriteAllText(filePath, fileContent);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResVersionGenerator.OutputClientResVersionFile failed!" + ex);
      return false;
    }

    AssetDatabase.Refresh();
    ResBuildLog.Info("ResVersionGenerator.OutputClientResVersionFile Success");
    return true;
  }
  #endregion
  #region IncrementalResVersion
  private static bool BuildIncrementalResVersion()
  {
    if (!ResBuildConfig.ResVersionIncrementalEnable) {
      ResBuildLog.Info("ResVersionGenerator.BuildResVersion BuildIncrementalResVersionEnable Off.");
      return true;
    }

    string filePath = ResBuildHelper.FormatIncrementalResListFilePath();
    if (!OutputIncrementalResVersionFile(filePath)) {
      ResBuildLog.Warn("ResVersionGenerator.BuildResVersion OutputIncrementalResVersionFile failed.");
      return false;
    }
    ResBuildLog.Info("ResVersionGenerator.BuildIncrementalResVersion Success");
    return true;
  }
  private static bool OutputIncrementalResVersionFile(string filePath)
  {
    string fileContent = ResBuildConfig.ResVersionIncrementalHeader + "\n";
    //foreach (ResBuildData config in ResBuildProvider.Instance.GetDict().Values) {
    //  string curResFilePath = ResBuildHelper.GetFilePathAbs(config.m_Resources);
    //  string curResMD5 = ResBuildHelper.GetFileMd5(curResFilePath);
    //  IncrementalResVersionProvider.Instance.SetData(config.m_Resources, curResMD5, DateTime.Now.ToString());
    //}
    foreach (IncrementalResVersionData config in IncrementalResVersionProvider.Instance.GetArray().Values) {
      if (config != null) {
        string abInfo = string.Format(ResBuildConfig.ResVersionIncrementalFormat + "\n",
          config.m_Name,
          config.m_MD5,
          config.m_BuildTime);
        fileContent += abInfo;
      }
    }
    try {
      if (!ResBuildHelper.CheckFilePath(filePath)) {
        ResBuildLog.Warn("ResVersionGenerator.OutputIncrementalResVersionFile file not exist.");
        return false;
      }
      File.WriteAllText(filePath, fileContent);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResVersionGenerator.OutputIncrementalResVersionFile failed!" + ex);
      return false;
    }

    AssetDatabase.Refresh();
    ResBuildLog.Info("ResVersionGenerator.OutputIncrementalResVersionFile Success");
    return true;
  }
  #endregion
  private static bool GenerateProperty()
  {
    foreach (ResBuildData config in ResBuildProvider.Instance.GetDict().Values) {
      if (config != null) {
        config.Reset();
      }
    }
    foreach (ResBuildData config in ResBuildProvider.Instance.GetDict().Values) {
      if (config != null) {
        string pathName = ResBuildHelper.FormatResPathFromConfig(config);
        string filePath = ResBuildHelper.GetFilePathAbs(pathName);
        if (!File.Exists(filePath)) {
          ResBuildLog.Warn("ResVersionGenerator.GenerateProperty file not exist.filePath:" + filePath);
        }

        // Chapter
        int curChapter = ResBuildHelper.GetResChapter(config.GetId());
        if (curChapter != -1 && (config.m_Chapter == -1 || config.m_Chapter > curChapter)) {
          config.m_Chapter = curChapter;
          if (config.m_Depends != null && config.m_Depends.Count > 0) {
            foreach (int dependAssetId in config.m_Depends) {
              ResBuildData childConfig = ResBuildProvider.Instance.GetDataById(dependAssetId);
              if (childConfig != null) {
                if (childConfig.m_Chapter == -1) {
                  childConfig.m_Chapter = curChapter;
                } else {
                  childConfig.m_Chapter = Mathf.Min(childConfig.m_Chapter, curChapter);
                }
              }
            }
          }
        }

        // MD5
        config.m_MD5 = ResBuildHelper.GetFileMd5(filePath);
        // Size
        config.m_Size = ResBuildHelper.GetFileSize(filePath);
      }
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("ResVersionGenerator.GenerateProperty Success");
    return true;
  }
  public static bool LoadResVersion()
  {
    string filePath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.FormatResListFilePath());
    if (string.IsNullOrEmpty(filePath)) {
      ResBuildLog.Warn("ResVersionGenerator.LoadResVersion failed:");
      return false;
    }
    try {
      ResVersionProvider.Instance.Clear();
      byte[] buffer = File.ReadAllBytes(filePath);
      bool ret = ResVersionProvider.Instance.CollectDataFromDBC(buffer);
      if (!ret) {
        ResBuildLog.Warn("ResVersionGenerator.LoadResVersion CollectDataFromDBC failed! filePath:" + filePath);
        return false;
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResVersionGenerator.LoadResVersion failed! ex:" + ex);
      return false;
    }
    ResBuildLog.Info("ResVersionGenerator.LoadResVersion Success");
    return true;
  }
}
