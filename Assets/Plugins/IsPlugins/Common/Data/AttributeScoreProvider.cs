using System;
using System.Collections.Generic;
using System.Text;

namespace DashFire {
  public enum AttributeScoreName : int
  {
    HP = 1,
    Energy = 2,
    AD = 3,
    ADP = 4,
    MDP = 5,
    Cri = 6,
    CriDamage = 7,
    BackHitPow = 8,
    CrackPow = 9,
    FireDamage = 10,
    IceDamage = 11,
    PoisonDamage = 12,
    FireResistance = 13,
    IceResistance = 14,
    PoisonResistance = 15,
  }
  [Serializable]
  public class AttributeScoreConfig : IData
  {
    public int m_Id = 0;
    public string m_Name = "";
    public float m_Score = 0;
    public float m_BaseValue = 0;

    public bool CollectDataFromDBC(DBC_Row node)
    {
      m_Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
      m_Name = DBCUtil.ExtractString(node, "Name", "", true);
      m_Score = DBCUtil.ExtractNumeric<float>(node, "Score", 0, false);
      m_BaseValue = DBCUtil.ExtractNumeric<float>(node, "BaseValue", 0, false);
      return true;
    }

    public int GetId()
    {
      return m_Id;
    }
  }

  public class AttributeScoreConfigProvider
  {
    public DataDictionaryMgr<AttributeScoreConfig> AttributeScoreConfigMgr
    {
      get { return m_AttributeScoreConfigMgr; }
    }
    private AttributeScoreConfig GetDataById(int id)
    {
      return m_AttributeScoreConfigMgr.GetDataById(id);
    }
    public void Load(string file, string root)
    {
      m_AttributeScoreConfigMgr.CollectDataFromDBC(file, root);
    }
    private float GetAloneAttributeScore(AttributeScoreName name, float value)
    {
      float return_score = 0;
      if (0 == value) {
        return return_score;
      }
      AttributeScoreConfig config_data = GetDataById((int)name);
      if (config_data.m_BaseValue != 0) {
        return_score = value / config_data.m_BaseValue * config_data.m_Score;
      }
      return return_score;
    }
    public float CalcAttributeScore(float hp, float energy, float ad, float adp, float mdp, float cri, float cridamage,
      float backhitpow, float crackpow,
      float firedamage, float icedamage, float poisondamage, float fireresistance, float iceresistance, float poisonresistance)
    {
      float total_score = 0f;
      total_score += GetAloneAttributeScore(AttributeScoreName.HP, hp);
      total_score += GetAloneAttributeScore(AttributeScoreName.Energy, energy);
      total_score += GetAloneAttributeScore(AttributeScoreName.AD, ad);
      total_score += GetAloneAttributeScore(AttributeScoreName.ADP, adp);
      total_score += GetAloneAttributeScore(AttributeScoreName.MDP, mdp);
      total_score += GetAloneAttributeScore(AttributeScoreName.Cri, cri);
      total_score += GetAloneAttributeScore(AttributeScoreName.CriDamage, cridamage);
      total_score += GetAloneAttributeScore(AttributeScoreName.BackHitPow, backhitpow);
      total_score += GetAloneAttributeScore(AttributeScoreName.CrackPow, crackpow);
      total_score += GetAloneAttributeScore(AttributeScoreName.FireDamage, firedamage);
      total_score += GetAloneAttributeScore(AttributeScoreName.IceDamage, icedamage);
      total_score += GetAloneAttributeScore(AttributeScoreName.PoisonDamage, poisondamage);
      total_score += GetAloneAttributeScore(AttributeScoreName.FireResistance, fireresistance);
      total_score += GetAloneAttributeScore(AttributeScoreName.IceResistance, iceresistance);
      total_score += GetAloneAttributeScore(AttributeScoreName.PoisonResistance, poisonresistance);
      return total_score;
    }

    private DataDictionaryMgr<AttributeScoreConfig> m_AttributeScoreConfigMgr = new DataDictionaryMgr<AttributeScoreConfig>();
    public static AttributeScoreConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static AttributeScoreConfigProvider s_Instance = new AttributeScoreConfigProvider();
  }
}
