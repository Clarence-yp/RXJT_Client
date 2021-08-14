using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire{
  class ImpactLogic_Invincible : AbstractImpactLogic{

    public override void Tick(CharacterInfo character, int impactId)
    {
      ImpactInfo impactInfo = character.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null == impactInfo) {
        return;
      }
      if (!impactInfo.m_IsActivated) {
        return;
      }
      if (!impactInfo.m_HasEffectApplyed) {
        character.SetStateFlag(Operate_Type.OT_AddBit, CharacterState_Type.CST_Invincible);
        impactInfo.m_HasEffectApplyed = true;
      }
      if (TimeUtility.GetServerMilliseconds() > impactInfo.m_StartTime + impactInfo.m_ImpactDuration) {
        character.SetStateFlag(Operate_Type.OT_RemoveBit, CharacterState_Type.CST_Invincible);
        impactInfo.m_IsActivated = false;
      }
    }
  }
}
