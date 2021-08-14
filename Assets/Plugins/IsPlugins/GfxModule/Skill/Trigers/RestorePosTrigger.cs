using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class StorePosTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      StorePosTrigger copy = new StorePosTrigger();
      copy.m_StartTime = m_StartTime;
      return copy;
    }

    public override void Reset()
    {
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 1) {
        m_StartTime = long.Parse(callData.GetParamId(0));
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
      Vector3 pos = obj.transform.position;
      instance.CustomDatas.AddData<Vector3>(pos);
      return false;
    }
  }

  public class RestorePosTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      RestorePosTrigger copy = new RestorePosTrigger();
      copy.m_StartTime = m_StartTime;
      return copy;
    }

    public override void Reset()
    {
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 1) {
        m_StartTime = long.Parse(callData.GetParamId(0));
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
      Vector3 old_pos = instance.CustomDatas.GetData<Vector3>();
      if (old_pos != null) {
        obj.transform.position = old_pos;
        TriggerUtil.UpdateObjPosition(obj);
      }
      return false;
    }
  }
}
