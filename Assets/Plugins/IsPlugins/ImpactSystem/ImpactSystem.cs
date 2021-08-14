using System;
using ScriptRuntime;
using System.Collections.Generic;

namespace DashFire {
  #region Delegate
  public delegate void ImpactEventHandler(CharacterInfo sender, int targetId, int impactId);
  public delegate void EffectEventHandler(CharacterInfo obj, int impactId, int effectId);
  public delegate void SendImpactEventHandler(CharacterInfo sender, int targetId, int impactId, Vector3 srcPos, float srcDir);
  #endregion
  public sealed class ImpactSystem {
    #region Event
    public static SendImpactEventHandler EventSendImpact;
    public static ImpactEventHandler EventStopImpact;
    public static ImpactEventHandler EventGfxStopImpact;
    #endregion
    public void Tick(CharacterInfo obj)
    {
      List<ImpactInfo> impactInfos = obj.GetSkillStateInfo().GetAllImpact();
      int ct = impactInfos.Count;
      for (int i = ct - 1; i >= 0; --i) {
        ImpactInfo info = impactInfos[i];
        IImpactLogic logic = ImpactLogicManager.Instance.GetImpactLogic(info.ConfigData.ImpactLogicId);
        if (info.m_IsActivated) {
          if (null != logic) {
            logic.Tick(obj, info.m_ImpactId);
          }
        } else if(!info.m_IsGfxControl){
          logic.OnInterrupted(obj, info.m_ImpactId);
          obj.GetSkillStateInfo().RemoveImpact(info.m_ImpactId);
        }
      }
      obj.GetSkillStateInfo().CleanupImpactInfoForCheck(TimeUtility.GetServerMilliseconds(), 5000);//用于校验的impact比正常时间晚5秒清除
    }

    public bool SendImpactToCharacter(CharacterInfo sender, int impactId, int targetId, int skillId, int duration, Vector3 srcPos, float srcDir){
      //LogSystem.Debug("cn_mr SendImpactToCharacter impactId = {0} skillId = {1}", impactId, skillId);
      bool ret = SendImpactImpl(sender, impactId, targetId, skillId, duration);
      if (ret) {
        CharacterInfo target = sender.SceneContext.GetCharacterInfoById(targetId);
        Vector3 tarPos = target.GetMovementStateInfo().GetPosition3D();
        //LogSystem.Debug("cn_mr SendImpactToCharacter impactId = {0} skillId = {1}, scrPos = ({2},{3},{4}), scrDir = {5}, tarPos = ({6},{7},{8}})", impactId, skillId, srcPos.X, srcPos.Y, srcPos.Z, srcDir, tarPos.X,
          // tarPos.Y, tarPos.Z);
        if (null != target) {
          OnAddImpact(target, impactId);
        }
        if (null != EventSendImpact) {
          EventSendImpact(sender, targetId, impactId, srcPos, srcDir);
        }
      }
      return ret;
    }

    public bool StopImpactById(CharacterInfo target, int impactId) {
      bool ret = StopImpactImpl(target, impactId);
      if (ret) {
        if (null != EventStopImpact) {
          EventStopImpact(null, target.GetId(), impactId);
        }
      }
      return ret;
    }

    private void OnAddImpact(CharacterInfo target, int impactId)
    {
      foreach (ImpactInfo info in target.GetSkillStateInfo().GetAllImpact()) {
        if (info.m_ImpactId != impactId) {
          IImpactLogic logic = ImpactLogicManager.Instance.GetImpactLogic(info.ConfigData.ImpactLogicId);
          if (null != logic) {
            logic.OnAddImpact(target, info.m_ImpactId, impactId);
          }
        }
      }
    }
    private bool StopImpactImpl(CharacterInfo target, int impactId) {
      ImpactInfo impactInfo = target.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        IImpactLogic logic = ImpactLogicManager.Instance.GetImpactLogic(impactInfo.ConfigData.ImpactLogicId);
        if (null != logic) {
          logic.OnInterrupted(target, impactId);
        }
        impactInfo.m_IsActivated = false;
        return true;
      }
      return false;
    }

    public bool IsImpactControlTarget(CharacterInfo target) {
      List<ImpactInfo> impactInfos = target.GetSkillStateInfo().GetAllImpact();
      int ct = impactInfos.Count;
      for (int i = ct - 1; i >= 0; --i) {
        ImpactInfo info = impactInfos[i];
        if (info.m_IsGfxControl) {
          return true;
        }
      }
      return false;
    }

    public void OnGfxStopImpact(CharacterInfo target, int impactId) {
      if (null != target) {
        ImpactInfo impactInfo = target.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          impactInfo.m_IsGfxControl = false;
          CharacterInfo sender = target.SceneContext.GetCharacterInfoById(impactInfo.m_ImpactSenderId);
          if (null != EventGfxStopImpact) {
            EventGfxStopImpact(sender, target.GetId(), impactId);
          }
        }
      }
    }

    private bool SendImpactImpl(CharacterInfo sender, int impactId, int targetId, int skillId, int duration) {
      //LogSystem.Debug("character {0} send impact {1} to character {2}", sender.GetId(), impactId, targetId);
      if (null != sender) {
        CharacterInfo target = sender.SceneContext.GetCharacterInfoById(targetId);
        if (null != target) {
          if (target.IsUser && target.IsDead()) return false;
          if (target.IsNpc && target.IsDead()) {
            NpcInfo npc = target.CastNpcInfo();
            if ((int)NpcTypeEnum.SceneObject == npc.NpcType) {
              return false;
            }
          }
          ImpactLogicData impactLogicData = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_IMPACT, impactId) as ImpactLogicData;
          if (null != impactLogicData) {
            IImpactLogic logic = ImpactLogicManager.Instance.GetImpactLogic(impactLogicData.ImpactLogicId);
            if (null != logic) {
              ImpactInfo oldImpactInfo = target.GetSkillStateInfo().GetImpactInfoById(impactId);
              if (null != oldImpactInfo) {
                logic.OnInterrupted(target, impactId);
                target.GetSkillStateInfo().RemoveImpact(impactId);
              }
              ImpactInfo impactInfo = new ImpactInfo();
              impactInfo.m_IsActivated = true;
              impactInfo.m_SkillId = skillId;
              impactInfo.m_ImpactId = impactLogicData.ImpactId;
              impactInfo.m_ImpactType = impactLogicData.ImpactType;
              impactInfo.m_BuffDataId = impactLogicData.BuffDataId;
              impactInfo.ConfigData = impactLogicData;
              impactInfo.m_StartTime = TimeUtility.GetServerMilliseconds();
              impactInfo.m_ImpactDuration = impactLogicData.ImpactTime;
              if (-1 == duration || duration > impactLogicData.ImpactTime) {
                impactInfo.m_ImpactDuration = impactLogicData.ImpactTime;
              } else {
                impactInfo.m_ImpactDuration = duration;
              }
              impactInfo.m_HasEffectApplyed = false;
              if (0 == impactInfo.ConfigData.ImpactGfxLogicId) {
                impactInfo.m_IsGfxControl = false;
              } else {
                impactInfo.m_IsGfxControl = true;
              }
              impactInfo.m_ImpactSenderId = sender.GetId();
              impactInfo.m_MaxMoveDistanceSqr = impactLogicData.CalcMaxMoveDistanceSqr();
              if (impactLogicData.ImpactGfxLogicId == 0) {
                impactInfo.m_LeftEnableMoveCount = 0;//禁止位移
              } else {
                impactInfo.m_LeftEnableMoveCount = 1;//允许位移
              }

              //LogSystem.Warn("impact {0} EnableMoveCount {1} MaxMoveDistanceSqr {2}", impactId, impactInfo.m_LeftEnableMoveCount, impactInfo.m_MaxMoveDistanceSqr);

              target.GetSkillStateInfo().AddImpact(impactInfo);
              logic.StartImpact(target, impactId);

              if ((int)ImpactType.INSTANT == impactInfo.m_ImpactType) {
                impactInfo.m_IsActivated = false;
              }
              return true;
            }
          }
        }
      }
      return false;
    }

    private ImpactSystem() { }

    public static ImpactSystem Instance
    {
      get
      {
        return s_Instance;
      }
    }
    private static ImpactSystem s_Instance = new ImpactSystem();
  }
}
