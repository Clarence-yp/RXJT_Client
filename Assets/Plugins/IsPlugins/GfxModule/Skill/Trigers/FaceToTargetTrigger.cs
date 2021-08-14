using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class FaceToTargetTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      FaceToTargetTrigger copy = new FaceToTargetTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_RemainTime = m_RemainTime;
      copy.m_IsHaveRotateSpeed = m_IsHaveRotateSpeed;
      copy.m_RotateSpeed = m_RotateSpeed;
      return copy;
    }

    public override void Reset()
    {
      m_IsExecuted = false;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 2) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_RemainTime = long.Parse(callData.GetParamId(1));
      }
      if (callData.GetParamNum() >= 3) {
        m_IsHaveRotateSpeed = true;
        m_RotateSpeed = Vector3.zero;
        m_RotateSpeed.y = (float)(float.Parse(callData.GetParamId(2)) * Math.PI / 180.0f);
      }
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      if (m_IsExecuted && curSectionTime > (m_StartTime + m_RemainTime)) {
        return false;
      }
      GameObject obj = sender as GameObject;
      if (obj == null) {
        return false;
      }
      m_IsExecuted = true;
      MoveTargetInfo target_info = instance.CustomDatas.GetData<MoveTargetInfo>();
      if (target_info == null || target_info.Target == null) {
        return true;
      }

      Vector3 dir = target_info.Target.transform.position - obj.transform.position;
      if (!m_IsHaveRotateSpeed || m_RotateSpeed.y == 0) {
        GfxSkillSystem.ChangeDir(obj, (float)Math.Atan2(dir.x, dir.z));
      } else {
        float maxRotateDelta = m_RotateSpeed.y * TriggerUtil.ConvertToSecond(delta);
        Vector3 now_dir = Vector3.RotateTowards(obj.transform.forward, dir, maxRotateDelta, 0);
        GfxSkillSystem.ChangeDir(obj, (float)Math.Atan2(now_dir.x, now_dir.z));
      }
      return true;
    }

    private long m_RemainTime;
    private bool m_IsHaveRotateSpeed = false;
    private Vector3 m_RotateSpeed;
    private bool m_IsExecuted = false;
  }
}
