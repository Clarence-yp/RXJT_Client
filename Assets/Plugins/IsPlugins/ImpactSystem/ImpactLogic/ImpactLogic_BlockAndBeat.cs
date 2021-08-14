using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  class ImpactLogic_BlockAndBeat : AbstractImpactLogic
  {
    public override void StartImpact(CharacterInfo obj, int impactId) {
      if (null != obj) {
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          //obj.SuperArmor = true;
        }
      }
      base.StartImpact(obj, impactId);
    }
    public override void Tick(CharacterInfo character, int impactId) {
      ImpactInfo impactInfo = character.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        if (impactInfo.m_IsActivated) {
          if (TimeUtility.GetServerMilliseconds() > impactInfo.m_StartTime + impactInfo.m_ImpactDuration) {
            impactInfo.m_IsActivated = false;
          }
        }
      }
    }

    public override void OnAddImpact(CharacterInfo obj, int impactId, int addImpactId)
    {
      ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        if (impactInfo.m_IsActivated) {
          ImpactInfo addImpactInfo = obj.GetSkillStateInfo().GetImpactInfoById(addImpactId);
          if (null != addImpactInfo) {
            if (addImpactInfo.m_ImpactSenderId == obj.GetId()) return;
            if (impactInfo.ConfigData.ParamNum >= 4) {
              int beatSkillId = int.Parse(impactInfo.ConfigData.ExtraParams[0]);
              int blockImpactId = int.Parse(impactInfo.ConfigData.ExtraParams[1]);
              string EffectAndBone = impactInfo.ConfigData.ExtraParams[2];
              float maxDis = float.Parse(impactInfo.ConfigData.ExtraParams[3]);
              CharacterInfo target = obj.SceneContext.GetCharacterInfoById(addImpactInfo.m_ImpactSenderId);
              if (null != target) {
                ScriptRuntime.Vector3 srcPos = obj.GetMovementStateInfo().GetPosition3D();
                ScriptRuntime.Vector3 tarPos = target.GetMovementStateInfo().GetPosition3D();
                if (Geometry.DistanceSquare(srcPos, tarPos) < maxDis * maxDis) {
                  ImpactSystem.Instance.SendImpactToCharacter(obj,
                                                              blockImpactId,
                                                              addImpactInfo.m_ImpactSenderId,
                                                              -1,
                                                              -1,
                                                              srcPos,
                                                              obj.GetMovementStateInfo().GetFaceDir());
                }
                ImpactSystem.Instance.StopImpactById(obj, impactId);
                impactInfo.m_IsActivated = false;
                //obj.SuperArmor = false;
                if (null != EventImpactLogicSkill) {
                  EventImpactLogicSkill(obj, beatSkillId);
                }
                if (null != EventImpactLogicEffect) {
                  string[] EffectBonePair = EffectAndBone.Split('|');
                  if(EffectBonePair.Length >= 2){
                    EventImpactLogicEffect(obj, EffectBonePair[0], EffectBonePair[1], 2.0f);
                  }
                }
              }
            }
            }
        }
      }
    }
  }
}
