using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DashFire;

namespace DashFire
{
  public class IncrementalResVersionData : IData
  {
    public string m_Name;
    public string m_MD5;
    public string m_BuildTime;
    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Name = DBCUtil.ExtractString(node, "Name", "", true);
      m_MD5 = DBCUtil.ExtractString(node, "MD5", "", true);
      m_BuildTime = DBCUtil.ExtractString(node, "m_BuildTime", "", true);
      return true;
    }
    public int GetId()
    {
      return 0;
    }
  }

  public class IncrementalResVersionProvider
  {
    #region Singleton
    private static IncrementalResVersionProvider s_instance_ = new IncrementalResVersionProvider();
    public static IncrementalResVersionProvider Instance
    {
      get { return s_instance_; }
    }
    #endregion
    Dictionary<string, IncrementalResVersionData> m_ConfigDict = new Dictionary<string, IncrementalResVersionData>();
    public void Clear()
    {
      m_ConfigDict.Clear();
    }
    public bool CollectDataFromDBC(string file)
    {
      bool result = true;
      DBC document = new DBC();
      document.Load(file);
      for (int index = 0; index < document.RowNum; index++) {
        DBC_Row node = document.GetRowByIndex(index);
        if (node != null) {
          IncrementalResVersionData data = new IncrementalResVersionData();
          bool ret = data.CollectDataFromDBC(node);
          if (ret) {
            m_ConfigDict.Add(data.m_Name, data);
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
            IncrementalResVersionData data = new IncrementalResVersionData();
            bool ret = data.CollectDataFromDBC(node);
            if (ret) {
              m_ConfigDict.Add(data.m_Name, data);
            } else {
              string info = string.Format("ClCollectDataFromDBC collectData Row:{0} failed!", index);
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
    public IncrementalResVersionData GetDataByName(string name)
    {
      if (m_ConfigDict.ContainsKey(name)) {
        return m_ConfigDict[name];
      }
      return null;
    }
    public void SetData(string name, string md5, string buildtime)
    {
      if (m_ConfigDict.ContainsKey(name)) {
        IncrementalResVersionData data = m_ConfigDict[name];
        data.m_Name = name;
        data.m_MD5 = md5;
        data.m_BuildTime = buildtime;
      } else {
        IncrementalResVersionData data = new IncrementalResVersionData();
        m_ConfigDict.Add(name, data);
        data.m_Name = name;
        data.m_MD5 = md5;
        data.m_BuildTime = buildtime;
      }
    }
    public void AddData(IncrementalResVersionData data)
    {
      if (!m_ConfigDict.ContainsKey(data.m_Name)) {
        m_ConfigDict.Add(data.m_Name, data);
      }
    }
    public Dictionary<string, IncrementalResVersionData> GetArray()
    {
      return m_ConfigDict;
    }
  }
}

