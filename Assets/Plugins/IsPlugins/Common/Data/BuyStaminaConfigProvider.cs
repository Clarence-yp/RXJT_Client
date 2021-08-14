using System;
using System.Collections.Generic;
using System.Text;

namespace DashFire
{
  [Serializable]
  public class BuyStaminaConfig : IData
  {
    public int m_BuyCount = 0;
    public int m_CostGold = 0;
    public int m_GainStamina = 0;
    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_BuyCount = DBCUtil.ExtractNumeric<int>(node, "BuyCount", 0, true);
      m_CostGold = DBCUtil.ExtractNumeric<int>(node, "CostGold", 0, true);
      m_GainStamina = DBCUtil.ExtractNumeric<int>(node, "GainStamina", 0, true);
      return true;
    }
    public int GetId()
    {
      return m_BuyCount;
    }
  }
  public class BuyStaminaConfigProvider
  {
    public DataDictionaryMgr<BuyStaminaConfig> BuyStaminaConfigMgr
    {
      get { return m_BuyStaminaConfigMgr; }
    }
    public BuyStaminaConfig GetDataById(int id)
    {
      return m_BuyStaminaConfigMgr.GetDataById(id);
    }
    public int GetDataCount()
    {
      return m_BuyStaminaConfigMgr.GetDataCount();
    }
    public void Load(string file, string root)
    {
      m_BuyStaminaConfigMgr.CollectDataFromDBC(file, root);
    }
    private DataDictionaryMgr<BuyStaminaConfig> m_BuyStaminaConfigMgr = new DataDictionaryMgr<BuyStaminaConfig>();
    public static BuyStaminaConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static BuyStaminaConfigProvider s_Instance = new BuyStaminaConfigProvider();
  }
}
