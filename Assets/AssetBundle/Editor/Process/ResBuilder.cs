using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;

public class ResBuilder
{
  public static bool BuildAllResources()
  {
    ResBuilder tResBuilder = new ResBuilder();
    if (!tResBuilder.ExecuteBuild()) {
      ResBuildLog.Warn("ResBuilder.BuildAllResources failed:");
      return false;
    }
    ResBuildLog.Info("ResBuilder.BuildAllResources Success");
    return true;
  }

  private ResBuildGraph m_GlobalResGraph = null;
  private ResBuildGraph m_CurResGraph = null;
  private List<List<int>> m_PushedResList = new List<List<int>>();

  public ResBuilder()
  {
    Reset();
  }
  public void Reset()
  {
    m_GlobalResGraph = null;
    m_CurResGraph = null;
    m_PushedResList.Clear();
  }
  public bool ExecuteBuild()
  {
    if (!CheckEnvironment()) {
      ResBuildLog.Warn("ResBuilder.CheckEnvironment failed:");
    }
    if (!BuildGlobalGraph()) {
      ResBuildLog.Warn("ResBuilder.BuildGraph failed:");
    }
    return BuildResources();
  }
  #region Build Steps
  private bool CheckEnvironment()
  {
    try {
      string outputPath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.GetPlatformPath(ResBuildConfig.BuildOptionTarget));
      if (!System.IO.Directory.Exists(outputPath)) {
        System.IO.Directory.CreateDirectory(outputPath);
        ResBuildLog.Info("ResBuilder.CheckEnvironment Create Directory " + outputPath);
      }
      if (!System.IO.Directory.Exists(outputPath)) {
        ResBuildLog.Warn("ResBuilder.CheckEnvironment directory create failed Path:" + outputPath);
        return false;
      }

      // Enalbe Incremental build
      string incrementalResVersionPath = ResBuildHelper.FormatIncrementalResListFilePath();
      if (ResBuildConfig.ResVersionIncrementalEnable) {
        if (!System.IO.File.Exists(incrementalResVersionPath)) {
          try {
            string fileContent = ResBuildConfig.ResVersionIncrementalHeader + "\n";
            if (!ResBuildHelper.CheckFilePath(incrementalResVersionPath)) {
              ResBuildLog.Warn("ResBuilder.CheckEnvironment check file failed!.");
              return false;
            }
            File.WriteAllText(fileContent, fileContent);
          } catch (System.Exception ex) {
            ResBuildLog.Warn("ResVersionGenerator.CheckEnvironment write IncrementalResVersionFile failed!" + ex);
            return false;
          }
        }
        IncrementalResVersionProvider.Instance.Clear();
        byte[] buffer = File.ReadAllBytes(incrementalResVersionPath);
        bool ret = IncrementalResVersionProvider.Instance.CollectDataFromDBC(buffer);
        if (!ret) {
          ResBuildLog.Warn("ResVersionGenerator.CheckEnvironment CollectDataFromDBC failed! filePath:" + incrementalResVersionPath);
          return false;
        }
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuilder.CheckEnvironment directory check failed!ex:" + ex);
      return false;
    }
    ResBuildLog.Info("ResBuilder.CheckEnvironment Success");
    return true;
  }
  private bool BuildGlobalGraph()
  {
    m_GlobalResGraph = new ResBuildGraph();
    bool ret = m_GlobalResGraph.Build(ResBuildProvider.Instance.GetDict());
    if (!ret) {
      ResBuildLog.Warn("ResBuildProcessor.BuildResGraph failed:");
      return false;
    }
    ResBuildLog.Info("ResBuilder.BuildGraph Success");
    return true;
  }
  private bool BuildResources()
  {
    OnBuildAllResourcesBefore();
    foreach (int id in m_GlobalResGraph.RootNode.m_ChildDepends) {
      m_PushedResList.Clear();
      ResBuildGraphNode node = TryFindGlobalResNode(id);
      if (node != null) {
        OnBuildNodeBefore(node);
        m_CurResGraph = m_GlobalResGraph.ExtractSubGraph(node);
        if (m_CurResGraph == null) {
          ResBuildLog.Warn("ResBuilder.ExtractNodeGraph failed.nodeId:" + node.GetId());
          continue;
        }
        if (ResBuildConfig.ResVersionIncrementalEnable && !m_CurResGraph.CheckReBuildByIncremental()) {
          ResBuildLog.Info("ResBuilder.BuildResources CheckReBuildByIncremental Res Exist, Skip ResId:" + node.GetId());
          OnBuildNodeSkipAfter(node);
          continue;
        }
        if (!BuildCurResGraph()) {
          ResBuildLog.Warn("ResBuilder.BuildResRecursively failed.nodeId:" + node.GetId());
        }
        OnBuildNodeAfter(node);
      } else {
        ResBuildLog.Warn("ResBuilder.BuildResRecursively failed. node null. nodeId:" + id);
      }
    }
    OnBuildAllResourcesAfter();
    return true;
  }
  #endregion
  private bool BuildCurResGraph()
  {
    List<int> toBuildNode = new List<int>();
    int tPushIndex = 0;
    while (true) {
      toBuildNode.Clear();
      foreach (ResBuildGraphNode curNode in m_CurResGraph.NodeDict.Values) {
        if (IsCurReadyToBuild(curNode)) {
          toBuildNode.Add(curNode.GetId());
        }
      }
      if (toBuildNode.Count > 0) {
        tPushIndex++;
        List<int> toBuildNodeInPushedList = new List<int>();
        toBuildNodeInPushedList.AddRange(toBuildNode);
        m_PushedResList.Add(toBuildNodeInPushedList);

        ResBuildLog.Info("BuildResRecursively PushAssetDependencies! Layer:" + tPushIndex
          + " Ids:" + ResBuildHelper.FormatListContent(toBuildNode));
        BuildPipeline.PushAssetDependencies();
        foreach (int nodeId in toBuildNode) {
          ResBuildGraphNode parentNode = TryFindCurResNode(nodeId);
          if (!BuildResSingle(parentNode)) {
            ResBuildLog.Warn("BuildResRecursively BuildResSingle failed! Id:" + parentNode);
          }
        }
      } else {
        if (m_PushedResList.Count > 0) {
          for (int index = m_PushedResList.Count - 1; index >= 0; index--) {
            List<int> toPopNodeList = m_PushedResList[index];
            BuildPipeline.PopAssetDependencies();
            ResBuildLog.Info("BuildResRecursively PopAssetDependencies! Layer:" + (index + 1)
              + " Ids:" + ResBuildHelper.FormatListContent(toPopNodeList));
          }
          m_PushedResList.Clear();
        }
        break;
      }
    }
    return true;
  }
  private bool BuildResSingle(ResBuildGraphNode node)
  {
    ResBuildData config = TryFindResBuildConfig(node.GetId());
    if (config == null) {
      SetNodeStatus(node.GetId(), ResBuildGraphNodeStatus.Invalid);
      ResBuildLog.Warn("BuildResRecursively ResBuildData miss! Id" + node.GetId());
      return false;
    }

    if (IsCurReadyToBuild(node)) {
      try {
        string pathName = ResBuildHelper.FormatResPathFromConfig(config);
        string pDir = Path.GetDirectoryName(pathName);
        if (!Directory.Exists(pDir)) {
          Directory.CreateDirectory(pDir);
        }

        if (ResBuildHelper.IsSceneRes(config)) {
          if (!string.IsNullOrEmpty(config.m_Resources)) {
            string[] levels = { config.m_Resources };
            config.m_AssetNameList.AddRange(levels);
            BuildPipeline.BuildStreamedSceneAssetBundle(levels, pathName, ResBuildConfig.BuildOptionTarget);
            if (ResBuildConfig.ResVersionIncrementalEnable) {
              string curResFilePath = ResBuildHelper.GetFilePathAbs(config.m_Resources);
              string curResMD5 = ResBuildHelper.GetFileMd5(curResFilePath);
              IncrementalResVersionProvider.Instance.SetData(config.m_Resources, curResMD5, DateTime.Now.ToString());
            }
            ResBuildLog.Info("ResBuilder CurPushed:" + GetCurResPushedInfo());
            ResBuildLog.Info("ResBuilder Build AB:" + config.ToString());
          } else {
            throw new Exception("BuildResRecursively ResBuildData failed!");
          }
        } else if (ResBuildHelper.IsFileRes(config)) {
          UnityEngine.Object targetAssets = AssetDatabase.LoadMainAssetAtPath(config.m_Resources);
          UnityEngine.Object[] assets = { targetAssets };
          string[] assetNames = { config.m_Resources };
          if (targetAssets != null) {
            config.m_AssetNameList.AddRange(assetNames);
            BuildPipeline.BuildAssetBundleExplicitAssetNames(
              assets,
              assetNames,
              pathName,
              ResBuildConfig.BuildOptionRes,
              ResBuildConfig.BuildOptionTarget);
            if (ResBuildConfig.ResVersionIncrementalEnable) {
              string curResFilePath = ResBuildHelper.GetFilePathAbs(config.m_Resources);
              string curResMD5 = ResBuildHelper.GetFileMd5(curResFilePath);
              IncrementalResVersionProvider.Instance.SetData(config.m_Resources, curResMD5, DateTime.Now.ToString());
            }
            ResBuildLog.Info("ResBuilder CurPushed:" + GetCurResPushedInfo());
            ResBuildLog.Info("ResBuilder Build AB:" + config.ToString());
          } else {
            throw new Exception("BuildResRecursively ResBuildData failed!");
          }
        } else if (ResBuildHelper.IsDirectoryRes(config)) {
          List<UnityEngine.Object> targetAssets = new List<UnityEngine.Object>();
          List<string> targetAssetsNames = new List<string>();
          DirectoryInfo dir = new DirectoryInfo(config.m_Resources);
          ResBuildHelper.FindFilesByDirectoryTree(dir, config.m_Resources, targetAssets, targetAssetsNames);
          UnityEngine.Object[] assets = targetAssets.ToArray();
          string[] assetNames = targetAssetsNames.ToArray();
          if (assets != null && assets.Length > 0) {
            config.m_AssetNameList.AddRange(assetNames);
            BuildPipeline.BuildAssetBundleExplicitAssetNames(
              assets,
              assetNames,
              pathName,
              ResBuildConfig.BuildOptionRes,
              ResBuildConfig.BuildOptionTarget);
            if (ResBuildConfig.ResVersionIncrementalEnable) {
              foreach (string res in targetAssetsNames) {
                string curResFilePath = ResBuildHelper.GetFilePathAbs(res);
                string curResMD5 = ResBuildHelper.GetFileMd5(curResFilePath);
                IncrementalResVersionProvider.Instance.SetData(config.m_Resources, curResMD5, DateTime.Now.ToString());
              }
            }
            ResBuildLog.Info("ResBuilder CurPushed:" + GetCurResPushedInfo());
            ResBuildLog.Info("ResBuilder Build AB:" + config.ToString());
          } else {
            throw new Exception("BuildResRecursively ResBuildData failed!");
          }
        } else {
          throw new Exception("BuildResRecursively ResBuildData failed!");
        }
        SetNodeStatus(node.GetId(), ResBuildGraphNodeStatus.Finished);
      } catch (System.Exception ex) {
        SetNodeStatus(node.GetId(), ResBuildGraphNodeStatus.Invalid);
        ResBuildLog.Warn("BuildResRecursively ResBuildData failed! Config:" + config.ToString() + " ex" + ex);
        return false;
      }
    }
    return true;
  }
  #region Global ResGraph
  private ResBuildData TryFindResBuildConfig(int id)
  {
    if (m_GlobalResGraph.ConfigDict.ContainsKey(id)) {
      return m_GlobalResGraph.ConfigDict[id];
    }
    return null;
  }
  private ResBuildGraphNode TryFindGlobalResNode(int id)
  {
    if (m_GlobalResGraph.NodeDict.ContainsKey(id)) {
      return m_GlobalResGraph.NodeDict[id];
    }
    return null;
  }
  private List<int> GetGlobalResFinished()
  {
    List<int> list = new List<int>();
    foreach (ResBuildGraphNode node in m_GlobalResGraph.NodeDict.Values) {
      if (node != null
        && node.GetId() != m_GlobalResGraph.RootNode.ConfigId
        && node.Status == ResBuildGraphNodeStatus.Finished) {
        list.Add(node.GetId());
      }
    }
    return list;
  }
  private List<int> GetGlobalResInvalid()
  {
    List<int> list = new List<int>();
    foreach (ResBuildGraphNode node in m_GlobalResGraph.NodeDict.Values) {
      if (node != null
        && node.GetId() != m_GlobalResGraph.RootNode.ConfigId
        && node.Status == ResBuildGraphNodeStatus.Invalid) {
        list.Add(node.GetId());
      }
    }
    return list;
  }
  private List<int> GetGlobalResMissed()
  {
    List<int> list = new List<int>();
    foreach (ResBuildGraphNode node in m_GlobalResGraph.NodeDict.Values) {
      if (node != null
        && node.GetId() != m_GlobalResGraph.RootNode.ConfigId
        && node.Status == ResBuildGraphNodeStatus.None) {
        list.Add(node.GetId());
      }
    }
    return list;
  }
  private string GetGlobalStatusInfo()
  {
    string retStatusInfo = "BuildInfo:\n";
    retStatusInfo += string.Format("Configs Finished:{0}\n", ResBuildHelper.FormatListContent(GetGlobalResFinished()));
    retStatusInfo += string.Format("Configs Missed:{0}\n", ResBuildHelper.FormatListContent(GetGlobalResMissed()));
    retStatusInfo += string.Format("Configs Invalid:{0}\n", ResBuildHelper.FormatListContent(GetGlobalResInvalid()));
    return retStatusInfo;
  }
  private string GetGlobalResName(int nodeId)
  {
    ResBuildData data = TryFindResBuildConfig(nodeId);
    if (data != null) {
      return data.m_Resources;
    }
    return "invalid";
  }
  #endregion
  #region Current ResGraph
  private bool IsCurReadyToBuild(ResBuildGraphNode node)
  {
    if (node == null
      || node.GetId() == m_CurResGraph.RootNode.ConfigId) {
      return false;
    }
    if (node.Status != ResBuildGraphNodeStatus.None) {
      return false;
    }
    if (node.m_Depends.Count > 0) {
      foreach (int dependId in node.m_Depends) {
        ResBuildGraphNode parentNode = TryFindCurResNode(dependId);
        if (parentNode != null && parentNode.Status != ResBuildGraphNodeStatus.Finished) {
          return false;
        }
      }
    }
    return true;
  }
  private ResBuildGraphNode TryFindCurResNode(int id)
  {
    if (m_CurResGraph.NodeDict.ContainsKey(id)) {
      return m_CurResGraph.NodeDict[id];
    }
    return null;
  }
  private List<int> GetCurResPushed()
  {
    List<int> list = new List<int>();
    foreach (List<int> subList in m_PushedResList) {
      list.AddRange(subList);
    }
    return list;
  }
  private string GetCurResPushedInfo()
  {
    StringBuilder sb = new StringBuilder();
    sb.Append("Push Deep:" + m_PushedResList.Count);
    for (int index = 0; index < m_PushedResList.Count; index++) {
      List<int> tPushList = m_PushedResList[index];
      sb.Append("Layer:" + (index + 1));
      sb.Append("->:" + ResBuildHelper.FormatListContent(tPushList));
      sb.Append("|");
    }
    return sb.ToString();
  }
  private void SetNodeStatus(int nodeId, ResBuildGraphNodeStatus status)
  {
    ResBuildGraphNode globalNode = TryFindGlobalResNode(nodeId);
    if (globalNode != null) {
      globalNode.Status = status;
    }
    ResBuildGraphNode curNode = TryFindCurResNode(nodeId);
    if (curNode != null) {
      curNode.Status = status;
    }
  }
  private List<int> GetCurResFinished()
  {
    List<int> list = new List<int>();
    foreach (ResBuildGraphNode node in m_CurResGraph.NodeDict.Values) {
      if (node != null
        && node.GetId() != m_CurResGraph.RootNode.ConfigId
        && node.Status == ResBuildGraphNodeStatus.Finished) {
        list.Add(node.GetId());
      }
    }
    return list;
  }
  private List<int> GetCurResInvalid()
  {
    List<int> list = new List<int>();
    foreach (ResBuildGraphNode node in m_CurResGraph.NodeDict.Values) {
      if (node != null
        && node.GetId() != m_CurResGraph.RootNode.ConfigId
        && node.Status == ResBuildGraphNodeStatus.Invalid) {
        list.Add(node.GetId());
      }
    }
    return list;
  }
  private List<int> GetCurResMissed()
  {
    List<int> list = new List<int>();
    foreach (ResBuildGraphNode node in m_CurResGraph.NodeDict.Values) {
      if (node != null
        && node.GetId() != m_CurResGraph.RootNode.ConfigId
        && node.Status == ResBuildGraphNodeStatus.None) {
        list.Add(node.GetId());
      }
    }
    return list;
  }
  private string GetCurStatusInfo()
  {
    string retStatusInfo = "BuildInfo:\n";
    retStatusInfo += string.Format("Configs Finished:{0}\n", ResBuildHelper.FormatListContent(GetCurResFinished()));
    retStatusInfo += string.Format("Configs Missed:{0}\n", ResBuildHelper.FormatListContent(GetCurResMissed()));
    retStatusInfo += string.Format("Configs Invalid:{0}\n", ResBuildHelper.FormatListContent(GetCurResInvalid()));
    return retStatusInfo;
  }
  private void OnBuildAllResourcesBefore()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("**********************************************************");
    sb.AppendLine("BuildAllResources Count:" + m_GlobalResGraph.RootNode.m_ChildDepends.Count);
    sb.AppendLine("ResList:" + ResBuildHelper.FormatListContent(m_GlobalResGraph.RootNode.m_ChildDepends));
    ResBuildLog.Info(sb.ToString());
  }
  private void OnBuildAllResourcesAfter()
  {
    ResBuildLog.Info("ResBuilder Status:\n" + GetGlobalStatusInfo());
  }
  private void OnBuildNodeBefore(ResBuildGraphNode node)
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("**********************************************************");
    sb.AppendLine("BuildRes Start:" + node.GetId()
      + " Res:" + GetGlobalResName(node.GetId()) + " FullRes:" + ResBuildHelper.FormatListContent(node.m_FullDepends));
    ResBuildLog.Info(sb.ToString());
  }
  private void OnBuildNodeAfter(ResBuildGraphNode node)
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("BuildRes End:" + node.GetId() + " Res:" + GetGlobalResName(node.GetId()));
    sb.AppendLine("ResBuilder Status:\n" + GetCurStatusInfo());
    sb.AppendLine("**********************************************************");
    ResBuildLog.Info(sb.ToString());
  }
  private void OnBuildNodeSkipAfter(ResBuildGraphNode node)
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("BuildRes End Skipped:" + node.GetId() + " Res:" + GetGlobalResName(node.GetId()));
    //sb.AppendLine("ResBuilder Status:\n" + GetCurStatusInfo());
    sb.AppendLine("**********************************************************");
    ResBuildLog.Info(sb.ToString());
  }
  #endregion
}