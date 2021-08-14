using System;
using System.Collections.Generic;

namespace DashFire
{
  public class ImpactLogic_SuperArmorShield : AbstractImpactLogic
  {
    public delegate void SuperArmorShieldEffectDelegate(CharacterInfo entity, int impactId);
    public static SuperArmorShieldEffectDelegate EventStartSuperArmorShield;
    public static SuperArmorShieldEffectDelegate EventUpdateSuperArmorShield;
    public static SuperArmorShieldEffectDelegate EventStopSuperArmorShield;
    public class SuperArmorShieldInfo
    {
      public int DamageRemain = 0;
      public int DamageMax = 0;
      public int EffectActor = 0;
      public string EffectPath = "";
    }
    public override void StartImpact(CharacterInfo obj, int impactId)
    {
      if (null != obj) {
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          obj.UltraArmor = true;
          if (impactInfo.ConfigData.ParamNum > 1) {
            SuperArmorShieldInfo info = impactInfo.LogicDatas.GetData<SuperArmorShieldInfo>();
            if (null == info) {
              info = new SuperArmorShieldInfo();
              impactInfo.LogicDatas.AddData<SuperArmorShieldInfo>(info);
            }
            info.DamageRemain = (int)(float.Parse(impactInfo.ConfigData.ExtraParams[0]) * obj.GetActualProperty().HpMax);
            info.DamageMax = info.DamageRemain;
            info.EffectPath = impactInfo.ConfigData.ExtraParams[1];
            if (null != EventStartSuperArmorShield) {
              EventStartSuperArmorShield(obj, impactId);
            }
          }
        }
      }
    }

    public override void Tick(CharacterInfo obj, int impactId)
    {
      ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        if (impactInfo.m_IsActivated) {
          if (TimeUtility.GetServerMilliseconds() > impactInfo.m_StartTime + impactInfo.m_ImpactDuration) {
          }
        }
      }
    }

    public override int RefixHpDamage(CharacterInfo obj, int impactId, int hpDamage, int senderId)
    {
      ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        SuperArmorShieldInfo info = impactInfo.LogicDatas.GetData<SuperArmorShieldInfo>();
        if (null != info) {
          info.DamageRemain -= hpDamage;
          if (info.DamageRemain > 0) {
            if (null != EventUpdateSuperArmorShield) {
              EventUpdateSuperArmorShield(obj, impactId);
            }
          } else {
            if (null != EventStopSuperArmorShield) {
              EventStopSuperArmorShield(obj, impactId);
            }
            impactInfo.m_IsActivated = false;
            obj.UltraArmor = false;
          }
        }
      }
      return hpDamage;
    }
  }
}
