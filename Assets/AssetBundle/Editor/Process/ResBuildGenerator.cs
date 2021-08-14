using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

public class ResBuildGenerator
{
  class AssetBuildInfo
  {
    public int AssetId;
    public string AssetPath;
    public List<int> AssetDependency = new List<int>();
  }
  //NOTE:ID=0有特殊含义，供ResBuildGraph的Root节点使用 
  static int s_IdGen = ResBuildConfig.ResGraphRootId;

  public static bool GenBuildConfig()
  {
    string filePath = ResBuildHelper.FormatResBuildFilePath();
    s_IdGen = ResBuildConfig.ResGraphRootId;
    Dictionary<string, AssetBuildInfo> assetBuildDict = new Dictionary<string, AssetBuildInfo>();

    string[] tResBuildDirs = ResBuildConfig.ResBuildDirs.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
    if (tResBuildDirs == null) {
      ResBuildLog.Warn("GenBuildConfig ResBuildDirs error:" + ResBuildConfig.ResBuildDirs);
      return false;
    }
    foreach (string dir in tResBuildDirs) {
      SearchDependencyByDir(dir, assetBuildDict);
    }
    if (!OutputResBuildFile(filePath, assetBuildDict)) {
      ResBuildLog.Warn("ResBuildGenerator.OutputResBuildFile failed");
      return false;
    }
    ResBuildLog.Info("ResBuildGenerator.GenBuildConfig Success");
    return true;
  }
  public static bool LoadResConfig()
  {
    string filePath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.FormatResBuildFilePath());
    if (string.IsNullOrEmpty(filePath)) {
      ResBuildLog.Warn("ResBuildGenerator.LoadResConfig failed:");
      return false;
    }
    try {
      ResBuildProvider.Instance.Clear();
      byte[] buffer = File.ReadAllBytes(filePath);
      bool ret = ResBuildProvider.Instance.CollectDataFromDBC(buffer);
      if (!ret) {
        ResBuildLog.Warn("ResBuildGenerator.LoadResConfig CollectDataFromDBC failed! filePath:" + filePath);
        return false;
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuildGenerator.LoadResConfig failed! ex:" + ex);
      return false;
    }
    ResBuildLog.Info("ResBuildGenerator.LoadResConfig Success");
    return true;
  } 
  private static bool OutputResBuildFile(string filePath, Dictionary<string, AssetBuildInfo> assetBuildDict)
  {
    string fileContent = ResBuildConfig.ResBuildConfigHeader + "\n";
    foreach (AssetBuildInfo info in assetBuildDict.Values) {
      string abInfo = string.Format(ResBuildConfig.ResBuildConfigFormat + "\n",
        info.AssetId,
        info.AssetPath,
        FormatABNameByAssetPath(info),
        ResBuildHelper.FormatListContent(info.AssetDependency));
      fileContent += abInfo;
    }
    try {
      if (!ResBuildHelper.CheckFilePath(filePath)) {
        ResBuildLog.Warn("ResBuildGenerator.OutputResBuildFile file not exist.filepath:" + filePath);
        return false;
      }
      File.WriteAllText(filePath, fileContent);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuildGenerator.OutputResBuildFile file failed!" + ex);
      return false;
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("ResBuildGenerator.OutputResBuildFile Success!");
    return true;
  }
  private static void SearchDependencyByDir(string dir, Dictionary<string, AssetBuildInfo> assetBuildDict)
  {
    FileAttributes fileAttr = File.GetAttributes(dir);
    if ((fileAttr & FileAttributes.Directory) == FileAttributes.Directory) {
      DirectoryInfo source = new DirectoryInfo(dir);
      if (!source.Exists) {
        ResBuildLog.Warn("SearchDependencyByDir not exist dir:" + dir);
        return;
      }
      string[] tResBuildPattern = ResBuildConfig.ResBuildPattern.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
      if (tResBuildPattern == null) {
        ResBuildLog.Warn("SearchDependencyByDir ResBuildPattern error:" + ResBuildConfig.ResBuildPattern);
        return;
      }
      foreach (string pattern in tResBuildPattern) {
        FileInfo[] files = source.GetFiles(pattern, SearchOption.AllDirectories);
        foreach (FileInfo fInfo in files) {
          string assetPath = FormatAssetPath(TranslateFilePathToAssetPath(fInfo.FullName));
          if (string.IsNullOrEmpty(assetPath)) {
            continue;
          }
          SearchDependencyByFile(assetPath, assetBuildDict);
        }
      }
    } else {
      FileInfo fInfo = new FileInfo(dir);
      if (!fInfo.Exists) {
        ResBuildLog.Warn("SearchDependencyByDir not exist dir:" + dir);
        return;
      }
      string tResBuildPatternWithoutStar = ResBuildConfig.ResBuildPattern.Replace("*", "");
      string[] tResBuildPattern = tResBuildPatternWithoutStar.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
      if (tResBuildPattern == null) {
        ResBuildLog.Warn("SearchDependencyByDir ResBuildPattern error:" + ResBuildConfig.ResBuildPattern);
        return;
      }
      if (!ResBuildHelper.CheckFilePatternEndWith(dir, tResBuildPattern)) {
        ResBuildLog.Warn("SearchDependencyByDir CheckFilePatternRegex failed. file:" + dir);
        return;
      }
      string assetPath = FormatAssetPath(TranslateFilePathToAssetPath(fInfo.FullName));
      if (string.IsNullOrEmpty(assetPath)) {
        return;
      }
      SearchDependencyByFile(assetPath, assetBuildDict);
    }
  }
  private static AssetBuildInfo SearchDependencyByFile(string assetPath, Dictionary<string, AssetBuildInfo> assetBuildDict)
  {
    //NOTE:防止策划配置错误，一律转化为小写，by lixiaojiang
    assetPath = assetPath.ToLower();
    if (assetBuildDict.ContainsKey(assetPath)) {
      return assetBuildDict[assetPath];
    }
    AssetBuildInfo abInfo = new AssetBuildInfo();
    abInfo.AssetId = ++s_IdGen;
    abInfo.AssetPath = assetPath;
    string pattern = @"^[A-Za-z0-9\/._]+$";
    Regex regex = new Regex(pattern);
    if (!regex.IsMatch(assetPath)) {
      ResBuildLog.Warn("PathError:" + assetPath);
    }
    abInfo.AssetDependency.Clear();
    assetBuildDict.Add(abInfo.AssetPath, abInfo);
    string[] dependency = AssetDatabase.GetDependencies(new string[] { assetPath });

    List<string> dependencyList = new List<string>(dependency);
    dependencyList = dependencyList.ConvertAll<string>(low => {
      return low.ToLower();
    });
    dependency = dependencyList.ToArray();
    if (dependency != null && dependency.Length > 0) {
      foreach (string dFile in dependency) {
        string[] tResBuildAssetPattern =
          ResBuildConfig.ResBuildAssetPattern.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
        if (tResBuildAssetPattern != null && ResBuildHelper.CheckFilePatternEndWith(dFile, tResBuildAssetPattern)) {
          AssetBuildInfo dAbInfo = SearchDependencyByFile(FormatAssetPath(dFile), assetBuildDict);
          if (dAbInfo.AssetId != abInfo.AssetId) {
            abInfo.AssetDependency.Add(dAbInfo.AssetId);
          }
        }
      }
    }
    return abInfo;
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
  private static string FormatABNameByAssetPath(AssetBuildInfo info)
  {
    //NOTE:unity 只要assetbundle的文件名相同，就行认为是重复加载，因此不能使用资源的原始名字,
    //NOTE:如果使用ID，则会造成同一个资源的ID可能不同，造成多余的下载。
    //NOTE:因此，使用guid代替
    string dirPath = Path.GetDirectoryName(info.AssetPath);
    //NOTE:用于确保资源路径格式正确
    dirPath = dirPath.Replace(" ", "_");
    dirPath = dirPath.Replace("-", "_");
    dirPath = dirPath.Replace("(", "_");
    dirPath = dirPath.Replace(")", "_");

    string abName = AssetDatabase.AssetPathToGUID(info.AssetPath);
    return string.Format("{0}/{1}", dirPath, abName);
  }
  private static string FormatAssetPath(string assetPath)
  {
    return assetPath.Replace("\\", "/");
  }
}
