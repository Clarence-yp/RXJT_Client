using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class UiConfig:IData
  {
    public int m_Id = 0;
    public string m_WindowName = "";
    public string m_WindowPath = "";
    public int m_OffsetLeft = -1;
    public int m_OffsetRight = -1;
    public int m_OffsetBottom = -1;
    public int m_OffsetTop = -1;
    public bool m_IsExclusion = false;
    public int m_ShowType = -1;
    public int m_OwnToSceneId = -1;
    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "ID", 0, true);
      m_WindowName = DBCUtil.ExtractString(node, "WindowName", "", true);
      m_WindowPath = DBCUtil.ExtractString(node, "WindowPath", "", true);
      m_OffsetLeft = DBCUtil.ExtractNumeric<int>(node, "OffsetLeft", -1, false);
      m_OffsetRight = DBCUtil.ExtractNumeric<int>(node, "OffsetRight", -1, false);
      m_OffsetTop = DBCUtil.ExtractNumeric<int>(node, "OffsetTop", -1, false);
      m_OffsetBottom = DBCUtil.ExtractNumeric<int>(node, "OffsetBottom", -1, false);
      m_IsExclusion = DBCUtil.ExtractBool(node, "IsExclusion", false, false);
      m_ShowType = DBCUtil.ExtractNumeric<int>(node, "ShowType", 0, true);
      m_OwnToSceneId = DBCUtil.ExtractNumeric<int>(node, "SceneType", 0, true);
      return true;
    }
    public int GetId()
    {
      return m_Id;
    }
  }
  public class UiConfigProvider
  {
    public DataDictionaryMgr<UiConfig> UiConfigMgr
    {
      get { return m_UiConfigMgr; }
    }
    public UiConfig GetDataById(int id)
    {
      return m_UiConfigMgr.GetDataById(id);
    }
     public MyDictionary<int, object> GetData()
    {
      return m_UiConfigMgr.GetData();
    }
    public void Load(string file, string root)
    {
      m_UiConfigMgr.CollectDataFromDBC(file, root);
    }
    public int GetDataCount()
    {
      return m_UiConfigMgr.GetDataCount();
    }
    public void Clear()
    {
      m_UiConfigMgr.Clear();
    }

    private DataDictionaryMgr<UiConfig> m_UiConfigMgr = new DataDictionaryMgr<UiConfig>();

    public static UiConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static UiConfigProvider s_Instance = new UiConfigProvider();
  }
  
}
