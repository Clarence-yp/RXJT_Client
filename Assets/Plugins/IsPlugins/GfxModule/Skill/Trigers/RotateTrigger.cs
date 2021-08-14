using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class RotateTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      RotateTrigger copy = new RotateTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_RemainTime = m_RemainTime;
      copy.m_RotateSpeed = m_RotateSpeed;
      return copy;
    }

    public override void Reset()
    {
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 3) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_RemainTime = long.Parse(callData.GetParamId(1));
        m_RotateSpeed = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
      }
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      if (curSectionTime > m_StartTime + m_RemainTime || !instance.IsRotateEnable) {
        return false;
      }
      GameObject obj = sender as GameObject;
      if (obj == null) {
        return false;
      }

      obj.transform.Rotate(m_RotateSpeed * TriggerUtil.ConvertToSecond(delta));
      return true;
    }

    private long m_RemainTime;
    private Vector3 m_RotateSpeed;
  }
}
