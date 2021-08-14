using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class ResShaderFinder
{
  static Dictionary<string, List<string>> s_AssetShaderDict = new Dictionary<string, List<string>>();
  public static bool FinderShaderData()
  {
    ResBuildConfig.Load();
    bool ret = ResBuildGenerator.LoadResConfig();
    if (!ret) {
      ResBuildLog.Warn("ResShaderFinder.FinderShaderData LoadResConfig failed！");
      return false;
    }
    if (!CollectShaderData()) {
      ResBuildLog.Warn("ResShaderFinder.FinderShaderData CollectShaderData failed!");
      return false;
    }
    string shaderDataOutputFile = ResBuildHelper.FormatResFinderShaderFilePath();
    if (!OutputResFinderShaderFile(shaderDataOutputFile)) {
      ResBuildLog.Warn("ResShaderFinder.FinderShaderData OutputResBuildExtendFile failed!");
      return false;
    }
    ResBuildLog.Info("ResShaderFinder.FinderShaderData Success filePath:" + shaderDataOutputFile);
    return true;
  }

  private static bool CollectShaderData()
  {
    s_AssetShaderDict.Clear();

    foreach (ResBuildData config in ResBuildProvider.Instance.GetDict().Values) {
      if (config != null && config.m_Resources.EndsWith(".mat")) {
        string filePath = ResBuildHelper.GetFilePathAbs(config.m_Resources);
        if (!File.Exists(filePath)) {
          ResBuildLog.Warn("ResShaderFinder.CollectShaderData file not exist.filePath:" + filePath);
          return false;
        }

        UnityEngine.Material materialObj = AssetDatabase.LoadAssetAtPath(config.m_Resources, typeof(Material)) as Material;
        if (materialObj != null) {
          if (materialObj.shader != null) {
            string shaderName = materialObj.shader.name;
            if (s_AssetShaderDict.ContainsKey(shaderName)) {
              List<string> matList = s_AssetShaderDict[shaderName];
              matList.Add(config.m_Resources);
            } else {
              List<string> matList = new List<string>();
              s_AssetShaderDict.Add(shaderName, matList);
              matList.Add(config.m_Resources);
            }
          } else {
            ResBuildLog.Warn("ResShaderFinder.CollectShaderData shader miss. material:" + config.m_Resources);
          }
        } else {
          ResBuildLog.Warn("ResShaderFinder.CollectShaderData material load failed. material:" + config.m_Resources);
        }
      }
    }
    ResBuildLog.Info("ResShaderFinder.CollectShaderData Success");
    return true;
  }
  private static bool OutputResFinderShaderFile(string filePath)
  {
    string fileContent = "ShaderList:" + "\n";
    foreach (string shaderName in s_AssetShaderDict.Keys) {
      string abInfo = "	" + shaderName + "\n";
      fileContent += abInfo;
    }
    fileContent += "ShaderMatList:" + "\n";
    foreach (string shaderName in s_AssetShaderDict.Keys) {
      string abInfo = "	" + shaderName + "\n";
      List<string> matList = s_AssetShaderDict[shaderName];
      if (matList != null && matList.Count > 0) {
        foreach (string matFile in matList) {
          abInfo += "		" + matFile + "\n";
        }
      }
      fileContent += abInfo;
    }

    try {
      if (!ResBuildHelper.CheckFilePath(filePath)) {
        ResBuildLog.Warn("ResShaderFinder.OutputResFinderShaderFile file not exist! filePath:" + filePath);
        return false;
      }
      File.WriteAllText(filePath, fileContent);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResShaderFinder.OutputResFinderShaderFile failed! ex:" + ex);
      return false;
    }
    ResBuildLog.Info("ResShaderFinder.OutputResFinderShaderFile Success");
    return true;
  }
}
