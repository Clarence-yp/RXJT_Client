using System;
using System.Collections.Generic;
using DashFire;

namespace DashFire
{
  class ImpactLogic_RefixDamage : AbstractImpactLogic
  {
    public override int RefixHpDamage(CharacterInfo obj, int impactId, int hpDamage, int senderId)
    {
      if (null != obj) {
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          if (impactInfo.ConfigData.ExtraParams.Count > 0) {
            float damageRate = float.Parse(impactInfo.ConfigData.ExtraParams[0]);
            hpDamage = (int)(hpDamage * damageRate);
          }
        }
      }
      return hpDamage;
    }
  }
}
