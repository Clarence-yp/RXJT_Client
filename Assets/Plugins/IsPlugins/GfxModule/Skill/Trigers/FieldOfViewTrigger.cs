using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class FieldOfViewTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      FieldOfViewTrigger copy = new FieldOfViewTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_RemainTime = m_RemainTime;
      copy.m_FOVRemain = m_FOVRemain;
      copy.m_FOVStart = FieldOfView;
      return copy;
    }
    public override void Reset()
    {
      m_IsInited = false;
      FieldOfView = m_FOVStart;
    }
    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      if (curSectionTime > m_StartTime + m_RemainTime) {
        return false;
      }
      if (!m_IsInited) {
        m_IsInited = true;
        FieldOfView = m_FOVRemain;
      }
      return true;
    }
    public override void Analyze(object sender, SkillInstance instance)
    {

    }
    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 3) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_RemainTime = long.Parse(callData.GetParamId(1));
        m_FOVRemain = float.Parse(callData.GetParamId(2));
      }
    }
    private float FieldOfView
    {
      get
      {
        Camera cm = Camera.main;
        if (cm != null) {
          Camera cc = cm.GetComponent<Camera>();
          if (cc != null) {
            return cc.fieldOfView;
          }
        }
        //wrong
        return 0f;
      }
      set
      {
        Camera cm = Camera.main;
        if (cm != null) {
          Camera cc = cm.GetComponent<Camera>();
          if (cc != null) {
            cc.fieldOfView = value;
          }
        }
      }
    }
    private long m_RemainTime = 0;
    private float m_FOVStart = 0f;
    private float m_FOVRemain = 0f;
    private bool m_IsInited = false;
  }
}
