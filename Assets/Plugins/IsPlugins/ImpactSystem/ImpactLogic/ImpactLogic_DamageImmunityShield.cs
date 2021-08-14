using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class ImpactLogic_DamageImmunityShield : AbstractImpactLogic
  {
    public delegate void DamageImmunityShiledEffectDelegate(CharacterInfo entity, int impactId);
    public static DamageImmunityShiledEffectDelegate EventStartDamageImmunityShiled;
    public static DamageImmunityShiledEffectDelegate EventUpdateDamageImmunityShiled;
    public static DamageImmunityShiledEffectDelegate EventStopDamageImmunityShiled;
    public class DamageImmunityShieldInfo
    {
      public int CountRemain = 0;
      public float RefreshInterval = 0.0f;
      public long LastHitTime = 0;
      public int CountMax = 0;
      public int EffectActor = 0;
      public string EffectPath = "";
    }
    public override void StartImpact(CharacterInfo obj, int impactId)
    {
      if (null != obj) {
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          if (impactInfo.ConfigData.ParamNum > 2) {
            DamageImmunityShieldInfo info = impactInfo.LogicDatas.GetData<DamageImmunityShieldInfo>();
            if (null == info) {
              info = new DamageImmunityShieldInfo();
              impactInfo.LogicDatas.AddData<DamageImmunityShieldInfo>(info);
            }
            info.CountRemain = int.Parse(impactInfo.ConfigData.ExtraParams[0]);
            info.RefreshInterval = float.Parse(impactInfo.ConfigData.ExtraParams[1]);
            info.EffectPath = impactInfo.ConfigData.ExtraParams[2];
            info.CountMax = info.CountRemain;
            info.LastHitTime = TimeUtility.GetServerMilliseconds();
            if (null != EventStartDamageImmunityShiled) {
              EventStartDamageImmunityShiled(obj, impactId);
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
          DamageImmunityShieldInfo info = impactInfo.LogicDatas.GetData<DamageImmunityShieldInfo>();
          if(null != info){
            if (TimeUtility.GetServerMilliseconds() - info.LastHitTime > info.RefreshInterval * 1000) {
              if (info.CountRemain <= 0) {
                if (null != EventStartDamageImmunityShiled) {
                  EventStartDamageImmunityShiled(obj, impactId);
                }
              }
              info.CountRemain = info.CountMax;
              // refresh ui
              if (null != EventUpdateDamageImmunityShiled) {
                EventUpdateDamageImmunityShiled(obj, impactId);
              }
            }
          }
        }
      }
    }
    public override int RefixHpDamage(CharacterInfo obj, int impactId, int hpDamage, int senderId)
    {
      int result = 0;
      ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        DamageImmunityShieldInfo info = impactInfo.LogicDatas.GetData<DamageImmunityShieldInfo>();
        if (null != info) {
          info.CountRemain -= 1;
          info.LastHitTime = TimeUtility.GetServerMilliseconds();
          if (info.CountRemain > 0) {
            if (null != EventUpdateDamageImmunityShiled) {
              EventUpdateDamageImmunityShiled(obj, impactId);
            }
            if (null != EventImpactLogicDamage) {
              EventImpactLogicDamage(obj, senderId, 0, false, false, IsImpactDamageOrdinary(obj, impactId));
            }
          } else {
            if (null != EventStopDamageImmunityShiled) {
              EventStopDamageImmunityShiled(obj, impactId);
            }
            result = hpDamage;
          }
        }
      }
      return result;
    }
  }
}
