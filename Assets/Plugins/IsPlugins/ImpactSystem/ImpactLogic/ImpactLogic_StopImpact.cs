using System;
using System.Collections.Generic;

namespace DashFire{
  class ImpactLogic_StopImpact : AbstractImpactLogic {

    private enum RemoveType
    {
      IMPACT_ID,
      LOGIC_ID,
      GFX_ID,
    }
    public override void StartImpact(CharacterInfo obj, int impactId) {
      if (null != obj) {
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (impactInfo.ConfigData.ParamNum > 1) {
          if (!String.IsNullOrEmpty(impactInfo.ConfigData.ExtraParams[0]) && !String.IsNullOrEmpty(impactInfo.ConfigData.ExtraParams[1])) {
            int targetLogicId = int.Parse(impactInfo.ConfigData.ExtraParams[0]);
            int removeType = int.Parse(impactInfo.ConfigData.ExtraParams[1]);
            RemoveImpact(obj, targetLogicId, (RemoveType)removeType);
          }
        }
      }
    }

    private void RemoveImpact(CharacterInfo target, int id, RemoveType removeType)
    {
      switch (removeType) {
        case RemoveType.IMPACT_ID:
          foreach (ImpactInfo impact in target.GetSkillStateInfo().GetAllImpact()) {
            if (-1 == id || impact.m_ImpactId == id) {
              ImpactSystem.Instance.StopImpactById(target, impact.m_ImpactId);
            }
          }
          break;
        case RemoveType.LOGIC_ID:
          foreach (ImpactInfo impact in target.GetSkillStateInfo().GetAllImpact()) {
            if (-1 == id || impact.ConfigData.ImpactLogicId == id) {
              ImpactSystem.Instance.StopImpactById(target, impact.m_ImpactId);
            }
          }
          break;
        case RemoveType.GFX_ID:
          foreach (ImpactInfo impact in target.GetSkillStateInfo().GetAllImpact()) {
            if (-1 == id || impact.ConfigData.ImpactGfxLogicId == id) {
              ImpactSystem.Instance.StopImpactById(target, impact.m_ImpactId);
            }
          }
          break;
      }
    }
  }
}

