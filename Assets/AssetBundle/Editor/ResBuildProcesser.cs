using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class ResBuildProcesser
{
  public static bool BuildAllResources(int buildTarget = -1)
  {
    bool ret = true;
    try {
      LogBuildStatus("ResBuildConfig.Load() Start");
      if (!ResBuildConfig.Load()) {
        ResBuildLog.Warn("BuildAllResources.LoadConfig failed!");
        return false;
      }

      LogBuildStatus("ResBuildConfig.SetBuildTargetPlatform Start");
      if ((buildTarget >= 0 && !ResBuildConfig.SetBuildTargetPlatform((BuildTarget)buildTarget))) {
        ResBuildLog.Warn("BuildAllResources.SetBuildTargetPlatform failed!");
        return false;
      }

      //LogBuildStatus("ResBuildConfig.SetBuildTargetDir Start");
      //if (!string.IsNullOrEmpty(buildTargetDir) && !ResBuildConfig.SetBuildTargetDir(buildTargetDir)) {
      //  ResBuildLog.Warn("BuildAllResources.SetBuildTargetDir failed!");
      //  return false;
      //}

      LogBuildStatus("ResBuildLog.SetFileLog Start");
      if (!ResBuildLog.SetFileLog(true, ResBuildHelper.FormatResBuildLogFilePath())) {
        ResBuildLog.Warn("BuildAllResources.RedirectLog failed!");
        return false;
      }

      LogBuildStatus("ResBuildConfig.Check Start");
      if (!ResBuildConfig.Check()) {
        ResBuildLog.Warn("BuildAllResources.CheckConfig failed!");
        return false;
      }

      LogBuildStatus("ResDeployer.CleanCache Start");
      if (!ResDeployer.CleanCache()) {
        ResBuildLog.Warn("BuildAllResources.CleanCache failed!");
        return false;
      }

      LogBuildStatus("ResDeployer.CleanOutputResDir Start");
      if (ResBuildConfig.ResVersionIncrementalEnable) {
        ResBuildLog.Info("BuildAllResources.ResVersionIncrementalEnable On Skip CleanOutputResDir!");
      } else {
        if (!ResDeployer.CleanOutputResDir()) {
          ResBuildLog.Warn("BuildAllResources.CleanOutputRes failed!");
          return false;
        }
      }

      LogBuildStatus("ResBuildGenerator.GenBuildConfig Start");
      if (!ResBuildGenerator.GenBuildConfig()) {
        ResBuildLog.Warn("BuildAllResources.GenResConfig failed!");
        return false;
      }

      LogBuildStatus("ResBuildGenerator.LoadResConfig Start");
      if (!ResBuildGenerator.LoadResConfig()) {
        ResBuildLog.Warn("BuildAllResources.LoadResConfig failed!");
        return false;
      }

      LogBuildStatus("ResCacheGenerator.BuildResCacheFile Start");
      if (!ResCacheGenerator.BuildResCacheFile()) {
        ResBuildLog.Warn("BuildAllResources.BuildResCacheFile failed!");
        return false;
      }

      LogBuildStatus("ResBuilder.BuildAllResources Start");
      if (!ResBuilder.BuildAllResources()) {
        ResBuildLog.Warn("BuildAllResources.BuildAllResource failed!");
        return false;
      }

      LogBuildStatus("ResVersionGenerator.BuildResVersionFiles Start");
      if (!ResVersionGenerator.BuildResVersionFiles()) {
        ResBuildLog.Warn("BuildAllResources.BuildResVersionFiles failed!");
        return false;
      }

      LogBuildStatus("ResSheetGenerator.BuildResSheetFile Start");
      if (!ResSheetGenerator.BuildResSheetFile()) {
        ResBuildLog.Warn("BuildAllResources.BuildResSheetFile failed!");
        return false;
      }

      LogBuildStatus("VersionGenerator.GenVersionFile Start");
      if (!VersionGenerator.GenVersionFile()) {
        ResBuildLog.Warn("BuildAllResources.GenVersionFile failed!");
        return false;
      }
      ResBuildLog.Info("ResBuildProcesser.BuildAllResources Success");
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuildProcesser.BuildAllResources failed! ex:" + ex);
      ret = false;
    } finally {
      ResBuildLog.ResetLog();
    }
    return ret;
  }
  //public static bool BuildSelectResources(int buildTarget = -1, string buildTargetDir = "")
  //{
  //  bool ret = true;
  //  try {
  //    LogBuildStatus("ResBuildConfig.Load() Start");
  //    if (!ResBuildConfig.Load()) {
  //      ResBuildLog.Warn("BuildSelectResources.LoadConfig failed!");
  //      return false;
  //    }

  //    LogBuildStatus("ResBuildConfig.SetBuildTargetPlatform Start");
  //    if ((buildTarget >= 0 && !ResBuildConfig.SetBuildTargetPlatform((BuildTarget)buildTarget))) {
  //      ResBuildLog.Warn("BuildSelectResources.SetBuildTargetPlatform failed!");
  //      return false;
  //    }

  //    LogBuildStatus("ResBuildConfig.SetBuildTargetDir Start");
  //    if (!string.IsNullOrEmpty(buildTargetDir) && !ResBuildConfig.SetBuildTargetDir(buildTargetDir)) {
  //      ResBuildLog.Warn("BuildSelectResources.SetBuildTargetDir failed!");
  //      return false;
  //    }

  //    LogBuildStatus("ResBuildLog.SetFileLog Start");
  //    if (!ResBuildLog.SetFileLog(true, ResBuildHelper.FormatResBuildLogFilePath())) {
  //      ResBuildLog.Warn("BuildSelectResources.RedirectLog failed!");
  //      return false;
  //    }

  //    LogBuildStatus("ResBuildConfig.Check Start");
  //    if (!ResBuildConfig.Check()) {
  //      ResBuildLog.Warn("BuildSelectResources.CheckConfig failed!");
  //      return false;
  //    }

  //    LogBuildStatus("ResBuildGenerator.GenBuildConfig Start");
  //    if (!ResBuildGenerator.GenBuildConfig()) {
  //      ResBuildLog.Warn("BuildSelectResources.GenResConfig failed!");
  //      return false;
  //    }

  //    LogBuildStatus("ResBuildGenerator.LoadResConfig Start");
  //    if (!ResBuildGenerator.LoadResConfig()) {
  //      ResBuildLog.Warn("BuildSelectResources.LoadResConfig failed!");
  //      return false;
  //    }

  //    LogBuildStatus("ResBuilder.BuildSelectResources Start");
  //    if (!ResBuilder.BuildAllResources()) {
  //      ResBuildLog.Warn("BuildSelectResources.BuildAllResource failed!");
  //      return false;
  //    }

  //    ResBuildLog.Info("ResBuildProcesser.BuildSelectResources Success");
  //  } catch (System.Exception ex) {
  //    ResBuildLog.Warn("ResBuildProcesser.BuildSelectResources failed! ex:" + ex);
  //    ret = false;
  //  } finally {
  //    ResBuildLog.ResetLog();
  //  }
  //  return ret;
  //}
  public static string GetSelectedResources()
  {
    string targetDir = string.Empty;
    UnityEngine.Object[] selectedObj = Selection.objects;
    if (selectedObj == null || selectedObj.Length == 0) {
      EditorUtility.DisplayDialog(
        "Confirm",
        "BuildSelectedResourceAsConfig Selected nothing!",
        "OK");
      return targetDir;
    }

    StringBuilder sb = new StringBuilder();
    foreach (UnityEngine.Object selObj in selectedObj) {
      string selObjPath = AssetDatabase.GetAssetPath(selObj.GetInstanceID());
      if (!string.IsNullOrEmpty(selObjPath)) {
        if (System.IO.Directory.Exists(selObjPath)) {
          sb.Append(selObjPath + ";");
        } else {
          sb.Append(selObjPath + ";");
        }
      }
    }
    targetDir = sb.ToString().Trim().ToLower();
    return targetDir;
  }
  private static void LogBuildStatus(string step)
  {
    ResBuildLog.Info("*********************************************");
    ResBuildLog.Info(step);
  }
}