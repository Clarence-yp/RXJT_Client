using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class CriticalConfig : IData
  {
    public const int c_IntRate = 1000;
    public const float c_FloatRate = 1000.0f;
    public int m_Id = 0;
    public int m_Critical = 0;
    public int m_C = 0;

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "ID", 0, true);
      m_Critical = (int)(DBCUtil.ExtractNumeric<float>(node, "Critical", 0, true) * c_IntRate);
      m_C = (int)(DBCUtil.ExtractNumeric<float>(node, "C", 0, true) * c_IntRate * 10);
      return true;
    }

    public int GetId()
    {
      return m_Id;
    }
  }
  public class CriticalConfigProvider
  {
    public DataDictionaryMgr<CriticalConfig> CriticalConfigMgr
    {
      get { return m_CriticalConfigMgr; }
    }
    public CriticalConfig GetDataById(int id)
    {
      return m_CriticalConfigMgr.GetDataById(id);
    }
    public float GetC(float critical)
    {
      float val = 0;
      int rate = (int)(critical * CriticalConfig.c_IntRate);
      if (m_Critical2C.ContainsKey(rate)) {
        int c = m_Critical2C[rate];
        val = c / CriticalConfig.c_FloatRate / 10;
      }
      return val;
    }
    public void Load(string file, string root)
    {
      m_CriticalConfigMgr.CollectDataFromDBC(file, root);
      foreach (CriticalConfig cfg in m_CriticalConfigMgr.GetData().Values) {
        if(!m_Critical2C.ContainsKey(cfg.m_Critical))
          m_Critical2C.Add(cfg.m_Critical, cfg.m_C);
      }
    }

    private MyDictionary<int, int> m_Critical2C = new MyDictionary<int, int>();
    private DataDictionaryMgr<CriticalConfig> m_CriticalConfigMgr = new DataDictionaryMgr<CriticalConfig>();

    public static CriticalConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static CriticalConfigProvider s_Instance = new CriticalConfigProvider();
  }
}
