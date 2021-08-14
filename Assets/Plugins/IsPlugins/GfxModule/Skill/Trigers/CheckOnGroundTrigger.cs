using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class CheckOnGroundTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      CheckOnGroundTrigger copy = new CheckOnGroundTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_GroundHeight = m_GroundHeight;
      copy.m_Message  = m_Message;
      return copy;
    }

    public override void Reset()
    {
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 3) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_GroundHeight = float.Parse(callData.GetParamId(1));
        m_Message = callData.GetParamId(2);
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
      float height = TriggerUtil.GetHeightWithGround(obj);
      if (height <= m_GroundHeight) {
        instance.SendMessage(m_Message);
        return false;
      } else {
        return true;
      }
    }

    private float m_GroundHeight;
    private string m_Message;
  }
}
