using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire{
  class ImpactLogic_ChangeSkill : AbstractImpactLogic {
    public override void StartImpact(CharacterInfo obj, int impactId) {
      if (null == obj) {
        return;
      }
      ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (impactInfo.ConfigData.ExtraParams.Count < 2) {
        return;
      }
      SkillCategory category = (SkillCategory)(int.Parse(impactInfo.ConfigData.ExtraParams[0]));
      int head_skillid = int.Parse(impactInfo.ConfigData.ExtraParams[1]);
      SkillInfo head_skill = obj.GetSkillStateInfo().GetSkillInfoById(head_skillid);
      if (head_skill == null) {
        return;
      }
      ChangeSkillCategory(obj, head_skill.SkillId, category);
      // change category head to new skill
      List<SkillLogicData> player_skill_config = new List<SkillLogicData>();
      List<SkillInfo> player_skills = obj.GetSkillStateInfo().GetAllSkill();
      foreach (SkillInfo info in player_skills) {
        player_skill_config.Add(info.ConfigData);
      }
      if (null != obj.SkillController) {
        SkillNode new_head = obj.SkillController.InitCategorySkillNode(player_skill_config, head_skill.ConfigData);
        if (new_head != null) {
          SkillNode old_head = obj.SkillController.ChangeCategorySkill(category, new_head);
          impactInfo.LogicDatas.AddData<SkillNode>(old_head);
        }
      }
    }

    private void ChangeSkillCategory(CharacterInfo obj, int skillid, SkillCategory category)
    {
      SkillInfo head_skill = obj.GetSkillStateInfo().GetSkillInfoById(skillid);
      if (head_skill == null) {
        return;
      }
      SkillInfo skillinfo = head_skill;
      while (skillinfo != null && skillinfo.ConfigData != null) {
        skillinfo.ConfigData.Category = category;
        int next_skill = skillinfo.ConfigData.NextSkillId;
        skillinfo = obj.GetSkillStateInfo().GetSkillInfoById(next_skill);
      }
    }

    public override void Tick(CharacterInfo character, int impactId) {
      ImpactInfo impactInfo = character.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (null != impactInfo) {
        if (impactInfo.m_IsActivated) {
          if (TimeUtility.GetServerMilliseconds() > impactInfo.m_StartTime + impactInfo.m_ImpactDuration) {
            StopImpact(character, impactId);
          }
        }
      };
    }

    public override void StopImpact(CharacterInfo obj, int impactId)
    {
      ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
      if (impactInfo == null) {
        return;
      }
      if (impactInfo.ConfigData.ExtraParams.Count < 2) {
        return;
      }
      SkillCategory category = (SkillCategory)(int.Parse(impactInfo.ConfigData.ExtraParams[0]));
      int head_skillid = int.Parse(impactInfo.ConfigData.ExtraParams[1]);
      //change category skill back
      SkillNode old_head = impactInfo.LogicDatas.GetData<SkillNode>();
      if (old_head != null && null!=obj.SkillController) {
        SkillNode head = obj.SkillController.ChangeCategorySkill(category, old_head);
        ChangeSkillCategory(obj, head_skillid, SkillCategory.kNone);
      }
      impactInfo.m_IsActivated = false;
    }
  }
}
