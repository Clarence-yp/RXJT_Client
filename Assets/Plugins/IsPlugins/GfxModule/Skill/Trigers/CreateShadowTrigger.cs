using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class ShadowInfo
  {
    public GameObject ShadowObject;
    public long StartTime;
    public long HoldTime;
    public long FadeOutTime;
    public float StartAlpha;
  }

  public class CreateShadowTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      CreateShadowTrigger copy = new CreateShadowTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_CreateInterval = m_CreateInterval;
      copy.m_MaxShadowCount = m_MaxShadowCount;
      copy.m_HoldTime = m_HoldTime;
      copy.m_FadeOutTime = m_FadeOutTime;
      copy.m_ShadowMaterial = m_ShadowMaterial;
      copy.m_ShaderName = m_ShaderName;
      copy.m_StartAlpha = m_StartAlpha;
      copy.m_IgnoreList.AddRange(m_IgnoreList);
      return copy;
    }

    public override void Reset()
    {
      m_IsInited = false;
      m_IsCreateOver = false;
      m_CreatedCount = 0;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 7) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_CreateInterval = long.Parse(callData.GetParamId(1));
        m_MaxShadowCount = int.Parse(callData.GetParamId(2));
        m_HoldTime = long.Parse(callData.GetParamId(3));
        m_FadeOutTime = long.Parse(callData.GetParamId(4));
        m_ShadowMaterial = callData.GetParamId(5);
        m_ShaderName = callData.GetParamId(6);
        m_StartAlpha = float.Parse(callData.GetParamId(7));
      }
    }

    protected override void Load(ScriptableData.FunctionData funcData)
    {
      ScriptableData.CallData callData = funcData.Call;
      if (null != callData) {
        Load(callData);

        foreach (ScriptableData.ISyntaxComponent statement in funcData.Statements) {
          ScriptableData.CallData stCall = statement as ScriptableData.CallData;
          if (null == stCall || stCall.GetParamNum() <= 0) {
            continue;
          }
          string id = stCall.GetId();
          if (id == "ignorelist") {
            m_IgnoreList.Add(stCall.GetParamId(0));
          }
        }
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

      if (!m_IsInited) {
        Init(instance, curSectionTime);
      }
      if (!m_IsCreateOver && curSectionTime >= m_NextCreateTime) {
        m_NextCreateTime += m_CreateInterval;
        if (m_CreateInterval <= 0) {
          m_IsCreateOver = true;
        }
        CreateShadow(obj, instance, curSectionTime);
        if (m_CreatedCount >= m_MaxShadowCount) {
          m_IsCreateOver = true;
        }
      }
      if (m_IsCreateOver) {
        return false;
      }
      return true;
    }

    public void Init(SkillInstance instance, long curSectionTime)
    {
      m_IsInited = true;
      m_NextCreateTime = curSectionTime;
    }

    private void CreateShadow(GameObject obj, SkillInstance instance, long curSectionTime)
    {
      ShadowProduct shadow = new ShadowProduct(obj, m_ShadowMaterial, m_ShaderName,
                                               m_HoldTime, m_FadeOutTime,
                                               m_StartAlpha, m_IgnoreList);
      GfxSkillSystem.Instance.AddSkillProduct(shadow);
      m_CreatedCount++;
    }

    private long m_CreateInterval;
    private int m_MaxShadowCount;
    private long m_HoldTime;
    private long m_FadeOutTime;
    private string m_ShadowMaterial;
    private string m_ShaderName;
    private float m_StartAlpha;

    private int m_CreatedCount = 0;
    private long m_NextCreateTime;
    private bool m_IsInited = false;
    private bool m_IsCreateOver = false;
    private List<ShadowInfo> m_ShadowList = new List<ShadowInfo>();
    private List<String> m_IgnoreList = new List<String>();
  }
}
