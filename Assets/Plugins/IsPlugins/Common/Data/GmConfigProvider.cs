using System;
using System.Collections.Generic;

namespace DashFire
{
  [Serializable]
  public class GmListConfig : IData
  {
    public int m_Id = 0;
    public string m_Account = "";

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
      m_Account = DBCUtil.ExtractString(node, "Account", "", true);
      return true;
    }
    public int GetId()
    {
      return m_Id;
    }
  }
  public class GmConfigProvider
  {
    public bool IsGmAccount(string account)
    {
      return m_GmAccounts.Contains(account);
    }
    public void Load(string file, string root)
    {
      m_GmListConfigMgr.CollectDataFromDBC(file, root);
      int ct = m_GmListConfigMgr.GetDataCount();
      List<GmListConfig> list = m_GmListConfigMgr.GetData();
      for (int i = 0; i < ct; ++i) {
        GmListConfig cfg = list[i];
        if (!m_GmAccounts.Contains(cfg.m_Account)) {
          m_GmAccounts.Add(cfg.m_Account);
        }
      }
    }

    private DataListMgr<GmListConfig> m_GmListConfigMgr = new DataListMgr<GmListConfig>();
    private HashSet<string> m_GmAccounts = new HashSet<string>();

    public static GmConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static GmConfigProvider s_Instance = new GmConfigProvider();
  }
}
