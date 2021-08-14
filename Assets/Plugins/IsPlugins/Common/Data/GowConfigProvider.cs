using System;
using System.Collections.Generic;

namespace DashFire
{
  public sealed class GowPrizeConfig : IData
  {
    public int m_Ranking;
    public int m_Money;
    public int m_Gold;

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Ranking = DBCUtil.ExtractNumeric<int>(node, "Ranking", 0, true);
      m_Money = DBCUtil.ExtractNumeric<int>(node, "Money", 0, false);
      m_Gold = DBCUtil.ExtractNumeric<int>(node, "Gold", 0, false);
      return true;
    }

    public int GetId()
    {
      return m_Ranking;
    }
  }

  public sealed class GowTimeConfig : IData
  {
    public enum TimeTypeEnum : int
    {
      PrizeTime = 1,
      MatchTime,
    }

    public int m_Id;
    public int m_Type;
    public int m_StartHour;
    public int m_StartMinute;
    public int m_StartSecond;
    public int m_EndHour;
    public int m_EndMinute;
    public int m_EndSecond;
    
    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "ID", 0, true);
      m_Type = DBCUtil.ExtractNumeric<int>(node, "Type", 1, true);
      m_StartHour = DBCUtil.ExtractNumeric<int>(node, "StartHour", 0, true);
      m_StartMinute = DBCUtil.ExtractNumeric<int>(node, "StartMinute", 0, true);
      m_StartSecond = DBCUtil.ExtractNumeric<int>(node, "StartSecond", 0, true);
      m_EndHour = DBCUtil.ExtractNumeric<int>(node, "EndHour", 0, true);
      m_EndMinute = DBCUtil.ExtractNumeric<int>(node, "EndMinute", 0, true);
      m_EndSecond = DBCUtil.ExtractNumeric<int>(node, "EndSecond", 0, true);
      
      return true;
    }

    public int GetId()
    {
      return m_Id;
    }
  }

  public sealed class GowConstConfig : IData
  {
    public int m_TotalPoint;
    public float m_HighRate;
    public float m_LowRate;

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_TotalPoint = DBCUtil.ExtractNumeric<int>(node, "TotalPoint", 0, true);
      m_HighRate = DBCUtil.ExtractNumeric<float>(node, "HighRate", 0.0f, true);
      m_LowRate = DBCUtil.ExtractNumeric<float>(node, "LowRate", 0.0f, true);
      return true;
    }

    public int GetId()
    {
      return m_TotalPoint;
    }
  }

  public sealed class GowFormulaConfig : IData
  {
    public enum FormulaNameEnum : int
    {
      Upper = 1,
      Lower,
      K2_1,
      K2_2,
      K1,
      K3,
      TC,
    }

    public int m_Index;
    public string m_Name;
    public int m_Value;

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Index = DBCUtil.ExtractNumeric<int>(node, "Index", 0, true);
      m_Name = DBCUtil.ExtractNumeric<string>(node, "Name", "", true);
      m_Value = DBCUtil.ExtractNumeric<int>(node, "Value", 0, true);
      return true;
    }

    public int GetId()
    {
      return m_Index;
    }
  }

  public sealed class GowConfigProvider
  {
    public DataListMgr<GowPrizeConfig> GowPrizeConfigMgr
    {
      get { return m_GowPrizeConfigMgr; }
    }
    public DataListMgr<GowTimeConfig> GowTimeConfigMgr
    {
      get { return m_GowTimeConfigMgr; }
    }
    public DataListMgr<GowConstConfig> GowConstConfigMgr
    {
      get { return m_GowConstConfigMgr; }
    }
    public DataDictionaryMgr<GowFormulaConfig> GowFormulaConfigMgr
    {
      get { return m_GowFormulaConfigMgr; }
    }

    public GowPrizeConfig FindGowPrizeConfig(int gowElo)
    {
      int ct = m_GowPrizeConfigMgr.GetDataCount();
      List<GowPrizeConfig> prizes = m_GowPrizeConfigMgr.GetData();
      int st = 0;
      int ed = ct - 1;
      for (int findCt = 0; findCt < ct && st < ed; ++findCt) {
        int mid = (st + ed) / 2;
        int ranking = prizes[mid].m_Ranking;
        if (ranking < gowElo) {
          if (mid > 0 && prizes[mid - 1].m_Ranking >= gowElo) {
            return prizes[mid];
          } else {
            ed = mid;
          }
        } else if (ranking == gowElo) {
          return prizes[mid];
        } else {
          if (mid < ed && prizes[mid + 1].m_Ranking < gowElo) {
            return prizes[mid];
          } else {
            st = mid;
          }
        }
      }
      return null;
    }
    public GowConstConfig FindGowConstConfig(int point)
    {
      int ct = m_GowConstConfigMgr.GetDataCount();
      List<GowConstConfig> consts = m_GowConstConfigMgr.GetData();
      int st = 0;
      int ed = ct - 1;
      for (int findCt = 0; findCt < ct && st < ed; ++findCt) {
        int mid = (st + ed) / 2;
        int ranking = consts[mid].m_TotalPoint;
        if (ranking > point) {
          if (mid > 0 && consts[mid - 1].m_TotalPoint <= point) {
            return consts[mid];
          } else {
            ed = mid;
          }
        } else if (ranking == point) {
          return consts[mid];
        } else {
          if (mid < ed && consts[mid + 1].m_TotalPoint > point) {
            return consts[mid];
          } else {
            st = mid;
          }
        }
      }
      return null;
    }
    public GowFormulaConfig GetGowFormulaConfig(int id)
    {
      return m_GowFormulaConfigMgr.GetDataById(id);
    }

    public void LoadForClient()
    {
      m_GowPrizeConfigMgr.CollectDataFromDBC(FilePathDefine_Client.C_GowPrizeConfig, "GowPrize");
      m_GowTimeConfigMgr.CollectDataFromDBC(FilePathDefine_Client.C_GowTimeConfig, "GowTime");
    }
    public void LoadForServer()
    {
      m_GowPrizeConfigMgr.CollectDataFromDBC(FilePathDefine_Server.C_GowPrizeConfig, "GowPrize");
      m_GowTimeConfigMgr.CollectDataFromDBC(FilePathDefine_Server.C_GowTimeConfig, "GowTime");
      m_GowConstConfigMgr.CollectDataFromDBC(FilePathDefine_Server.C_GowConstConfig, "GowConst");
      m_GowFormulaConfigMgr.CollectDataFromDBC(FilePathDefine_Server.C_GowFormulaConfig, "GowFormula");
    }

    private DataListMgr<GowPrizeConfig> m_GowPrizeConfigMgr = new DataListMgr<GowPrizeConfig>();
    private DataListMgr<GowTimeConfig> m_GowTimeConfigMgr = new DataListMgr<GowTimeConfig>();
    private DataListMgr<GowConstConfig> m_GowConstConfigMgr = new DataListMgr<GowConstConfig>();
    private DataDictionaryMgr<GowFormulaConfig> m_GowFormulaConfigMgr = new DataDictionaryMgr<GowFormulaConfig>();

    public static GowConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static GowConfigProvider s_Instance = new GowConfigProvider();
  }
}
