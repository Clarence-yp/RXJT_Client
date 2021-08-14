using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class ResTypeFinder
{
  static Dictionary<string, List<string>> s_AssetTypeDict = new Dictionary<string, List<string>>();
  static string s_TargetDir = string.Empty;
  static string s_IncludePattern = string.Empty;
  static string s_ExcludePattern = string.Empty;
  public static bool FinderTypeData()
  {
    ResBuildConfig.Load();

    s_TargetDir = ResBuildHelper.GetFilePathAbs(ResBuildConfig.ResFinderTypeTargetDir);
    s_IncludePattern = ResBuildConfig.ResFinderTypeIncludePattern;
    s_ExcludePattern = ResBuildConfig.ResFinderTypeExcludePattern;

    if (!CollectTypeData()) {
      ResBuildLog.Warn("ResTypeFinder.FinderTypeData CollectTypeData failed!");
      return false;
    }
    string TypeDataOutputFile = ResBuildHelper.FormatResFinderTypeFilePath();
    if (!OutputResFindTypeFile(TypeDataOutputFile)) {
      ResBuildLog.Warn("ResTypeFinder.FinderTypeData OutputResTypeFile failed!");
      return false;
    }
    ResBuildLog.Info("ResTypeFinder.FinderTypeData Success filePath:" + TypeDataOutputFile);
    return true;
  }

  private static bool CollectTypeData()
  {
    s_AssetTypeDict.Clear();

    DirectoryInfo source = new DirectoryInfo(s_TargetDir);
    if (!source.Exists) {
      ResBuildLog.Warn("ResTypeFinder not exist dir:" + s_TargetDir);
      return false;
    }

    string[] tExcludePattern = s_ExcludePattern.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
    FileInfo[] files = source.GetFiles(s_IncludePattern, SearchOption.AllDirectories);
    foreach (FileInfo fInfo in files) {
      if (tExcludePattern != null
        && tExcludePattern.Length > 0) {
        if (ResBuildHelper.CheckFilePatternEndWith(fInfo.FullName, tExcludePattern)) {
          continue;
        }
      }
      string assetExtention = fInfo.Extension;
      string assetPath = FormatAssetPath(TranslateFilePathToAssetPath(fInfo.FullName));

      if (s_AssetTypeDict.ContainsKey(assetExtention)) {
        List<string> matList = s_AssetTypeDict[assetExtention];
        matList.Add(assetPath);
      } else {
        List<string> matList = new List<string>();
        s_AssetTypeDict.Add(assetExtention, matList);
        matList.Add(assetPath);
      }
    }

    ResBuildLog.Info("ResTypeFinder.CollectTypeData Success");
    return true;
  }
  private static bool OutputResFindTypeFile(string filePath)
  {
    string fileContent = "TypeList:" + "\n";
    foreach (string typeName in s_AssetTypeDict.Keys) {
      string abInfo = "	" + typeName + "\n";
      fileContent += abInfo;
    }
    fileContent += "TypeAssetList:" + "\n";
    foreach (string typeName in s_AssetTypeDict.Keys) {
      string abInfo = "	" + typeName + "\n";
      List<string> assetList = s_AssetTypeDict[typeName];
      if (assetList != null && assetList.Count > 0) {
        foreach (string matFile in assetList) {
          abInfo += "		" + matFile + "\n";
        }
      }
      fileContent += abInfo;
    }

    try {
      if (!ResBuildHelper.CheckFilePath(filePath)) {
        ResBuildLog.Warn("ResTypeFinder.OutputResFindTypeFile file not exist! filePath:" + filePath);
        return false;
      }
      File.WriteAllText(filePath, fileContent);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResTypeFinder.OutputResFindTypeFile failed! ex:" + ex);
      return false;
    }
    ResBuildLog.Info("ResTypeFinder.OutputResFindTypeFile Success");
    return true;
  }
  private static string TranslateFilePathToAssetPath(string filePath)
  {
    if (string.IsNullOrEmpty(filePath)) {
      ResBuildLog.Warn("TranslateFilePathToAssetPath filePath invalid filePath:" + filePath);
      return string.Empty;
    }
    filePath = filePath.Trim();
    int assetsIndex = filePath.IndexOf("assets");
    if (assetsIndex < 0) {
      ResBuildLog.Warn("TranslateFilePathToAssetPath filePath invalid filePath:" + filePath);
      return string.Empty;
    }
    return filePath.Substring(assetsIndex);
  }
  private static string FormatAssetPath(string assetPath)
  {
    return assetPath.Replace("\\", "/");
  }
}
