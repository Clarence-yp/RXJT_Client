using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class ResSheetGenerator
{
  public static bool BuildResSheetFile()
  {
    if (string.IsNullOrEmpty(ResBuildConfig.ResSheetFilePath)
      || string.IsNullOrEmpty(ResBuildConfig.ResSheetZipPath)) {
        ResBuildLog.Warn("ResSheetGenerator.BuildResSheetFile ResSheetFile config invalid:");
      return false;
    }
    try {
      string outputPath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.GetPlatformPath(ResBuildConfig.BuildOptionTarget));
      if (!System.IO.Directory.Exists(outputPath)) {
        System.IO.Directory.CreateDirectory(outputPath);
      }
      if (!System.IO.Directory.Exists(outputPath)) {
        ResBuildLog.Warn("ResSheetGenerator.BuildResSheetFile directory create failed Path:" + outputPath);
        return false;
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResSheetGenerator.BuildResSheetFile directory check failed! ex:" + ex);
      return false;
    }

    string[] tResSheetPattern =
          ResBuildConfig.ResSheetPattern.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
    if (tResSheetPattern == null) {
      ResBuildLog.Warn("ResSheetGenerator.BuildResSheetFile ResBuildAssetPattern invalid:" + ResBuildConfig.ResSheetPattern);
      return false;
    }

    List<UnityEngine.Object> targetAssets = new List<UnityEngine.Object>();
    List<string> targetAssetsNames = new List<string>();
    string[] sheetListContent = null;
    int totalNum = 0;

    string sheetListFile = Path.Combine("Assets/StreamingAssets", ResBuildConfig.ResSheetFilePath);
    string sheetListPathAbs = Path.Combine(Application.streamingAssetsPath, ResBuildConfig.ResSheetFilePath);
    try {
      UnityEngine.TextAsset asset = AssetDatabase.LoadAssetAtPath(sheetListFile, typeof(UnityEngine.TextAsset)) as TextAsset;
      targetAssets.Add(asset);
      targetAssetsNames.Add(ResBuildConfig.ResSheetFilePath);

      // Copy sheet file to *.bytes see:http://docs.unity3d.com/Manual/binarydata.html
      sheetListContent = File.ReadAllLines(sheetListPathAbs);
      totalNum = int.Parse(sheetListContent[0]);
      for (int sheetIndex = 1; sheetIndex <= totalNum; sheetIndex++) {
        string sheetFile = sheetListContent[sheetIndex];
        if (!string.IsNullOrEmpty(sheetFile.Trim()) && ResBuildHelper.CheckFilePatternEndWith(sheetFile, tResSheetPattern)) {
          string sheetPath = Path.Combine(Application.streamingAssetsPath, sheetFile);
          string sheetPathNew = Path.Combine(Application.streamingAssetsPath, sheetFile + ".bytes");
          ResBuildLog.Info("ResSheetGenerator.BuildResSheetFile build copy to *.bytes file:" + sheetPathNew);
          if (File.Exists(sheetPath)) {
            File.Copy(sheetPath, sheetPathNew);
          }
        } else {
          continue;
        }
      }
      AssetDatabase.Refresh();

      // Collet Sheet file to TextAsset
      for (int sheetIndex = 1; sheetIndex <= totalNum; sheetIndex++) {
        string sheetFile = sheetListContent[sheetIndex];
        if (!string.IsNullOrEmpty(sheetFile.Trim()) && ResBuildHelper.CheckFilePatternEndWith(sheetFile, tResSheetPattern)) {
          string sheetPathNew = Path.Combine("Assets/StreamingAssets", sheetFile + ".bytes");
          asset = AssetDatabase.LoadAssetAtPath(sheetPathNew, typeof(UnityEngine.TextAsset)) as TextAsset;
          if (asset == null) {
            ResBuildLog.Warn("ResSheetGenerator.BuildResSheetFile TextAsset miss:" + sheetFile);
            continue;
          } else {
            ResBuildLog.Info("ResSheetGenerator.BuildResSheetFile build Collet Sheet file:" + sheetPathNew);
          }
          targetAssets.Add(asset);
          targetAssetsNames.Add(sheetFile);
        } else {
          continue;
        }
      }

      UnityEngine.Object[] assets = targetAssets.ToArray();
      string[] assetNames = targetAssetsNames.ToArray();
      if (assets != null && assets.Length > 0) {
        BuildPipeline.BuildAssetBundleExplicitAssetNames(
          assets,
          assetNames,
          ResBuildHelper.FormatResSheetZipPath(),
          ResBuildConfig.BuildOptionRes,
          ResBuildConfig.BuildOptionTarget);
        ResBuildLog.Info("BuildResSheetFile Build AB:" + sheetListFile);
      } else {
        ResBuildLog.Warn("BuildResSheetFile Build AB Failed:" + sheetListFile);
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResSheetGenerator.BuildResSheetFile parse failedPath:" + sheetListFile + "ex:" + ex);
      return false;
    } finally {
      // Revert:Rename sheet file to .bytes see:http://docs.unity3d.com/Manual/binarydata.html
      if (sheetListContent != null && totalNum > 0) {
        for (int sheetIndex = 1; sheetIndex <= totalNum; sheetIndex++) {
          string sheetFile = sheetListContent[sheetIndex];
          if (!string.IsNullOrEmpty(sheetFile.Trim()) && ResBuildHelper.CheckFilePatternEndWith(sheetFile, tResSheetPattern)) {
            //string sheetPath = Path.Combine("Assets/StreamingAssets", sheetFile);
            string sheetPathNew = Path.Combine("Assets/StreamingAssets", sheetFile + ".bytes");
            if (File.Exists(sheetPathNew)) {
              File.Delete(sheetPathNew);
            }
          } else {
            continue;
          }
        }
      }
    }

    AssetDatabase.Refresh();
    ResBuildLog.Info("ResProcess.BuildResSheetFile Success");
    return true;
  }
}
