using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public abstract class Condition
  {
    public virtual void Load(ScriptableData.CallData stCall) {
    }
    public virtual bool IsSatisfied(GameObject obj, SkillInstance instance) {
      return false;
    }
  }

  public class TargetCondition : Condition
  {
    public override void Load(ScriptableData.CallData stCall)
    {
      if (stCall.GetParamNum() >= 1) {
        m_IsNeedTarget = bool.Parse(stCall.GetParamId(0));
      }
    }

    public override bool IsSatisfied(GameObject obj, SkillInstance instance)
    {
      MoveTargetInfo targetinfo = instance.CustomDatas.GetData<MoveTargetInfo>();
      if (m_IsNeedTarget && targetinfo != null && targetinfo.Target != null) {
        return true;
      }
      if (!m_IsNeedTarget && (targetinfo == null || targetinfo.Target == null)) {
        return true;
      }
      return false;
    }

    private bool m_IsNeedTarget;
  }

  public class StateCondition : Condition
  {
    public override void Load(ScriptableData.CallData stCall)
    {
      if (stCall.GetParamNum() >= 2) {
        m_IsEquals = bool.Parse(stCall.GetParamId(0));
        m_TargetState = TriggerUtil.GetBeHitStateFromStr(stCall.GetParamId(1));
      }
    }

    public override bool IsSatisfied(GameObject obj, SkillInstance instance)
    {
      BeHitState obj_state = SkillDamageManager.GetBeHitState(obj);
      if (m_IsEquals && m_TargetState == obj_state) {
        return true;
      }
      if (!m_IsEquals && m_TargetState != obj_state) {
        return true;
      }
      return false;
    }
    private bool m_IsEquals;
    private BeHitState m_TargetState;
  }

  public class HeightCondition : Condition
  {
    public HeightCondition(bool is_judge_fly) {
      m_IsJudgeFly = is_judge_fly;
    }
    public override void Load(ScriptableData.CallData stCall)
    {
      if (stCall.GetParamNum() >= 2) {
        m_Height = float.Parse(stCall.GetParamId(0));
        m_IsIncludeBehitState = bool.Parse(stCall.GetParamId(1));
      }
    }

    public override bool IsSatisfied(GameObject obj, SkillInstance instance)
    {
      if (!m_IsIncludeBehitState) {
        BeHitState state = SkillDamageManager.GetBeHitState(obj);
        if (state != BeHitState.kStand) {
          return false;
        }
      }
      float height = TriggerUtil.GetHeightWithGround(obj);
      if (m_IsJudgeFly && height >= m_Height) {
        return true;
      }
      if (!m_IsJudgeFly && height <= m_Height) {
        return true;
      }
      return false;
    }

    private bool m_IsIncludeBehitState;
    private bool m_IsJudgeFly;
    private float m_Height;
  }

  public class GotoSectionTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      GotoSectionTrigger copy = new GotoSectionTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_SectionNum = m_SectionNum;
      copy.m_RemainTime = m_RemainTime;
      copy.m_IsHaveCondition = m_IsHaveCondition;
      copy.m_Conditions.AddRange(m_Conditions);
      return copy;
    }

    public override void Reset()
    {
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 3) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_SectionNum = int.Parse(callData.GetParamId(1));
        m_RemainTime = long.Parse(callData.GetParamId(2));
      }
    }

    protected override void Load(ScriptableData.FunctionData funcData)
    {
      ScriptableData.CallData callData = funcData.Call;
      if (null != callData) {
        Load(callData);

        foreach (ScriptableData.ISyntaxComponent statement in funcData.Statements) {
          ScriptableData.CallData stCall = statement as ScriptableData.CallData;
          if (null == stCall) {
            continue;
          }
          string id = stCall.GetId();
          Condition condition = null;
          if (id == "targetcondition") {
            condition = new TargetCondition();
          } else if (id == "statecondition") {
            condition = new StateCondition();
          } else if (id == "flycondition") {
            condition = new HeightCondition(true);
          } else if (id == "groundcondition") {
            condition = new HeightCondition(false);
          }
          if (condition != null) {
            m_IsHaveCondition = true;
            condition.Load(stCall);
            m_Conditions.Add(condition);
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
      if (m_SectionNum < 0) {
        return false;
      }
      if (m_IsHaveCondition) {
        if (IsConditionTriggered(obj, instance)) {
          instance.GoToSection = m_SectionNum;
          return false;
        }
        if (m_RemainTime <= 0 || curSectionTime > m_StartTime + m_RemainTime) {
          return false;
        }
        return true;
      } else {
        instance.GoToSection = m_SectionNum;
        return false;
      }
    }

    private bool IsConditionTriggered(GameObject obj, SkillInstance instance)
    {
      foreach (Condition condition in m_Conditions) {
        if (!condition.IsSatisfied(obj, instance)) {
          return false;
        }
      }
      return true;
    }

    private int m_SectionNum = -1;
    private long m_RemainTime = 0;
    private bool m_IsHaveCondition = false;
    private List<Condition> m_Conditions = new List<Condition>();
  }
}
