using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class SetChildVisibleTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      SetChildVisibleTrigger copy = new SetChildVisibleTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_ChildName = m_ChildName;
      copy.m_IsShow = m_IsShow;
      return copy;
    }

    public override void Reset()
    {
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 3) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_ChildName = callData.GetParamId(1);
        m_IsShow = bool.Parse(callData.GetParamId(2));
      }
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      GameObject obj = sender as GameObject;
      if (obj == null) {
        return false;
      }
      Transform child = TriggerUtil.GetChildNodeByName(obj, m_ChildName);
      if (child != null && child.gameObject != null) {
        TriggerUtil.SetObjVisible(child.gameObject, m_IsShow);
      }
      return false;
    }

    private string m_ChildName;
    private bool m_IsShow;
  }
}
