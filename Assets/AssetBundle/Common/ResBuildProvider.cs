using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DashFire
{
  public class ResBuildData : IData
  {
    public int m_Id;
    public string m_Resources;
    public string m_TargetName;
    public int m_Chapter;
    public List<int> m_Depends;
    public List<int> m_ChildDepends;
    public List<string> m_AssetNameList;
    public string m_MD5;
    public long m_Size;
    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "Id", -1, true);
      m_Resources = DBCUtil.ExtractString(node, "Resources", "", true).Trim();
      m_TargetName = DBCUtil.ExtractString(node, "TargetName", "", true).Trim();
      m_Depends = DBCUtil.ExtractNumericList<int>(node, "Depends", 0, false);
      m_Chapter = -1;
      m_ChildDepends = new List<int>();
      m_AssetNameList = new List<string>();
      m_MD5 = string.Empty;
      m_Size = 0;
      return true;
    }
    public int GetId()
    {
      return m_Id;
    }
    public void Reset()
    {
      m_Chapter = -1;
      m_ChildDepends = new List<int>();
      m_AssetNameList = new List<string>();
      m_MD5 = string.Empty;
      m_Size = 0;
    }
    public override string ToString()
    {
      return string.Format("ID:{0} Resources:{1} TargetName:{2} Chapter:{3} Depends:{4} ChildDepends:{5} MD5:{6} Size:{7}",
        m_Id,
        m_Resources,
        m_TargetName,
        m_Chapter,
        FormatListContent(m_Depends),
        FormatListContent(m_ChildDepends),
        m_MD5,
        m_Size);
    }
    public static string FormatListContent(List<int> list)
    {
      string ret = string.Empty;
      if (list != null && list.Count > 0) {
        for (int index = 0; index < list.Count; index++) {
          if (index != 0) {
            ret += " ";
          }
          ret += list[index].ToString().Trim();
        }
      }
      return ret;
    }
  }

  public class ResBuildProvider
  {
    #region Singleton
    private static ResBuildProvider s_instance_ = new ResBuildProvider();
    public static ResBuildProvider Instance
    {
      get { return s_instance_; }
    }
    #endregion

    Dictionary<int, ResBuildData> m_ConfigDict = new Dictionary<int, ResBuildData>();
    Dictionary<string, int> m_ReferenceDict = new Dictionary<string, int>();
    public void Clear()
    {
      m_ConfigDict.Clear();
      m_ReferenceDict.Clear();
    }
    public bool CollectDataFromDBC(string file)
    {
      bool result = true;
      DBC document = new DBC();
      document.Load(file);
      for (int index = 0; index < document.RowNum; index++) {
        DBC_Row node = document.GetRowByIndex(index);
        if (node != null) {
          ResBuildData data = new ResBuildData();
          bool ret = data.CollectDataFromDBC(node);
          if (ret) {
            m_ConfigDict.Add(data.GetId(), data);
            m_ReferenceDict.Add(data.m_Resources, data.GetId());
          } else {
            string info = string.Format("DataTempalteMgr.CollectDataFromXml collectData Row:{0} failed!", index);
            LogSystem.Assert(ret, info);
            result = false;
          }
        }
      }
      return result;
    }
    public bool CollectDataFromDBC(byte[] bytes)
    {
      bool result = true;
      MemoryStream ms = null;
      StreamReader sr = null;
      try {
        DBC document = new DBC();
        ms = new MemoryStream(bytes);
        ms.Seek(0, SeekOrigin.Begin);
        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        sr = new StreamReader(ms, encoding);
        document.LoadFromStream(sr);
        for (int index = 0; index < document.RowNum; index++) {
          DBC_Row node = document.GetRowByIndex(index);
          if (node != null) {
            ResBuildData data = new ResBuildData();
            bool ret = data.CollectDataFromDBC(node);
            if (ret) {
              m_ConfigDict.Add(data.GetId(), data);
              m_ReferenceDict.Add(data.m_Resources, data.GetId());
            } else {
              string info = string.Format("DataTempalteMgr.CollectDataFromXml collectData Row:{0} failed!", index);
              LogSystem.Assert(ret, info);
              result = false;
            }
          }
        }
      } catch (System.Exception ex) {
        string info = string.Format("DataTempalteMgr.CollectDataFromXml exception ex:", ex);
        LogSystem.Assert(false, info);
      } finally {
        if (ms != null) {
          ms.Close();
        }
        if (sr != null) {
          sr.Close();
        }
      }
      return result;
    }
    public ResBuildData GetDataById(int id)
    {
      if (m_ConfigDict.ContainsKey(id)) {
        return m_ConfigDict[id];
      }
      return null;
    }
    public ResBuildData GetDataByName(string name)
    {
      if (m_ReferenceDict.ContainsKey(name)) {
        return GetDataById(m_ReferenceDict[name]);
      }
      return null;
    }
    public List<ResBuildData> GetArray()
    {
      List<ResBuildData> list = new List<ResBuildData>();
      foreach (ResBuildData config in m_ConfigDict.Values) {
        list.Add(config);
      }
      return list;
    }
    public Dictionary<int, ResBuildData> GetDict()
    {
      return m_ConfigDict;
    }
    public List<ResBuildData> SearchResByNamePrefix(string namePrefix)
    {
      List<ResBuildData> resBuildData = new List<ResBuildData>();
      foreach (ResBuildData config in m_ConfigDict.Values) {
        if (config.m_Resources.StartsWith(namePrefix)) {
          resBuildData.Add(config);
        }
      }
      return resBuildData;
    }
    public List<ResBuildData> SearchResByNamePostfix(string namePostfix)
    {
      List<ResBuildData> resBuildData = new List<ResBuildData>();
      foreach (ResBuildData config in m_ConfigDict.Values) {
        if (config.m_Resources.EndsWith(namePostfix)) {
          resBuildData.Add(config);
        }
      }
      return resBuildData;
    }
  }
}

