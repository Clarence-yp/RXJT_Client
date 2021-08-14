using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DashFire;
using UnityEngine;
using UnityEditor;

public enum ResBuildGraphNodeStatus
{
  None,
  Finished,
  Invalid
}
public class ResBuildGraphNode
{
  public int ConfigId;
  public ResBuildGraphNodeStatus Status = ResBuildGraphNodeStatus.None;
  public List<int> m_Depends = new List<int>();
  public List<int> m_FullDepends = new List<int>();
  public List<int> m_ChildDepends = new List<int>();
  public List<string> m_AssetNameList = new List<string>();
  public int GetId() { return ConfigId; }
  public ResBuildGraphNode Clone()
  {
    ResBuildGraphNode newNode = new ResBuildGraphNode();
    newNode.ConfigId = ConfigId;
    newNode.Status = Status;
    newNode.m_Depends.AddRange(m_Depends);
    newNode.m_FullDepends.AddRange(m_FullDepends);
    newNode.m_ChildDepends.AddRange(m_ChildDepends);
    newNode.m_AssetNameList.AddRange(m_AssetNameList);
    return newNode;
  }
}

public class ResBuildGraph
{
  public ResBuildGraphNode RootNode = null;
  public Dictionary<int, ResBuildGraphNode> NodeDict = null;

  public Dictionary<int, ResBuildData> ConfigDict = null;
  public ResBuildGraph()
  {
    Reset();
  }
  public void Reset()
  {
    ConfigDict = null;
    if (NodeDict == null) {
      NodeDict = new Dictionary<int, ResBuildGraphNode>();
    }
    NodeDict.Clear();

    if (RootNode == null) {
      RootNode = new ResBuildGraphNode();
    }
    RootNode.ConfigId = ResBuildConfig.ResGraphRootId;
    RootNode.m_Depends.Clear();
    RootNode.m_FullDepends.Clear();
    RootNode.m_ChildDepends.Clear();
    RootNode.m_AssetNameList.Clear();
    //NodeDict.Add(RootNode.ConfigId, RootNode);
  }
  public bool Build(Dictionary<int, ResBuildData> dict)
  {
    Reset();

    ConfigDict = dict;
    if (ConfigDict == null || ConfigDict.Count <= 0) {
      ResBuildLog.Warn("ResGraph.Build ResBuildProvider empty");
      return false;
    }
    CreateNodeGraph();
    SimpleGraphGraph();
    AddChildDepends();
    ResBuildLog.Info("ResBuildGraph Status:\n" + GetBuildStatusInfo());
    return true;
  }
  public ResBuildGraph ExtractSubGraph(ResBuildGraphNode rootNode)
  {
    ResBuildGraph subGraph = new ResBuildGraph();
    subGraph.ConfigDict = ConfigDict;
    ExtractGraphByNode(rootNode, ref subGraph);
    return subGraph;
  }
  public bool CheckReBuildByIncremental()
  {
    foreach (ResBuildGraphNode curNode in NodeDict.Values) {
      ResBuildData buildData = GetResConfigById(curNode.ConfigId);
      if (buildData == null) {
        ResBuildLog.Info("CheckReBuildByIncremental miss ResBuildData null. Id:" + curNode.ConfigId);
        return true;
      }

      // 确定IncrementalResVersion表格中存在记录
      IncrementalResVersionData buildedData = IncrementalResVersionProvider.Instance.GetDataByName(buildData.m_Resources);
      if (buildedData == null) {
        ResBuildLog.Info("CheckReBuildByIncremental miss IncrementalResVersionData null. Res:" + buildData.m_Resources);
        return true;
      }

      // 确定assetbundle确实存在
      string buildedAbFile = ResBuildHelper.FormatResPathFromConfig(buildData);
      if (!System.IO.File.Exists(buildedAbFile)) {
        ResBuildLog.Info("CheckReBuildByIncremental miss assetbundle not exist. Res:" + buildData.m_Resources);
        return true;
      }

      // 确定资源的md5值与IncrementalResVersion表格中的一致
      string buildResMD5 = buildedData.m_MD5;
      string curResFilePath = ResBuildHelper.GetFilePathAbs(buildData.m_Resources);
      string curResMD5 = ResBuildHelper.GetFileMd5(curResFilePath);
      if (buildResMD5 == curResMD5) {
        continue;
      } else {
        ResBuildLog.Info("CheckReBuildByIncremental md5 not equal. Res:" + buildData.m_Resources);
        return true;
      }
    }
    return false;
  }
  private bool ExtractGraphByNode(ResBuildGraphNode curNode, ref ResBuildGraph subGraph)
  {
    if (!subGraph.NodeDict.ContainsKey(curNode.GetId())) {
      ResBuildGraphNode curNewNode = curNode.Clone();
      curNewNode.Status = ResBuildGraphNodeStatus.None;
      subGraph.NodeDict.Add(curNode.GetId(), curNewNode);
    }

    if (curNode.m_Depends != null && curNode.m_Depends.Count > 0) {
      foreach (int parentDependId in curNode.m_Depends) {
        ResBuildGraphNode parentNode = GetResNodeById(parentDependId);
        if (parentNode != null) {
          ExtractGraphByNode(parentNode, ref subGraph);
        }
      }
    }
    return true;
  }
  private void CreateNodeGraph()
  {
    foreach (ResBuildData config in ConfigDict.Values) {
      if (config != null) {
        if (config.m_Depends != null && config.m_Depends.Count > 0) {
          foreach (int parentDependId in config.m_Depends) {
            ResBuildData parentConfig = GetResConfigById(parentDependId);
            if (parentConfig != null) {
              parentConfig.m_ChildDepends.Add(config.GetId());
            } else {
              string info = string.Format("ResGraph.CreateNodeGraph Depend id not exist! CurId:{0} Depend:{1}",
                config.GetId(), parentDependId);
              ResBuildLog.Warn(info);
            }
          }
        }

        ResBuildGraphNode node = new ResBuildGraphNode();
        node.ConfigId = config.GetId();
        node.m_FullDepends.AddRange(config.m_Depends);
        node.m_Depends.AddRange(config.m_Depends);
        node.m_ChildDepends.Clear();
        NodeDict.Add(node.ConfigId, node);
      }
    }
  }
  private void SimpleGraphGraph()
  {
    HashSet<int> parentDependSet = new HashSet<int>();
    HashSet<int> curNodeSet = new HashSet<int>();
    foreach (ResBuildGraphNode curNode in NodeDict.Values) {
      parentDependSet.Clear();
      curNodeSet.Clear();
      if (curNode.m_Depends != null && curNode.m_Depends.Count > 0) {
        GetNodeParentDependencies(curNode, ref parentDependSet);
        if (parentDependSet.Count > 0) {
          curNodeSet.UnionWith(curNode.m_Depends);
          curNodeSet.ExceptWith(parentDependSet);
          curNode.m_Depends.Clear();
          curNode.m_Depends.AddRange(curNodeSet);
        }
      }
    }
  }
  private void AddChildDepends()
  {
    foreach (ResBuildGraphNode node in NodeDict.Values) {
      if (node.m_Depends != null && node.m_Depends.Count > 0) {
        foreach (int parentDependId in node.m_Depends) {
          ResBuildGraphNode parentNode = GetResNodeById(parentDependId);
          if (parentNode != null) {
            parentNode.m_ChildDepends.Add(node.GetId());
          } else {
            string info = string.Format("ResGraph.Build Depend id not exist! CurId:{0} Depend:{1}",
              node.GetId(), parentDependId);
            ResBuildLog.Warn(info);
          }
        }
      }
    }
    foreach (ResBuildGraphNode node in NodeDict.Values) {
      if (node.m_ChildDepends == null || node.m_ChildDepends.Count == 0) {
        RootNode.m_ChildDepends.Add(node.GetId());
      }
    }
  }
  private void GetNodeParentDependencies(ResBuildGraphNode curNode, ref HashSet<int> dependList)
  {
    if (curNode.m_Depends != null && curNode.m_Depends.Count > 0) {
      foreach (int pDependId in curNode.m_Depends) {
        if (NodeDict.ContainsKey(pDependId)) {
          ResBuildGraphNode pNode = NodeDict[pDependId];
          dependList.UnionWith(pNode.m_Depends);
          GetNodeParentDependencies(pNode, ref dependList);
        }
      }
    }
  }
  private ResBuildData GetResConfigById(int id)
  {
    if (ConfigDict != null && ConfigDict.ContainsKey(id)) {
      return ConfigDict[id];
    }
    return null;
  }
  private ResBuildGraphNode GetResNodeById(int id)
  {
    if (NodeDict != null && NodeDict.ContainsKey(id)) {
      return NodeDict[id];
    }
    return null;
  }
  private string GetBuildStatusInfo()
  {
    //m_ResContent = ResConfig.s_ResNodeHeader + "\n";
    string m_ResContent = string.Empty;
    ResBuildGraphNode root = this.RootNode;
    m_ResContent += GetResNodeInfo(root);
    Dictionary<int, ResBuildGraphNode> dict = this.NodeDict;
    if (dict != null && dict.Count > 0) {
      foreach (ResBuildGraphNode node in dict.Values) {
        m_ResContent += GetResNodeInfo(node);
      }
    }
    return m_ResContent;
  }
  private string GetResNodeInfo(ResBuildGraphNode node)
  {
    string graphNodecontent = "GraphData:";
    graphNodecontent += string.Format(ResBuildConfig.ResGraphNodeInfoFormat,
      node.GetId(),
      ResBuildHelper.FormatListContent(node.m_Depends),
      ResBuildHelper.FormatListContent(node.m_ChildDepends));

    string buildDatacontent = "BuildData:";
    ResBuildData buildData = GetResConfigById(node.GetId());
    if (buildData != null) {
      buildDatacontent += string.Format(ResBuildConfig.ResGraphNodeInfoFormat,
        buildData.GetId(),
        ResBuildHelper.FormatListContent(buildData.m_Depends),
        ResBuildHelper.FormatListContent(buildData.m_ChildDepends));
    }

    StringBuilder sb = new StringBuilder();
    sb.AppendLine(graphNodecontent);
    sb.AppendLine(buildDatacontent);
    return sb.ToString();
  }
}