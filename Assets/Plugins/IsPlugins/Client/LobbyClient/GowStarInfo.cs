using System;
using System.Collections.Generic;

namespace DashFire
{
  public sealed class GowDataForMsg
  {
    public ulong m_Guid;
    public int m_GowElo;
    public string m_Nick;
    public int m_Heroid;
    public int m_Level;
    public int m_FightingScore;
    public List<ItemDataInfo> m_Equips = new List<ItemDataInfo>();
    public List<SkillInfo> m_Skills = new List<SkillInfo>();
  }
  public sealed class GowInfo
  {
    public int GowElo
    {
      get { return m_GowElo; }
      set { m_GowElo = value; }
    }
    public int GowMatches
    {
      get { return m_GowMatches; }
      set { m_GowMatches = value; }
    }
    public int GowWinMatches
    {
      get { return m_GowWinMatches; }
      set { m_GowWinMatches = value; }
    }
    public int LeftMatchCount
    {
      get { return m_LeftMatchCount; }
      set { m_LeftMatchCount = value; }
    }
    public DateTime LastBuyTime
    {
      get { return m_LastBuyTime; }
      set { m_LastBuyTime = value; }
    }
    public int LeftBuyCount
    {
      get { return m_LeftBuyCount; }
      set { m_LeftBuyCount = value; }
    }
    public List<GowDataForMsg> GowTop
    {
      get { return m_GowTop; }
    }

    private int m_GowElo = 1400;
    private int m_GowMatches = 0;
    private int m_GowWinMatches = 0;
    private int m_LeftMatchCount = 0;
    private DateTime m_LastBuyTime;
    private int m_LeftBuyCount = 0;
    private List<GowDataForMsg> m_GowTop = new List<GowDataForMsg>();
  }
}