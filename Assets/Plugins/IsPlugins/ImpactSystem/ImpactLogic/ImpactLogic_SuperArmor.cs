using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire{
  class ImpactLogic_SuperArmor : AbstractImpactLogic {

    private enum SuperArmorType
    {
      SUPER_ARMOR = 0,
      ULTRA_ARMOR = 1,
    }
    public override void StartImpact(CharacterInfo obj, int impactId) {
      if (null != obj) {
        ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
        if (null != impactInfo) {
          if (impactInfo.ConfigData.ExtraParams.Count > 0) {
            int superArmorType = int.Parse(impactInfo.ConfigData.ExtraParams[0]);
            if((int)SuperArmorType.SUPER_ARMOR == superArmorType){
              obj.SuperArmor = true;
            } else if((int)SuperArmorType.ULTRA_ARMOR == superArmorType) {
              obj.UltraArmor = true;
            }
          } else {
            obj.SuperArmor = true;
          }
        }
      }
      base.StartImpact(obj, impactId);
    }

    public override void Tick(CharacterInfo character, int impactId) {
      ImpactInfo impactInfo = character.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        if (impactInfo.m_IsActivated) {
          if (TimeUtility.GetServerMilliseconds() > impactInfo.m_StartTime + impactInfo.m_ImpactDuration) {
            if (impactInfo.ConfigData.ExtraParams.Count > 0) {
              int superArmorType = int.Parse(impactInfo.ConfigData.ExtraParams[0]);
              if((int)SuperArmorType.SUPER_ARMOR == superArmorType){
                character.SuperArmor = false;
              } else if((int)SuperArmorType.ULTRA_ARMOR == superArmorType) {
                character.UltraArmor = false;
              }
            } else {
              character.SuperArmor = false;
            }
            impactInfo.m_IsActivated = false;
          }
        }
      }
    }
  }
}
