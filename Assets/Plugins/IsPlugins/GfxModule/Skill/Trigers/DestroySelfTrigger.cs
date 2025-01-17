﻿using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class DestroySelfTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      DestroySelfTrigger copy = new DestroySelfTrigger();
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
      DashFire.LogicSystem.NotifyGfxDestroyObj(obj);
      return false;
    }
  }
}
