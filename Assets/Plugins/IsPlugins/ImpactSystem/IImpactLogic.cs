using System;
using System.Collections.Generic;

namespace DashFire
{
  public interface IImpactLogic
  {
    void StartImpact(CharacterInfo obj, int impactId);
    void Tick(CharacterInfo obj, int impactId);
    void StopImpact(CharacterInfo obj, int impactId);
    void OnInterrupted(CharacterInfo obj, int impactId);
    int RefixHpDamage(CharacterInfo obj, int impactId, int hpDamage, int senderId);
    void OnAddImpact(CharacterInfo obj, int impactId, int addImpactId);
  }

  public abstract class AbstractImpactLogic : IImpactLogic {

    public delegate void ImpactLogicDamageDelegate(CharacterInfo entity, int attackerId, int damage, bool isKiller, bool isCritical, bool isOrdinary);
    public delegate void ImpactLogicSkillDelegate(CharacterInfo entity, int skillId);
    public delegate void ImpactLogicEffectDelegate(CharacterInfo entity, string effectPath, string bonePath, float recycleTime);
    public delegate void ImpactLogicScreenTipDelegate(CharacterInfo entity, string tip);
    public delegate void ImpactLogicRageDelegate(CharacterInfo entity, int rage);
    public static ImpactLogicDamageDelegate EventImpactLogicDamage;
    public static ImpactLogicSkillDelegate EventImpactLogicSkill;
    public static ImpactLogicEffectDelegate EventImpactLogicEffect;
    public static ImpactLogicScreenTipDelegate EventImpactLogicScreenTip;
    public static ImpactLogicRageDelegate EventImpactLogicRage;
    public virtual void StartImpact(CharacterInfo obj, int impactId) {
      if (null != obj) {
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          if (impactInfo.ConfigData.BreakSuperArmor) {
            obj.SuperArmor = false;
          }
        }
        if (obj is NpcInfo) {
          NpcInfo npcObj = obj as NpcInfo;
          NpcAiStateInfo aiInfo = npcObj.GetAiStateInfo();
          if (null != aiInfo && 0 == aiInfo.HateTarget) {
            aiInfo.HateTarget = impactInfo.m_ImpactSenderId;
          }
        }
      }
    }
    public virtual void Tick(CharacterInfo obj, int impactId) {
      ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo && impactInfo.m_IsActivated) {
        long curTime = TimeUtility.GetServerMilliseconds();
        if (curTime > impactInfo.m_StartTime + impactInfo.m_ImpactDuration) {
          impactInfo.m_IsActivated = false;
        }
      }
    }

    public virtual void StopImpact(CharacterInfo obj, int impactId) {
    }

    public virtual void OnInterrupted(CharacterInfo obj, int impactId) {
      StopImpact(obj, impactId);
    }

    public virtual int RefixHpDamage(CharacterInfo obj, int impactId, int hpDamage, int senderId)
    {
      return hpDamage;
    }

    public virtual void OnAddImpact(CharacterInfo obj, int impactId, int addImpactId)
    {
    }

    protected bool IsImpactDamageOrdinary(CharacterInfo target, int impactId)
    {
      if (null != target) {
        ImpactInfo impactInfo = target.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          CharacterInfo sender = target.SceneContext.GetCharacterInfoById(impactInfo.m_ImpactSenderId);
          if (null != sender) {
            SkillInfo skillInfo = sender.GetSkillStateInfo().GetSkillInfoById(impactInfo.m_SkillId);
            if (null != skillInfo) {
              if (skillInfo.ConfigData.Category == SkillCategory.kAttack) {
                return true;
              }
            }
          }
        }
      }
      return false;
    }
    protected void ApplyDamage(CharacterInfo obj, int impactId) {
      if (null != obj && !obj.IsDead()) {
        if (GlobalVariables.Instance.IsClient && obj.SceneContext.IsRunWithRoomServer) {
          return;
        }
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          CharacterInfo sender = obj.SceneContext.GetCharacterInfoById(impactInfo.m_ImpactSenderId);
          int skillLevel = 0;
          bool isCritical = false;
          bool isOrdinary = false;
          bool isKiller = false;
          if (null != sender) {
            SkillInfo skillInfo = sender.GetSkillStateInfo().GetSkillInfoById(impactInfo.m_SkillId);
            if (null != skillInfo) {
              skillLevel = skillInfo.SkillLevel;
              if (skillInfo.ConfigData.Category == SkillCategory.kAttack) {
                isOrdinary = true;
              }
            }
            int curDamage = DamageCalculator.CalcImpactDamage(
              sender,
              obj,
              (SkillDamageType)impactInfo.ConfigData.DamageType,
              ElementDamageType.DC_None == (ElementDamageType)impactInfo.ConfigData.ElementType ? sender.GetEquipmentStateInfo().WeaponDamageType : (ElementDamageType)impactInfo.ConfigData.ElementType,
              impactInfo.ConfigData.DamageRate + skillLevel * impactInfo.ConfigData.LevelRate,
              impactInfo.ConfigData.DamageValue,
              out isCritical);
            foreach(ImpactInfo ii in obj.GetSkillStateInfo().GetAllImpact())
            {
              IImpactLogic logic = ImpactLogicManager.Instance.GetImpactLogic(ii.ConfigData.ImpactLogicId);
              if(null != logic)
              {
                curDamage = logic.RefixHpDamage(obj, ii.m_ImpactId, curDamage, sender.GetId());
              }
            }
            // 计算出的伤害小于1时， 不处理
            if (curDamage < 1) {
              return;
            }
            UserInfo user = obj as UserInfo;
            if (null != user) {
              user.GetCombatStatisticInfo().AddTotalDamageToMyself(curDamage);
            }
            UserInfo senderUser = sender as UserInfo;
            if (null != senderUser) {
              senderUser.GetCombatStatisticInfo().AddTotalDamageFromMyself(curDamage);
            }
            curDamage = curDamage * -1;
            int realDamage = curDamage;
            if (obj.Hp + curDamage < 0) {
              realDamage = 0 - obj.Hp;
            }
            obj.SetHp(Operate_Type.OT_Relative, realDamage);
            if (obj.Hp <= 0) {
              isKiller = true;
            }
            if (null != EventImpactLogicDamage) {
              EventImpactLogicDamage(obj, sender.GetId(), curDamage, isKiller, isCritical, isOrdinary);
            }
          }
        }
      }
    }
  }
}
